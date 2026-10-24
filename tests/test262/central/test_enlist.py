import copy
import json
from pathlib import Path
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch

from scripts.test262.central import enlist


class EnlistTests(unittest.TestCase):
    def setUp(self):
        self.args=SimpleNamespace(repository='repo',producer='worker',root='/upstream',
            limit=1,seconds=60,run_key='session-1',output=None,outbox='/unused')
        self.context={'repository':'repo','producer':'coordinator','run':'prepare','budget':'budget',
            'kind':'native','provenance':'pid','corpus':'corpus','revision':'a'*40,
            'registration_snapshot':'snapshot','validation':'validation',
            'cap_ms':120000,'identity':{'upstream':{'commit':'b'*40},
                'timeouts':{'runtime':30,'compile':60,'cap_ms':120000}}}
        self.client=Mock()
        self.client.contract={'repository_id':'repo','producer_id':'worker','permission':'coordinator',
            'trust_class':'trusted','deployment_state':'active'}
        self.rows={'runs':{'trust_class':'trusted','producer_id':'coordinator',
                'source_revision':'a'*40,'provenance_id':'pid'},
            'provenances':{'corpus_id':'corpus','evidence_kind':'native','identity_document':self.context['identity']},
            'reporting_targets':{'provenance_id':'pid','registration_snapshot_id':'snapshot','validation_id':'validation'},
            'registration_snapshots':{'state':'sealed','source_revision':'a'*40},
            'validation_events':{'conclusion':'success','target_revision':'a'*40},
            'budget_scopes':{'state':'open','external_key':'shared-session'}}
        self.client.one.side_effect=lambda table,**kw:self.rows[table]

    def test_distinct_worker_identity_can_join_coordinator_context(self):
        self.assertEqual(enlist.check_scope(self.client,self.args,self.context),self.rows['budget_scopes'])
        self.client.call.assert_not_called()

    def test_scope_checks_fail_before_writes(self):
        cases=[('runs','producer_id','foreign'),('runs','trust_class','untrusted'),
            ('runs','source_revision','c'*40),('provenances','identity_document',{}),
            ('provenances','corpus_id','foreign'),('reporting_targets','provenance_id','old'),
            ('reporting_targets','validation_id','old'),('budget_scopes','state','closed'),
            ('registration_snapshots','state','draft'),('registration_snapshots','source_revision','old'),
            ('validation_events','conclusion','failure'),('validation_events','target_revision','old'),
            ('budget_scopes','external_key','actions:123')]
        original=copy.deepcopy(self.rows)
        for table,key,value in cases:
            with self.subTest(table=table,key=key):
                self.rows=copy.deepcopy(original);self.rows[table][key]=value
                with self.assertRaises(ValueError):
                    enlist.check_scope(self.client,self.args,self.context)
                self.client.call.assert_not_called()

    def test_repository_trust_and_authority_are_required(self):
        original=dict(self.client.contract)
        for key,value in [('repository_id','foreign'),('producer_id','other'),
                ('trust_class','untrusted'),('permission','ingest'),('deployment_state','shadow')]:
            self.client.contract=dict(original,**{key:value})
            with self.assertRaises(ValueError):
                enlist.check_scope(self.client,self.args,self.context)
        self.client.call.assert_not_called()

    def test_exact_clean_checkout_and_execution_identity(self):
        clean=['a'*40,'','b'*40,'']
        with patch.object(enlist.subprocess,'check_output',side_effect=clean), \
             patch.object(enlist,'execution_identity',return_value=self.context['identity']):
            self.assertEqual(enlist.check_checkout(self.args,self.context),self.context['identity'])
        for values in [['c'*40,'','b'*40,''],['a'*40,' M tracked','b'*40,''],
                       ['a'*40,'','c'*40,''],['a'*40,'','b'*40,' M harness']]:
            with patch.object(enlist.subprocess,'check_output',side_effect=values), \
                 patch.object(enlist,'execution_identity') as identity:
                with self.assertRaises(ValueError):
                    enlist.check_checkout(self.args,self.context)
                identity.assert_not_called()
        with patch.object(enlist.subprocess,'check_output',side_effect=clean), \
             patch.object(enlist,'execution_identity',return_value={'binaries':'different'}):
            with self.assertRaises(ValueError):
                enlist.check_checkout(self.args,self.context)

    def test_stale_target_blocks_next_claim_but_not_historical_ingestion(self):
        checked=enlist.CurrentTarget(self.client,self.args,self.context)
        self.rows['reporting_targets']['provenance_id']='old'
        with self.assertRaises(ValueError):
            checked.call('claim','run','budget',120000,150)
        self.client.call.assert_not_called()
        checked.call('ingest','request',[{'historical':True}])
        self.client.call.assert_called_once_with('ingest','request',[{'historical':True}])

    def test_invalid_bounds_or_action_identity_never_start_run(self):
        with tempfile.TemporaryDirectory() as directory:
            path=Path(directory)/'context.json';path.write_text(json.dumps(self.context))
            self.args.context=str(path)
            for limit,key in [(0,'session'),(1,'actions:123:1'),(1,'bad key')]:
                self.args.limit=limit;self.args.run_key=key
                with self.assertRaises(ValueError):
                    enlist.run(self.client,self.args)
            self.client.call.assert_not_called()

    def test_offline_preflight_never_executes_or_creates_outbox(self):
        with tempfile.TemporaryDirectory() as directory:
            path=Path(directory)/'context.json';path.write_text(json.dumps(self.context))
            self.args.context=str(path)
            self.client.one.side_effect=ConnectionError('offline')
            with patch.object(enlist,'work') as execute,patch.object(enlist,'Outbox') as spool:
                with self.assertRaises(ConnectionError):
                    enlist.run(self.client,self.args)
                execute.assert_not_called();spool.assert_not_called()
                self.client.call.assert_not_called()
