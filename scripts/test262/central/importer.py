"""Read-only, replayable schema-1 MVP and schema-4 native snapshot import.

Never upgrades the source database. Legacy runs cannot seal native acceptance.
"""
from datetime import datetime, timezone
import hashlib
import json
from pathlib import Path
import sqlite3
from .client import bytea, canonical, identity, Outbox, sha
from .inventory import normalize_fixture, register


def timestamp(seconds):
    return datetime.fromtimestamp(float(seconds), timezone.utc).isoformat()


def open_snapshot(filename):
    db = sqlite3.connect(Path(filename).resolve().as_uri() + '?mode=ro', uri=True)
    db.row_factory = sqlite3.Row
    db.execute('PRAGMA query_only=ON')
    return db


def source_hash(db):
    # A semantic content digest is stable across WAL/checkpoint/vacuum and row order.
    digest = hashlib.sha256()
    for (table,) in db.execute("SELECT name FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%' ORDER BY name"):
        digest.update((table + '\n').encode())
        for row in sorted(canonical(dict(r)) for r in db.execute('SELECT * FROM "' + table.replace('"', '""') + '"')):
            digest.update((row + '\n').encode())
    return digest.hexdigest()


def import_snapshot(client, args):
    db = open_snapshot(args.source)
    tables = {r[0] for r in db.execute("SELECT name FROM sqlite_master WHERE type='table'")}
    if 'settings' in tables:
        schema = dict(db.execute('SELECT key,value FROM settings')).get('schema')
        if schema != '1':
            raise ValueError('Only MVP schema 1 is supported; source remains unchanged')
        kind = 'mvp-v1'
    elif 'meta' in tables:
        schema = dict(db.execute('SELECT key,value FROM meta')).get('schema_version')
        if schema != '4':
            raise ValueError('Only native schema 4 is supported; source remains unchanged')
        kind = 'native-v4'
    else:
        raise ValueError('Unknown catalogue schema')
    digest = source_hash(db)
    imported = identity(args.repository, digest, kind)
    counts = {table: db.execute('SELECT count(*) FROM "' + table.replace('"','""') + '"').fetchone()[0] for table in sorted(tables) if not table.startswith('sqlite_')}
    limitations = {'artifact_history_complete': False, 'missing_artifacts': args.missing_artifacts or [],
                   'metadata_and_dependencies': 'legacy inventory unresolved; fresh inventory must verify closure',
                   'native_control_state': 'preserved in legacy_control_records; not promoted to active authority',
                   'mvp_history': 'overwritten attempts cannot be recovered', 'source_counts': counts}
    client.put('imports', [{'import_id': imported, 'repository_id': args.repository, 'source_sha256': bytea(digest),
                            'source_schema': kind, 'source_uri': 'sha256:'+digest,
                            'expected_counts': counts}])
    excluded={'fixtures','results','provenance','attempts','sqlite_sequence'}
    source_uri=args.source_uri or Path(args.source).name
    client.put('legacy_control_records',[{'import_id':imported,'source_table':'artifact_source','source_key':sha(source_uri),'document':{'uri':source_uri}}])
    control_count=1
    for table in sorted(tables-excluded):
        rows=sorted(canonical(dict(row)) for row in db.execute('SELECT * FROM "'+table.replace('"','""')+'"'))
        client.put('legacy_control_records',[{'import_id':imported,'source_table':table,'source_key':sha(json.loads(row)),
                                             'document':json.loads(row)} for row in rows])
        control_count+=len(rows)
    outbox = Outbox(args.outbox, args.repository, args.producer, client.epoch)
    if kind == 'mvp-v1':
        observed = import_mvp(client, db, args, imported, outbox)
    else:
        observed = import_native(client, db, args, imported, outbox)
    outbox.flush(client)
    # Completeness describes historical coverage, not whether this one snapshot uploaded.
    client.call('transition', 'imports', {'import_id': imported}, 0,
                {'state': 'incomplete', 'imported_counts': {'observations': observed,'legacy_control_records':control_count}, 'limitations': limitations})
    db.close()
    return {'import_id': imported, 'observations': observed, 'state': 'incomplete', 'limitations': limitations}


def provenance(client, repository, corpus, kind, document):
    pid = identity(repository, kind, sha(document))
    client.put('provenances', [{'provenance_id': pid, 'repository_id': repository, 'corpus_id': corpus,
                              'evidence_kind': kind, 'identity_version': 1, 'identity_sha256': bytea(sha(document)),
                              'identity_document': document, 'capability_sha256': bytea(sha(document.get('capabilities', {})))}])
    return pid


def start_run(client, args, pid, key, revision, kind='import'):
    run = identity(args.repository, args.producer, key)
    client.call('start_run', {'run_id': run, 'provenance_id': pid, 'external_run_key': key,
                              'source_revision': revision, 'run_kind': kind})
    return run


def observation(args, run, pid, fixture, row, key):
    phase = row.get('phase', 'unknown')
    if phase not in ('load','parse','early','resolution','compile','runtime','execution','timeout','planning','unknown'):
        phase = 'unknown'
    return {'observation_id': identity(args.repository, 'legacy-observation', key), 'repository_id': args.repository,
            'run_id': run, 'provenance_id': pid, 'fixture_id': fixture, 'variant': row['variant'],
            'outcome': row['outcome'], 'phase': phase, 'observed_error_type': row.get('observed_error_type'),
            'failure_class': row.get('failure_class'), 'diagnostic_summary': row.get('diagnostic','')[-8000:],
            'started_at': timestamp(row['started_at']), 'finished_at': timestamp(row['finished_at']),
            'active_ms': max(0, int((row['finished_at']-row['started_at'])*1000)), 'source_kind': 'legacy-snapshot',
            'payload': row}


def import_mvp(client, db, args, imported, outbox):
    count = 0
    settings = dict(db.execute('SELECT key,value FROM settings'))
    for item in db.execute('SELECT * FROM provenance ORDER BY id'):
        print('Processing MVP provenance:', item['id'], flush=True)
        checkpoint_key = sha(item['id'])
        checkpoint = client.one('legacy_control_records', import_id=imported,
                                source_table='mvp_import_checkpoint', source_key=checkpoint_key)
        expected_observations = db.execute('SELECT count(*) FROM results WHERE provenance=?', (item['id'],)).fetchone()[0]
        if checkpoint:
            if checkpoint['document'] != {'observations': expected_observations}:
                raise ValueError('MVP import checkpoint count mismatch')
            count += expected_observations
            print('Reusing completed provenance:', expected_observations, 'observations', flush=True)
            continue
        doc = json.loads(item['document'])
        fixtures = []
        originals = list(db.execute('SELECT * FROM fixtures WHERE provenance=? ORDER BY path', (item['id'],)))
        for f in originals:
            row = dict(f)
            row['variants'] = json.loads(row['variants'])
            fixtures.append(normalize_fixture(row))
        corpus, ids = register(client, args.repository, doc['upstream'], fixtures, reuse_sealed=True)
        pid = provenance(client, args.repository, corpus, 'mvp-composite', {'legacy_provenance': item['id'], 'identity': doc, 'legacy_inventory': corpus})
        run = start_run(client, args, pid, 'legacy-mvp:' + item['id'], settings.get('compiler_commit') or doc['upstream']['commit'])
        eligibility = [{'repository_id': args.repository, 'provenance_id': pid, 'fixture_id': ids[f['path']],
                        'eligibility': 'runnable' if f['state']=='runnable' else 'unresolved', 'reason_codes': [],
                        'diagnostic': {'legacy_state': f['state'], 'reasons': json.loads(f['reasons'])}} for f in originals]
        client.put('provenance_fixture_eligibility', eligibility)
        records = []
        mappings = []
        for result in db.execute('SELECT * FROM results WHERE provenance=? ORDER BY path,variant', (item['id'],)):
            raw = dict(result)
            raw['document'] = json.loads(raw['document'])
            row = dict(raw, started_at=raw['finished'], finished_at=raw['finished'],
                       outcome='pass' if raw['verdict']=='matched' else 'fail', failure_class=None if raw['verdict']=='matched' else 'unresolved',
                       diagnostic=canonical(raw['document']))
            records.append(observation(args, run, pid, ids[raw['path']], row, [item['id'],raw['path'],raw['variant'],raw['finished'],raw['document']]))
            mappings.append({'import_id':imported,'source_table':'results','source_key':canonical([item['id'],raw['path'],raw['variant']]),
                             'entity_kind':'observation','entity_id':records[-1]['observation_id']})
            count += 1
            if len(records)==100:
                outbox.enqueue(records, identity(imported, count))
                client.put('import_records', mappings)
                records=[]; mappings=[]
        if records:
            outbox.enqueue(records, identity(imported, count))
            client.put('import_records', mappings)
        # ACK all observations before recording the immutable completion checkpoint.
        outbox.flush(client)
        client.put('legacy_control_records', [{'import_id': imported, 'source_table': 'mvp_import_checkpoint',
                    'source_key': checkpoint_key, 'document': {'observations': expected_observations}}])
        print('Completed provenance; total observations:', count, flush=True)
    return count


def import_native(client, db, args, imported, outbox):
    count = 0
    upstream_url = json.loads((Path(__file__).resolve().parents[3]/'tests/test262/test262.pin.json').read_text())['upstream']['cloneUrl']
    if not args.root:
        raise ValueError('Native import requires --root containing the complete pinned inventory')
    from .inventory import REPO
    import sys
    sys.path.insert(0, str(REPO/'scripts/test262'))
    import catalog
    full_inventory = catalog.bridge({'command':'inventory','root':str(Path(args.root).resolve())})
    for run in db.execute('SELECT * FROM runs ORDER BY run_id'):
        if run['pin'] != json.loads((REPO/'tests/test262/test262.pin.json').read_text())['upstream']['commit']:
            raise ValueError('Native history pin does not match the materialized full inventory')
        candidates = list(db.execute('SELECT * FROM candidates WHERE run_id=? ORDER BY path', (run['run_id'],)))
        if not candidates:
            continue
        actual = {r['path']: r['sha256'] for r in full_inventory}
        if any(actual.get(c['path']) != c['sha256'] for c in candidates):
            raise ValueError('Legacy fixture bytes do not match the pinned full inventory')
        rows = [normalize_fixture(r, args.root) for r in full_inventory]
        corpus, ids = register(client, args.repository, {'cloneUrl':upstream_url,'commit':run['pin']}, rows)
        for attempt in db.execute('SELECT * FROM attempts WHERE run_id=? ORDER BY attempt_id', (run['run_id'],)):
            row = dict(attempt)
            doc = {'compiler':row['compiler_identity'],'harness':row['harness_identity'],'environment':row['environment_identity'],'pin':row['pin'],'legacy_inventory':corpus}
            pid = provenance(client,args.repository,corpus,'native',doc)
            rid = start_run(client,args,pid,'legacy-native:'+run['run_id']+':'+sha(doc),run['trigger_revision'])
            # Do not deduplicate real reruns having distinct original run/attempt identity.
            key = [run['run_id'],row['attempt_id'],row]
            outbox.enqueue([observation(args,rid,pid,ids[row['path']],row,key)],identity(imported,row['attempt_id']))
            client.put('import_records',[{'import_id':imported,'source_table':'attempts','source_key':str(row['attempt_id']),
                                          'entity_kind':'observation','entity_id':identity(args.repository,'legacy-observation',key)}])
            count += 1
    return count
