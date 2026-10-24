import hashlib
import io
import json
from pathlib import Path
import subprocess
import unittest
from unittest.mock import Mock
import zipfile

from scripts.test262.central import monitoring
from scripts.test262.central.monitoring_dashboard import alerts, extend, SIGNALS


class ReceiptTests(unittest.TestCase):
    def setUp(self):
        self.document={'backup_verified':True,'contains_performance_data':False,
            'bucket_retention_verified':True,'stage':'complete','checked_at':'2026-10-23T03:50:00+00:00',
            'objects':{name:{'version_id':'version','sha256':'a'*64,'retention_verified':True,
                'retain_until':'2026-11-27T03:50:00+00:00'} for name in ('catalogue.dump','manifest.json')},
            'bucket':'not-for-dashboard','password':'must-not-be-retained'}

    def artifact(self,document=None,names=('backup-report.json',)):
        stream=io.BytesIO()
        with zipfile.ZipFile(stream,'w',zipfile.ZIP_DEFLATED) as archive:
            for name in names:
                archive.writestr(name,json.dumps(document or self.document))
        raw=stream.getvalue()
        return {'id':123,'size_in_bytes':len(raw),'digest':'sha256:'+hashlib.sha256(raw).hexdigest()},raw

    def test_receipt_requires_both_exact_objects_and_strips_sensitive_fields(self):
        metadata,raw=self.artifact()
        result=monitoring.receipt_summary(metadata,raw,'backup-report.json','backup')
        self.assertTrue(result['receipt_verified']);self.assertTrue(result['retention_verified'])
        self.assertNotIn('must-not-be-retained',json.dumps(result))
        self.assertNotIn('not-for-dashboard',json.dumps(result))
        del self.document['objects']['manifest.json']
        metadata,raw=self.artifact()
        with self.assertRaises(ValueError):
            monitoring.receipt_summary(metadata,raw,'backup-report.json','backup')

    def test_tamper_expiry_ambiguous_reports_and_false_success_are_not_healthy(self):
        metadata,raw=self.artifact()
        for patch in ({'digest':'sha256:'+'b'*64},{'expired':True},{'size_in_bytes':2_000_001}):
            with self.subTest(patch=patch),self.assertRaises(ValueError):
                monitoring.receipt_summary(dict(metadata,**patch),raw,'backup-report.json','backup')
        metadata,raw=self.artifact(names=('a/backup-report.json','b/backup-report.json'))
        with self.assertRaises(ValueError):
            monitoring.receipt_summary(metadata,raw,'backup-report.json','backup')
        self.document['backup_verified']=False
        metadata,raw=self.artifact()
        self.assertFalse(monitoring.receipt_summary(metadata,raw,'backup-report.json','backup')['receipt_verified'])

    def test_restore_requires_shadow_authority_disabled_bindings_and_no_cutover(self):
        document=dict(self.document,restore_verified=True,independent_retention_verified=True,
            production_cutover=False,writer_bindings_disabled=True,restored_authority='shadow')
        metadata,raw=self.artifact(document,names=('restore-report.json',))
        self.assertTrue(monitoring.receipt_summary(metadata,raw,'restore-report.json','restore')['receipt_verified'])
        for field,value in [('production_cutover',True),('writer_bindings_disabled',False),('restored_authority','active')]:
            metadata,raw=self.artifact(dict(document,**{field:value}),names=('restore-report.json',))
            self.assertFalse(monitoring.receipt_summary(metadata,raw,'restore-report.json','restore')['receipt_verified'])

    def test_failed_latest_run_keeps_previous_verified_success_separate(self):
        metadata,raw=self.artifact()
        success={'id':20,'run_attempt':2,'head_sha':'a'*40,'head_branch':'master','status':'completed',
            'conclusion':'success','event':'schedule','updated_at':'2026-10-23T03:50:00Z',
            'head_repository':{'full_name':'owner/repo'}}
        latest=dict(success,id=21,conclusion='failure')
        metadata.update(name='test262-catalogue-backup-20-2',workflow_run={'id':20,'head_sha':'a'*40})
        def fetch(path,binary=False):
            if path.endswith('/runs?per_page=20'):
                return {'workflow_runs':[success,latest,dict(success,id=99,head_repository={'full_name':'fork/repo'})]}
            if '/runs/20/artifacts?' in path:
                return {'artifacts':[metadata]}
            if binary:
                return raw
            raise AssertionError(path)
        result=monitoring.collect_check('owner/repo','backup',fetch)
        self.assertEqual(result['latest_run_id'],21)
        self.assertEqual(result['latest_conclusion'],'failure')
        self.assertEqual(result['success_run_id'],20)
        self.assertTrue(result['receipt_verified'])
        metadata['workflow_run']['head_sha']='b'*40
        self.assertIsNone(monitoring.collect_check('owner/repo','backup',fetch)['receipt_verified'])

    def test_unavailable_or_absent_workflow_is_unknown_without_exception_text(self):
        fetch=Mock(side_effect=subprocess.CalledProcessError(1,['gh'],stderr='secret'))
        result=monitoring.collect_check('owner/repo','backup',fetch)
        self.assertEqual(result['fetch_state'],'unavailable')
        self.assertIsNone(result['receipt_verified'])
        self.assertNotIn('secret',json.dumps(result))
        result=monitoring.collect_check('owner/repo','backup',lambda *a,**kw:{'workflow_runs':[]})
        self.assertEqual(result,{'fetch_state':'observed'})
        with self.assertRaises(ValueError):
            monitoring.collect('owner/repo/../../other',Mock())


class DashboardPolicyTests(unittest.TestCase):
    def test_all_rules_are_paused_and_unknown_data_is_not_normal(self):
        rules=alerts()['groups'][0]['rules']
        self.assertEqual(len(rules),len(SIGNALS))
        for rule in rules:
            self.assertTrue(rule['isPaused'])
            self.assertEqual(rule['noDataState'],'NoData')
            self.assertEqual(rule['execErrState'],'Error')
            self.assertNotIn('notification_settings',rule)
            self.assertIn('breach IS NOT NULL',rule['data'][0]['model']['rawSql'])

    def test_owned_panel_regeneration_is_idempotent_and_preserves_existing_panels(self):
        dashboard={'description':'Baseline','panels':[{'id':1,'type':'text','gridPos':{'x':0,'y':0,'w':24,'h':3}}]}
        original=json.loads(json.dumps(dashboard['panels'][0]))
        first=json.dumps(extend(dashboard),sort_keys=True)
        second=json.dumps(extend(dashboard),sort_keys=True)
        self.assertEqual(first,second)
        self.assertEqual(dashboard['panels'][0],original)
        self.assertEqual(len({p['id'] for p in dashboard['panels']}),len(dashboard['panels']))

    def test_monitor_has_no_fixture_or_publication_permissions(self):
        root=Path(__file__).resolve().parents[3]
        workflow=(root/'.github/workflows/test262-operational-monitor.yml').read_text()
        self.assertIn('actions: read',workflow)
        self.assertNotIn('pull-requests: write',workflow)
        self.assertNotIn('contents: write',workflow)
        self.assertNotIn('central.cli work',workflow)
        self.assertNotIn('central.cli publish',workflow)
