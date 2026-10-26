"""Find candidate versions for unmatched #2242 archives; metadata only, no payload reads."""
import argparse
from datetime import datetime, timezone
import json
import os
from pathlib import Path
import re

from .archive_inventory import encoded, sha, validate
from .retained_evidence import require
from .retained_objects import destination

ROOT = Path(__file__).resolve().parents[3]
INVENTORY = ROOT/'docs/ECMA262/evidence/historical-archive-inventory-2242.json'
PAGE_SIZE = 100
MAX_PAGES = 10
MAX_CANDIDATES = 100
LOCATION = re.compile(r'([1-9][0-9]*)-([1-9][0-9]*)/([1-9][0-9]*)\.zip')


def initial_report(raw):
    inventory = json.loads(raw)
    validate(inventory)
    rows = [a for a in inventory['archives']
            if a['required_for_scope'] and a['retained_object_artifact_id'] is None]
    require(rows and all(a['classification'] == 'import_recovery' for a in rows),
            'Unmatched archive scope requires review')
    return {
        'schema_version': 1, 'repository': inventory['repository'],
        'mode': 'discover-retained', 'scope': 'Unmatched import/recovery archives under configured cutover-evidence prefix only',
        'inventory_sha256': sha(raw), 'listing_complete': False, 'discovery_complete': False,
        'full_archive_coverage_verified': False, 'history_complete': False,
        'payload_reads': False, 'object_store_changes': False, 'database_access': False,
        'pages_read': 0, 'entries_scanned': 0, 'delete_markers_scanned': 0,
        'out_of_layout_entries': 0,
        'archives': [{'artifact_id': a['artifact_id'], 'expected_zip_sha256': a['zip_sha256'],
                      'expected_size_in_bytes': a['size_in_bytes'], 'state': 'not_assessed',
                      'candidates': []} for a in rows],
        'limits': [
            'Only the configured cutover-evidence prefix and reviewed run-attempt/artifact.zip layout are searched.',
            'A completed bounded listing is not an atomic bucket snapshot or a search of other prefixes, buckets or providers.',
            'No candidate in this scope is not proof of irretrievable loss or absence of another retained copy.',
            'Candidate identity, length, encryption and lock metadata do not establish payload SHA-256 or independent preservation.',
            'Expected ZIP hashes are source inventory pins, never hashes of discovered object bytes.',
            'ETags, S3 checksum metadata and filenames are not substituted for a full exact-version payload readback.',
            'No upload, decrypt, payload download, retention extension, policy change, import or fixture execution is performed.'],
    }


def failure(error):
    result = {'error_class': type(error).__name__}
    response = getattr(error, 'response', {})
    code = response.get('Error', {}).get('Code') if isinstance(response, dict) else None
    if isinstance(code, str) and re.fullmatch(r'[A-Za-z0-9_.-]{1,80}', code):
        result['aws_error_code'] = code
    return result


def discover(s3, uri, kms, report):
    bucket, base = destination(uri)
    prefix = '/'.join(filter(None, (base, 'cutover-evidence')))+'/'
    rows = {a['artifact_id']: a for a in report['archives']}
    token = {}
    seen_tokens, seen_versions = set(), set()
    candidates = []
    for _ in range(MAX_PAGES):
        page = s3.list_object_versions(Bucket=bucket, Prefix=prefix, MaxKeys=PAGE_SIZE, **token)
        versions, deleted = page.get('Versions', []), page.get('DeleteMarkers', [])
        require(len(versions)+len(deleted) <= PAGE_SIZE, 'Listing page exceeds bound')
        report['pages_read'] += 1
        report['entries_scanned'] += len(versions)+len(deleted)
        report['delete_markers_scanned'] += len(deleted)
        for item in [*versions, *deleted]:
            require(isinstance(item.get('Key'), str) and item['Key'].startswith(prefix),
                    'Listing returned an entry outside requested prefix')
            relative = item['Key'][len(prefix):]
            match = LOCATION.fullmatch(relative)
            if match is None:
                report['out_of_layout_entries'] += 1
                continue
            if item in deleted:
                continue
            aid = int(match[3])
            if aid not in rows:
                continue
            version = item.get('VersionId')
            require(isinstance(version, str) and version not in ('', 'null')
                    and type(item.get('Size')) is int and item['Size'] >= 0,
                    'Candidate version metadata is invalid')
            identity = (item['Key'], version)
            require(identity not in seen_versions, 'Repeated version in listing')
            seen_versions.add(identity)
            require(len(candidates) < MAX_CANDIDATES, 'Candidate inspection bound exceeded')
            candidates.append((aid, item, int(match[1]), int(match[2])))
        require(type(page.get('IsTruncated')) is bool, 'Listing completion state is missing')
        if not page['IsTruncated']:
            report['listing_complete'] = True
            break
        markers = (page.get('NextKeyMarker'), page.get('NextVersionIdMarker'))
        require(isinstance(markers[0], str) and markers[0].startswith(prefix)
                and isinstance(markers[1], str) and markers[1] and markers not in seen_tokens,
                'Missing, repeated or out-of-scope listing continuation')
        seen_tokens.add(markers)
        token = {'KeyMarker': markers[0], 'VersionIdMarker': markers[1]}
    require(report['listing_complete'], 'Listing page budget exhausted')
    errors = False
    for aid, item, run_id, attempt in candidates:
        row = rows[aid]
        candidate = {
            'preservation_run_id': run_id, 'preservation_run_attempt': attempt,
            'version_id': item['VersionId'], 'listed_size_in_bytes': item['Size'],
            'payload_digest_verified': False, 'metadata_checked': False,
        }
        row['candidates'].append(candidate)
        try:
            request = {'Bucket': bucket, 'Key': item['Key'], 'VersionId': item['VersionId']}
            head = s3.head_object(**request)
            require(head['VersionId'] == item['VersionId'] and head['ContentLength'] == item['Size'],
                    'Candidate metadata changed between listing and head')
            retention = s3.get_object_retention(**request).get('Retention', {})
            until = retention.get('RetainUntilDate')
            require(until is None or (isinstance(until, datetime) and until.tzinfo is not None),
                    'Invalid retention timestamp')
            encrypted = head.get('ServerSideEncryption') == 'aws:kms' and head.get('SSEKMSKeyId') == kms
            protected = (retention.get('Mode') == 'COMPLIANCE' and until is not None
                         and until > datetime.now(timezone.utc))
            candidate.update(
                metadata_checked=True, length_matches_inventory=head['ContentLength'] == row['expected_size_in_bytes'],
                configured_kms_matches=encrypted, current_compliance_lock=protected,
                metadata_candidate_qualifies=encrypted and protected and head['ContentLength'] == row['expected_size_in_bytes'],
                checked_at=datetime.now(timezone.utc).isoformat())
            if until is not None:
                candidate['retain_until'] = until.isoformat()
        except Exception as error:
            candidate.update(failure(error))
            errors = True
    for row in rows.values():
        row['state'] = ('no_candidate_in_scoped_listing' if not row['candidates']
                        else 'candidate_metadata_error' if any(not c['metadata_checked'] for c in row['candidates'])
                        else 'candidate_versions_require_payload_verification')
    report['discovery_complete'] = not errors
    require(not errors, 'One or more candidate metadata checks failed')


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output', required=True, type=Path)
    args = parser.parse_args(argv)
    require(not args.output.exists(), 'Discovery must not overwrite an existing report')
    report = {'mode': 'discover-retained', 'discovery_complete': False, 'listing_complete': False,
              'history_complete': False, 'full_archive_coverage_verified': False}
    status = 1
    try:
        report = initial_report(INVENTORY.read_bytes())
        import boto3
        discover(boto3.client('s3'), os.environ['TEST262_BACKUP_S3_URI'],
                 os.environ['TEST262_BACKUP_KMS_KEY'], report)
        status = 0
    except Exception as error:
        report.update(failure(error))
    finally:
        report['checked_at'] = datetime.now(timezone.utc).isoformat()
        report['source_revision'] = os.environ.get('GITHUB_SHA', 'local')
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_bytes(encoded(report))
    print(json.dumps({'discovery_complete': report['discovery_complete'],
                      'listing_complete': report['listing_complete']}))
    return status


if __name__ == '__main__':
    raise SystemExit(main())
