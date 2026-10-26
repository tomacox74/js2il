"""Assemble or verify sanitized #2242 readback evidence offline; no AWS/GitHub access."""
import argparse
from datetime import datetime, timezone
import json
from pathlib import Path
import re

from .archive_inventory import encoded, sha, validate as validate_inventory
from .cutover_evidence import REPOSITORY
from .retained_evidence import require

ROOT = Path(__file__).resolve().parents[3]
BASELINE = ROOT/'docs/ECMA262/evidence/historical-archive-inventory-2242.json'
REVISION = 'cd90c4edda81988c7adb6b0457c760714f37da23'
GUARDS = {
    'Validate reviewed operation': 'success',
    'Verify reports/outboxes and retain original evidence in encrypted S3': 'skipped',
    'Read back one existing retained archive without writes': 'success',
}


def point(value):
    result = datetime.fromisoformat(value.replace('Z', '+00:00'))
    require(result.tzinfo is not None, 'Evidence timestamp needs a timezone')
    return result


def digest(value):
    require(isinstance(value, str) and re.fullmatch(r'[0-9a-f]{64}', value) is not None,
            'Invalid evidence digest')


def validate_readback(row, baseline):
    require(set(row) == {'requested_artifact_id', 'run_id', 'run_attempt', 'source_revision',
                        'created_at', 'completed_at', 'run_conclusion', 'operation', 'job_id',
                        'execution_guards', 'actions_artifact', 'report_sha256', 'report'},
            'Unexpected readback provenance fields')
    require(row['source_revision'] == REVISION and row['run_attempt'] == 1
            and row['run_conclusion'] == 'success' and row['operation'] == 'verify-retained'
            and row['execution_guards'] == GUARDS
            and type(row['run_id']) is int and row['run_id'] > 0
            and type(row['job_id']) is int and row['job_id'] > 0, 'Read-only run provenance differs')
    meta = row['actions_artifact']
    require(set(meta) == {'artifact_id', 'size_in_bytes', 'zip_sha256', 'expires_at'}
            and type(meta['artifact_id']) is int and meta['artifact_id'] > 0
            and type(meta['size_in_bytes']) is int and 0 < meta['size_in_bytes'] <= 2_000_000,
            'Invalid sanitized Actions artifact provenance')
    digest(meta['zip_sha256'])
    report = row['report']
    require(sha(encoded(report)) == row['report_sha256'], 'Sanitized report bytes differ')
    reference = baseline['reused_readback']['report']
    require(set(report) == set(reference), 'Unexpected report fields')
    for key in reference.keys()-{'object', 'checked_at'}:
        require(encoded(report[key]) == encoded(reference[key]), 'Readback scope or receipt attribution differs')
    aid = row['requested_artifact_id']
    objects = {r['artifact_id']: r for r in baseline['retained_objects']}
    require(type(aid) is int and aid in objects
            and aid != baseline['reused_readback']['object']['artifact_id'],
            'Requested object is outside the seven new readbacks')
    proof = report['object']
    require(set(proof) == {'version_id', 'content_length', 'sha256', 'encryption', 'lock_mode',
                          'retain_until', 'retention_verified', 'artifact_id'},
            'Unexpected retained-object fields')
    original = objects[aid]
    archive = next(a for a in baseline['archives'] if a['artifact_id'] == aid)
    require(proof['artifact_id'] == aid and proof['version_id'] == original['version_id']
            and proof['sha256'] == original['sha256']
            and type(proof['content_length']) is int
            and proof['content_length'] == archive['size_in_bytes']
            and proof['encryption'] == 'aws:kms' and proof['lock_mode'] == 'COMPLIANCE'
            and proof['retention_verified'] is True, 'Exact retained version/bytes/protection differs')
    checked = point(report['checked_at'])
    require(point(row['created_at']) <= checked <= point(row['completed_at'])
            and point(proof['retain_until']) > checked
            and point(proof['retain_until']) >= point(original['retain_until'])
            and point(meta['expires_at']) > point(row['completed_at']),
            'Readback timing or recorded retention differs')


def summary(batch, baseline):
    return {
        'new_readbacks': len(batch['readbacks']),
        'reused_readbacks': 1,
        'verified_selected_objects': len(batch['readbacks'])+1,
        'required_containers': baseline['summary']['required_archives'],
        'required_containers_without_identified_retained_object':
            baseline['summary']['required_archives_without_identified_retained_object'],
        'original_archive_gaps': len(baseline['original_history']['manifest']['gaps']),
    }


def validate(batch, baseline_path=BASELINE):
    baseline_raw = baseline_path.read_bytes()
    baseline = json.loads(baseline_raw)
    validate_inventory(baseline)
    require(batch['schema_version'] == 1 and batch['repository'] == REPOSITORY
            and batch['selected_set_readback_verified'] is True
            and batch['full_archive_coverage_verified'] is False
            and batch['history_complete'] is False and batch['database_access'] is False
            and batch['object_store_changes'] is False
            and batch['verification_source_revision'] == REVISION,
            'Invalid readback batch scope')
    require(batch['inventory'] == {'path': str(BASELINE.relative_to(ROOT)),
                                  'sha256': sha(baseline_raw), 'source_revision': REVISION}
            and batch['prior_readback'] == baseline['reused_readback'],
            'Frozen inventory or reused readback attribution differs')
    rows = batch['readbacks']
    expected = {r['artifact_id'] for r in baseline['retained_objects']} - {
        baseline['reused_readback']['object']['artifact_id']}
    require(len(rows) == len(expected) and {r['requested_artifact_id'] for r in rows} == expected
            and len({r['run_id'] for r in rows}) == len(rows)
            and len({r['actions_artifact']['artifact_id'] for r in rows}) == len(rows),
            'Missing, duplicate or unexpected readbacks')
    for row in rows:
        validate_readback(row, baseline)
    chronological = sorted(rows, key=lambda row: point(row['created_at']))
    require(all(point(a['completed_at']) <= point(b['created_at'])
                for a, b in zip(chronological, chronological[1:])),
            'Readbacks were not serialized in the shared concurrency group')
    require(point(batch['assembled_at']) >= max(point(r['completed_at']) for r in rows)
            and batch['summary'] == summary(batch, baseline), 'Batch timing or summary differs')


def assemble(paths, baseline_path=BASELINE):
    baseline_raw = baseline_path.read_bytes()
    baseline = json.loads(baseline_raw)
    result = {
        'schema_version': 1, 'repository': REPOSITORY,
        'scope': 'Seven new exact-version readbacks plus one reused readback of the original eight selected cutover archives',
        'assembled_at': datetime.now(timezone.utc).isoformat(),
        'verification_source_revision': REVISION,
        'inventory': {'path': str(BASELINE.relative_to(ROOT)), 'sha256': sha(baseline_raw),
                      'source_revision': REVISION},
        'selected_set_readback_verified': True, 'full_archive_coverage_verified': False,
        'history_complete': False, 'database_access': False, 'object_store_changes': False,
        'prior_readback': baseline['reused_readback'],
        'readbacks': sorted((json.loads(path.read_bytes()) for path in paths),
                            key=lambda row: row['requested_artifact_id']),
        'limits': [
            'Exact-version full-file readbacks establish retrieval, digest, length and protection at each recorded checked_at, not indefinitely.',
            'The eighth readback is reused from run 40251228151; it was not dispatched again.',
            'Only the original eight selected containers are covered; 33 recovery containers still lack identified independent-retention proof.',
            'The frozen inventory is unchanged; its historical statements retain their original observation time.',
            'Original ZIP/SQLite membership and source pins are not new per-member rehashes, outbox-acknowledgement checks or database reconciliation.',
            'No preservation upload, import, restore, fixture execution, policy change or production authority change was performed.',
            'COMPLIANCE retention deadlines remain finite; proposed retention and recovery policies are not approved by these readbacks.'],
    }
    result['summary'] = summary(result, baseline)
    validate(result, baseline_path)
    return result


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    mode = parser.add_mutually_exclusive_group(required=True)
    mode.add_argument('--reports', nargs='+', type=Path, help='Downloaded sanitized per-run provenance JSON files')
    mode.add_argument('--verify', type=Path, help='Verify the durable batch entirely offline')
    parser.add_argument('--output', type=Path)
    args = parser.parse_args(argv)
    if args.verify:
        require(args.output is None, 'Offline verification does not replace evidence')
        result = json.loads(args.verify.read_bytes())
        validate(result)
    else:
        require(args.output is not None and not args.output.exists(), 'Assembly needs a new output path')
        result = assemble(args.reports)
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_bytes(encoded(result))
    print(json.dumps(result['summary'], sort_keys=True))


if __name__ == '__main__':
    main()
