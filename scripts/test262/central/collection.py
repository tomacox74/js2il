"""Prepare, execute and reconcile a bounded Actions worker matrix."""

from contextlib import closing
from datetime import datetime, timezone
import json
import os
from pathlib import Path
import re
import sqlite3

from .client import Outbox, canonical, identity, sha
from .enlist import check_checkout, require
from .lifecycle import finish, gh
from .worker import work

WORKFLOW = '.github/workflows/test262-central.yml'
EXECUTE_JOB = 'execute'
RECOVERY_STEP = 'Retain split worker recovery'
MAX_WORKER_SECONDS = 600
MAX_WORKER_ATTEMPTS = 100


def write_json(path, value):
    path = Path(path)
    path.parent.mkdir(parents=True, exist_ok=True)
    temporary = path.with_suffix(path.suffix + '.partial')
    temporary.write_text(canonical(value) + '\n', encoding='utf-8')
    temporary.replace(path)


def capability(client):
    require(client.contract.get('managed_actions_collection_version') == 1,
            'Managed collection API version 1 is required')


def actions_origin():
    run_id = os.environ['GITHUB_RUN_ID']
    attempt = os.environ['GITHUB_RUN_ATTEMPT']
    event_sha = os.environ['GITHUB_SHA']
    require(re.fullmatch(r'[1-9][0-9]*', run_id) and attempt == '1'
            and re.fullmatch(r'[0-9a-f]{40}', event_sha)
            and os.environ.get('GITHUB_REF') == 'refs/heads/master',
            'Split collection version 1 requires the original master workflow attempt')
    return int(run_id), int(attempt), event_sha


def digest_string(value):
    require(isinstance(value, str) and re.fullmatch(r'(?:sha256:)?[0-9a-f]{64}', value),
            'Expected a SHA-256 digest')
    return value.removeprefix('sha256:')


def prepared_context(client, args):
    capability(client)
    max_slots = client.contract.get('managed_actions_collection_slots', 0)
    require(1 <= len(args.managed_worker) <= max_slots,
            'Database does not support the requested execution matrix')
    run_id, attempt, _ = actions_origin()
    require(args.run_key == f'actions:{run_id}:{attempt}' and args.budget_key == f'actions:{run_id}',
            'Managed preparation keys must match the Actions origin')
    existing = client.call('collection', identity(args.repository, args.producer, args.run_key))
    if existing is None:
        return None
    context = existing['context']
    expected = preparation_fields(args)
    require(existing['phase'] == 'prepared' and all(context.get(k) == v for k, v in expected.items())
            and context['revision'] == args.revision and context['kind'] == args.kind
            and context['compilation_coverage'] == args.compilation_coverage,
            'Preparation replay differs from the frozen plan')
    write_json(args.output, context)
    return {'run': context['run'], 'replayed': True}


def preparation_fields(args):
    run_id, attempt, event_sha = actions_origin()
    return {
        'managed_version': 1, 'actions_run_id': run_id, 'actions_attempt': attempt,
        'event_sha': event_sha, 'worker_producers': args.managed_worker,
        'runtime_digest': digest_string(args.runtime_digest), 'image_digest': digest_string(args.image_digest),
        'area': args.area,
        'limits': {'candidate_limit': args.candidate_limit, 'accepted_limit': args.accepted_limit,
                   'attempt_limit': args.attempt_limit, 'time_limit_ms': args.budget_ms},
    }


def register_plan(client, args, context, rows, reconciliation):
    context.update(preparation_fields(args))
    context['work_count'] = len(rows)
    context['work_digest'] = sha(sorted(row['work_item_id'] for row in rows))
    registered = client.call('register_collection', {
        'context': context, 'work': rows, 'reconciliation': reconciliation,
        'next_cursor': context['recovery']['cursor_after'],
    })
    require(registered['context'] == context, 'Registered plan differs from prepared context')
    write_json(args.output, context)
    return {'run': context['run'], 'work_count': len(rows)}


def load_context(client, path):
    capability(client)
    context = json.loads(Path(path).read_text(encoding='utf-8'))
    run_id, attempt, event_sha = actions_origin()
    require((context['actions_run_id'], context['actions_attempt'], context['event_sha']) ==
            (run_id, attempt, event_sha) and context['managed_version'] == 1,
            'Context belongs to a different workflow attempt')
    registered = client.call('collection', context['run'])
    require(registered and registered['context'] == context
            and registered['context_sha256'] == '\\x' + sha(context),
            'Context is not the registered preparation')
    return context, registered


def open_collection(client, args):
    context, _ = load_context(client, args.context)
    name = f"test262-split-prepare-{context['actions_run_id']}-{context['actions_attempt']}"
    artifact, digest = artifact_proof(context, args.repository_name, name)
    require(artifact == args.artifact_id and digest == digest_string(args.artifact_digest),
            'Preparation upload does not match its Actions receipt')
    result = client.call('open_collection', context['run'], artifact, digest)
    return {'run': context['run'], 'phase': result['phase'], 'artifact_id': result['artifact_id']}


def history(context, repository, *, completed, slots, read=gh):
    run_id, attempt = context['actions_run_id'], context['actions_attempt']
    runs = read(f'repos/{repository}/actions/runs/{run_id}/attempts/{attempt}')
    require(len(runs) == 1, 'Ambiguous Actions attempt')
    run = runs[0]
    require(run.get('id') == run_id and run.get('run_attempt') == attempt
            and run.get('head_sha') == context['event_sha'] and run.get('head_branch') == 'master'
            and run.get('path') == WORKFLOW
            and (run.get('repository') or {}).get('full_name') == repository
            and (run.get('head_repository') or {}).get('full_name') == repository,
            'Actions attempt does not match the prepared context')
    if completed:
        require(run.get('status') == 'completed', 'Actions attempt is still active')
    pages = read(f'repos/{repository}/actions/runs/{run_id}/attempts/{attempt}/jobs?per_page=100')
    jobs = [job for page in pages for job in page['jobs']]
    result = {}
    for slot in slots:
        name = f'{EXECUTE_JOB} ({slot})' if 'worker_producers' in context else EXECUTE_JOB
        matches = [job for job in jobs if job['name'] == name]
        require(len(matches) == 1, f'Missing or ambiguous execution job for slot {slot}')
        job = matches[0]
        require(job.get('run_id') == run_id and job.get('run_attempt') == attempt
                and job.get('head_sha') == context['event_sha'] and type(job.get('id')) is int,
                'Execution job does not match the prepared context')
        result[slot] = job
    require(len({job['id'] for job in result.values()}) == len(result), 'Duplicate execution job ID')
    return result


def artifact_proof(context, repository, name, read=gh):
    run_id = context['actions_run_id']
    pages = read(f'repos/{repository}/actions/runs/{run_id}/artifacts?per_page=100')
    artifacts = [artifact for page in pages for artifact in page['artifacts'] if artifact['name'] == name]
    require(len(artifacts) == 1, 'Missing or ambiguous immutable artifact')
    artifact = artifacts[0]
    origin = artifact.get('workflow_run') or {}
    require(not artifact['expired'] and origin.get('id') == run_id
            and origin.get('head_sha') == context['event_sha']
            and origin.get('head_branch') == 'master' and type(artifact.get('id')) is int,
            'Artifact origin does not match the prepared context')
    return artifact['id'], digest_string(artifact.get('digest'))


def work_collection(client, args):
    context, registered = load_context(client, args.context)
    require(registered['artifact_id'] == args.artifact_id
            and registered['artifact_digest'] == '\\x' + digest_string(args.artifact_digest),
            'Execution artifact differs from the committed preparation')
    producers = context.get('worker_producers') or [context['worker_producer']]
    require(1 <= args.slot <= len(producers) and args.producer == producers[args.slot - 1]
            and args.producer != context['producer'],
            'Execution requires its registered worker identity')
    args.kind = context['kind']
    args.budget = context['budget']
    args.cap_ms = context['cap_ms']
    args.runtime_timeout = context['identity']['timeouts']['runtime']
    args.compile_timeout = context['identity']['timeouts']['compile']
    args.compilation_coverage = context.get('compilation_coverage', False)
    args.seconds = MAX_WORKER_SECONDS
    args.limit = MAX_WORKER_ATTEMPTS
    check_checkout(args, context)
    job = history(context, args.repository_name, completed=False, slots=[args.slot])[args.slot]
    require(job.get('status') == 'in_progress', 'Execution job is not active')
    key = f"worker:{args.producer}:actions:{context['actions_run_id']}:{context['actions_attempt']}:{args.slot}:1"
    args.run = identity(args.repository, args.producer, key)
    require(not Path(args.outbox).exists(), 'Existing execution outbox requires upload-only recovery')
    child = client.call('join_collection', context['run'], args.run, job['id'], sha(context))
    result = work(client, args)
    with closing(Outbox(args.outbox, args.repository, args.producer, client.epoch).db) as db:
        pending = db.execute('SELECT count(*) FROM messages WHERE receipt IS NULL').fetchone()[0]
        pending += db.execute('SELECT count(*) FROM completions WHERE receipt IS NULL').fetchone()[0]
        require(pending == 0, 'Unacknowledged worker outbox')
    finish(client, child, 'complete', datetime.now(timezone.utc).isoformat(),
           'supervisor', 'collection-finished', True)
    result.update(collection_run_id=context['run'], job_id=job['id'], slot=args.slot, flush_verified=True)
    write_json(args.output, result)
    return result


def terminal_proof(context, repository, *, completed=False, read=gh):
    slots = range(1, len(context.get('worker_producers', [None])) + 1)
    jobs = history(context, repository, completed=completed, slots=slots, read=read)
    proofs = [dict(worker_proof(context, repository, job, slot, read), slot=slot)
              for slot, job in jobs.items()]
    if 'worker_producers' in context:
        return {'workers': proofs}
    legacy = proofs[0]
    del legacy['slot']
    return legacy


def worker_proof(context, repository, job, slot, read):
    require(job.get('status') == 'completed' and job.get('completed_at'), 'Execution is not proven terminal')
    state = job.get('conclusion')
    if state in ('timed_out', 'action_required', 'stale'):
        state = 'incomplete'
    require(state in ('success', 'failure', 'cancelled', 'incomplete', 'skipped'),
            'Unknown execution conclusion')
    retained = False
    if state == 'success' and any(step['name'] == RECOVERY_STEP and step.get('conclusion') == 'success'
                                  for step in job.get('steps', [])):
        name = f"test262-split-worker-{context['actions_run_id']}-{context['actions_attempt']}-{slot}"
        try:
            artifact_proof(context, repository, name, read)
            retained = True
        except (ValueError, KeyError, OSError):
            pass
    return {'job_id': job['id'], 'state': state, 'retained': retained}


def finish_collection(client, args):
    capability(client)
    run_id, attempt, _ = actions_origin()
    owner_run = identity(args.repository, args.producer, f'actions:{run_id}:{attempt}')
    registered = client.call('collection', owner_run)
    if registered is None:
        result = {'state': 'unprepared', 'run_id': owner_run}
    else:
        context = registered['context']
        proof = terminal_proof(context, args.repository_name)
        result = client.call('finish_collection', owner_run, proof)
        write_json(args.context_output, context)
    write_json(args.output, result)
    budget = result.get('budget', {})
    completed = result['state'] == 'complete' and budget.get('disposition') in ('terminal', 'unchanged')
    with open(args.github_output, 'a', encoding='utf-8') as stream:
        stream.write(f'complete={str(completed).lower()}\n')
    return result


def check_seal(client, args, context):
    _, registered = load_context(client, args.context)
    owner = client.one('runs', run_id=context['run'])
    require(registered['phase'] == 'closing' and owner and owner['state'] == 'complete',
            'Managed collection has not reconciled successfully')
    from .commands import current_master
    require(current_master(args.repository_name) == context['revision'],
            'Master advanced before managed native acceptance')


def snapshot_outbox(source, destination):
    """Capture committed WAL content after the worker exits."""
    if not Path(source).is_file():
        return
    destination = Path(destination)
    destination.parent.mkdir(parents=True, exist_ok=True)
    require(not destination.exists(), 'Recovery snapshot already exists')
    with closing(sqlite3.connect(Path(source).resolve().as_uri() + '?mode=ro', uri=True)) as src:
        with closing(sqlite3.connect(destination)) as dest:
            src.backup(dest)
