"""Verify an existing pinned archive version without uploads, imports or fixture execution."""
import argparse
from datetime import datetime, timezone
import hashlib
import io
import json
import os
from pathlib import Path
import zipfile

from .cutover_evidence import DEFAULT_MANIFEST, REPOSITORY, load_manifest
from .monitoring import github
from .reconciliation import decode
from .retained_objects import destination, public_proof, verify_bucket, verify_object

PINS = Path(__file__).with_name('retained-evidence-receipt.json')


def require(condition, message):
    if not condition:
        raise ValueError(message)


def fetch_receipt(pins, fetch=github):
    base = f'repos/{REPOSITORY}/actions'
    run = fetch(f"{base}/runs/{pins['run_id']}")
    require(run['head_sha'] == pins['source_revision'] and run['run_attempt'] == pins['run_attempt']
            and run['head_branch'] == 'master' and run['conclusion'] == 'success'
            and run['status'] == 'completed'
            and run['path'] == '.github/workflows/test262-cutover-evidence.yml',
            'Source run provenance differs')
    meta = fetch(f"{base}/artifacts/{pins['artifact_id']}")
    require(meta['id'] == pins['artifact_id'] and meta.get('expired') is False
            and meta['size_in_bytes'] <= 2_000_000
            and meta['digest'] == 'sha256:'+pins['zip_sha256']
            and meta['workflow_run']['id'] == pins['run_id']
            and meta['workflow_run']['head_sha'] == pins['source_revision'],
            'Source receipt metadata differs')
    return fetch(f"{base}/artifacts/{pins['artifact_id']}/zip", binary=True)


def inspect_receipt(raw, pins, manifest=DEFAULT_MANIFEST):
    require(pins['schema_version'] == 1 and pins['repository'] == REPOSITORY,
            'Unsupported receipt scope')
    require(len(raw) <= 2_000_000 and hashlib.sha256(raw).hexdigest() == pins['zip_sha256'],
            'Original receipt ZIP digest differs')
    with zipfile.ZipFile(io.BytesIO(raw)) as archive:
        files = archive.infolist()
        require(len(files) == 1 and files[0].filename == 'evidence-report.json'
                and files[0].file_size <= 262144, 'Unexpected receipt member')
        encoded = archive.read(files[0])
    require(hashlib.sha256(encoded).hexdigest() == pins['report_sha256'],
            'Original report bytes differ')
    receipt = decode(encoded)
    require(receipt['complete'] is True and receipt['stage'] == 'complete'
            and receipt['history_complete'] is False and receipt['production_changed'] is False
            and receipt['checked_at'] == pins['source_checked_at']
            and receipt['missing_artifact_gaps'] == pins['missing_artifact_gaps']
            and receipt['final_legacy_checkpoint_disposition'] == 'retained-only, not imported',
            'Historical receipt did not verify the expected scope')
    _, artifacts, expected, manifest_sha = load_manifest(manifest)
    objects = receipt['preserved_objects']
    require(len(objects) == len(artifacts)
            and {row['artifact_id'] for row in objects} == set(artifacts),
            'Historical retained object coverage differs')
    for row in objects:
        until = datetime.fromisoformat(row['retain_until'])
        require(row['sha256'] == artifacts[row['artifact_id']]
                and isinstance(row['version_id'], str) and row['version_id'] not in ('', 'null')
                and until.tzinfo is not None
                and until > datetime.fromisoformat(pins['source_checked_at']),
                'Invalid historical object proof')
    snapshots = receipt['verified_snapshots']
    require(len(snapshots) == len(expected)
            and {r['artifact_id'] for r in snapshots} == set(expected),
            'Historical source coverage differs')
    for row in snapshots:
        outbox = row['outbox']
        require(row['observations'] == expected[row['artifact_id']]
                and outbox['messages_observations'] == row['observations']
                and outbox['pending'] == 0 and outbox['integrity'] == 'ok',
                'Historical outbox verification differs')
    return receipt, {
        'schema_version': 1, 'repository': REPOSITORY, 'scope': pins['scope'],
        'historical_receipt_verified': True, 'selected_object_verified': False,
        'full_archive_coverage_verified': False, 'history_complete': False,
        'missing_artifact_gaps': receipt['missing_artifact_gaps'],
        'source_receipt': {key: pins[key] for key in
            ('run_id', 'run_attempt', 'source_revision', 'artifact_id', 'zip_sha256',
             'report_sha256', 'source_checked_at')},
        'manifest_sha256': manifest_sha,
        'historical_objects': [dict(public_proof(row), artifact_id=row['artifact_id']) for row in objects],
        'verified_sources': [{'artifact_id': row['artifact_id'], 'observations': row['observations'],
                             'pending': row['outbox']['pending']} for row in snapshots],
        'limitations': [
            'Historical receipt inspection is not a fresh retained-object readback.',
            'The legacy receipt omits content lengths and explicit KMS/lock flags; fresh checks must query S3.',
            'The selected eight objects are not the complete original/report/failed-outbox inventory.',
            'Missing history remains incomplete; no import, fixture execution, upload or authority change is performed.']}


def verify_selected(s3, uri, kms_key, receipt, pins, artifact_id):
    objects = [row for row in receipt['preserved_objects'] if row['artifact_id'] == artifact_id]
    require(len(objects) == 1, 'Requested artifact is outside reviewed receipt scope')
    row = objects[0]
    bucket, base = destination(uri)
    key = '/'.join(filter(None, (base, 'cutover-evidence',
        str(pins['run_id'])+'-'+str(pins['run_attempt']), str(artifact_id)+'.zip')))
    require(row['key'] == key, 'Retained object location differs from configured scope')
    verify_bucket(s3, bucket)
    # Legacy receipts omit lengths; read metadata for the pinned version, never latest.
    head = s3.head_object(Bucket=bucket, Key=key, VersionId=row['version_id'])
    proof = verify_object(s3, bucket, key, kms_key, pinned={
        'version_id': row['version_id'], 'sha256': row['sha256'],
        'content_length': head['ContentLength']})
    require(datetime.fromisoformat(proof['retain_until']) >= datetime.fromisoformat(row['retain_until']),
            'Recorded immutable retention was shortened')
    return dict(public_proof(proof), artifact_id=artifact_id)


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--receipt-zip', type=Path, help='Offline original pinned receipt ZIP')
    parser.add_argument('--receipt-only', action='store_true', help='Inspect original receipt, without S3 access')
    parser.add_argument('--artifact-id', type=int, default=11522307991)
    parser.add_argument('--output', required=True, type=Path)
    args = parser.parse_args(argv)
    require(args.output.resolve() not in {PINS.resolve(), DEFAULT_MANIFEST.resolve(),
            args.receipt_zip.resolve() if args.receipt_zip else None}, 'Output must not overwrite inputs')
    report = {'historical_receipt_verified': False, 'selected_object_verified': False,
              'history_complete': False, 'full_archive_coverage_verified': False}
    stage = 'receipt'
    status = 1
    try:
        pins = decode(PINS.read_bytes())
        if args.receipt_zip:
            with args.receipt_zip.open('rb') as stream:
                raw = stream.read(2_000_001)
        else:
            raw = fetch_receipt(pins)
        receipt, report = inspect_receipt(raw, pins)
        report['source_metadata_verified'] = args.receipt_zip is None
        report['mode'] = 'receipt-only' if args.receipt_only else 'verify-retained'
        if not args.receipt_only:
            stage = 'retained-object-readback'
            import boto3
            uri, kms_key = os.environ['TEST262_BACKUP_S3_URI'], os.environ['TEST262_BACKUP_KMS_KEY']
            report['object'] = verify_selected(boto3.client('s3'),
                uri, kms_key, receipt, pins, args.artifact_id)
            report['selected_object_verified'] = True
        status = 0
    except Exception as error:
        report.update(error_class=type(error).__name__, failed_stage=stage)
        response = getattr(error, 'response', {})
        if isinstance(response, dict):
            report['aws_error_code'] = response.get('Error', {}).get('Code')
    finally:
        report['checked_at'] = datetime.now(timezone.utc).isoformat()
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(json.dumps(report, indent=2)+'\n')
    print(json.dumps({key: report[key] for key in
                     ('historical_receipt_verified', 'selected_object_verified')}))
    return status


if __name__ == '__main__':
    raise SystemExit(main())
