"""Budget lifecycle follows proven collection termination, never test verdicts."""
from itertools import islice
import re
import time


def reconcile_budgets(client, args):
    if not 1 <= args.limit <= 100:
        raise ValueError('Reconciliation limit must be 1..100')
    filters = {'scope_kind': 'run'}
    if args.actions_run_id is not None:
        if args.actions_run_id < 1:
            raise ValueError('Positive Actions run ID required')
        filters['external_key'] = f'actions:{args.actions_run_id}'
    report = []
    deadline = time.monotonic() + 600
    stopped = False
    for budget in islice(client.read('budget_scopes', filters), args.limit):
        if time.monotonic() >= deadline:
            stopped = True
            break
        if budget['repository_id'] != args.repository or not re.fullmatch(r'actions:[1-9][0-9]*', budget['external_key']):
            continue
        # Plan and apply share the database's claim/settlement lock. A dry run
        # never calls api_settle; an interrupted apply is safe to replay.
        report.append(client.call('finish_budget', budget['budget_scope_id'], args.apply))
    return {'apply': args.apply, 'limit': args.limit, 'deadline_reached': stopped, 'budgets': report}
