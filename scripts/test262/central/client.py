"""Versioned PostgreSQL API and durable, at-least-once outbox.

The DSN must be a dedicated scoped login, never postgres or a service-role key.
Only this supervisor uses the DSN. Keep the outbox on durable local storage.
"""
import hashlib
import json
import os
import time
from pathlib import Path
import sqlite3
import uuid
from contextlib import contextmanager

EXPORT_TABLES = ('corpora', 'repository_corpora', 'fixtures', 'fixture_variants', 'fixture_dependencies',
                 'provenances', 'provenance_fixture_eligibility', 'runs', 'observations', 'observation_payloads',
                 'observation_invalidations', 'validation_events', 'work_items', 'work_leases', 'budget_reservations',
                 'budget_scopes', 'budget_work_items', 'registration_snapshots', 'registrations', 'reporting_targets',
                 'native_batches', 'batch_fixtures', 'batch_evidence', 'publications', 'publication_checks', 'imports',
                 'import_records', 'legacy_control_records', 'reconciliation_state', 'reconciliations', 'run_metrics')


def canonical(value):
    return json.dumps(value, sort_keys=True, separators=(',', ':'), ensure_ascii=False)


def sha(value):
    return hashlib.sha256(canonical(value).encode()).hexdigest()


def identity(*parts):
    return str(uuid.uuid5(uuid.NAMESPACE_URL, canonical(parts)))


def bytea(hex_value):
    return '\\x' + hex_value if hex_value else None


class Client:
    def __init__(self, dsn=None, epoch=None):
        import psycopg
        from psycopg.types.json import Jsonb
        self.Jsonb = Jsonb
        self.epoch = int(epoch if epoch is not None else os.environ['TEST262_AUTHORITY_EPOCH'])
        self.db = psycopg.connect(dsn or os.environ['TEST262_DATABASE_URL'], autocommit=True)
        self.db.execute("SET statement_timeout='120s'")
        self.db.execute("SET lock_timeout='15s'")
        self.contract = self.db.execute('SELECT test262.api_contract()').fetchone()[0]
        if not self.contract or self.contract['api_contract_version'] != 1 or self.contract['minimum_writer_epoch'] != self.epoch:
            self.db.close()
            raise ValueError('Unbound login or incompatible catalogue contract/epoch')

    def close(self):
        self.db.close()

    def call(self, operation, *args):
        arities = {'put': 2, 'start_run': 1, 'ingest': 2, 'claim': 4,
                   'renew': 3, 'complete': 3, 'transition': 4, 'reconcile': 4, 'metric': 6}
        if operation not in arities or len(args) != arities[operation]:
            raise ValueError('Unsupported API operation')
        values = [self.epoch] + [self.Jsonb(x) if isinstance(x, (dict, list)) else x for x in args]
        return self.db.execute('SELECT test262.api_' + operation + '(' + ','.join(['%s'] * len(values)) + ')', values).fetchone()[0]

    def put(self, table, rows):
        started = time.monotonic()
        for start in range(0, len(rows), 250):
            self.call('put', table, rows[start:start+250])
            if os.getenv('TEST262_PROGRESS') == '1':
                print('Uploaded', table, min(start+250, len(rows)), '/', len(rows),
                      'records in', round(time.monotonic()-started, 1), 'seconds', flush=True)

    def read(self, table, filters=None, page=500):
        """Yield one table's scoped rows in bounded primary-key pages."""
        after = None
        while True:
            result = self.db.execute('SELECT test262.api_read(%s,%s,%s,%s,%s)',
                                     (self.epoch, table, self.Jsonb(filters or {}),
                                      self.Jsonb(after) if after else None, page)).fetchone()[0]
            yield from result['rows']
            after = result['next']
            if not after:
                return

    def rows(self, table, **filters):
        return list(self.read(table, filters))

    def one(self, table, **filters):
        rows = list(self.read(table, filters, page=2))
        if len(rows) > 1:
            raise ValueError('Ambiguous catalogue lookup for ' + table)
        return rows[0] if rows else None

    @contextmanager
    def view(self):
        # Not a sequence-max cursor. Every page read inside shares one MVCC snapshot.
        with self.db.transaction():
            self.db.execute('SET TRANSACTION ISOLATION LEVEL REPEATABLE READ READ ONLY')
            yield self

    def export(self, output):
        """Stream a coherent scoped export as NDJSON; memory stays bounded by one page."""
        output = Path(output)
        output.parent.mkdir(parents=True, exist_ok=True)
        temporary = output.with_name(output.name + '.partial')
        digest = hashlib.sha256()
        counts = {}
        with self.view(), temporary.open('w', encoding='utf-8') as stream:
            token, as_of = self.db.execute('SELECT txid_current_snapshot()::text, transaction_timestamp()').fetchone()
            header = {'schema': 1, 'api': 1, 'epoch': self.epoch, 'repository_id': self.contract['repository_id'],
                      'snapshot_token': token, 'as_of': as_of.isoformat()}

            def emit(record):
                line = canonical(record) + '\n'
                digest.update(line.encode())
                stream.write(line)

            emit({'header': header})
            for table in EXPORT_TABLES:
                counts[table] = 0
                for row in self.read(table):
                    emit({'table': table, 'row': row})
                    counts[table] += 1
            stream.write(canonical({'footer': {'counts': counts, 'sha256': digest.hexdigest()}}) + '\n')
        temporary.replace(output)
        return dict(header, counts=counts, sha256=digest.hexdigest())


class Outbox:
    def __init__(self, filename, repository, producer, epoch):
        Path(filename).parent.mkdir(parents=True, exist_ok=True)
        self.db = sqlite3.connect(filename)
        self.db.execute('PRAGMA journal_mode=WAL')
        self.db.execute('PRAGMA synchronous=FULL')
        self.db.executescript('''
          CREATE TABLE IF NOT EXISTS scope(singleton INTEGER PRIMARY KEY CHECK(singleton=1), identity TEXT NOT NULL);
          CREATE TABLE IF NOT EXISTS messages(request_id TEXT PRIMARY KEY, payload TEXT NOT NULL, digest TEXT NOT NULL, receipt TEXT);
          CREATE TABLE IF NOT EXISTS completions(observation_id TEXT PRIMARY KEY, work_id TEXT NOT NULL, generation INTEGER NOT NULL, receipt TEXT);
        ''')
        scope = canonical([repository, producer, int(epoch)])
        self.db.execute('INSERT OR IGNORE INTO scope VALUES(1,?)', (scope,))
        if self.db.execute('SELECT identity FROM scope').fetchone()[0] != scope:
            raise ValueError('Outbox belongs to another repository, producer, or authority epoch')
        self.db.commit()

    def enqueue(self, rows, request_id=None):
        request_id = request_id or str(uuid.uuid4())
        payload = canonical(rows)
        with self.db:
            previous = self.db.execute('SELECT payload FROM messages WHERE request_id=?', (request_id,)).fetchone()
            if previous and previous[0] != payload:
                raise ValueError('Conflicting outbox request')
            self.db.execute('INSERT OR IGNORE INTO messages VALUES(?,?,?,NULL)', (request_id, payload, sha(rows)))
            for row in rows:
                if row.get('work_item_id'):
                    self.db.execute('INSERT OR IGNORE INTO completions VALUES(?,?,?,NULL)',
                                    (row['observation_id'], row['work_item_id'], row['lease_generation']))
        return request_id

    def flush(self, client):
        # Persist the exact server receipt only after ACK. Commit-after-timeout is safe to replay.
        for request, payload, digest in self.db.execute('SELECT request_id,payload,digest FROM messages WHERE receipt IS NULL ORDER BY rowid').fetchall():
            rows = json.loads(payload)
            if sha(rows) != digest:
                raise ValueError('Corrupt outbox')
            receipt = client.call('ingest', request, rows)
            with self.db:
                self.db.execute('UPDATE messages SET receipt=? WHERE request_id=?', (canonical(receipt), request))
            if os.getenv('TEST262_PROGRESS') == '1':
                print('Acknowledged observation batch:', len(rows), 'records', flush=True)
        for observation, work, generation in self.db.execute('SELECT observation_id,work_id,generation FROM completions WHERE receipt IS NULL ORDER BY rowid').fetchall():
            receipt = client.call('complete', work, generation, observation)
            with self.db:
                self.db.execute('UPDATE completions SET receipt=? WHERE observation_id=?', (canonical(receipt), observation))


def fixture_environment(source=None):
    source = os.environ if source is None else source
    # Allowlist, not a list of guessed secret names. No PG*, AWS*, GitHub tokens or DSN.
    allowed = ('PATH','HOME','TMPDIR','TEMP','TMP','LANG','LC_ALL','DOTNET_ROOT','DOTNET_NOLOGO',
               'DOTNET_SKIP_FIRST_TIME_EXPERIENCE','DOTNET_CLI_TELEMETRY_OPTOUT')
    return {key: source[key] for key in allowed if key in source}
