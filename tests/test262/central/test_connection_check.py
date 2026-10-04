import os
import unittest
from unittest.mock import MagicMock, patch

from scripts.test262.central.connection_check import CheckError, check, failure_message


class ConnectionCheckTests(unittest.TestCase):
    settings = {'TEST262_DATABASE_URL': 'postgresql://test262_importer.project:example@db.example/postgres',
                'TEST262_REPOSITORY_ID': 'repository', 'TEST262_PRODUCER_ID': 'producer',
                'TEST262_AUTHORITY_EPOCH': '1'}

    def connection(self, login='test262_importer', **changes):
        contract = {'repository_id': 'repository', 'producer_id': 'producer',
                    'minimum_writer_epoch': 1, 'api_contract_version': 1,
                    'permission': 'coordinator', 'trust_class': 'legacy',
                    'deployment_state': 'schema-only'}
        contract.update(changes)
        db = MagicMock()
        db.__enter__.return_value = db
        def execute(sql):
            cursor = MagicMock()
            if sql == 'SHOW transaction_read_only':
                cursor.fetchone.return_value = ('on',)
            elif sql.startswith('SELECT session_user'):
                cursor.fetchone.return_value = (login, contract)
            return cursor
        db.execute.side_effect = execute
        return db

    def test_explicit_readonly_transaction_and_tls_without_startup_options(self):
        db = self.connection()
        with patch.dict(os.environ, self.settings), patch('psycopg.connect', return_value=db) as connect:
            check('test262_importer', 'legacy')
        self.assertEqual(connect.call_args.kwargs['sslmode'], 'require')
        self.assertNotIn('options', connect.call_args.kwargs)
        queries = [call.args[0] for call in db.execute.call_args_list]
        self.assertEqual(queries[0], 'BEGIN READ ONLY')
        self.assertEqual(queries[-1], 'ROLLBACK')
        self.assertFalse(any('INSERT' in q or 'UPDATE' in q for q in queries))

    def test_wrong_producer_trust_and_active_authority_rejected(self):
        for change in ({'producer_id': 'another'}, {'trust_class': 'trusted'}, {'deployment_state': 'active'}):
            with self.subTest(change=change), patch.dict(os.environ, self.settings), \
                    patch('psycopg.connect', return_value=self.connection(**change)), self.assertRaises(ValueError):
                check('test262_importer', 'legacy')

    def test_tls_downgrade_rejected_before_connect(self):
        values = dict(self.settings, TEST262_DATABASE_URL=self.settings['TEST262_DATABASE_URL'] + '?sslmode=disable')
        with patch.dict(os.environ, values), patch('psycopg.connect') as connect, self.assertRaises(ValueError):
            check('test262_importer', 'legacy')
        connect.assert_not_called()

    def test_wrong_login_and_binding_mismatch_have_specific_safe_diagnostics(self):
        for changes, expected in (
                ({'login': 'postgres'}, 'Wrong database login'),
                ({'producer_id': 'another'}, 'Catalogue binding mismatch: producer_id')):
            with self.subTest(changes=changes), patch.dict(os.environ, self.settings), \
                    patch('psycopg.connect', return_value=self.connection(**changes)):
                with self.assertRaises(CheckError) as caught:
                    check('test262_importer', 'legacy')
                self.assertIn(expected, failure_message(caught.exception))
                self.assertNotIn('postgresql://', failure_message(caught.exception))

    def test_external_error_text_is_never_displayed(self):
        sensitive = 'postgresql://user:private-password@host/database'
        for error in (ValueError(sensitive), RuntimeError(sensitive)):
            self.assertNotIn(sensitive, failure_message(error))
            self.assertNotIn('private-password', failure_message(error))

    def test_invalid_epoch_reports_setting_without_value_or_connecting(self):
        values = dict(self.settings, TEST262_AUTHORITY_EPOCH='private-value')
        with patch.dict(os.environ, values), patch('psycopg.connect') as connect:
            with self.assertRaises(CheckError) as caught:
                check('test262_importer', 'legacy')
            self.assertIn('TEST262_AUTHORITY_EPOCH must be an integer', failure_message(caught.exception))
            self.assertNotIn('private-value', failure_message(caught.exception))
        connect.assert_not_called()
