import copy
from decimal import Decimal
import hashlib
import json
from pathlib import Path
import tempfile
import unittest

from scripts.test262.central.client import EXPORT_TABLES
from scripts.test262.central.reconciliation import CONTRACT, logical_json, reconcile

IDENTITY = dict(repository_id='repo',epoch=2,schema=1,api=1)
HEADER = dict(IDENTITY,snapshot_token='1:2:',as_of='2026-10-24T00:00:00+00:00')


def row(table, **values):
    spec = CONTRACT[table]
    keys = set(spec['primary_key']) | set(spec.get('required',[])) | {k for ref in spec['references'] for k in ref['columns']}
    result = dict.fromkeys(keys)
    if 'repository_id' in keys:
        result['repository_id'] = 'repo'
    result.update(values)
    return table,result


def frozen():
    return [row('corpora',corpus_id='c',inventory_state='sealed',expected_fixture_count=1,revision='a'*40),
      row('repository_corpora',corpus_id='c'),
      row('fixtures',fixture_id='f',corpus_id='c',upstream_path='test/f.js',content_sha256='hash',metadata_state='valid',is_support_file=False,dependency_manifest_digest='\\x'+hashlib.sha256(b'[]').hexdigest()),
      row('fixture_variants',fixture_id='f',variant='strict',required=True),
      row('provenances',provenance_id='p',corpus_id='c',evidence_kind='native'),
      row('runs',run_id='r',producer_id='producer',provenance_id='p',trust_class='trusted',state='complete'),
      row('observations',observation_id='o',run_id='r',provenance_id='p',fixture_id='f',variant='strict',
          outcome='pass',phase='execution',source_kind='live'),
      row('observation_payloads',observation_id='o',availability='hot',payload={'number':Decimal('1.00'),'array':[1,2]})]


def write_export(path, rows, header=None):
    digest = hashlib.sha256()
    counts = dict.fromkeys(EXPORT_TABLES,0)
    with path.open('wb') as stream:
        def emit(record):
            data = (logical_json(record)+'\n').encode()
            digest.update(data);stream.write(data)
        emit({'header':header or HEADER})
        for table, data in rows:
            emit({'table':table,'row':data});counts[table]+=1
        stream.write((logical_json({'footer':{'counts':counts,'sha256':digest.hexdigest()}})+'\n').encode())
    return dict(header or HEADER,counts=counts,sha256=digest.hexdigest())


class ReconciliationTests(unittest.TestCase):
    def report(self, rows, **kwargs):
        with tempfile.TemporaryDirectory() as directory:
            path=Path(directory)/'snapshot.ndjson'
            receipt=write_export(path,rows)
            return reconcile(path,receipt,**kwargs)

    def test_frozen_content_reorders_rows_keys_and_decimal_scale(self):
        a=self.report(frozen())
        rows=list(reversed(frozen()))
        for i,(table,data) in enumerate(rows):
            rows[i]=(table,dict(reversed(list(data.items()))))
            if table=='observation_payloads':data['payload']['number']=Decimal('1e0')
        b=self.report(rows)
        self.assertTrue(a['verified']);self.assertTrue(b['verified'])
        self.assertEqual(a['logical_sha256'],b['logical_sha256'])
        self.assertNotEqual(a['export']['sha256'],b['export']['sha256'])
        self.assertEqual(set(a['tables']),set(EXPORT_TABLES))
        self.assertTrue(a['external_references_not_exported'])

    def test_content_and_array_order_changes_are_detected(self):
        a=self.report(frozen())
        rows=frozen();rows[-1][1]['payload']['array']=[2,1]
        self.assertNotEqual(a['logical_sha256'],self.report(rows)['logical_sha256'])
        self.assertNotEqual(logical_json(Decimal('9007199254740993.1')),logical_json(Decimal('9007199254740993.2')))
        self.assertEqual(logical_json(Decimal('-0.00')),'0')

    def test_snapshot_time_changes_receipt_but_not_logical_content(self):
        with tempfile.TemporaryDirectory() as directory:
            p=Path(directory)/'snapshot.ndjson'
            a=write_export(p,frozen());first=reconcile(p,a)
            b=write_export(p,frozen(),dict(HEADER,snapshot_token='3:4:',as_of='later'))
            self.assertEqual(first['logical_sha256'],reconcile(p,b)['logical_sha256'])
            with self.assertRaisesRegex(ValueError,'receipt mismatch'):reconcile(p,a)

    def test_corruption_partial_footer_identity_duplicate_rows_and_keys_rejected(self):
        with tempfile.TemporaryDirectory() as directory:
            p=Path(directory)/'snapshot.ndjson';receipt=write_export(p,frozen());original=p.read_bytes()
            for data in [original.replace(b'"pass"',b'"fail"'),b'\n'.join(original.splitlines()[:-1])+b'\n',original+b'{}\n',
                         original.replace(b'"epoch":2',b'"epoch":2,"epoch":2')]:
                p.write_bytes(data)
                with self.assertRaises(ValueError):reconcile(p,receipt)
            write_export(p,frozen()+[frozen()[0]])
            with self.assertRaisesRegex(ValueError,'Duplicate primary key'):reconcile(p,IDENTITY)
            foreign=frozen();foreign[2][1]['repository_id']='foreign';write_export(p,foreign)
            with self.assertRaisesRegex(ValueError,'Foreign repository'):reconcile(p,IDENTITY)

    def test_orphan_and_cross_provenance_relationships_are_not_healthy(self):
        rows=frozen();rows[-2][1]['run_id']='missing'
        report=self.report(rows)
        self.assertFalse(report['verified'])
        self.assertEqual(report['relationship_checks']['observations.repository_id,run_id->runs'],1)
        rows=frozen();rows.append(row('provenances',provenance_id='other',corpus_id='c',evidence_kind='native'))
        rows[-3][1]['provenance_id']='other'
        self.assertEqual(self.report(rows)['semantic_checks']['observations.run_provenance'],1)

    def test_missing_reference_field_is_rejected_not_treated_as_nullable(self):
        rows=frozen();del rows[-2][1]['work_item_id']
        with self.assertRaisesRegex(ValueError,'Missing identity/reference'):self.report(rows)

    def test_accepted_native_requires_complete_trusted_nonconflicting_variants(self):
        rows=frozen()+[row('provenance_fixture_eligibility',provenance_id='p',fixture_id='f',eligibility='runnable'),row('budget_scopes',budget_scope_id='budget',reserved_attempts=0,reserved_ms=0,charged_attempts=0,charged_ms=0,attempt_limit=10,time_limit_ms=10000,state='closed'),
          row('validation_events',validation_id='v',conclusion='success',target_revision='a'*40),
          row('registration_snapshots',snapshot_id='s',source_revision='a'*40,state='sealed'),
          row('native_batches',batch_id='b',provenance_id='p',validation_id='v',registration_snapshot_id='s',budget_scope_id='budget',state='sealed'),
          row('batch_fixtures',batch_id='b',fixture_id='f',state='accepted'),
          row('batch_evidence',batch_id='b',fixture_id='f',variant='strict',observation_id='o')]
        self.assertTrue(self.report(rows)['verified'])
        for change in ('missing','untrusted','conflict','invalidated','legacy','excluded','dependency'):
            broken=copy.deepcopy(rows)
            if change=='missing':broken.pop()
            if change=='untrusted':broken[5][1]['trust_class']='legacy'
            if change=='legacy':broken[6][1]['source_kind']='legacy-snapshot'
            if change=='excluded':broken[8][1]['eligibility']='policy-excluded'
            if change=='dependency':broken[2][1]['dependency_manifest_digest']='wrong'
            if change=='conflict':broken.append(row('observations',observation_id='other',run_id='r',provenance_id='p',fixture_id='f',variant='strict',outcome='fail',phase='execution',source_kind='live'))
            if change=='invalidated':broken.append(row('observation_invalidations',observation_id='o'))
            with self.subTest(change=change):self.assertEqual(sum(n for k,n in self.report(broken)['semantic_checks'].items() if k.startswith('native_acceptance.')),1)

    def test_history_entries_can_share_import_without_erasing_gaps(self):
        rows=frozen()+[row('imports',import_id='i',source_schema='native-v4',expected_counts={'attempts':1},imported_counts={'observations':1},limitations={'artifact_history_complete':False}),
          row('import_records',import_id='i',source_table='attempts',source_key='1',entity_kind='observation',entity_id='o')]
        manifest={'repository':'test/repo','artifacts':[],'gaps':[{'reason':'expired'}]*11}
        for aid in [1,2]:
            uri=f'github-actions:test/repo/test262-native-port.yml/10/{aid}'
            manifest['artifacts'].append(dict(workflow='test262-native-port.yml',run_id=10,artifact_id=aid,databases=[{'source_uri':uri}]))
            rows.append(row('legacy_control_records',import_id='i',source_table='artifact_source',source_key=str(aid),document={'uri':uri}))
        result=self.report(rows,manifest=manifest,repository_name='test/repo')
        self.assertTrue(result['verified']);h=result['history']
        self.assertEqual((h['source_entries'],h['distinct_imports'],h['unavailable_gaps']),(2,1,11))
        self.assertFalse(h['archive_preservation_verified']);self.assertFalse(h['history_complete'])
        rows.pop();self.assertFalse(self.report(rows,manifest=manifest,repository_name='test/repo')['verified'])

    def test_import_parity_missing_payload_and_conflict_are_explicit(self):
        rows=frozen()+[row('imports',import_id='i',source_schema='mvp-v1',expected_counts={'results':2},imported_counts={'observations':1},limitations={}),
          row('import_records',import_id='i',source_table='results',source_key='1',entity_kind='observation',entity_id='o')]
        rows.pop(7)
        report=self.report(rows)
        self.assertFalse(report['verified']);self.assertFalse(report['evidence']['imports'][0]['parity'])
        self.assertEqual(report['evidence']['observations_without_payload'],1)



class StoredHistoryTests(unittest.TestCase):
    report=ReconciliationTests.report
    def test_stored_receipts_join_without_original_archive(self):
        rows=frozen()+[row('imports',import_id='i',source_schema='native-v4',expected_counts={'attempts':1},imported_counts={'observations':1},
          limitations={'missing_artifacts':['See archived discovery manifest sha256:'+'a'*64,json.dumps([{'reason':'unavailable'}]*11)],'artifact_history_complete':False}),
          row('import_records',import_id='i',source_table='attempts',source_key='1',entity_kind='observation',entity_id='o')]
        for aid in (1,2):
            uri=f'github-actions:test/repo/test262-native-port.yml/10/{aid}'
            rows.append(row('legacy_control_records',import_id='i',source_table='artifact_source',source_key=str(aid),document={'uri':uri}))
            rows.append(row('legacy_control_records',import_id='i',source_table='history_verified_snapshot',source_key=str(aid),document={
              'source_uri':uri,'sha256':'b'*64,'archive_sha256':'c'*64,'observations':1}))
        result=self.report(rows,use_stored_history=True)
        self.assertTrue(result['verified']);h=result['history']
        self.assertEqual((h['source_entries'],h['distinct_imports'],h['unavailable_gaps']),(2,1,11))
        self.assertTrue(h['attribution_complete']);self.assertFalse(h['original_manifest_inspected'])
        self.assertFalse(h['archive_preservation_verified']);self.assertFalse(h['history_complete'])
        broken=copy.deepcopy(rows);broken[-1][1]['document']['archive_sha256']='invalid'
        self.assertFalse(self.report(broken,use_stored_history=True)['verified'])
        self.assertFalse(self.report(rows[:-1],use_stored_history=True)['verified'])
        self.assertFalse(self.report(frozen(),use_stored_history=True)['verified'])
        ambiguous=copy.deepcopy(rows)
        for table,data in copy.deepcopy(rows):
            if table in ('imports','import_records','legacy_control_records'):
                data['import_id']='other';ambiguous.append((table,data))
        self.assertFalse(self.report(ambiguous,use_stored_history=True)['verified'])

    def test_offline_deadline_interrupts_work(self):
        from unittest.mock import patch
        clock=[0]
        def now():clock[0]+=1;return clock[0]
        with patch('scripts.test262.central.reconciliation.time.monotonic',side_effect=now):
            import sqlite3
            with self.assertRaises(TimeoutError):self.report(frozen(),seconds=.1)


class ProjectionTests(unittest.TestCase):
    report=ReconciliationTests.report
    def test_version_one_digest_unchanged_by_storage_optimization(self):
        self.assertEqual(self.report(frozen())['logical_sha256'],
            'f6a7be8889dceb05f634a374f4b7d134f2ddfb83d47c00f1dd4612c3647b20dc')

    def test_unread_payload_and_legacy_document_still_change_logical_digest(self):
        a=frozen()+[row('imports',import_id='i',source_schema='native-v4',expected_counts={'attempts':0},imported_counts={'observations':0},limitations={}),
          row('legacy_control_records',import_id='i',source_table='historical_control',source_key='k',document={'raw':'first'})]
        b=copy.deepcopy(a);b[-1][1]['document']['raw']='second'
        self.assertNotEqual(self.report(a)['logical_sha256'],self.report(b)['logical_sha256'])

    def test_append_load_uses_typed_projections_and_binary_hashes(self):
        import sqlite3
        from scripts.test262.central.reconciliation import load_export,relationships
        with tempfile.TemporaryDirectory() as d,sqlite3.connect(':memory:') as db:
            p=Path(d)/'snapshot';receipt=write_export(p,frozen());events=[]
            load_export(db,p,receipt,progress=lambda **event:events.append(event))
            self.assertEqual(db.execute('SELECT length(digest) FROM observation_payloads').fetchone()[0],32)
            self.assertNotIn('payload',{c[1] for c in db.execute('PRAGMA table_info(observation_payloads)')})
            relationships(db)
            plan=db.execute('EXPLAIN QUERY PLAN SELECT 1 FROM fixtures p WHERE p.fixture_id=?',('f',)).fetchall()
            self.assertTrue(any('COVERING INDEX' in r[3] for r in plan),plan)
            self.assertTrue(any(e['stage']=='identity-index' for e in events))
            self.assertEqual(db.execute("SELECT typeof(required) FROM fixture_variants").fetchone()[0],'integer')

    def test_decimal_scaled_integer_identity_stays_numeric(self):
        import sqlite3
        from scripts.test262.central.reconciliation import load_export
        rows=frozen()+[row('work_leases',work_item_id='w',lease_generation=Decimal('1.00'),producer_id='producer',run_id='r')]
        with tempfile.TemporaryDirectory() as d,sqlite3.connect(':memory:') as db:
            p=Path(d)/'snapshot';receipt=write_export(p,rows);load_export(db,p,receipt)
            self.assertEqual(db.execute('SELECT lease_generation,typeof(lease_generation) FROM work_leases').fetchone(),(1,'integer'))



if __name__=='__main__':unittest.main()
