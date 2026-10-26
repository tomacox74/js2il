"""Read-only, bounded backup/restore and reviewed archive availability inventory.

No database, object-store access, downloads of original archives, fixture execution,
notification delivery or policy adoption. Unknown independently retained coverage
never becomes a successful preservation claim.
"""
import argparse
from datetime import datetime, timezone
import json
import hashlib
from pathlib import Path
import subprocess

from .cutover_evidence import DEFAULT_MANIFEST, REPOSITORY, load_manifest
from .monitoring import collect_check, github


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
    config, pins, _, manifest_sha = load_manifest(DEFAULT_MANIFEST)
    checks = {kind: collect_check(REPOSITORY, kind, fetch) for kind in ('backup', 'restore')}
    findings = assess(checks, now)
    archives = []
    for artifact_id, expected in pins.items():
        state = 'unavailable'
        try:
            meta = fetch(f'repos/{REPOSITORY}/actions/artifacts/{artifact_id}')
            if type(meta.get('expired')) is not bool:
                raise ValueError('Artifact expiration state is missing')
            if meta['id'] != artifact_id or meta['digest'] != 'sha256:'+expected:
                state = 'metadata_mismatch'
            elif meta.get('expired') is True:
                state = 'expired'
            else:
                expiration = datetime.fromisoformat(meta['expires_at'].replace('Z', '+00:00'))
                if expiration.tzinfo is None:
                    raise ValueError('Artifact expiration requires timezone')
                state = 'expired' if expiration <= now else 'metadata_available'
        except (ValueError, KeyError, TypeError, subprocess.SubprocessError):
            # Only GitHub metadata is queried; no fallback asserts object-store coverage.
            state = 'unavailable'
        archives.append({'artifact_id': artifact_id, 'sha256': expected,
                         'actions_state': state, 'independent_archive_state': 'not_assessed'})
        if state != 'metadata_available':
            findings.append({'artifact_id': artifact_id, 'code': 'original_actions_archive_'+state,
                             'state': 'unknown' if state == 'unavailable' else 'attention',
                             'owner_issue': 2242})
    return {
        'schema_version': 1, 'checked_at': now.isoformat(), 'repository': REPOSITORY,
        'collector_sha256': hashlib.sha256(Path(__file__).read_bytes()).hexdigest(),
        'scope': 'Latest bounded workflow receipts and existing reviewed artifact manifest only',
        'database_access': False, 'object_store_access': False, 'notifications_enabled': False,
        'history_complete': False, 'archive_preservation_verified': False,
        'manifest_scope': config['scope'], 'manifest_sha256': manifest_sha,
        'checks': checks, 'archives': archives, 'findings': findings,
        'limits': [
            'GitHub metadata availability is not a download, original byte rehash or independent archive proof.',
            'Receipt timestamps and lock deadlines are historical assertions, not fresh object-store readbacks.',
            '36h backup monitoring threshold is not the proposed 24h RPO.',
            'Eight-day drill review window is a reporting check, not an adopted recovery objective.',
            'Successful isolated restore timing is not full-service RTO.',
            'Useful failed outboxes, remaining original archives and logs require the #2242 coverage ledger.',
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
