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


def load_ledger(path=LEDGER):
    raw = Path(path).read_bytes()
    if hashlib.sha256(raw).hexdigest() != LEDGER_SHA256:
        raise ValueError('Accepted historical retention ledger digest differs')
    return json.loads(raw)


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
    archives = []
    for row in ledger['archives']:
        artifact_id = row['artifact_id']
        deadline = datetime.fromisoformat(row['object']['retain_until'])
        expiration = row['actions_expires_at_from_frozen_inventory']
        archives.append({
            'artifact_id': artifact_id, 'sha256': row['original_zip_sha256'],
            'actions_state': 'not_assessed',
            'independent_archive_state': 'historical_exact_version_proof',
            'recorded_verified_at': row['verification']['checked_at'],
            'recorded_retain_until': deadline.isoformat(),
            'recorded_lock_state': 'deadline_elapsed' if deadline <= now else 'deadline_future'})
        if deadline <= now:
            findings.append({'artifact_id': artifact_id, 'code': 'recorded_archive_lock_expired',
                             'state': 'attention', 'owner_issue': 2343})
        if not expiration or deadline <= datetime.fromisoformat(expiration.replace('Z', '+00:00')):
            findings.append({
                'artifact_id': artifact_id,
                'code': 'historical_archive_protection_beyond_actions_expiry_unproven',
                'state': 'attention', 'owner_issue': 2343})
    return {
        'schema_version': 2, 'checked_at': now.isoformat(), 'repository': REPOSITORY,
        'collector_sha256': hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'scope': 'Latest bounded workflow receipts and owner-accepted 41-container historical ledger',
        'database_access': False, 'object_store_access': False, 'notifications_enabled': False,
        'history_complete': False, 'archive_preservation_verified': False,
        'accepted_historical_container_coverage': True,
        'accepted_historical_gap_count': len(ledger['original_gap_ledger']),
        'ledger_sha256': LEDGER_SHA256,
        'summary': {'required_container_count': len(archives),
                    'earliest_recorded_lock_deadline': min(
                        row['recorded_retain_until'] for row in archives)},
        'checks': checks, 'archives': archives, 'findings': findings,
        'limits': [
            'GitHub metadata availability is not a download, original byte rehash or independent archive proof.',
            'Receipt timestamps and lock deadlines are historical assertions, not fresh object-store readbacks.',
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
