import json
from pathlib import Path
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import Mock

from scripts.test262.central import budgets, lifecycle


class BudgetRecoveryTests(unittest.TestCase):
    def test_bounded_dry_run_and_exact_scope(self):
        client=Mock()
        client.read.return_value=iter([{'repository_id':'repo','external_key':'actions:12','budget_scope_id':'b'},
                                       {'repository_id':'repo','external_key':'actions:13','budget_scope_id':'c'}])
        args=SimpleNamespace(repository='repo',limit=1,actions_run_id=12,apply=False)
        report=budgets.reconcile_budgets(client,args)
        self.assertEqual(len(report['budgets']),1)
        client.read.assert_called_once_with('budget_scopes',{'scope_kind':'run','external_key':'actions:12'})
        client.call.assert_called_once_with('finish_budget','b',False)

    def test_terminal_run_replay_recovers_interrupted_budget_finalization(self):
        with tempfile.TemporaryDirectory() as directory:
            context=Path(directory)/'context.json'
            context.write_text(json.dumps(dict(repository='repo',producer='producer',run='run',budget='b',revision='a'*40)))
            args=SimpleNamespace(repository='repo',producer='producer',context=context,run_key='actions:12:1')
            client=Mock()
            client.one.return_value=dict(repository_id='repo',producer_id='producer',run_id='run',
                trust_class='trusted',run_kind='native',external_run_key=args.run_key,source_revision='a'*40,
                state='complete',ended_at='2026-10-23T01:00:00Z')
            client.call.side_effect=[TimeoutError('lost budget response'),{'disposition':'unchanged'}]
            with self.assertRaises(TimeoutError):lifecycle.finalize(client,args)
            result=lifecycle.finalize(client,args)
            self.assertEqual(result['budget']['disposition'],'unchanged')
            self.assertEqual([c.args for c in client.call.call_args_list],[('finish_budget','b',True)]*2)
