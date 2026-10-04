"""Kill and restart an owned disposable supervisor after real fixture execution."""
import multiprocessing
from pathlib import Path
import signal
import sqlite3
from types import SimpleNamespace
import uuid

from .client import Client, Outbox
from .importer import provenance, start_run
from .pilot_drills import require
from .worker import work_staged


def validate_connection(dsn):
    from psycopg.conninfo import conninfo_to_dict
    values = conninfo_to_dict(dsn)
    require(values.get('host') in ('localhost', '127.0.0.1') and
            values.get('hostaddr') in (None, '127.0.0.1') and
            values.get('dbname') == 'catalogue_pilot' and
            values.get('user') == 'catalogue_pilot_worker',
            'Process drill requires the disposable loopback pilot connection')


class PauseBeforeUpload:
    def __init__(self, client, sender, gate):
        self.client, self.sender, self.gate = client, sender, gate

    def __getattr__(self, name):
        return getattr(self.client, name)

    def call(self, operation, *args):
        if operation == 'ingest':
            # Production work_staged has committed the observation and completion
            # to its FULL-synchronous WAL outbox before calling this operation.
            self.sender.send({'stage': 'durable-before-upload',
                              'observation_id': args[1][0]['observation_id']})
            if not self.gate.wait(timeout=300):
                raise TimeoutError('Disposable supervisor was not terminated in time')
        return self.client.call(operation, *args)


def supervisor(dsn, args, sender, gate=None):
    client = None
    try:
        validate_connection(dsn)
        client = Client(dsn=dsn, epoch=1)
        worker = PauseBeforeUpload(client, sender, gate) if gate is not None else client
        result = work_staged(worker, args)
        sender.send({'stage': 'resumed', 'new_attempts': result['attempts']})
    except Exception as error:
        # Driver exceptions can contain DSN fragments; never emit raw exceptions.
        sender.send({'stage': 'failed', 'error_type': type(error).__name__})
        raise SystemExit(1)
    finally:
        if client:
            client.close()
        sender.close()


def spool_counts(args):
    require(Path(args.outbox).is_file(), 'Missing durable supervisor outbox')
    outbox = Outbox(args.outbox, args.repository, args.producer, 1)
    try:
        return {'messages': outbox.db.execute('SELECT count(*) FROM messages').fetchone()[0],
                'pending_uploads': outbox.db.execute('SELECT count(*) FROM messages WHERE receipt IS NULL').fetchone()[0],
                'pending_completions': outbox.db.execute('SELECT count(*) FROM completions WHERE receipt IS NULL').fetchone()[0]}
    finally:
        outbox.db.close()


def receive(connection, timeout):
    require(connection.poll(timeout), 'Disposable supervisor checkpoint timed out')
    return connection.recv()


def run_process_drill(client, dsn, args, corpus, fixture, revision, document, output, report):
    validate_connection(dsn)
    require(client.contract['repository_id'] == args.repository and
            client.contract['producer_id'] == args.producer,
            'Supervisor process scope mismatch')
    report.update(complete=False, scope='real fixture in disposable database only', stage='prepare')
    pid = provenance(client, args.repository, corpus, args.kind, dict(document, process_kill_drill=True))
    worker_args = SimpleNamespace(**vars(args))
    worker_args.limit = 1
    worker_args.outbox = str(Path(output)/'process-kill-outbox.sqlite')
    worker_args.run = start_run(client, worker_args, pid, 'process-kill-pilot', revision, 'mvp')
    worker_args.budget = str(uuid.uuid4())
    client.put('provenance_fixture_eligibility', [{'repository_id': args.repository,
                'provenance_id': pid, 'fixture_id': fixture, 'eligibility': 'runnable', 'reason_codes': []}])
    item = str(uuid.uuid4())
    client.put('work_items', [{'work_item_id': item, 'repository_id': args.repository,
                'provenance_id': pid, 'fixture_id': fixture, 'variant': 'strict',
                'coherent_area': 'process-kill-pilot', 'attempt_limit': 1}])
    client.put('budget_scopes', [{'budget_scope_id': worker_args.budget, 'repository_id': args.repository,
                'scope_kind': 'run', 'external_key': 'process-kill-pilot', 'candidate_limit': 1,
                'accepted_limit': 1, 'attempt_limit': 1, 'time_limit_ms': args.cap_ms}])
    client.put('budget_work_items', [{'repository_id': args.repository,
                'budget_scope_id': worker_args.budget, 'work_item_id': item}])
    context = multiprocessing.get_context('spawn')
    receiver, sender = context.Pipe(duplex=False)
    child = context.Process(target=supervisor, args=(dsn, worker_args, sender, context.Event()))
    try:
        report['stage'] = 'await-durable-spool'
        child.start()
        sender.close()
        checkpoint = receive(receiver, 180)
        require(checkpoint.get('stage') == 'durable-before-upload', 'Worker did not reach the persisted result checkpoint')
        report['persisted_observation_id'] = checkpoint['observation_id']
        before = spool_counts(worker_args)
        budget = client.one('budget_scopes', budget_scope_id=worker_args.budget)
        require(before == {'messages': 1, 'pending_uploads': 1, 'pending_completions': 1} and
                not list(client.read('observations', {'run_id': worker_args.run})) and
                budget['reserved_attempts'] == 1 and budget['charged_attempts'] == 0,
                'Pre-kill spool/server/budget checkpoint mismatch')
        report.update(stage='kill-supervisor', before_kill=before, server_observations_before_kill=0,
                      reserved_attempts_before_kill=1, wal_present_before_kill=Path(worker_args.outbox+'-wal').exists())
        # Only this Process object's owned child is terminated; fixtures have
        # already returned and their Docker cleanup has completed at this boundary.
        child.kill()
        child.join(timeout=10)
        require(not child.is_alive() and child.exitcode == -signal.SIGKILL, 'Supervisor did not exit through SIGKILL')
        report['kill_exitcode'] = child.exitcode
        require(spool_counts(worker_args) == before, 'SIGKILL lost the durable pending spool')
        with sqlite3.connect(worker_args.outbox) as source, sqlite3.connect(Path(output)/'process-kill-pending.sqlite') as saved:
            source.backup(saved)
    finally:
        if child.pid is not None and child.is_alive():
            child.kill()
            child.join(timeout=10)
        receiver.close()
        sender.close()

    receiver, sender = context.Pipe(duplex=False)
    restart_args = SimpleNamespace(**vars(worker_args))
    restart_args.limit = 0  # Flush existing upload/completion; never rerun the fixture.
    restarted = context.Process(target=supervisor, args=(dsn, restart_args, sender))
    try:
        report['stage'] = 'restart-from-spool'
        restarted.start()
        sender.close()
        checkpoint = receive(receiver, 120)
        restarted.join(timeout=10)
        require(not restarted.is_alive() and restarted.exitcode == 0 and
                checkpoint == {'stage': 'resumed', 'new_attempts': 0}, 'Fresh supervisor did not replay without execution')
    finally:
        if restarted.pid is not None and restarted.is_alive():
            restarted.kill()
            restarted.join(timeout=10)
        receiver.close()
        sender.close()
    report['stage'] = 'verify'
    observations = list(client.read('observations', {'run_id': worker_args.run}))
    item_row = client.one('work_items', work_item_id=item)
    budget = client.one('budget_scopes', budget_scope_id=worker_args.budget)
    counts = spool_counts(worker_args)
    require(len(observations) == 1 and observations[0]['observation_id'] ==
            report['persisted_observation_id'] and
            observations[0]['outcome'] not in ('incomplete', 'infrastructure-error') and
            item_row['state'] == 'completed' and item_row['lease_generation'] == 1 and
            budget['charged_attempts'] == 1 and budget['reserved_attempts'] == 0 and budget['reserved_ms'] == 0 and
            counts == {'messages': 1, 'pending_uploads': 0, 'pending_completions': 0},
            'Process-kill recovery lost/duplicated work or leaked budget')
    report.update(complete=True, stage='verified', observations=1, outcome=observations[0]['outcome'],
                  completed_work=1, charged_attempts=1, reserved_attempts=0,
                  new_attempts_after_restart=0, after_restart=counts,
                  pending_checkpoint='process-kill-pending.sqlite')
