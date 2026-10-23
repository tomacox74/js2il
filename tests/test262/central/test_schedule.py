import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest

from scripts.test262.central.schedule import collection_kind


class ScheduledPolicyTests(unittest.TestCase):
    def test_schedule_selects_native_and_manual_keeps_mvp_default(self):
        self.assertEqual(collection_kind('schedule'),'native')
        self.assertEqual(collection_kind('workflow_dispatch'),'mvp-composite')
        self.assertEqual(collection_kind('workflow_dispatch','native'),'native')
        for event,kind in [('pull_request','native'),('workflow_dispatch','unknown')]:
            with self.assertRaises(ValueError):collection_kind(event,kind)

    def test_actual_workflow_command_retains_nonpublishing_policy_receipt(self):
        with tempfile.TemporaryDirectory() as directory:
            output=Path(directory)/'schedule.json';github=Path(directory)/'github-output'
            subprocess.run([sys.executable,'-m','scripts.test262.central.schedule','--event','schedule',
                            '--output',str(output),'--github-output',str(github)],check=True)
            policy=json.loads(output.read_text())
            self.assertEqual(github.read_text(),'kind=native\n')
            self.assertEqual(policy['event'],'schedule')
            self.assertFalse(policy['automatic_publication'])
            workflow=Path('.github/workflows/test262-central.yml').read_text()
            self.assertEqual(workflow.count("cron: '15 6 * * 1'"),1)
            self.assertIn('KIND: ${{ steps.collection_plan.outputs.kind }}',workflow)
            self.assertIn("github.event_name == 'workflow_dispatch' && inputs.kind == 'native' && inputs.publish == true",workflow)
