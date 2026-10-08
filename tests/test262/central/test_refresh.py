from contextlib import ExitStack
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import Mock, patch

from scripts.test262.central import cli, commands


class RefreshTests(unittest.TestCase):
    def setUp(self):
        self.directory=tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)
        self.args=cli.parser().parse_args(['--repository','repo','--producer','producer','refresh',
            '--revision','current','--root',self.directory.name,'--output',self.directory.name+'/report.json'])
        self.client=Mock(contract={'deployment_state':'active'})
        self.client.one.return_value=None
        self.stack=ExitStack();self.addCleanup(self.stack.close)
        self.stack.enter_context(patch.object(commands,'git',side_effect=lambda *a:'current' if a==('rev-parse','HEAD') else ''))
        self.master=self.stack.enter_context(patch.object(commands,'current_master',return_value='current'))
        self.proof=self.stack.enter_context(patch.object(commands,'github_validation',return_value={'id':42,'html_url':'https://github.com/run/42'}))
        self.inventory=self.stack.enter_context(patch.object(commands,'prepare_inventory',return_value={'pid':'provenance','snapshot':'snapshot'}))
        self.start=self.stack.enter_context(patch.object(commands,'start_run'))

    def test_refresh_never_allocates_work_budget_claims_or_publication(self):
        result=commands.refresh(self.client,self.args)
        self.assertEqual(result['status'],'refreshed')
        self.assertFalse(result['screening_started']);self.assertFalse(result['publication_started'])
        self.start.assert_not_called()
        self.assertEqual([c.args[0] for c in self.client.put.call_args_list],['validation_events','reporting_targets'])
        self.client.call.assert_not_called()
        self.assertEqual(json.loads(Path(self.args.output).read_text()),result)

    def test_duplicate_validation_is_read_only(self):
        self.client.one.side_effect=[{'registration_snapshot_id':'snapshot','validation_id':'valid','provenance_id':'prov','version':3},
            {'source_revision':'current','state':'sealed'},{'target_revision':'current','conclusion':'success'}]
        self.assertEqual(commands.refresh(self.client,self.args)['status'],'already-current')
        self.inventory.assert_not_called();self.client.put.assert_not_called();self.client.call.assert_not_called()

    def test_obsolete_event_does_not_prepare_inventory(self):
        self.master.return_value='newer'
        self.assertEqual(commands.refresh(self.client,self.args)['status'],'superseded')
        self.proof.assert_not_called();self.inventory.assert_not_called();self.client.put.assert_not_called()

    def test_push_during_preparation_never_updates_reporting_target(self):
        self.master.side_effect=['current','newer']
        self.assertEqual(commands.refresh(self.client,self.args)['status'],'superseded')
        self.assertEqual([c.args[0] for c in self.client.put.call_args_list],['validation_events'])
        self.client.call.assert_not_called()

    def test_target_update_uses_observed_version(self):
        self.client.one.side_effect=[{'registration_snapshot_id':'old','validation_id':None,'version':8},
                                    {'source_revision':'older','state':'sealed'}]
        commands.refresh(self.client,self.args)
        args=self.client.call.call_args.args
        self.assertEqual(args[:2],('transition','reporting_targets'))
        self.assertEqual(args[3],8)
        self.assertEqual(args[4]['registration_snapshot_id'],'snapshot')

    def test_failed_exact_sha_ci_or_inactive_authority_cannot_mutate(self):
        self.proof.side_effect=ValueError('No exact master push validation')
        with self.assertRaises(ValueError):commands.refresh(self.client,self.args)
        self.inventory.assert_not_called();self.client.put.assert_not_called()
        self.client.contract['deployment_state']='shadow'
        with self.assertRaises(ValueError):commands.refresh(self.client,self.args)
        self.client.put.assert_not_called()

    def test_validation_rejects_pr_schedule_wrong_revision_and_failed_runs(self):
        runs=[dict(id=i,head_sha=sha,head_branch=branch,event=event,conclusion=conclusion,status='completed')
              for i,(sha,branch,event,conclusion) in enumerate([
                  ('current','master','pull_request','success'),('current','master','schedule','success'),
                  ('wrong','master','push','success'),('current','feature','push','success'),
                  ('current','master','push','failure')])]
        # Exercise the real validator, separately from the refresh test mock.
        self.stack.close()
        with patch.object(commands.subprocess,'check_output',return_value=json.dumps({'workflow_runs':runs})):
            with self.assertRaises(ValueError):commands.github_validation('tomacox74/js2il','current')
