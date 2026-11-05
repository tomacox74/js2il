"""Read-only, allocation-scoped operator snapshots for managed collections."""
import argparse
from collections import Counter
from datetime import datetime, timezone
import json
import os
from pathlib import Path

from .client import Client, identity, sha
from .collection import write_json


def snapshot(client, context, now=None):
    registered = client.call('collection', context['run'])
    if (not registered or registered['context'] != context
            or registered['context_sha256'] != '\\x' + sha(context)):
        raise ValueError('Context differs from the registered collection')
    now = now or datetime.now(timezone.utc)
    membership = {row['work_item_id'] for row in client.read(
        'budget_work_items', {'budget_scope_id': context['budget']})}
    work = [row for row in client.read('work_items', {'provenance_id': context['provenance']})
            if row['work_item_id'] in membership]
    counts = Counter(row['state'] for row in work)
    completions = [row['completion_observation_id'] for row in work
                   if row.get('completion_observation_id')]
    expired = [row for row in work if row['state'] == 'leased'
               and datetime.fromisoformat(row['lease_expires_at']) <= now]
    workers = []
    for member in registered['workers']:
        run_id = member.get('run_id')
        metrics = list(client.read('run_metrics', {'run_id': run_id})) if run_id else []
        metric = next((row for row in metrics if row['stage'] == 'screening'), None)
        rate = None
        if metric and metric['duration_ms'] > 0:
            rate = round(metric['attempt_count'] * 60000 / metric['duration_ms'], 2)
        workers.append({
            'slot': member['slot'], 'producer_id': member['producer_id'],
            'run_id': run_id, 'job_id': member.get('actions_job_id'),
            'state': member.get('run_state') or 'not-started',
            'leased': sum(row['state'] == 'leased' and row['lease_owner'] == member['producer_id']
                          for row in work),
            'expired_leases': sum(row['lease_owner'] == member['producer_id'] for row in expired),
            'attempts_per_minute': rate,
        })
    return {
        'schema_version': 1, 'observed_at': now.isoformat(),
        'collection_run_id': context['run'], 'actions_run_id': context['actions_run_id'],
        'actions_attempt': context['actions_attempt'], 'revision': context['revision'],
        'provenance_id': context['provenance'], 'kind': context['kind'],
        'registration_snapshot_id': context.get('registration_snapshot'),
        'candidate_count': len(context.get('candidate_ids', [])),
        'context_sha256': sha(context), 'phase': registered['phase'],
        'scope': 'frozen budget membership; current-state snapshot',
        'expected_work': context['work_count'], 'enrolled_work': len(membership),
        'missing_work': len(membership - {row['work_item_id'] for row in work}),
        'counts': {state: counts[state] for state in
                   ('pending', 'leased', 'completed', 'deferred', 'cancelled')},
        'expired_leases': len(expired),
        'completion_references': len(completions),
        'distinct_completion_observations': len(set(completions)),
        'budget': client.one('budget_scopes', budget_scope_id=context['budget']),
        'workers': workers,
    }


def markdown(report):
    counts = report['counts']
    lines = ['## Test262 collection', '',
             f"Revision `{report['revision']}`, provenance `{report['provenance_id']}`.", '',
             f"Phase **{report['phase']}**. Pending {counts['pending']}, leased {counts['leased']}, "
             f"completed {counts['completed']}, deferred {counts['deferred']}, "
             f"cancelled {counts['cancelled']}, expired leases {report['expired_leases']}.", '',
             f"Enrolled {report['enrolled_work']}, expected {report['expected_work']}, "
             f"missing rows {report['missing_work']}.", '',
             '| Slot | Producer | Run | Job | State | Leased | Expired leases | Attempts/min |',
             '|---|---|---|---|---|---:|---:|---:|']
    for worker in report['workers']:
        values = [worker[key] for key in ('slot', 'producer_id', 'run_id', 'job_id', 'state',
                                          'leased', 'expired_leases', 'attempts_per_minute')]
        lines.append('| ' + ' | '.join('unknown' if value is None else str(value)
                                      for value in values) + ' |')
    lines += ['', 'Expired leases require lifecycle/recovery inspection. Throughput uses recorded '
              'screening attempts and active duration when available. This read-only snapshot '
              'does not certify sealing or publication.', '']
    return '\n'.join(lines)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    source = parser.add_mutually_exclusive_group(required=True)
    source.add_argument('--context')
    source.add_argument('--actions-run-id', type=int)
    parser.add_argument('--output', required=True)
    args = parser.parse_args()
    client = Client()
    try:
        if (client.contract['repository_id'] != os.environ['TEST262_REPOSITORY_ID']
                or client.contract['producer_id'] != os.environ['TEST262_PRODUCER_ID']):
            raise ValueError('Configured identities differ from authenticated subject')
        if args.context:
            context = json.loads(Path(args.context).read_text())
        else:
            if args.actions_run_id <= 0:
                raise ValueError('Expected a positive Actions run ID')
            run_id = identity(os.environ['TEST262_REPOSITORY_ID'],
                              os.environ['TEST262_PRODUCER_ID'],
                              f'actions:{args.actions_run_id}:1')
            registered = client.call('collection', run_id)
            if not registered:
                raise ValueError('No registered managed collection for that Actions run')
            context = registered['context']
        report = snapshot(client, context)
        write_json(args.output, report)
        if os.environ.get('GITHUB_STEP_SUMMARY'):
            with Path(os.environ['GITHUB_STEP_SUMMARY']).open('a', encoding='utf-8') as stream:
                stream.write(markdown(report))
    finally:
        client.close()


if __name__ == '__main__':
    try:
        main()
    except Exception:
        raise SystemExit('Collection status unavailable; inspect the collection and coordinator configuration') from None
