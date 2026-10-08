import hashlib
from contextlib import nullcontext, redirect_stderr, redirect_stdout
from datetime import datetime, timezone
import io
import json
import os
from pathlib import Path
import tempfile
import sys
import unittest
from unittest.mock import MagicMock, patch

from scripts.test262.central import inventory
from scripts.test262 import catalog
from scripts.test262.central import cli, diagnostics
from scripts.test262.central.client import Client


class RecordingClient:
    def __init__(self):
        self.tables = {}

    def put(self, table, rows):
        self.tables.setdefault(table, []).extend(rows)

    def call(self, *args):
        return {}


class RegistrationPathTests(unittest.TestCase):
    def setUp(self):
        alias = patch.dict(sys.modules, {'catalog': catalog})
        alias.start()
        self.addCleanup(alias.stop)

    def fixture(self, repo, source, name, content):
        root = repo / 'tests/Jroc.Test262.Tests'
        caller = root / source
        caller.parent.mkdir(parents=True, exist_ok=True)
        caller.write_text('void Test() => ExecutionTestFromFile("' + name + '");')
        fixture = caller.parent / 'JavaScript' / (name + '.js')
        fixture.parent.mkdir(parents=True, exist_ok=True)
        fixture.write_bytes(content)
        return fixture

    def test_nested_registration_resolves_from_callers_javascript_directory(self):
        with tempfile.TemporaryDirectory() as directory:
            repo = Path(directory)
            fixture = self.fixture(repo, 'built-ins/ArrayBuffer/ExecutionTests.cs',
                                   'isView/example', b'assert(true);')
            client = RecordingClient()
            with patch.object(inventory, 'REPO', repo):
                inventory.registrations(client, 'repository', 'revision', {})
            rows = client.tables['registrations']
            self.assertEqual(len(rows), 1)
            self.assertEqual(rows[0]['upstream_path'], 'test/built-ins/ArrayBuffer/isView/example.js')
            self.assertEqual(rows[0]['source_file'], 'built-ins/ArrayBuffer/ExecutionTests.cs')
            self.assertEqual(rows[0]['native_fixture_sha256'], '\\x' + hashlib.sha256(fixture.read_bytes()).hexdigest())

    def test_same_upstream_path_hashes_each_callers_actual_fixture(self):
        with tempfile.TemporaryDirectory() as directory:
            repo = Path(directory)
            sources = {
                'built-ins/ArrayBuffer/ExecutionTests.cs': ('isView/example', b'parent fixture'),
                'built-ins/ArrayBuffer/isView/ExecutionTests.cs': ('example', b'leaf fixture'),
            }
            expected = {source: '\\x' + hashlib.sha256(self.fixture(repo, source, name, content).read_bytes()).hexdigest()
                        for source, (name, content) in sources.items()}
            client = RecordingClient()
            with patch.object(inventory, 'REPO', repo):
                inventory.registrations(client, 'repository', 'revision', {})
            rows = client.tables['registrations']
            self.assertEqual({r['source_file']: r['native_fixture_sha256'] for r in rows}, expected)
            self.assertEqual({r['upstream_path'] for r in rows}, {'test/built-ins/ArrayBuffer/isView/example.js'})


class DiagnosticTests(unittest.TestCase):
    def test_missing_repository_file_is_identified_without_exception_message(self):
        missing = diagnostics.REPO / 'tests/test262/missing.js'
        try:
            with diagnostics.stage('prepare.registrations'):
                raise FileNotFoundError(2, 'SECRET_CONNECTION_STRING', str(missing))
        except FileNotFoundError as error:
            result = diagnostics.failure_record(error)
        self.assertEqual(result['stage'], 'prepare.registrations')
        self.assertEqual(result['missing_path'], 'tests/test262/missing.js')
        self.assertTrue(result['locations'])
        self.assertNotIn('SECRET_CONNECTION_STRING', json.dumps(result))

    def test_connection_errors_and_unsafe_filenames_are_redacted(self):
        secret = 'postgresql://writer:secret@host/db?sslmode=require'
        error = RuntimeError(secret)
        error.sqlstate = '42501'
        result = diagnostics.failure_record(error)
        self.assertEqual(result['sqlstate'], '42501')
        self.assertNotIn(secret, json.dumps(result))
        for filename in (secret, '/private/password', str(diagnostics.REPO / 'bad\n::warning::secret')):
            with self.subTest(filename=filename):
                result = diagnostics.failure_record(FileNotFoundError(2, secret, filename))
                self.assertEqual(result['missing_path'], '<outside repository or redacted>')
        self.assertEqual(diagnostics.failure_record(FileNotFoundError(2, secret, 'node'))['missing_path'], 'node')

    def test_cli_preserves_sanitized_failure_and_nonzero_exit(self):
        with tempfile.TemporaryDirectory() as directory:
            diagnostic = Path(directory) / 'diagnostic.json'
            secret = 'postgresql://writer:secret@host/db'
            stderr = io.StringIO()
            with patch.dict(os.environ, {'TEST262_DIAGNOSTIC_FILE': str(diagnostic)}), \
                 patch.object(cli, 'Client', side_effect=RuntimeError(secret)), redirect_stderr(stderr):
                code = cli.run(['--repository', 'repository', '--producer', 'producer',
                                'export', '--output', str(Path(directory) / 'snapshot.ndjson')])
            result = json.loads(diagnostic.read_text())
            self.assertEqual(code, 1)
            self.assertEqual(result['stage'], 'connect')
            self.assertEqual(result['error_type'], 'RuntimeError')
            self.assertNotIn(secret, stderr.getvalue() + diagnostic.read_text())


class ExportDeadlineTests(unittest.TestCase):
    def client(self, rows):
        client = Client.__new__(Client)
        client.epoch = 2
        client.contract = {'repository_id': 'repository'}
        client.db = MagicMock()
        client.db.execute.return_value.fetchone.return_value = ('snapshot-token', datetime.now(timezone.utc))
        client.view = lambda: nullcontext()
        client.read = lambda table: rows()
        return client

    def test_completed_export_has_matching_footer_and_no_payload_in_progress(self):
        client = self.client(lambda: iter([{'observation_id': 'one', 'payload': 'PRIVATE_PAYLOAD'}]))
        stdout = io.StringIO()
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / 'snapshot.ndjson'
            with patch('scripts.test262.central.client.EXPORT_TABLES', ('observations',)), \
                 patch.dict(os.environ, {'TEST262_PROGRESS': '1'}), redirect_stdout(stdout):
                result = client.export(output, seconds=30)
            lines = output.read_bytes().splitlines(keepends=True)
            footer = json.loads(lines[-1])['footer']
            self.assertEqual(footer['sha256'], hashlib.sha256(b''.join(lines[:-1])).hexdigest())
            self.assertEqual(result['counts'], {'observations': 1})
            self.assertFalse(output.with_name(output.name + '.partial').exists())
            self.assertNotIn('PRIVATE_PAYLOAD', stdout.getvalue())

    def test_deadline_never_promotes_incomplete_export_or_writes_completion_footer(self):
        clock = [0]

        def rows():
            yield {'observation_id': 'one'}
            clock[0] = 2
            yield {'observation_id': 'two'}

        client = self.client(rows)
        with tempfile.TemporaryDirectory() as directory:
            output = Path(directory) / 'snapshot.ndjson'
            with patch('scripts.test262.central.client.EXPORT_TABLES', ('observations',)), \
                 patch('scripts.test262.central.client.time.monotonic', side_effect=lambda: clock[0]):
                with self.assertRaises(TimeoutError):
                    client.export(output, seconds=1)
            self.assertFalse(output.exists())
            partial = output.with_name(output.name + '.partial')
            records = [json.loads(line) for line in partial.read_text().splitlines()]
            self.assertEqual(len(records), 2)
            self.assertFalse(any('footer' in row for row in records))

    def test_invalid_deadline_is_rejected_before_database_access(self):
        client = Client.__new__(Client)
        with self.assertRaises(ValueError):
            client.export('unused', seconds=0)
