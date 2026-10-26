"""Preserve the 33 reviewed recovery ZIPs verbatim; never import or repair their outboxes."""
import argparse
from datetime import datetime, timezone
import hashlib
import json
import os
from pathlib import Path
import re
import subprocess
import tempfile
import zipfile

from .archive_inventory import encoded, members, sha, small_json, validate
from .cutover_evidence import REPOSITORY
from .monitoring import github
from .retained_discovery import failure
from .retained_evidence import require
from .retained_objects import destination, public_proof, verify_bucket, verify_object

ROOT = Path(__file__).resolve().parents[3]
INVENTORY = ROOT/'docs/ECMA262/evidence/historical-archive-inventory-2242.json'
INVENTORY_SHA256 = 'ec75aea22258310fbb80f33b5c2f5c853e8be4575bf5efee565ca9ab90268bb7'
AUTHORIZATION = 'preserve-33-recovery-archives'


def reviewed_sources(raw):
    require(sha(raw) == INVENTORY_SHA256, 'Recovery preservation inventory requires renewed review')
    inventory = json.loads(raw)
    validate(inventory)
    rows = [a for a in inventory['archives']
            if a['required_for_scope'] and a['retained_object_artifact_id'] is None]
    require(len(rows) == 33 and all(a['classification'] == 'import_recovery' for a in rows),
            'Recovery preservation scope differs from the 33 authorized archives')
    return rows


def initial_report(raw):
    rows = reviewed_sources(raw)
    return {
        'schema_version': 1, 'repository': REPOSITORY, 'mode': 'preserve-recovery',
        'scope': 'Verbatim independent preservation of 33 frozen unmatched import/recovery ZIPs',
        'inventory_sha256': sha(raw), 'complete': False,
        'archive_preservation_complete': False, 'receipt_verified': False,
        'full_archive_coverage_verified': False, 'history_complete': False,
        'database_access': False, 'policy_changes': False, 'imports': False,
        'restores': False, 'fixture_execution': False, 'production_changed': False,
        'object_store_writes_attempted': False, 'objects_uploaded': 0, 'stage': 'scope',
        'archives': [{
            'artifact_id': a['artifact_id'], 'source_run_id': a['run_id'],
            'source_revision': a['source_revision'], 'expected_zip_sha256': a['zip_sha256'],
            'expected_size_in_bytes': a['size_in_bytes'],
            'original_import_report_complete': a['import_report']['complete'],
            'state': 'not_assessed',
        } for a in rows],
        'limits': [
            'Only the 33 frozen recovery containers are preserved, not newly selected sources or other storage locations.',
            'Original ZIP bytes, failed/partial reports, outboxes and WAL/SHM sidecars are retained without modification.',
            'ZIP preservation does not establish successful imports, new acknowledgements, SQLite integrity or fresh fixture acceptance.',
            'The eight earlier object proofs and eleven original source gaps remain separate and unchanged.',
            'Full inventory coverage and the original private receipt self-retention require a separate ledger reconciliation.',
            'New versions use the existing finite COMPLIANCE policy; no policy, retention setting or existing object is changed.',
            'The retained_report is the exact pre-receipt document; the outer receipt proves its bytes and stored version.',
            'A failed write may have reached S3 even without a response; attempted writes must be reconciled before retrying.',
        ],
    }


def source_metadata(source, metadata, run):
    workflow = metadata.get('workflow_run', {})
    require(metadata.get('id') == source['artifact_id'] and metadata.get('expired') is False
            and metadata.get('digest') == 'sha256:'+source['zip_sha256']
            and metadata.get('size_in_bytes') == source['size_in_bytes']
            and workflow.get('id') == source['run_id']
            and workflow.get('head_sha') == source['source_revision']
            and workflow.get('head_branch') == 'master'
            and run.get('id') == source['run_id'] and run.get('run_attempt') == 1
            and run.get('head_sha') == source['source_revision']
            and run.get('head_branch') == 'master' and run.get('status') == 'completed'
            and run.get('path') == '.github/workflows/test262-history-import.yml',
            'Recovery source metadata differs from frozen pins')


def download_zip(artifact_id, path):
    with Path(path).open('wb') as target:
        subprocess.run(['gh', 'api', f'repos/{REPOSITORY}/actions/artifacts/{artifact_id}/zip'],
                       stdout=target, stderr=subprocess.DEVNULL, timeout=180, check=True)


def verify_zip(path, source):
    require(path.stat().st_size == source['size_in_bytes'], 'Original recovery ZIP length differs')
    with path.open('rb') as stream:
        require(hashlib.file_digest(stream, 'sha256').hexdigest() == source['zip_sha256'],
                'Original recovery ZIP SHA-256 differs')
    with zipfile.ZipFile(path) as archive:
        index = members(archive)
        require(index == source['members'] and sha(encoded(index)) == source['member_index_sha256'],
                'Original recovery ZIP membership differs')
        imported, digest = small_json(archive, 'import-report.json')
        require(digest == source['import_report']['sha256']
                and imported.get('complete') is source['import_report']['complete'],
                'Original import report bytes or completion state differ')


def upload_exact(s3, bucket, key, kms, path, pinned_sha, pinned_size, record):
    record['upload_attempted'] = True
    with path.open('rb') as stream:
        response = s3.put_object(Bucket=bucket, Key=key, Body=stream,
                                 ServerSideEncryption='aws:kms', SSEKMSKeyId=kms)
    record.update(upload_completed=True, state='uploaded_pending_readback')
    version = response.get('VersionId')
    require(isinstance(version, str) and version not in ('', 'null'),
            'Uploaded recovery evidence lacks an immutable version')
    record['uploaded_version_id'] = version
    proof = verify_object(s3, bucket, key, kms, pinned={
        'version_id': version, 'sha256': pinned_sha, 'content_length': pinned_size})
    return public_proof(proof)


def preserve(s3, uri, kms, report, *, fetch=github, download=download_zip):
    sources = {a['artifact_id']: a for a in reviewed_sources(INVENTORY.read_bytes())}
    rows = report['archives']
    require(report['inventory_sha256'] == INVENTORY_SHA256 and len(rows) == 33
            and {r['artifact_id'] for r in rows} == sources.keys()
            and all(r['state'] == 'not_assessed' for r in rows),
            'Preservation report selection differs or was already attempted')
    for row in rows:
        source = sources[row['artifact_id']]
        require(row['expected_zip_sha256'] == source['zip_sha256']
                and row['expected_size_in_bytes'] == source['size_in_bytes']
                and row['source_run_id'] == source['run_id']
                and row['source_revision'] == source['source_revision']
                and row['original_import_report_complete'] is source['import_report']['complete'],
                'Preservation report source pins differ')
    require(type(report['run_id']) is int and report['run_id'] > 0
            and type(report['run_attempt']) is int and report['run_attempt'] > 0,
            'Invalid preservation run identity')
    bucket, base = destination(uri)
    prefix = '/'.join(filter(None, (base, 'cutover-evidence',
                                   f"{report['run_id']}-{report['run_attempt']}")))
    report['stage'] = 'metadata'
    runs = {}
    for row in rows:
        source = sources[row['artifact_id']]
        metadata = fetch(f"repos/{REPOSITORY}/actions/artifacts/{row['artifact_id']}")
        run_id = source['run_id']
        if run_id not in runs:
            runs[run_id] = fetch(f'repos/{REPOSITORY}/actions/runs/{run_id}')
        source_metadata(source, metadata, runs[run_id])
        row['state'] = 'metadata_verified'
    # Authenticate the whole selected set before the first S3 write.
    with tempfile.TemporaryDirectory() as temporary:
        directory = Path(temporary)
        report['stage'] = 'download'
        for row in rows:
            path = directory/f"{row['artifact_id']}.zip"
            download(row['artifact_id'], path)
            verify_zip(path, sources[row['artifact_id']])
            row['state'] = 'source_bytes_verified'
        report['stage'] = 'bucket'
        verify_bucket(s3, bucket)
        report['stage'] = 'upload'
        for row in rows:
            aid = row['artifact_id']
            report['object_store_writes_attempted'] = True
            try:
                row['object'] = upload_exact(s3, bucket, f'{prefix}/{aid}.zip', kms,
                                             directory/f'{aid}.zip', row['expected_zip_sha256'],
                                             row['expected_size_in_bytes'], row)
            finally:
                report['objects_uploaded'] = sum(r.get('upload_completed') is True for r in rows)
            row.update(state='retained_verified', checked_at=datetime.now(timezone.utc).isoformat())
        report['archive_preservation_complete'] = True
        report['stage'] = 'receipt'
        report['checked_at'] = datetime.now(timezone.utc).isoformat()
        raw = encoded(report)
        path = directory/'recovery-preservation-report.json'
        path.write_bytes(raw)
        report['retained_report'] = json.loads(raw)
        report['receipt_upload'] = {}
        report['receipt'] = upload_exact(s3, bucket, f'{prefix}/{path.name}', kms, path,
                                        sha(raw), len(raw), report['receipt_upload'])
        report['receipt_upload']['state'] = 'retained_verified'
        report.update(receipt_verified=True, complete=True, stage='complete')


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--authorization', required=True)
    parser.add_argument('--output', required=True, type=Path)
    args = parser.parse_args(argv)
    require(not args.output.exists(), 'Preservation must not overwrite an existing report')
    report = {'schema_version': 1, 'mode': 'preserve-recovery', 'complete': False,
              'archive_preservation_complete': False, 'receipt_verified': False,
              'history_complete': False, 'full_archive_coverage_verified': False, 'stage': 'scope'}
    status = 1
    try:
        require(args.authorization == AUTHORIZATION, 'Explicit recovery preservation authorization required')
        require(os.environ.get('GITHUB_REF') == 'refs/heads/master', 'Preservation requires merged master')
        revision = os.environ.get('GITHUB_SHA', '')
        run, attempt = os.environ.get('GITHUB_RUN_ID', ''), os.environ.get('GITHUB_RUN_ATTEMPT', '')
        require(re.fullmatch(r'[0-9a-f]{40}', revision) is not None
                and re.fullmatch(r'[1-9][0-9]*', run) is not None
                and re.fullmatch(r'[1-9][0-9]*', attempt) is not None,
                'Invalid preservation source/run provenance')
        report = initial_report(INVENTORY.read_bytes())
        report.update(source_revision=revision, run_id=int(run), run_attempt=int(attempt))
        import boto3
        from botocore.config import Config
        preserve(boto3.client('s3', config=Config(retries={'total_max_attempts': 1})),
                 os.environ['TEST262_BACKUP_S3_URI'],
                 os.environ['TEST262_BACKUP_KMS_KEY'], report)
        status = 0
    except Exception as error:
        report.update(failed_stage=report['stage'], **failure(error))
        report['complete'] = False
    finally:
        report['checked_at'] = datetime.now(timezone.utc).isoformat()
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_bytes(encoded(report))
    print(json.dumps({'complete': report['complete'],
                      'archive_preservation_complete': report['archive_preservation_complete']}))
    return status


if __name__ == '__main__':
    raise SystemExit(main())
