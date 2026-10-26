"""Inventory frozen #2242 archives using GitHub reads only; never import or preserve."""
import argparse
from datetime import datetime, timezone
import hashlib
import io
import json
from pathlib import Path
import re
import subprocess
import threading
import zipfile

from .cutover_evidence import DEFAULT_MANIFEST, REPOSITORY, load_manifest, member
from .monitoring import github
from .retained_evidence import PINS, fetch_receipt, inspect_receipt, require

BASE = f'repos/{REPOSITORY}'
WORKFLOW = 'test262-history-import.yml'
WINDOW = '2026-10-01..2026-10-08'
FRESH = {
    'run_id': 40251228151, 'run_attempt': 1,
    'source_revision': 'a9983076b547cf1e3fb8753bf5845dc1878d1cd2',
    'artifact_id': 12620771637,
    'zip_sha256': '31393e10261bfc6d487da3dc4ab41e39e311378e8e2d559cc210212cbab8563d',
    'report_sha256': 'af32af38e77489942ac4d94767f9234d013bfb817229f2f76eac526e84a3e433',
}
AUXILIARY = {
    11522581310: 'cutover_restore_report',
    11523270256: 'cutover_native_checkpoint',
    11523056094: 'cutover_native_reconciled_checkpoint',
    11522307991: 'final_legacy_checkpoint_retained_only',
}
MEMBER = re.compile(r'(?:history-manifest\.json|auto-plan\.json|import-report\.json|'
                    r'restore-report\.json|native\.sqlite|'
                    r'[0-9]+/(?:source\.zip|catalog\.sqlite|native\.sqlite)|'
                    r'[0-9]+-outbox\.sqlite(?:-wal|-shm)?)')


def sha(raw):
    return hashlib.sha256(raw).hexdigest()


def encoded(document):
    return (json.dumps(document, indent=2)+'\n').encode()


class ArtifactRanges(io.RawIOBase):
    """Seekable, bounded absolute ranges; reject a server that returns the full ZIP."""
    BLOCK = 131072
    BUDGET = 16*1024*1024

    def __init__(self, artifact_id, size, fetch_range=None):
        require(type(artifact_id) is int and artifact_id > 0
                and type(size) is int and size > 0, 'Invalid artifact range scope')
        self.artifact_id, self.size, self.position = artifact_id, size, 0
        self.cache, self.downloaded = {}, 0
        self.fetch_range = fetch_range or self.github_range

    def readable(self):
        return True

    def seekable(self):
        return True

    def tell(self):
        return self.position

    def seek(self, offset, whence=0):
        require(whence in (0, 1, 2), 'Invalid range seek origin')
        position = offset+(0 if whence == 0 else self.position if whence == 1 else self.size)
        require(0 <= position <= self.size, 'ZIP seek outside artifact')
        self.position = position
        return position

    def github_range(self, start, end):
        with subprocess.Popen(['gh', 'api', f'{BASE}/actions/artifacts/{self.artifact_id}/zip',
                               '-H', f'Range: bytes={start}-{end}'],
                              stdout=subprocess.PIPE, stderr=subprocess.DEVNULL) as process:
            timer = threading.Timer(90, process.kill)
            timer.start()
            try:
                raw = process.stdout.read(end-start+2)
                require(len(raw) == end-start+1, 'GitHub ignored or truncated bounded ZIP range')
                require(process.wait(timeout=10) == 0, 'GitHub ZIP range request failed')
                return raw
            finally:
                timer.cancel()
                if process.poll() is None:
                    process.kill()
                process.wait(timeout=10)

    def read(self, size=-1):
        size = self.size-self.position if size < 0 else min(size, self.size-self.position)
        require(size <= 2*1024*1024, 'ZIP index read exceeds inventory bound')
        result = bytearray()
        while len(result) < size:
            start = self.position//self.BLOCK*self.BLOCK
            end = min(start+self.BLOCK, self.size)-1
            if start not in self.cache:
                require(self.downloaded+end-start+1 <= self.BUDGET, 'ZIP range budget exceeded')
                raw = self.fetch_range(start, end)
                require(len(raw) == end-start+1, 'ZIP range length differs')
                self.cache[start] = raw
                self.downloaded += len(raw)
            offset = self.position-start
            part = self.cache[start][offset:offset+size-len(result)]
            result.extend(part)
            self.position += len(part)
        return bytes(result)


def members(archive):
    result = []
    for info in archive.infolist():
        if info.is_dir():
            continue
        require(MEMBER.fullmatch(info.filename) is not None, 'Unexpected archive member name')
        result.append({'name': info.filename, 'size_in_bytes': info.file_size,
                       'crc32': f'{info.CRC:08x}'})
    require(len({r['name'] for r in result}) == len(result), 'Duplicate ZIP member')
    return sorted(result, key=lambda row: row['name'])


def small_json(archive, name):
    info = archive.getinfo(member(archive, name))
    require(info.file_size <= 512*1024, 'JSON member exceeds inventory bound')
    raw = archive.read(info)
    return json.loads(raw), sha(raw)


def project_import(report, history, manifest_sha):
    require(report['history_complete'] is False and type(report['complete']) is bool
            and report['manifest_sha256'] == manifest_sha and report['gaps'] == history['gaps'],
            'Import report manifest attribution differs')
    sources = {r['artifact_id']: r for r in history['artifacts']}
    selected = report['selected_artifact_ids']
    require(len(set(selected)) == len(selected) and set(selected) <= sources.keys(),
            'Invalid import selection')
    snapshots = []
    for row in report['snapshots']:
        source = sources[row['artifact_id']]
        databases = [db for db in source['databases'] if db['sha256'] == row['sha256']
                     and db['source_uri'] == row['source_uri']]
        require(len(databases) == 1 and row['archive_sha256'] == source['archive_sha256']
                and row['state'] in ('verified', 'uploading'), 'Import source attribution differs')
        public = {'artifact_id': row['artifact_id'], 'state': row['state']}
        if row['state'] == 'verified':
            count = row['verified_observation_mappings']
            require(type(count) is int and count >= 0 and row['result']['observations'] == count,
                    'Reported observation parity differs')
            public['verified_observation_mappings'] = count
        snapshots.append(public)
    started = [r['artifact_id'] for r in snapshots]
    require(len(set(started)) == len(started) and set(started) <= set(selected),
            'Duplicate or unselected report snapshot')
    if report['complete']:
        require(set(started) == set(selected) and all(r['state'] == 'verified' for r in snapshots),
                'Incomplete successful import report')
    result = {'complete': report['complete'], 'selected_artifact_ids': selected, 'snapshots': snapshots}
    if 'error_type' in report:
        require(re.fullmatch(r'[A-Za-z][A-Za-z0-9_]*', report['error_type']) is not None,
                'Invalid report error class')
        result['error_type'] = report['error_type']
    return result


def project_plan(plan):
    require(plan['state'] in ('ready', 'blocked', 'verified-selection-complete'),
            'Unknown controller plan state')
    result = {'state': plan['state']}
    for key in ('selected_artifact_ids', 'failed_run_id'):
        if key in plan:
            value = plan[key]
            require((key == 'selected_artifact_ids' and isinstance(value, list)
                     and all(type(i) is int and i > 0 for i in value))
                    or (key == 'failed_run_id' and (
                        (type(value) is int and value > 0)
                        or (isinstance(value, str) and re.fullmatch(r'[1-9][0-9]*', value)))),
                    'Invalid public plan identifiers')
            result[key] = value
    return result


def inspect_archive(meta, history=None, manifest_sha=None, fetch_range=None):
    require(not meta['expired'] and re.fullmatch(r'sha256:[0-9a-f]{64}', meta['digest']) is not None
            and meta['workflow_run']['head_branch'] == 'master', 'Unavailable or unattributed artifact')
    reader = ArtifactRanges(meta['id'], meta['size_in_bytes'], fetch_range)
    result = {'artifact_id': meta['id'], 'run_id': meta['workflow_run']['id'],
              'source_revision': meta['workflow_run']['head_sha'],
              'zip_sha256': meta['digest'][7:], 'size_in_bytes': meta['size_in_bytes'],
              'actions_state': 'metadata_available', 'expires_at': meta['expires_at']}
    with zipfile.ZipFile(reader) as archive:
        result['members'] = members(archive)
        names = {r['name'] for r in result['members']}
        if 'import-report.json' in names:
            report, digest = small_json(archive, 'import-report.json')
            result['import_report'] = dict(project_import(report, history, manifest_sha), sha256=digest)
        if 'auto-plan.json' in names:
            plan, digest = small_json(archive, 'auto-plan.json')
            result['auto_plan'] = dict(project_plan(plan), sha256=digest)
        if 'history-manifest.json' in names:
            manifest, digest = small_json(archive, 'history-manifest.json')
            require(digest == manifest_sha, 'Original manifest member digest differs')
            result['history_manifest'] = manifest
    result['member_index_sha256'] = sha(encoded(result['members']))
    result['range_bytes_downloaded'] = reader.downloaded
    result['whole_zip_digest_verified'] = reader.downloaded == reader.size
    if result['whole_zip_digest_verified']:
        require(sha(b''.join(reader.cache[i] for i in sorted(reader.cache))) == result['zip_sha256'],
                'Fully fetched ZIP digest differs')
    return result


def paged(endpoint, key, fetch, maximum):
    result = []
    expected = None
    for page in range(1, (maximum+99)//100+1):
        response = fetch(f'{endpoint}{"&" if "?" in endpoint else "?"}per_page=100&page={page}')
        total = response['total_count']
        require(total <= maximum and (expected is None or total == expected),
                'Inventory pagination bound or coherent count differs')
        expected = total
        result.extend(response[key])
        if len(result) >= expected:
            require(len(result) == expected, 'Inventory pagination count differs')
            return result
    raise ValueError('Incomplete inventory pagination')


def fresh_receipt(fetch):
    run = fetch(f"{BASE}/actions/runs/{FRESH['run_id']}")
    require(run['head_sha'] == FRESH['source_revision'] and run['run_attempt'] == FRESH['run_attempt']
            and run['conclusion'] == 'success' and run['head_branch'] == 'master'
            and run['path'] == '.github/workflows/test262-cutover-evidence.yml',
            'Reused verification run provenance differs')
    meta = fetch(f"{BASE}/actions/artifacts/{FRESH['artifact_id']}")
    require(not meta['expired'] and meta['digest'] == 'sha256:'+FRESH['zip_sha256']
            and meta['size_in_bytes'] <= 2_000_000
            and meta['workflow_run']['id'] == FRESH['run_id']
            and meta['workflow_run']['head_sha'] == FRESH['source_revision'],
            'Reused verification artifact provenance differs')
    raw = fetch(f"{BASE}/actions/artifacts/{FRESH['artifact_id']}/zip", binary=True)
    require(sha(raw) == FRESH['zip_sha256'], 'Reused verification ZIP digest differs')
    with zipfile.ZipFile(io.BytesIO(raw)) as archive:
        report, digest = small_json(archive, 'retained-verification-report.json')
    require(digest == FRESH['report_sha256'] and report['selected_object_verified'] is True
            and report['historical_receipt_verified'] is True and report['history_complete'] is False
            and report['full_archive_coverage_verified'] is False, 'Reused readback scope differs')
    from .retained_objects import public_proof
    return dict(FRESH, checked_at=report['checked_at'],
                object=dict(public_proof(report['object']), artifact_id=report['object']['artifact_id']),
                report=report)


def summary(report):
    archives = report['archives']
    recoveries = [a for a in archives if 'import_report' in a]
    required = [a for a in archives if a['required_for_scope']]
    verified = [s['artifact_id'] for a in recoveries for s in a['import_report']['snapshots']
                if s['state'] == 'verified']
    return {
        'workflow_runs': len(report['workflow_runs']),
        'original_sources': len(report['original_history']['manifest']['artifacts']),
        'original_archive_gaps': len(report['original_history']['manifest']['gaps']),
        'required_archives': len(required),
        'required_archives_with_retained_object': sum(a['retained_object_artifact_id'] is not None for a in required),
        'required_archives_without_identified_retained_object': sum(a['retained_object_artifact_id'] is None for a in required),
        'import_recovery_archives': len(recoveries),
        'incomplete_import_recovery_archives': sum(not a['import_report']['complete'] for a in recoveries),
        'outbox_databases': sum(m['name'].endswith('-outbox.sqlite') for a in recoveries for m in a['members']),
        'outbox_wal_shm_members': sum(m['name'].endswith(('-outbox.sqlite-wal', '-outbox.sqlite-shm'))
                                    for a in recoveries for m in a['members']),
        'verified_source_entries_in_reports': len(verified),
        'unique_verified_sources_in_reports': len(set(verified)),
        'planning_only_archives': sum(a['classification'] == 'planning_only' for a in archives),
        'runs_without_artifacts': sum(not r['artifact_ids'] for r in report['workflow_runs']),
        'unsuccessful_runs_without_artifacts': sum(not r['artifact_ids'] and r['conclusion'] != 'skipped'
                                                 for r in report['workflow_runs']),
    }


def validate(report):
    config, pins, _, _ = load_manifest(DEFAULT_MANIFEST)
    require(report['schema_version'] == 1 and report['repository'] == REPOSITORY
            and report['history_complete'] is False
            and report['full_archive_coverage_verified'] is False
            and report['object_store_access'] is False and report['database_access'] is False,
            'Invalid inventory scope')
    original = report['original_history']
    history = original['manifest']
    require(original['container_artifact_id'] == config['history_archive_id']
            and original['manifest_sha256'] == config['history_manifest_sha256']
            and sha(encoded(history)) == config['history_manifest_sha256'],
            'Original source or gap ledger differs from pinned manifest')
    archives = {a['artifact_id']: a for a in report['archives']}
    runs = {r['run_id']: r for r in report['workflow_runs']}
    require(len(archives) == len(report['archives']) and len(runs) == len(report['workflow_runs'])
            and len(runs) == report['workflow_query']['total_count'], 'Duplicate or omitted inventory rows')
    require(set(archives) == {i for r in runs.values() for i in r['artifact_ids']} | set(AUXILIARY),
            'Archive/run coverage differs')
    run_artifacts = [i for r in runs.values() for i in r['artifact_ids']]
    require(len(run_artifacts) == len(set(run_artifacts)), 'Artifact appears under multiple runs')
    public_receipt = json.loads((Path(__file__).resolve().parents[3]/
        'docs/ECMA262/evidence/retained-receipt-37715977350.json').read_bytes())
    require(report['preservation_receipt'] == public_receipt['source_receipt']
            and report['retained_objects'] == public_receipt['historical_objects'],
            'Historical receipt projection differs')
    objects = {r['artifact_id']: r for r in report['retained_objects']}
    require(set(objects) == set(pins) and len(objects) == len(report['retained_objects']),
            'Historical retained object coverage differs')
    for aid, expected in pins.items():
        require(objects[aid]['sha256'] == expected and archives[aid]['zip_sha256'] == expected,
                'Historical ZIP digest differs')
    for aid, archive in archives.items():
        names = [m['name'] for m in archive['members']]
        require(len(names) == len(set(names)) and all(MEMBER.fullmatch(n) for n in names)
                and sha(encoded(archive['members'])) == archive['member_index_sha256'],
                'Member index changed or contains unsafe names')
        classification = (AUXILIARY[aid] if aid in AUXILIARY else 'original_history' if aid == config['history_archive_id']
                          else 'import_recovery' if 'import_report' in archive else 'planning_only')
        require(archive['classification'] == classification
                and archive['required_for_scope'] == (classification != 'planning_only')
                and archive['retained_object_artifact_id'] == (aid if aid in objects else None),
                'Archive retention classification differs')
        if aid not in AUXILIARY:
            run = runs[archive['run_id']]
            require(aid in run['artifact_ids'] and archive['source_revision'] == run['source_revision'],
                    'Archive source run attribution differs')
        if 'import_report' in archive:
            imported = archive['import_report']
            selected = imported['selected_artifact_ids']
            started = [s['artifact_id'] for s in imported['snapshots']]
            require('import-report.json' in names
                    and len(selected) == len(set(selected)) and len(started) == len(set(started))
                    and set(started) <= set(selected)
                    and set(selected) <= {s['artifact_id'] for s in history['artifacts']}
                    and all(s['state'] in ('verified', 'uploading') for s in imported['snapshots']),
                    'Import report member or selection differs')
            if imported['complete']:
                require(set(started) == set(selected)
                        and all(s['state'] == 'verified' for s in imported['snapshots']),
                        'Successful report has an incomplete selection')
            require({f'{i}-outbox.sqlite' for i in started}
                    == {n for n in names if n.endswith('-outbox.sqlite')},
                    'Started snapshot/outbox coverage differs')
            if 'auto_plan' in archive:
                require(archive['auto_plan']['state'] == 'ready'
                        and archive['auto_plan']['selected_artifact_ids'] == selected,
                        'Report and controller selection differ')
        elif classification == 'planning_only':
            require(names == ['auto-plan.json'] and archive['auto_plan']['state'] != 'ready',
                    'Planning-only artifact contains import evidence')
    expected_names = {'history-manifest.json'}
    for source in history['artifacts']:
        expected_names.add(f"{source['artifact_id']}/source.zip")
        for db in source['databases']:
            expected_names.add(f"{source['artifact_id']}/{Path(db['path']).name}")
    require({m['name'] for m in archives[config['history_archive_id']]['members']} == expected_names,
            'Original source ZIP/SQLite membership coverage differs')
    verified_sources = {s['artifact_id'] for a in archives.values() if 'import_report' in a
                        for s in a['import_report']['snapshots'] if s['state'] == 'verified'}
    require(verified_sources == {s['artifact_id'] for s in history['artifacts']},
            'A recoverable original source has no verified report reference')
    fresh = report['reused_readback']
    proof = fresh['object']
    historical = objects[proof['artifact_id']]
    require(all(fresh[k] == value for k, value in FRESH.items())
            and sha(encoded(fresh['report'])) == FRESH['report_sha256']
            and fresh['checked_at'] == fresh['report']['checked_at']
            and all(proof[k] == fresh['report']['object'][k] for k in proof)
            and proof['sha256'] == historical['sha256'] and proof['version_id'] == historical['version_id']
            and proof['lock_mode'] == 'COMPLIANCE' and proof['encryption'] == 'aws:kms'
            and proof['retention_verified'] is True, 'Reused readback identity differs')
    known = set(archives) | {s['artifact_id'] for s in history['artifacts']} | {
        report['preservation_receipt']['artifact_id'], FRESH['artifact_id']}
    require(set(report['issue_referenced_artifact_ids']) <= known, 'Issue-linked artifact not inventoried')
    require(report['summary'] == summary(report), 'Inventory summary differs from rows')


def collect(fetch=github):
    config, pins, _, _ = load_manifest(DEFAULT_MANIFEST)
    receipt_pins = json.loads(PINS.read_bytes())
    _, receipt = inspect_receipt(fetch_receipt(receipt_pins, fetch), receipt_pins)
    original_id = config['history_archive_id']
    original = inspect_archive(fetch(f'{BASE}/actions/artifacts/{original_id}'),
                               manifest_sha=config['history_manifest_sha256'])
    history = original.pop('history_manifest')
    runs = paged(f'{BASE}/actions/workflows/{WORKFLOW}/runs?created={WINDOW}',
                 'workflow_runs', fetch, 200)
    # These are the entire GitHub-listed historical workflow, including later guard skips.
    total = fetch(f'{BASE}/actions/workflows/{WORKFLOW}/runs?per_page=1')['total_count']
    require(total == len(runs), 'Workflow runs outside the frozen inventory window require review')
    public_runs, archives = [], {original_id: original}
    for run in sorted(runs, key=lambda row: row['id']):
        artifacts = paged(f"{BASE}/actions/runs/{run['id']}/artifacts", 'artifacts', fetch, 100)
        public = {'run_id': run['id'], 'run_attempt': run['run_attempt'], 'event': run['event'],
                  'conclusion': run['conclusion'], 'source_revision': run['head_sha'],
                  'created_at': run['created_at'], 'artifact_ids': sorted(a['id'] for a in artifacts)}
        require(run['status'] == 'completed' and run['run_attempt'] == 1
                and run['path'] == '.github/workflows/'+WORKFLOW, 'Run/attempt scope requires review')
        for meta in artifacts:
            require(meta['workflow_run']['id'] == run['id']
                    and meta['workflow_run']['head_sha'] == run['head_sha'],
                    'Artifact workflow attribution differs')
            if meta['id'] not in archives:
                archives[meta['id']] = inspect_archive(meta, history, config['history_manifest_sha256'])
        if not artifacts and run['conclusion'] != 'skipped':
            jobs = paged(f"{BASE}/actions/runs/{run['id']}/jobs?filter=all", 'jobs', fetch, 100)
            public['job_evidence'] = [{'name': j['name'], 'conclusion': j['conclusion'],
                                      'recorded_steps': len(j.get('steps', [])),
                                      'import_step': next((s['conclusion'] for s in j.get('steps', [])
                                                          if 'import selected immutable snapshots' in s['name']),
                                                         'not_recorded')}
                                     for j in jobs if j['name'] == 'import']
        public_runs.append(public)
    for aid in AUXILIARY:
        try:
            meta = fetch(f'{BASE}/actions/artifacts/{aid}')
        except subprocess.CalledProcessError as error:
            # A freshly read-back retained version covers this known unavailable Actions artifact.
            require(aid == 11522307991 and json.loads(error.output)['status'] == '404',
                    'Unexpected artifact metadata failure')
            archives[aid] = {'artifact_id': aid, 'zip_sha256': pins[aid],
                             'actions_state': 'unavailable_http_404', 'members': [],
                             'member_index_sha256': sha(encoded([])),
                             'range_bytes_downloaded': 0, 'whole_zip_digest_verified': False}
        else:
            archives[aid] = inspect_archive(meta)
    for aid, archive in archives.items():
        archive['classification'] = (AUXILIARY[aid] if aid in AUXILIARY else 'original_history' if aid == original_id
                                     else 'import_recovery' if 'import_report' in archive else 'planning_only')
        archive['required_for_scope'] = archive['classification'] != 'planning_only'
        archive['retained_object_artifact_id'] = aid if aid in pins else None
    issue = fetch(f'{BASE}/issues/2242')
    comments = fetch(f'{BASE}/issues/2242/comments?per_page=100')
    require(len(comments) < 100, 'Issue comments require additional pagination')
    bodies = '\n'.join([issue['body'], *(c['body'] for c in comments)])
    referenced = sorted(set(map(int, re.findall(r'/artifacts/([0-9]+)', bodies))))
    report = {
        'schema_version': 1, 'repository': REPOSITORY,
        'checked_at': datetime.now(timezone.utc).isoformat(),
        'collector_sha256': sha(Path(__file__).read_bytes()),
        'scope': 'Frozen original discovery, all GitHub-listed import attempts, and eight reviewed cutover archives',
        'history_complete': False, 'full_archive_coverage_verified': False,
        'database_access': False, 'object_store_access': False,
        'workflow_query': {'workflow': WORKFLOW, 'created': WINDOW, 'total_count': total},
        'original_history': {'container_artifact_id': original_id,
                             'manifest_sha256': config['history_manifest_sha256'], 'manifest': history},
        'preservation_receipt': receipt['source_receipt'],
        'retained_objects': receipt['historical_objects'],
        'reused_readback': fresh_receipt(fetch),
        'issue_referenced_artifact_ids': referenced,
        'workflow_runs': public_runs,
        'archives': sorted(archives.values(), key=lambda row: row['artifact_id']),
        'limits': [
            'Original manifest bytes are hash-verified; ZIP indexes and small reports are inspected through bounded GitHub ranges.',
            'whole_zip_digest_verified refers only to GitHub bytes fetched during inventory, never a fresh S3 verification.',
            'Source ZIP/SQLite digests are original manifest pins; outbox payloads and acknowledgements are not rechecked here.',
            'An identified historical retained container is not a fresh readback of every member or ongoing retention.',
            'retained_object_artifact_id=null means no independent retained-object proof was identified, not irretrievable loss.',
            'Planning-only archives contain no import report/outbox and are excluded from required original/import/outbox scope.',
            'Unsuccessful invocations without recovery artifacts are not new missing-original-archive gaps.',
            'The raw preservation receipt has no recorded self-version proof; its sanitized Git projection is durable, not its private raw bytes.',
            'The single fresh readback is reused from its recorded run; this inventory performs no S3 access, upload or fixture execution.'],
    }
    report['summary'] = summary(report)
    validate(report)
    return report


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    mode = parser.add_mutually_exclusive_group(required=True)
    mode.add_argument('--collect', action='store_true', help='GitHub metadata, bounded ZIP ranges and small receipts only')
    mode.add_argument('--verify', type=Path, help='Verify the committed ledger offline, without credentials')
    parser.add_argument('--output', type=Path)
    args = parser.parse_args(argv)
    if args.verify:
        require(args.output is None, 'Offline verification does not write a replacement ledger')
        report = json.loads(args.verify.read_bytes())
        validate(report)
    else:
        require(args.output is not None and not args.output.exists(), 'Collection needs a new output path')
        report = collect()
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_bytes(encoded(report))
    print(json.dumps(report['summary'], sort_keys=True))


if __name__ == '__main__':
    main()
