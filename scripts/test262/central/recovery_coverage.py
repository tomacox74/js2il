"""Read-only backup/restore coverage and accepted historical retention deadlines.

No database, object-store access, downloads of original archives, fixture execution,
notification delivery or policy adoption. Unknown independently retained coverage
never becomes a successful preservation claim.
"""
import argparse
from datetime import datetime, timezone
import json
import hashlib
from pathlib import Path

from .cutover_evidence import REPOSITORY
from .monitoring import collect_check, github

LEDGER = Path(__file__).resolve().parents[3] / 'docs/ECMA262/evidence/historical-retention-ledger-2242.json'
LEDGER_SHA256 = '5b1f6081dc7b5e61229a1be44bca4afc43889b735d6f5201e32c3022c8132be4'
EXTENSION = LEDGER.with_name('retention-extension-40445723674.json')
EXTENSION_SHA256 = 'facd2a698d64137ca063ffdbf26903096370c2aeecc7f047728f8809853c3a59'


def load_ledger(path=LEDGER):
    raw = Path(path).read_bytes()
    if hashlib.sha256(raw).hexdigest() != LEDGER_SHA256:
        raise ValueError('Accepted historical retention ledger digest differs')
    return json.loads(raw)


def load_retention_extension(ledger, now, path=EXTENSION):
    raw = Path(path).read_bytes()
    if hashlib.sha256(raw).hexdigest() != EXTENSION_SHA256:
        raise ValueError('Recorded retention extension proof differs')
    try:
        return _retention_extension(ledger, now, json.loads(raw))
    except (ValueError, TypeError, KeyError):
        raise ValueError('Recorded retention extension proof differs') from None


def _retention_extension(ledger, now, evidence):
    def require(condition):
        if not condition:
            raise ValueError('Recorded retention extension proof differs')

    def point(value):
        require(isinstance(value, str))
        value = datetime.fromisoformat(value)
        require(value.tzinfo is not None)
        return value

    require(isinstance(evidence, dict) and isinstance(evidence.get('report'), dict))
    report = evidence['report']
    require(evidence['schema_version'] == report['schema_version'] == 1
            and evidence['repository'] == report['repository'] == REPOSITORY
            and evidence['operation'] == report['operation'] == 'extend-retention'
            and evidence['run_conclusion'] == 'success'
            and evidence['run_id'] == report['run_id'] == 40445723674
            and evidence['run_attempt'] == report['run_attempt'] == 1
            and evidence['source_revision'] == report['source_revision']
            == '968756594b68290acadb81f01336cc7c02454b99'
            and report['ledger_sha256'] == LEDGER_SHA256)
    require(all(report[field] is True for field in
                ('complete', 'preflight_complete', 'protection_verified')))
    require(all(report[field] is False for field in (
        'uploads', 'deletions', 'imports', 'restores', 'production_changed',
        'permissions_changed', 'bucket_configuration_changes', 'database_access',
        'fixture_execution', 'payload_reads', 'payload_digest_verified',
        'notifications_enabled', 'retention_policy_adopted')))
    observed = point(report['checked_at'])
    horizon = datetime(2027, 4, 30, tzinfo=timezone.utc)
    require(point(report['target_retain_until']) == horizon and now.tzinfo is not None)
    preservation = LEDGER.with_name('retained-recovery-preservation-40393597860.json').read_bytes()
    require(hashlib.sha256(preservation).hexdigest() == report['preservation_sha256'])
    expected = {('archive', row['artifact_id']): row['object'] for row in ledger['archives']}
    require(len(expected) == len(ledger['archives']) == 41)
    expected[('preservation_receipt', None)] = json.loads(preservation)['report']['receipt']
    rows = report['objects']
    require(isinstance(rows, list) and len(rows) == 42 and all(isinstance(row, dict) for row in rows)
            and len({(row['kind'], row.get('artifact_id')) for row in rows}) == 42
            and report['summary']['target_count'] == report['summary']['target_verified_count'] == 42)
    archives, receipt = {}, None
    for row in rows:
        identity = row['kind'], row.get('artifact_id')
        require(identity in expected)
        pin = expected[identity]
        final = row['final_verification']
        require(isinstance(final, dict))
        require(row['version_id'] == final['version_id'] == pin['version_id']
                and row['expected_sha256'] == final['expected_sha256'] == pin['sha256']
                and final['kind'] == identity[0] and final.get('artifact_id') == identity[1]
                and type(final['content_length']) is int
                and final['content_length'] == pin['content_length']
                and final['encryption'] == 'aws:kms' and final['lock_mode'] == 'COMPLIANCE'
                and final['state'] == 'observed'
                and final['payload_digest_verified'] is False
                and row['target_protection_verified'] is True
                and all(final[field] is True for field in (
                    'metadata_verified', 'version_matches', 'length_matches',
                    'kms_identity_matches', 'prior_lock_not_shortened', 'compliance_lock_current'))
                and point(final['retain_until']) >= max(horizon, point(pin['retain_until']))
                and point(final['retain_until']) > observed)
        proof = {key: final[key] for key in (
            'kind', 'version_id', 'expected_sha256', 'content_length', 'encryption',
            'lock_mode', 'retain_until', 'metadata_verified', 'payload_digest_verified')}
        if identity[0] == 'archive':
            proof['artifact_id'] = identity[1]
            archives[identity[1]] = proof
        else:
            receipt = proof
    applicable = observed <= now
    return {
        'state': 'recorded_verified' if applicable else 'not_yet_observed',
        'checked_at': observed.isoformat(), 'run_id': report['run_id'],
        'source_revision': report['source_revision'], 'evidence_sha256': EXTENSION_SHA256,
        'archive_count': 41 if applicable else 0, 'receipt_count': 1 if applicable else 0,
        'archives': archives if applicable else {}, 'receipt': receipt if applicable else None,
        'fresh_object_store_readback': False}


def assess(checks, now):
    findings = []
    for kind, check in checks.items():
        def add(code, state='attention'):
            findings.append({'kind': kind, 'code': code, 'state': state, 'owner_issue': 2343})
        if check.get('fetch_state') != 'observed' or 'latest_run_id' not in check:
            add('workflow_missing_or_unavailable', 'unknown')
        elif check.get('latest_state') != 'completed':
            add('latest_attempt_incomplete', 'unknown')
        elif check.get('latest_conclusion') != 'success':
            add('latest_attempt_unsuccessful')
        if check.get('receipt_verified') is not True or check.get('retention_verified') is not True:
            add('verified_receipt_missing_expired_or_invalid', 'unknown')
        for field, max_age, code in (
                ('receipt_checked_at', 36*3600 if kind == 'backup' else 8*24*3600,
                 'backup_older_than_monitor_threshold' if kind == 'backup' else 'weekly_drill_overdue'),
                ('retention_until', None, 'recorded_object_lock_expired')):
            if field not in check:
                add(code+'_unknown', 'unknown')
                continue
            point = datetime.fromisoformat(check[field])
            if point.tzinfo is None:
                raise ValueError('Coverage timestamps require timezones')
            seconds = (now-point).total_seconds()
            if (max_age is None and seconds >= 0) or (
                    max_age is not None and (seconds < 0 or seconds > max_age)):
                add(code)
    return findings


def collect(fetch=github, now=None):
    now = now or datetime.now(timezone.utc)
    if now.tzinfo is None:
        raise ValueError('Coverage time requires a timezone')
    ledger = load_ledger()
    checks = {kind: collect_check(REPOSITORY, kind, fetch) for kind in ('backup', 'restore')}
    findings = assess(checks, now)
    try:
        extension = load_retention_extension(ledger, now)
    except (OSError, ValueError, TypeError, KeyError) as error:
        extension = {'state': 'unavailable', 'error_class': type(error).__name__}
    if extension['state'] != 'recorded_verified':
        findings.append({'kind': 'retention-extension', 'code': 'recorded_retention_extension_unavailable',
                         'state': 'unknown', 'owner_issue': 2343})
    historical_findings = []
    archives = []
    for row in ledger['archives']:
        artifact_id = row['artifact_id']
        deadline = datetime.fromisoformat(row['object']['retain_until'])
        expiration = row['actions_expires_at_from_frozen_inventory']
        latest = extension.get('archives', {}).get(artifact_id)
        latest_deadline = datetime.fromisoformat(latest['retain_until']) if latest else deadline
        covers_expiry = (latest_deadline > datetime.fromisoformat(expiration.replace('Z', '+00:00'))
                         if expiration else None)
        archives.append({
            'artifact_id': artifact_id, 'sha256': row['original_zip_sha256'],
            'actions_state': 'not_assessed',
            'independent_archive_state': 'historical_exact_version_proof',
            'recorded_verified_at': row['verification']['checked_at'],
            'recorded_retain_until': deadline.isoformat(),
            'recorded_lock_state': 'deadline_elapsed' if deadline <= now else 'deadline_future',
            'latest_verified_at': extension['checked_at'] if latest else row['verification']['checked_at'],
            'latest_verified_retain_until': latest_deadline.isoformat(),
            'latest_recorded_lock_state': 'deadline_elapsed' if latest_deadline <= now else 'deadline_future',
            'protection_beyond_frozen_actions_expiry': covers_expiry})
        if deadline <= now:
            historical_findings.append({'artifact_id': artifact_id, 'code': 'recorded_archive_lock_expired',
                                        'state': 'attention', 'owner_issue': 2343})
        if not expiration or deadline <= datetime.fromisoformat(expiration.replace('Z', '+00:00')):
            historical_findings.append({
                'artifact_id': artifact_id,
                'code': 'historical_archive_protection_beyond_actions_expiry_unproven',
                'state': 'attention', 'owner_issue': 2343})
        if latest_deadline <= now:
            findings.append({'artifact_id': artifact_id, 'code': 'recorded_archive_lock_expired',
                             'state': 'attention', 'owner_issue': 2343})
        if covers_expiry is not True:
            findings.append({
                'artifact_id': artifact_id,
                'code': 'historical_archive_protection_beyond_actions_expiry_unproven',
                'state': 'attention', 'owner_issue': 2343})
    receipt = extension.get('receipt')
    if receipt and datetime.fromisoformat(receipt['retain_until']) <= now:
        findings.append({'kind': 'preservation_receipt', 'code': 'recorded_preservation_receipt_lock_expired',
                         'state': 'attention', 'owner_issue': 2343})
    return {
        'schema_version': 3, 'checked_at': now.isoformat(), 'repository': REPOSITORY,
        'collector_sha256': hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'scope': 'Bounded workflow receipts, accepted historical ledger and authenticated dated retention extension',
        'database_access': False, 'object_store_access': False, 'notifications_enabled': False,
        'history_complete': False, 'archive_preservation_verified': False,
        'accepted_historical_container_coverage': True,
        'accepted_historical_gap_count': len(ledger['original_gap_ledger']),
        'ledger_sha256': LEDGER_SHA256,
        'summary': {'required_container_count': len(archives),
                    'earliest_recorded_lock_deadline': min(
                        row['recorded_retain_until'] for row in archives),
                    'earliest_latest_verified_archive_lock_deadline': min(
                        datetime.fromisoformat(row['latest_verified_retain_until'])
                        for row in archives).isoformat(),
                    'extension_archive_count': extension.get('archive_count', 0),
                    'extension_receipt_count': extension.get('receipt_count', 0),
                    'known_actions_expiry_covered_archive_count': sum(
                        row['protection_beyond_frozen_actions_expiry'] is True for row in archives),
                    'unknown_actions_expiry_archive_count': sum(
                        row['protection_beyond_frozen_actions_expiry'] is None for row in archives)},
        'retention_extension': {key: value for key, value in extension.items() if key != 'archives'},
        'checks': checks, 'archives': archives, 'findings': findings,
        'historical_findings': historical_findings,
        'limits': [
            'GitHub metadata availability is not a download, original byte rehash or independent archive proof.',
            'Receipt timestamps and lock deadlines are historical assertions, not fresh object-store readbacks.',
            'The authenticated extension supersedes older recorded deadlines, not the immutable acceptance ledger.',
            'The original private receipt self-version remains unrecorded and outside the extension.',
            '36h backup monitoring threshold is not the proposed 24h RPO.',
            'Eight-day drill review window is a reporting check, not an adopted recovery objective.',
            'Successful isolated restore timing is not full-service RTO.',
            'The owner accepted the frozen 41-container/11-gap scope under closed #2242.',
            'Source Actions metadata is not re-inventoried; current retention and useful log coverage belong to #2343.',
            'No policy adoption or all-history/all-log completeness is implied.']}


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--output', required=True)
    args = parser.parse_args(argv)
    path = Path(args.output)
    path.unlink(missing_ok=True)
    report = collect()
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(report, indent=2)+'\n')
    print(json.dumps({'findings': len(report['findings']), 'archive_preservation_verified': False}))


if __name__ == '__main__':
    main()
