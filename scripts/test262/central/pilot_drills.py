"""Synthetic queue-control drills, exclusively inside the disposable worker pilot.

These observations describe control tests, never compiler test results. The caller
has provisioned a fresh loopback database and must keep these provenances separate
from the ten real fixture executions.
"""
from datetime import datetime, timezone
import time
from types import SimpleNamespace
import uuid

from .importer import provenance, start_run


def require(condition, message):
    if not condition:
        raise ValueError(message)


def expect_rejection(client, operation, args, sqlstate):
    import psycopg
    try:
        client.call(operation, *args)
    except psycopg.Error as error:
        require(error.sqlstate == sqlstate, 'Unexpected rejection SQLSTATE')
        return sqlstate
    raise ValueError('Expected API rejection did not occur')


def observation(args, pid, lease):
    now = datetime.now(timezone.utc).isoformat()
    return {'observation_id': str(uuid.uuid4()), 'repository_id': args.repository,
            'run_id': args.run, 'provenance_id': pid, 'fixture_id': lease['fixture_id'],
            'variant': lease['variant'], 'work_item_id': lease['work_item_id'],
            'lease_generation': lease['lease_generation'], 'outcome': 'unsupported',
            'failure_class': 'policy-exclusion', 'phase': 'planning', 'active_ms': 0,
            'started_at': now, 'finished_at': now, 'source_kind': 'live',
            'diagnostic_summary': 'Synthetic disposable queue-control drill; fixture not executed',
            'payload': {'synthetic_queue_drill': True}}


def prepare(client, args, corpus, fixtures, revision, scenario, count, attempts):
    pid = provenance(client, args.repository, corpus, 'mvp-composite',
                     {'pilot': True, 'synthetic_queue_drill': scenario})
    run_args = SimpleNamespace(**vars(args))
    run_args.run = start_run(client, run_args, pid, 'drill-'+scenario+'-a', revision, 'mvp')
    other = SimpleNamespace(**vars(run_args))
    other.run = start_run(client, other, pid, 'drill-'+scenario+'-b', revision, 'mvp')
    budget = str(uuid.uuid4())
    client.put('budget_scopes', [{'budget_scope_id': budget, 'repository_id': args.repository,
                'scope_kind': 'run', 'external_key': 'drill-'+scenario, 'candidate_limit': count,
                'accepted_limit': count, 'attempt_limit': attempts, 'time_limit_ms': attempts*1000}])
    rows = [{'work_item_id': str(uuid.uuid4()), 'repository_id': args.repository,
             'provenance_id': pid, 'fixture_id': fixture, 'variant': 'strict',
             'coherent_area': 'synthetic-control-drill', 'attempt_limit': 2,
             'selection_reason': {'synthetic_queue_drill': scenario}} for fixture in fixtures[:count]]
    client.put('work_items', rows)
    client.put('budget_work_items', [{'repository_id': args.repository,
                'budget_scope_id': budget, 'work_item_id': row['work_item_id']} for row in rows])
    return pid, run_args, other, budget


def run_drills(client, args, corpus, fixtures, revision, report=None):
    info = client.db.info
    require(info.host in ('localhost', '127.0.0.1') and info.dbname == 'catalogue_pilot' and
            info.user == 'catalogue_pilot_worker', 'Control drills require the disposable pilot login/database')
    report = {} if report is None else report
    report.update(scope='synthetic queue control in disposable database only', complete=False, stage='expired-lease')
    pid, first, second, budget_id = prepare(client, args, corpus, fixtures, revision, 'expiry', 1, 3)
    old = client.call('claim', first.run, budget_id, 1000, 31)
    require(old and old.get('lease_generation') == 1, 'Initial lease missing')
    # Wait for the real server-issued deadline, rather than mutating lease rows or clocks.
    deadline = datetime.fromisoformat(old['lease_expires_at'])
    remaining = (deadline-datetime.now(timezone.utc)).total_seconds()
    time.sleep(max(0, remaining)+0.2)
    renewed = expect_rejection(client, 'renew', (old['work_item_id'], 1, 31), '40001')
    replacement = client.call('claim', second.run, budget_id, 1000, 31)
    require(replacement and replacement['work_item_id'] == old['work_item_id'] and
            replacement['lease_generation'] == 2, 'Expired work was not reclaimed with a new generation')
    before = client.one('budget_scopes', budget_scope_id=budget_id)
    require(before['charged_attempts'] == 1 and before['charged_ms'] == 1000 and
            before['reserved_attempts'] == 1, 'Abandoned lease was not conservatively charged')
    old_row = observation(first, pid, old)
    client.call('ingest', str(uuid.uuid4()), [old_row])
    late = client.call('complete', old['work_item_id'], 1, old_row['observation_id'])
    require(late.get('completed') is False and late.get('retained_late_evidence') is True,
            'Stale completion was not fenced')
    current = client.one('work_items', work_item_id=old['work_item_id'])
    after_late = client.one('budget_scopes', budget_scope_id=budget_id)
    require(current['state'] == 'leased' and current['lease_generation'] == 2 and
            current['version'] == replacement['version'] and after_late == before,
            'Late completion changed the current lease or budget')
    row = observation(second, pid, replacement)
    request = str(uuid.uuid4())
    receipt = client.call('ingest', request, [row])
    require(client.call('complete', row['work_item_id'], 2, row['observation_id']).get('completed'),
            'Current lease completion failed')
    settled = client.one('budget_scopes', budget_scope_id=budget_id)
    require(settled['charged_attempts'] == 2 and settled['charged_ms'] == 1000 and
            settled['reserved_attempts'] == 0 and settled['reserved_ms'] == 0,
            'Recovered lease budget did not settle')
    report['expired_lease'] = {'old_generation': 1, 'new_generation': 2,
            'renewal_rejection_sqlstate': renewed, 'late_evidence_retained': True,
            'late_completion_fenced': True, 'charged_attempts': 2, 'charged_ms': 1000,
            'pending_reservations': 0, 'real_deadline_wait': True}

    # Exercise both request-ID conflict and observation-ID conflict through the real API.
    report['stage'] = 'conflicting-replay'
    stored = client.one('observations', observation_id=row['observation_id'])
    changed = dict(row, diagnostic_summary='conflicting synthetic replay')
    request_error = expect_rejection(client, 'ingest', (request, [changed]), 'P0001')
    observation_error = expect_rejection(client, 'ingest', (str(uuid.uuid4()), [changed]), 'P0001')
    require(client.call('ingest', request, [row]) == receipt and
            client.one('observations', observation_id=row['observation_id']) == stored and
            len(list(client.read('observations', {'provenance_id': pid}))) == 2,
            'Conflict rejection changed immutable evidence or replay receipt')
    report['conflicting_replay'] = {'request_rejection_sqlstate': request_error,
            'observation_rejection_sqlstate': observation_error,
            'original_unchanged': True, 'original_receipt_reused': True, 'observations': 2}

    report['stage'] = 'exhausted-budget'
    pid, first, second, budget_id = prepare(client, args, corpus, fixtures, revision, 'budget', 2, 1)
    claimed = client.call('claim', first.run, budget_id, 1000, 31)
    require(claimed and not claimed.get('budget_exhausted'), 'Budget drill initial claim missing')
    blocked_reserved = client.call('claim', second.run, budget_id, 1000, 31)
    require(blocked_reserved and blocked_reserved.get('budget_exhausted'),
            'Reserved budget allowed an excess claim')
    row = observation(first, pid, claimed)
    client.call('ingest', str(uuid.uuid4()), [row])
    require(client.call('complete', row['work_item_id'], row['lease_generation'], row['observation_id']).get('completed'),
            'Budget drill completion failed')
    blocked_charged = client.call('claim', second.run, budget_id, 1000, 31)
    items = list(client.read('work_items', {'provenance_id': pid}))
    remaining = [item for item in items if item['state'] == 'pending']
    budget = client.one('budget_scopes', budget_scope_id=budget_id)
    require(blocked_charged and blocked_charged.get('budget_exhausted') and
            len(remaining) == 1 and remaining[0]['lease_generation'] == 0 and
            budget['charged_attempts'] == 1 and budget['reserved_attempts'] == 0 and budget['reserved_ms'] == 0,
            'Exhausted budget lost work, exceeded limits or leaked reservations')
    report['exhausted_budget'] = {'reserved_limit_blocked': True, 'charged_limit_blocked': True,
            'charged_attempts': 1, 'pending_unattempted': 1, 'pending_reservations': 0}
    report['complete'] = True
    report['stage'] = 'verified'
    return report
