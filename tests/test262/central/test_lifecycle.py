import json
from pathlib import Path
from types import SimpleNamespace
import tempfile
import unittest
from unittest.mock import Mock, patch

from scripts.test262.central import lifecycle as life
from scripts.test262.central.client import Outbox


def row():
    return dict(run_id='run', repository_id='repo', producer_id='producer', trust_class='trusted',
                run_kind='native', external_run_key='actions:123:2', source_revision='a'*40,
                state='running', started_at='2026-10-23T01:00:00Z', ended_at=None)


def history(conclusion='success', flush='success'):
    run = dict(id=123,run_attempt=2,head_sha='a'*40,head_branch='master',status='completed',
               repository={'full_name':'test/repo'},head_repository={'full_name':'test/repo'},
               path='.github/workflows/test262-central.yml')
    job = dict(name='supervise',run_id=123,run_attempt=2,head_sha='a'*40,status='completed',
               completed_at='2026-10-23T01:01:00Z',conclusion=conclusion,
               steps=[{'name':life.PREPARE,'conclusion':'success'}, {'name':life.FLUSH,'conclusion':flush}])
    return run,job


class LifecycleTests(unittest.TestCase):
    def plan(self, run, job):
        reader=Mock(side_effect=[[run],[{'jobs':[job]}]])
        result=life.actions_plan(row(),'test/repo',reader)
        self.assertIn('/attempts/2',reader.call_args_list[0].args[0])
        return result

    def test_complete_collection_does_not_require_tests_to_pass(self):
        run,job=history()
        self.assertEqual(self.plan(run,job)['state'],'complete')
        job['steps'].append({'name':'tests failed: 6','conclusion':'success'})
        self.assertEqual(self.plan(run,job)['state'],'complete')

    def test_failed_cancelled_interrupted_and_unproven_flush(self):
        for conclusion,flush,expected in [('failure','success','failed'),('cancelled','skipped','cancelled'),
                                          ('timed_out','skipped','incomplete'),('success','failure','incomplete')]:
            with self.subTest(conclusion=conclusion,flush=flush):
                self.assertEqual(self.plan(*history(conclusion,flush))['state'],expected)

    def test_exact_attempt_sha_repo_workflow_and_timestamp_are_required(self):
        for field,value in [('run_attempt',3),('head_sha','b'*40),('head_branch','feature'),
                            ('repository',{'full_name':'foreign/repo'}),('path','other.yml')]:
            run,job=history();run[field]=value
            self.assertEqual(self.plan(run,job)['disposition'],'identity-mismatch')
        run,job=history();job['completed_at']='2026-10-23T00:00:00Z'
        self.assertEqual(self.plan(run,job)['disposition'],'invalid-timestamp')
        run,job=history();job['run_attempt']=1
        self.assertEqual(self.plan(run,job)['disposition'],'unproven-supervisor')

    def test_active_ambiguous_and_unknown_history_do_not_finalize(self):
        run,job=history();run['status']='in_progress'
        self.assertEqual(self.plan(run,job)['disposition'],'active')
        run,job=history();reader=Mock(side_effect=[[run],[{'jobs':[job,job]}]])
        self.assertEqual(life.actions_plan(row(),'test/repo',reader)['disposition'],'ambiguous-supervisor')
        run,job=history('neutral')
        self.assertEqual(self.plan(run,job)['disposition'],'unknown-conclusion')

    def test_dry_run_legacy_isolation_unavailable_and_exact_attempt_filter(self):
        client=Mock();client.read.return_value=[dict(row(),trust_class='legacy'),row()]
        args=SimpleNamespace(repository='repo',producer='producer',limit=100,actions_run_id=123,
                             actions_run_attempt=2,repository_name='test/repo',apply=False)
        with patch.object(life,'actions_plan',return_value={'disposition':'terminal','state':'complete'}):
            self.assertEqual(len(life.reconcile_runs(client,args)['runs']),1)
        client.call.assert_not_called()
        self.assertEqual(client.read.call_args.args[1]['external_run_key'],'actions:123:2')
        client.read.return_value=[row()]
        with patch.object(life,'actions_plan',side_effect=OSError('private message')):
            report=life.reconcile_runs(client,args)
        self.assertNotIn('private message',json.dumps(report))
        client.call.assert_not_called()

    def test_flush_precedes_finalization_and_interrupted_finalization_can_resume(self):
        with tempfile.TemporaryDirectory() as folder:
            context=Path(folder)/'context.json';outbox=Path(folder)/'outbox.sqlite'
            context.write_text(json.dumps(dict(repository='repo',producer='producer',run='run',revision='a'*40)))
            out=Outbox(outbox,'repo','producer',1)
            out.enqueue([{'observation_id':'o','work_item_id':'w','lease_generation':1}]);out.db.close()
            args=SimpleNamespace(repository='repo',producer='producer',context=context,outbox=outbox,
                                 run_key='actions:123:2',conclusion='success',flush_verified=True)
            client=Mock(epoch=1);client.one.return_value=row()
            calls=[]
            def call(op,*values):
                calls.append(op)
                if op=='finish_run':
                    if calls.count('finish_run')==1:
                        raise TimeoutError('lost finalization response')
                    return dict(row(),state=values[1],ended_at=values[2])
                return {'ack':True}
            client.call.side_effect=call
            with self.assertRaises(TimeoutError):life.finalize(client,args)
            self.assertEqual(life.finalize(client,args)['state'],'complete')
            self.assertEqual(calls,['ingest','complete','finish_run','finish_run'])

    def test_failed_flush_cannot_record_complete_and_terminal_retry_is_unchanged(self):
        with tempfile.TemporaryDirectory() as folder:
            p=Path(folder)/'context.json';p.write_text(json.dumps(dict(repository='repo',producer='producer',run='run',revision='a'*40)))
            args=SimpleNamespace(repository='repo',producer='producer',context=p,outbox=Path(folder)/'missing',
                                 run_key='actions:123:2',conclusion='success',flush_verified=False)
            client=Mock();client.one.return_value=row()
            client.call.return_value=dict(row(),state='incomplete',ended_at='2026-10-23T01:01:00Z')
            self.assertEqual(life.finalize(client,args)['state'],'incomplete')
            self.assertEqual(client.call.call_args.args[2],'incomplete')
            client.one.return_value=client.call.return_value;client.call.reset_mock()
            self.assertTrue(life.finalize(client,args)['unchanged']);client.call.assert_not_called()
