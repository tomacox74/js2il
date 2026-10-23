"""Collection completion is independent of test outcomes, coverage and publication.

Only trusted owner runs are finalized. Missing/ambiguous Actions history is a
reported limitation, never a guessed timestamp or a reason to touch legacy runs.
"""
from datetime import datetime, timezone
from itertools import islice
import json
from pathlib import Path
import re
import subprocess
import time

from .client import Outbox

PREPARE = 'Prepare and run centrally coordinated variants'
FLUSH = 'Flush durable recovery outbox'
KEY = re.compile(r'actions:([1-9][0-9]*):([1-9][0-9]*)\Z')
TERMINAL = ('complete', 'incomplete', 'failed', 'cancelled')


def timestamp(value):
    parsed = datetime.fromisoformat(value.replace('Z', '+00:00'))
    if parsed.tzinfo is None:
        raise ValueError('Lifecycle timestamp must have a timezone')
    return parsed.astimezone(timezone.utc)


def owned(client, row, args):
    return (row['repository_id'] == args.repository and row['producer_id'] == args.producer
            and row['trust_class'] == 'trusted' and row['run_kind'] in ('mvp', 'native'))


def finish(client, row, state, ended, source, reason, flushed):
    if timestamp(ended) < timestamp(row['started_at']):
        raise ValueError('Lifecycle end precedes catalogue start')
    evidence = {'source': source, 'external_run_key': row['external_run_key'],
                'source_revision': row['source_revision'], 'reason': reason,
                'flush_verified': flushed}
    result = client.call('finish_run', row['run_id'], state, ended, evidence)
    return {'run_id': result['run_id'], 'state': result['state'], 'ended_at': result['ended_at']}


def finalize(client, args):
    context = json.loads(Path(args.context).read_text())
    if (context['repository'], context['producer']) != (args.repository, args.producer):
        raise ValueError('Foreign supervisor context')
    row = client.one('runs', run_id=context['run'])
    if not row or not owned(client, row, args) or row['external_run_key'] != args.run_key or row['source_revision'] != context['revision']:
        raise ValueError('Collection identity mismatch')
    if row['state'] in TERMINAL:
        result = {'run_id': row['run_id'], 'state': row['state'], 'ended_at': row['ended_at'], 'unchanged': True}
        if context.get('budget'):
            result['budget'] = client.call('finish_budget', context['budget'], True)
        return result
    flushed = False
    if Path(args.outbox).is_file() and args.flush_verified:
        outbox = Outbox(args.outbox, args.repository, args.producer, client.epoch)
        try:
            # Replays an interrupted flush before accepting successful completion.
            outbox.flush(client)
            flushed = True
        finally:
            outbox.db.close()
    if args.conclusion == 'success' and flushed:
        state, reason = 'complete', 'collection-finished'
    elif args.conclusion == 'cancelled':
        state, reason = 'cancelled', 'job-cancelled'
    elif args.conclusion == 'failure':
        state, reason = 'failed', 'job-failed'
    else:
        state, reason = 'incomplete', 'flush-unproven'
    result = finish(client, row, state, datetime.now(timezone.utc).isoformat(), 'supervisor', reason, flushed)
    if context.get('budget'):
        result['budget'] = client.call('finish_budget', context['budget'], True)
    return result


def gh(path):
    # Bounded, repository-scoped REST reads. No logs, artifact bytes or exception text retained.
    result = subprocess.run(['gh', 'api', '--paginate', '--slurp', path],
                            capture_output=True, text=True, timeout=30, check=True)
    return json.loads(result.stdout)


def actions_plan(row, repository, read=gh):
    match = KEY.fullmatch(row['external_run_key'])
    if not match:
        return {'disposition': 'unsupported-key'}
    run_id, attempt = map(int, match.groups())
    runs = read(f'repos/{repository}/actions/runs/{run_id}/attempts/{attempt}')
    if len(runs) != 1:
        return {'disposition': 'ambiguous-run'}
    run = runs[0]
    if (run.get('id') != run_id or run.get('run_attempt') != attempt or
            run.get('head_sha') != row['source_revision'] or
            run.get('head_branch') != 'master' or
            (run.get('repository') or {}).get('full_name') != repository or
            (run.get('head_repository') or {}).get('full_name') != repository or
            run.get('path') != '.github/workflows/test262-central.yml'):
        return {'disposition': 'identity-mismatch'}
    if run.get('status') != 'completed':
        return {'disposition': 'active'}
    pages = read(f'repos/{repository}/actions/runs/{run_id}/attempts/{attempt}/jobs?per_page=100')
    jobs = [j for page in pages for j in page['jobs'] if j['name'] == 'supervise']
    if len(jobs) != 1:
        return {'disposition': 'ambiguous-supervisor'}
    job = jobs[0]
    if (job.get('run_id') != run_id or job.get('run_attempt') != attempt or
            job.get('head_sha') != row['source_revision'] or job.get('status') != 'completed' or not job.get('completed_at')):
        return {'disposition': 'unproven-supervisor'}
    ended = timestamp(job['completed_at'])
    if ended < timestamp(row['started_at']):
        return {'disposition': 'invalid-timestamp'}
    conclusion = job.get('conclusion')
    steps = job.get('steps', [])
    def succeeded(name):
        selected = [s for s in steps if s.get('name') == name]
        return len(selected) == 1 and selected[0].get('conclusion') == 'success'
    flushed = succeeded(FLUSH)
    if conclusion == 'success' and succeeded(PREPARE) and flushed:
        state, reason = 'complete', 'collection-finished'
    elif conclusion == 'failure':
        state, reason = 'failed', 'job-failed'
    elif conclusion == 'cancelled':
        state, reason = 'cancelled', 'job-cancelled'
    elif conclusion in ('timed_out', 'action_required', 'stale'):
        state, reason = 'incomplete', 'job-interrupted'
    elif conclusion == 'success':
        state, reason = 'incomplete', 'flush-unproven'
    else:
        return {'disposition': 'unknown-conclusion'}
    return {'disposition': 'terminal', 'state': state, 'ended_at': ended.isoformat(),
            'reason': reason, 'flush_verified': flushed}


def reconcile_runs(client, args):
    if not 1 <= args.limit <= 100:
        raise ValueError('Reconciliation limit must be 1..100')
    filters = {'producer_id': args.producer, 'trust_class': 'trusted', 'state': 'running'}
    if args.actions_run_attempt:
        if not args.actions_run_id or args.actions_run_id < 1 or args.actions_run_attempt < 1:
            raise ValueError('Exact Actions run and positive attempt required')
        filters['external_run_key'] = f'actions:{args.actions_run_id}:{args.actions_run_attempt}'
    rows = client.read('runs', filters)
    report = []
    deadline = time.monotonic() + 600
    stopped = False
    for row in islice(rows, args.limit):
        if time.monotonic() >= deadline:
            stopped = True
            break
        if not owned(client, row, args):
            continue
        match = KEY.fullmatch(row['external_run_key'])
        if args.actions_run_id and (not match or int(match[1]) != args.actions_run_id):
            continue
        item = {'run_id': row['run_id'], 'external_run_key': row['external_run_key']}
        try:
            plan = actions_plan(row, args.repository_name)
            item.update(plan)
            if args.apply and plan['disposition'] == 'terminal':
                item['result'] = finish(client, row, plan['state'], plan['ended_at'], 'github-actions', plan['reason'], plan['flush_verified'])
        except (subprocess.SubprocessError, OSError, ValueError, KeyError, TypeError):
            item['disposition'] = 'history-unavailable-or-invalid'
        report.append(item)
    return {'apply': args.apply, 'limit': args.limit, 'deadline_reached': stopped, 'runs': report}
