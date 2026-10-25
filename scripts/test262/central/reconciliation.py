"""Verify an existing coherent export offline; never connects to or writes PostgreSQL.

SQLite is a temporary, disposable index, not a catalogue authority. Reports contain
counts/digests only. Input rows, payloads, archive locations and credentials stay private.
"""
import argparse
from contextlib import closing
from decimal import Decimal
import hashlib
import json
from pathlib import Path
import sqlite3
import tempfile
import time

from .client import EXPORT_TABLES

CONTRACT = json.loads(Path(__file__).with_name('reconciliation_contract.json').read_text())
MAX_LINE = 32 * 1024 * 1024
COLUMNS = {table: set(spec['primary_key']) | set(spec.get('required',[])) |
           {k for ref in spec['references'] for k in ref['columns']} for table,spec in CONTRACT.items()}
for _spec in CONTRACT.values():
    for _ref in _spec['references']:
        if _ref['parent'] in COLUMNS:
            COLUMNS[_ref['parent']].update(_ref['parent_columns'])
COLUMNS = {table:tuple(sorted(columns)) for table,columns in COLUMNS.items()}


def logical_json(value):
    """Version-1 logical encoding: sorted keys, exact decimals, ordered arrays."""
    try:
        # Most rows contain no decimals. Use CPython's encoder for that common
        # case, retaining the existing exact-decimal encoding below.
        return json.dumps(value,sort_keys=True,ensure_ascii=False,allow_nan=False,separators=(',',':'))
    except TypeError:
        pass
    if isinstance(value, dict):
        return '{' + ','.join(json.dumps(k, ensure_ascii=False) + ':' + logical_json(v)
                              for k, v in sorted(value.items())) + '}'
    if isinstance(value, list):
        return '[' + ','.join(map(logical_json, value)) + ']'
    if isinstance(value, Decimal):
        if not value.is_finite():
            raise ValueError('Nonfinite JSON number')
        if abs(value.adjusted()) > 10000:
            raise ValueError('JSON number exponent exceeds logical encoding bound')
        text = format(value, 'f')
        if '.' in text:
            text = text.rstrip('0').rstrip('.')
        return '0' if value == 0 else text
    return json.dumps(value, ensure_ascii=False, allow_nan=False, separators=(',', ':'))


def decode(line):
    def pairs(items):
        result = {}
        for k, v in items:
            if k in result:
                raise ValueError('Duplicate JSON key')
            result[k] = v
        return result
    def invalid(value):
        raise ValueError('Nonfinite JSON number')
    return json.loads(line, parse_float=Decimal, parse_constant=invalid, object_pairs_hook=pairs)


def col(alias, field):
    # Identifiers come solely from the reviewed contract or fixed checks below.
    if not field.replace('_', '').isalnum():
        raise ValueError('Invalid contract column')
    return f'{alias}."{field}"'


def eq(a, b, fields):
    return ' AND '.join(f'{col(a,k)}={col(b,k)}' for k in fields)


def load_export(db, path, expected, check_deadline=lambda:None, progress=lambda **event:None):
    counts = dict.fromkeys(EXPORT_TABLES, 0)
    for table in EXPORT_TABLES:
        # Append first; build uniqueness and relationship indexes in sorted
        # passes instead of maintaining large random-key B-trees per record.
        db.execute(f'CREATE TABLE {table}(key TEXT NOT NULL,digest BLOB NOT NULL,'+
                   ','.join('"'+k+'"' for k in COLUMNS[table])+')')
    digest, whole = hashlib.sha256(), hashlib.sha256()
    header = footer = None
    with Path(path).open('rb') as stream:
        while True:
            check_deadline()
            line = stream.readline(MAX_LINE + 1)
            if not line:
                break
            if len(line) > MAX_LINE:
                raise ValueError('Oversized export record')
            whole.update(line)
            record = decode(line)
            if not isinstance(record, dict):
                raise ValueError('Invalid export record')
            if header is None:
                if set(record) != {'header'}:
                    raise ValueError('Missing export header')
                header = record['header']
                if not isinstance(header, dict) or header.get('schema') != 1 or header.get('api') != 1 or any(header.get(k) != expected[k] for k in ('repository_id','epoch','schema','api')):
                    raise ValueError('Wrong export identity')
                if not header.get('snapshot_token') or not header.get('as_of'):
                    raise ValueError('Missing snapshot identity')
            elif set(record) == {'footer'}:
                footer = record['footer']
                if stream.read(1):
                    raise ValueError('Data after export footer')
                break
            else:
                if set(record) != {'table','row'} or record['table'] not in counts or not isinstance(record['row'],dict):
                    raise ValueError('Unexpected export record')
                table, row = record['table'], record['row']
                spec = CONTRACT[table]
                required = set(COLUMNS[table])
                if not required <= row.keys() or any(row[k] is None for k in spec['primary_key']):
                    raise ValueError('Missing identity/reference fields: ' + table)
                if 'repository_id' in row and row['repository_id'] != header['repository_id']:
                    raise ValueError('Foreign repository row')
                data = logical_json(row)
                key = logical_json([row[k] for k in spec['primary_key']])
                values=[]
                for field in COLUMNS[table]:
                    value=row[field]
                    # Non-attribution legacy documents are bound by the full
                    # row digest but never read by any semantic check.
                    if table=='legacy_control_records' and field=='document' and row['source_table'] not in ('artifact_source','history_verified_snapshot'):
                        value=None
                    if isinstance(value,Decimal):
                        # JSON numeric scale is insignificant. Preserve numeric
                        # SQLite comparison for scalar integer identities/counts.
                        integer=int(value)
                        value=integer if value==integer and -(2**63)<=integer<2**63 else float(value)
                    elif isinstance(value,(dict,list)):
                        value=logical_json(value)
                    values.append(value)
                db.execute(f'INSERT INTO {table} VALUES('+','.join('?' for _ in range(2+len(values)))+')',
                           (key,hashlib.sha256(data.encode()).digest(),*values))
                counts[table] += 1
                if counts[table]%100000==0:progress(stage='load',table=table,rows=counts[table])
            digest.update(line)
    if not header or footer != {'counts':counts, 'sha256':digest.hexdigest()}:
        raise ValueError('Incomplete export or count/digest mismatch')
    actual = dict(header, **footer)
    if any(actual.get(k) != expected[k] for k in ('snapshot_token','as_of','counts','sha256') if k in expected):
        raise ValueError('Independent export receipt mismatch')
    db.commit()
    for table in EXPORT_TABLES:
        check_deadline();progress(stage='identity-index',table=table,rows=counts[table])
        try:
            db.execute(f'CREATE UNIQUE INDEX pk_{table} ON {table}(key)')
        except sqlite3.IntegrityError as error:
            raise ValueError('Duplicate primary key: '+table) from error
    return dict(header, **footer, file_sha256=whole.hexdigest())


def relationships(db,progress=lambda **event:None):
    checks, external = {}, []
    indexed = set()
    for table, spec in CONTRACT.items():
        for ref in spec['references']:
            parent, fields = ref['parent'], ref['parent_columns']
            name = table + '.' + ','.join(ref['columns']) + '->' + parent
            if parent not in CONTRACT:
                external.append(name)
                continue
            index = (parent, tuple(fields))
            if index not in indexed:
                progress(stage='relationship-index',table=parent)
                db.execute(f'CREATE INDEX r{len(indexed)} ON {parent}(' + ','.join('"'+k+'"' for k in fields) + ')')
                indexed.add(index)
            nonnull = ' AND '.join(col('c',k)+' IS NOT NULL' for k in ref['columns'])
            match = ' AND '.join(f'{col("c",a)}={col("p",b)}' for a,b in zip(ref['columns'],fields))
            sql = f'SELECT count(*) FROM {table} c WHERE {nonnull} AND NOT EXISTS(SELECT 1 FROM {parent} p WHERE {match})'
            progress(stage='relationships',table=table,check=name)
            checks[name] = db.execute(sql).fetchone()[0]
    return checks, sorted(external)


def stored_history(db):
    """Join only durable source/verification/gap records in this exact export.

    This does not reconstruct or claim inspection of the original 4 GB archive.
    """
    sources = {}
    receipts = {}
    for import_id,source_table,raw_document in db.execute("SELECT import_id,source_table,document FROM legacy_control_records WHERE source_table IN ('artifact_source','history_verified_snapshot')"):
        document=json.loads(raw_document)
        uri=document.get('uri' if source_table=='artifact_source' else 'source_uri')
        if not isinstance(uri,str) or not uri.startswith('github-actions:'):
            raise ValueError('Invalid stored source attribution')
        target=sources if source_table=='artifact_source' else receipts
        key=(import_id,uri)
        if key in target:
            raise ValueError('Ambiguous stored source attribution')
        target[key]=document
    entries=[];uri_imports={}
    for (import_id,uri),source in sorted(sources.items()):
        uri_imports.setdefault(uri,set()).add(import_id)
        proof=receipts.get((import_id,uri),{})
        mapped=db.execute("SELECT count(*) FROM import_records WHERE import_id=? AND entity_kind='observation'",(import_id,)).fetchone()[0]
        hashes=[proof.get(k) for k in ('sha256','archive_sha256')]
        valid=all(isinstance(h,str) and len(h)==64 and all(c in '0123456789abcdef' for c in h) for h in hashes)
        entries.append({'source_key_sha256':hashlib.sha256(uri.encode()).hexdigest(),'import_id':import_id,
                        'source_file_sha256':proof.get('sha256'),'archive_sha256':proof.get('archive_sha256'),
                        'receipt_observations':proof.get('observations'),'mapped_observations':mapped,
                        'receipt_matches':bool(valid and proof.get('observations')==mapped)})
    gaps={};manifest_hashes=set()
    for (data,) in db.execute('SELECT limitations FROM imports'):
        for value in json.loads(data).get('missing_artifacts',[]):
            if not isinstance(value,str):
                raise ValueError('Invalid stored gap record')
            if value.startswith('See archived discovery manifest sha256:'):
                manifest_hashes.add(value.split(':',1)[1])
            elif value.startswith('['):
                parsed=decode(value)
                if not isinstance(parsed,list):raise ValueError('Invalid stored gap list')
                gaps[logical_json(parsed)]=parsed
    lists=[gaps[k] for k in sorted(gaps)]
    return {'state':'stored_receipts_joined','source_entries':len(entries),
            'attribution_complete':bool(entries and len(lists)==1 and manifest_hashes and
                not any(len(ids)>1 for ids in uri_imports.values()) and
                not any(not e['receipt_matches'] for e in entries) and not (receipts.keys()-sources.keys()) and
                all(len(h)==64 and all(c in '0123456789abcdef' for c in h) for h in manifest_hashes)),
            'distinct_imports':len({e['import_id'] for e in entries}), 'entries':entries,
            'unmatched_entries':sum(not e['receipt_matches'] for e in entries),
            'ambiguous_entries':sum(len(ids)>1 for ids in uri_imports.values()),
            'orphan_receipts':len(receipts.keys()-sources.keys()),
            'recorded_manifest_sha256':sorted(manifest_hashes),'distinct_gap_lists':len(lists),
            'unavailable_gaps':len(lists[0]) if len(lists)==1 else None,
            'gap_ledger_sha256':hashlib.sha256(logical_json(lists).encode()).hexdigest(),
            'ledger_sha256':hashlib.sha256(logical_json(entries).encode()).hexdigest(),
            'original_manifest_inspected':False,'archive_preservation_verified':False,'history_complete':False}


def semantics(db,progress=lambda **event:None):
    checks = {}
    def check(name, query):
        progress(stage='semantics',check=name)
        checks[name] = db.execute(query).fetchone()[0]
    check('corpora.repository_scope', f'''SELECT count(*) FROM corpora c WHERE NOT EXISTS(
      SELECT 1 FROM repository_corpora r WHERE {eq('r','c',['corpus_id'])})''')
    # Repository/corpus scope is checked through the same parent relationships as
    # the exporter. These extra joins catch legal FKs with mismatched provenance.
    for table in ('observations','work_items','provenance_fixture_eligibility'):
        check(table+'.fixture_corpus', f'''SELECT count(*) FROM {table} x JOIN fixtures f
          ON {eq('x','f',['fixture_id'])} JOIN provenances p ON {eq('x','p',['provenance_id'])}
          WHERE {col('f','corpus_id')} IS NOT {col('p','corpus_id')}''')
    check('observations.run_provenance', f'''SELECT count(*) FROM observations x JOIN runs r ON {eq('x','r',['run_id'])}
      WHERE {col('x','provenance_id')} IS NOT {col('r','provenance_id')}''')
    check('work_items.completion_identity', f'''SELECT count(*) FROM work_items w JOIN observations o
      ON {col('w','completion_observation_id')}={col('o','observation_id')}
      WHERE {' OR '.join(col('w',k)+' IS NOT '+col('o',k) for k in ['repository_id','provenance_id','fixture_id','variant','work_item_id','lease_generation'])}''')
    check('batch_evidence.identity', f'''SELECT count(*) FROM batch_evidence e JOIN observations o ON {eq('e','o',['observation_id'])}
      JOIN native_batches b ON {eq('e','b',['batch_id'])}
      WHERE {col('o','provenance_id')} IS NOT {col('b','provenance_id')}
         OR {col('o','fixture_id')} IS NOT {col('e','fixture_id')} OR {col('o','variant')} IS NOT {col('e','variant')}''')
    check('reporting_targets.kind', f'''SELECT count(*) FROM reporting_targets t JOIN provenances p ON {eq('t','p',['provenance_id'])}
      WHERE {col('t','evidence_kind')} IS NOT {col('p','evidence_kind')}''')
    check('reporting_targets.validation', f'''SELECT count(*) FROM reporting_targets t
      JOIN validation_events v ON {eq('t','v',['validation_id'])}
      JOIN registration_snapshots s ON {col('t','registration_snapshot_id')}={col('s','snapshot_id')}
      WHERE {col('t','evidence_kind')}='native' AND
      ({col('v','conclusion')} IS NOT 'success' OR {col('v','target_revision')} IS NOT {col('s','source_revision')})''')
    check('native_batches.sealed_scope', f'''SELECT count(*) FROM native_batches b
      JOIN provenances p ON {eq('b','p',['provenance_id'])}
      JOIN validation_events v ON {eq('b','v',['validation_id'])}
      JOIN registration_snapshots s ON {col('b','registration_snapshot_id')}={col('s','snapshot_id')}
      WHERE {col('b','state')}='sealed' AND ({col('p','evidence_kind')} IS NOT 'native'
       OR {col('v','conclusion')} IS NOT 'success' OR {col('s','state')} IS NOT 'sealed'
       OR {col('v','target_revision')} IS NOT {col('s','source_revision')})''')
    check('sealed_inventory.fixture_count', f'''SELECT count(*) FROM corpora c WHERE {col('c','inventory_state')}='sealed'
      AND {col('c','expected_fixture_count')}<>(SELECT count(*) FROM fixtures f WHERE {eq('c','f',['corpus_id'])})''')
    check('annotations.fixture_identity', f'''SELECT count(*) FROM fixture_specification_metadata a
      JOIN fixtures f ON {eq('a','f',['fixture_id'])} JOIN corpora c ON {eq('f','c',['corpus_id'])}
      WHERE {col('a','upstream_revision')} IS NOT {col('c','revision')} OR {col('a','upstream_path')} IS NOT {col('f','upstream_path')}
      OR {col('a','content_sha256')} IS NOT {col('f','content_sha256')}''')
    check('registrations.path', f'''SELECT count(*) FROM registrations a JOIN fixtures f ON {eq('a','f',['fixture_id'])}
      WHERE {col('a','upstream_path')} IS NOT {col('f','upstream_path')}''')
    check('budget_scopes.reservations', f'''SELECT count(*) FROM budget_scopes b WHERE
      {col('b','reserved_attempts')}<>(SELECT coalesce(sum({col('r','reserved_attempts')}),0) FROM budget_reservations r
       WHERE {eq('b','r',['budget_scope_id'])} AND {col('r','state')}='reserved') OR
      {col('b','reserved_ms')}<>(SELECT coalesce(sum({col('r','reserved_ms')}),0) FROM budget_reservations r
       WHERE {eq('b','r',['budget_scope_id'])} AND {col('r','state')}='reserved')''')
    check('budget_scopes.limits', f'''SELECT count(*) FROM budget_scopes b WHERE
      {col('b','charged_attempts')}+{col('b','reserved_attempts')}>{col('b','attempt_limit')} OR
      {col('b','charged_ms')}+{col('b','reserved_ms')}>{col('b','time_limit_ms')}''')
    check('import_records.observations', f'''SELECT count(*) FROM import_records m WHERE {col('m','entity_kind')}='observation'
      AND NOT EXISTS(SELECT 1 FROM observations o WHERE {col('o','observation_id')}={col('m','entity_id')})''')
    # Accepted fixtures in a sealed batch must be backed by every required variant,
    # trusted passing evidence at the same provenance, and no invalidation/conflict.
    # Conflicted/awaiting-refresh batches are reported separately, never repaired.
    check('native_acceptance.sealed_evidence', f'''SELECT count(*) FROM batch_fixtures f
      JOIN native_batches b ON {eq('f','b',['batch_id'])}
      JOIN fixtures fixture ON {eq('f','fixture',['fixture_id'])}
      WHERE {col('f','state')}='accepted' AND {col('b','state')}='sealed' AND (
       {col('fixture','metadata_state')} IS NOT 'valid' OR {col('fixture','is_support_file')}=1
       OR NOT EXISTS(SELECT 1 FROM provenance_fixture_eligibility eligible
         WHERE {eq('eligible','f',['fixture_id'])} AND {eq('eligible','b',['provenance_id'])}
          AND {col('eligible','eligibility')}='runnable')
       OR
       NOT EXISTS(SELECT 1 FROM fixture_variants v WHERE {eq('f','v',['fixture_id'])} AND {col('v','required')}=1)
       OR EXISTS(SELECT 1 FROM fixture_variants v WHERE {eq('f','v',['fixture_id'])} AND {col('v','required')}=1
         AND NOT EXISTS(SELECT 1 FROM batch_evidence e JOIN observations o ON {eq('e','o',['observation_id'])}
          JOIN runs r ON {eq('o','r',['run_id'])}
          WHERE {eq('e','f',['batch_id','fixture_id'])} AND {eq('e','v',['variant'])}
           AND {col('o','provenance_id')}={col('b','provenance_id')} AND {col('o','outcome')}='pass'
           AND {col('r','trust_class')}='trusted' AND {col('o','source_kind')}='live'
           AND NOT EXISTS(SELECT 1 FROM observation_invalidations i WHERE {eq('o','i',['observation_id'])})))
       OR EXISTS(SELECT 1 FROM observations o JOIN runs r ON {eq('o','r',['run_id'])}
         WHERE {eq('o','b',['provenance_id'])} AND {eq('o','f',['fixture_id'])}
          AND {col('r','trust_class')}='trusted' AND {col('o','outcome')}<>'pass'
          AND NOT EXISTS(SELECT 1 FROM observation_invalidations i WHERE {eq('o','i',['observation_id'])})))''')
    checks['native_acceptance.dependency_manifest'] = 0
    for fixture_id,manifest_digest in db.execute(f'''SELECT DISTINCT fixture.fixture_id,fixture.dependency_manifest_digest FROM fixtures fixture
      JOIN batch_fixtures f ON {eq('fixture','f',['fixture_id'])} JOIN native_batches b ON {eq('f','b',['batch_id'])}
      WHERE {col('f','state')}='accepted' AND {col('b','state')}='sealed' '''):
        dependencies=[]
        for path,content_hash,kind,resolved in db.execute(f'''SELECT d.dependency_path,d.content_sha256,d.dependency_kind,{col('r','upstream_path')} FROM fixture_dependencies d
          LEFT JOIN fixtures r ON {col('d','resolved_fixture_id')}={col('r','fixture_id')}
          WHERE {col('d','fixture_id')}=? ORDER BY {col('d','dependency_path')}''',(fixture_id,)):
            dependencies.append(dict(dependency_path=path,content_sha256=content_hash.removeprefix('\\x'),
                                     dependency_kind=kind,resolved_path=resolved))
        expected='\\x'+hashlib.sha256(logical_json(dependencies).encode()).hexdigest()
        checks['native_acceptance.dependency_manifest'] += manifest_digest != expected
    return checks


def evidence_summary(db,progress=lambda **event:None):
    # Reports exposure rather than silently declaring missing/conflicted evidence
    # acceptable. This is a reconciliation, not a new native-acceptance algorithm.
    result = {}
    queries = {
      'invalidated_observations':'SELECT count(*) FROM observation_invalidations',
      'missing_payloads':"SELECT count(*) FROM observation_payloads p WHERE p.availability='missing'",
      'observations_without_payload':f'SELECT count(*) FROM observations o WHERE NOT EXISTS(SELECT 1 FROM observation_payloads p WHERE {eq("o","p",["observation_id"])})',
      'publication_attention':"SELECT count(*) FROM publications p WHERE p.state IN ('blocked','ownership-lost')",
      'historical_imports_incomplete':"SELECT count(*) FROM imports i WHERE coalesce(json_extract(i.limitations,'$.artifact_history_complete'),0)=0",
      'batches_requiring_refresh_or_conflicted':"SELECT count(*) FROM native_batches b WHERE b.state IN ('awaiting-refresh','conflicted')",
      'trusted_conflicting_variants':f'''SELECT count(*) FROM (SELECT {col('o','provenance_id')},{col('o','fixture_id')},{col('o','variant')}
        FROM observations o JOIN runs r ON {eq('o','r',['run_id'])} WHERE {col('r','trust_class')}='trusted'
        AND NOT EXISTS(SELECT 1 FROM observation_invalidations i WHERE {eq('o','i',['observation_id'])})
        GROUP BY {col('o','provenance_id')},{col('o','fixture_id')},{col('o','variant')}
        HAVING count(DISTINCT json_array({col('o','outcome')},{col('o','phase')},{col('o','observed_error_type')}))>1)''',
    }
    for k, query in queries.items():
        progress(stage='evidence',check=k)
        result[k] = db.execute(query).fetchone()[0]
    imports = []
    for import_id,source_schema,expected_counts,imported_counts,limitations in db.execute('SELECT import_id,source_schema,expected_counts,imported_counts,limitations FROM imports ORDER BY key'):
        progress(stage='import-parity')
        row=dict(import_id=import_id,source_schema=source_schema,expected_counts=json.loads(expected_counts),imported_counts=json.loads(imported_counts),limitations=json.loads(limitations))
        mapped, uploaded = db.execute(f'''SELECT count(*),count({col('o','observation_id')}) FROM import_records m
          LEFT JOIN observations o ON {col('o','observation_id')}={col('m','entity_id')}
          WHERE {col('m','import_id')}=? AND {col('m','entity_kind')}='observation' ''',(row['import_id'],)).fetchone()
        expected = row.get('expected_counts',{}).get('results',row.get('expected_counts',{}).get('attempts'))
        reported = row.get('imported_counts',{}).get('observations')
        imports.append({'source_schema':row.get('source_schema'),'expected':expected,'mapped':mapped,'uploaded':uploaded,
                        'reported':reported,'parity':expected==mapped==uploaded==reported,
                        'history_complete':row.get('limitations',{}).get('artifact_history_complete',False)})
    result['imports'] = imports
    return result


def join_history(db, manifest, repository_name):
    if manifest.get('repository') != repository_name or not isinstance(manifest.get('gaps'),list):
        raise ValueError('History manifest identity/gaps missing')
    sources = []
    for artifact in manifest['artifacts']:
        for source in artifact['databases']:
            uri = f'github-actions:{repository_name}/{artifact["workflow"]}/{artifact["run_id"]}/{artifact["artifact_id"]}'
            if source['source_uri'] != uri:
                raise ValueError('History source attribution mismatch')
            matches = db.execute("""SELECT DISTINCT import_id FROM legacy_control_records
              WHERE source_table='artifact_source' AND json_extract(document,'$.uri')=?""",(uri,)).fetchall()
            # Every distinct archive entry remains visible even when source bytes
            # deduplicate to one immutable import ID (62 entries != 61 imports).
            sources.append({'source_key_sha256':hashlib.sha256(uri.encode()).hexdigest(),
                            'matches':[r[0] for r in matches]})
    ids = {item for source in sources for item in source['matches']}
    return {'state':'attribution_joined','manifest_logical_sha256':hashlib.sha256(logical_json(manifest).encode()).hexdigest(),
            'source_entries':len(sources),'distinct_imports':len(ids),
            'entries':sources,
            'unmatched_entries':sum(not s['matches'] for s in sources),
            'ambiguous_entries':sum(len(s['matches'])>1 for s in sources),
            'unavailable_gaps':len(manifest['gaps']),
            'gap_ledger_sha256':hashlib.sha256(logical_json(manifest['gaps']).encode()).hexdigest(),
            'archive_preservation_verified':False,'history_complete':False}


def reconcile(path, expected, *, manifest=None, repository_name=None, scratch=None, use_stored_history=False, seconds=None, progress=lambda **event:None):
    if seconds is not None and seconds<=0:raise ValueError('Reconciliation deadline must be positive')
    deadline=time.monotonic()+seconds if seconds is not None else None
    def check_deadline():
        if deadline is not None and time.monotonic()>=deadline:
            raise TimeoutError('Offline reconciliation deadline exceeded')
    with tempfile.TemporaryDirectory(prefix='test262-reconcile-',dir=scratch) as directory, \
         closing(sqlite3.connect(Path(directory)/'index.sqlite')) as db:
        db.execute('PRAGMA temp_store=FILE')
        db.execute('PRAGMA cache_size=-262144')
        if deadline is not None:
            db.set_progress_handler(lambda:int(time.monotonic()>=deadline),10000)
        receipt = load_export(db,path,expected,check_deadline,progress)
        refs, external = relationships(db,progress)
        progress(stage='query-indexes')
        db.execute('CREATE INDEX import_mapping ON import_records(import_id,entity_kind)')
        db.execute('CREATE INDEX source_records ON legacy_control_records(source_table)')
        db.execute('CREATE INDEX dependency_fixture ON fixture_dependencies(fixture_id)')
        checks = semantics(db,progress)
        evidence = evidence_summary(db,progress)
        tables = {}
        root = hashlib.sha256()
        # Snapshot token/as_of are receipt metadata, not logical database content.
        root.update((logical_json({k:receipt[k] for k in ('repository_id','epoch','schema','api')})+'\n').encode())
        for table in sorted(EXPORT_TABLES):
            check_deadline()
            progress(stage='logical-digest',table=table)
            digest = hashlib.sha256()
            for (row_hash,) in db.execute(f'SELECT digest FROM {table} ORDER BY digest,key'):
                digest.update(row_hash.hex().encode()+b'\n')
            tables[table] = {'rows':receipt['counts'][table],'logical_sha256':digest.hexdigest(),
                             'relationship_failures':sum(n for k,n in refs.items() if k.startswith(table+'.'))}
            root.update((logical_json([table,tables[table]['rows'],digest.hexdigest()])+'\n').encode())
        progress(stage='history')
        history = (join_history(db,manifest,repository_name) if manifest is not None else stored_history(db) if use_stored_history
                   else {'state':'not_supplied','archive_preservation_verified':False})
        return {'report_schema':1,'logical_digest_version':1,'export':receipt,'tables':tables,
                'logical_sha256':root.hexdigest(),'relationship_checks':refs,'semantic_checks':checks,
                'external_references_not_exported':external,'evidence':evidence,'history':history,
                'verified':not any(refs.values()) and not any(checks.values()) and all(i['parity'] for i in evidence['imports'])
                  and not history.get('unmatched_entries',0) and not history.get('ambiguous_entries',0)
                  and (not use_stored_history or history.get('attribution_complete',False)),
                'limitations':['Scoped to one verified export; does not establish production equivalence to an earlier snapshot.',
                  'Nonexported producers, execution_events, retry_decisions, API bindings, projections and performance data are not reconciled.',
                  'Native acceptance remains governed by catalogue APIs/reporting; this report grants no execution/publication authority.',
                  'Source attribution/import parity does not verify retained archive bytes, restore objectives or complete history.']}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--export',required=True)
    parser.add_argument('--receipt',required=True,help='Independent expected export receipt')
    parser.add_argument('--history-manifest')
    parser.add_argument('--repository-name')
    parser.add_argument('--scratch',help='Private temporary disk with capacity for the indexed export')
    parser.add_argument('--output',required=True)
    args = parser.parse_args()
    expected = json.loads(Path(args.receipt).read_text())
    report = reconcile(args.export,expected,manifest=json.loads(Path(args.history_manifest).read_text()) if args.history_manifest else None,
                       repository_name=args.repository_name,scratch=args.scratch)
    if args.history_manifest:
        report['history']['manifest_file_sha256']=hashlib.sha256(Path(args.history_manifest).read_bytes()).hexdigest()
    if any(report['export'].get(k)!=expected[k] for k in ('snapshot_token','as_of','counts','sha256') if k in expected):
        raise ValueError('Independent export receipt mismatch')
    output = Path(args.output)
    output.parent.mkdir(parents=True,exist_ok=True)
    output.write_text(json.dumps(report,indent=2)+'\n')
    raise SystemExit(0 if report['verified'] else 1)


if __name__ == '__main__':
    main()
