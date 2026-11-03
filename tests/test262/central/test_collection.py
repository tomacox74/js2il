import io
import json
import os
from pathlib import Path
import sqlite3
import tarfile
import tempfile
from types import SimpleNamespace
import unittest
from unittest.mock import Mock, patch

from scripts.test262.central import collection, execution_identity
from scripts.test262.central.client import sha
from scripts.test262.central.collection_bundle import digest, install

REVISION = 'a' * 40
EVENT_SHA = 'b' * 40
REPO = 'test/repo'
ORIGIN = {'GITHUB_RUN_ID': '123', 'GITHUB_RUN_ATTEMPT': '1',
          'GITHUB_SHA': EVENT_SHA, 'GITHUB_REF': 'refs/heads/master'}


class CollectionTests(unittest.TestCase):
    def history(self, conclusion, artifacts=True, head_sha=EVENT_SHA):
        run = {'id': 123, 'run_attempt': 1, 'head_sha': head_sha,
               'head_branch': 'master', 'path': collection.WORKFLOW,
               'repository': {'full_name': REPO}, 'head_repository': {'full_name': REPO},
               'status': 'completed'}
        job = {'id': 46, 'name': 'execute', 'run_id': 123, 'run_attempt': 1, 'head_sha': EVENT_SHA,
               'status': 'completed', 'completed_at': '2026-11-03T01:00:00Z', 'conclusion': conclusion,
               'steps': [{'name': collection.RECOVERY_STEP, 'conclusion': 'success'}]}
        artifact = {'id': 47, 'name': 'test262-split-worker-123-1-1', 'expired': False,
                    'digest': 'sha256:' + 'c' * 64,
                    'workflow_run': {'id': 123, 'head_sha': EVENT_SHA, 'head_branch': 'master'}}
        return Mock(side_effect=[[run], [{'jobs': [job]}], [{'artifacts': [artifact] if artifacts else []}]])

    def context(self):
        return {'run': 'parent', 'actions_run_id': 123, 'actions_attempt': 1,
                'event_sha': EVENT_SHA, 'revision': REVISION, 'managed_version': 1}

    def test_worker_success_requires_exact_job_and_retained_recovery(self):
        context = self.context()
        self.assertEqual(collection.terminal_proof(context, REPO, read=self.history('success')),
                         {'job_id': 46, 'state': 'success', 'retained': True})
        self.assertFalse(collection.terminal_proof(context, REPO, read=self.history('success', False))['retained'])
        self.assertFalse(collection.terminal_proof(context, REPO, read=self.history('failure'))['retained'])
        with self.assertRaisesRegex(ValueError, 'does not match'):
            collection.terminal_proof(context, REPO, read=self.history('success', head_sha=REVISION))

    def test_matrix_requires_distinct_terminal_jobs_and_slot_recovery(self):
        context = dict(self.context(), worker_producers=['first', 'second'])
        run, jobs, artifacts = self.history('success').side_effect
        first = dict(jobs[0]['jobs'][0], name='execute (1)')
        second = dict(first, name='execute (2)', id=48, conclusion='failure')
        read = Mock(side_effect=[run, [{'jobs': [second, first]}], artifacts])
        self.assertEqual(collection.terminal_proof(context, REPO, read=read), {'workers': [
            {'slot': 1, 'job_id': 46, 'state': 'success', 'retained': True},
            {'slot': 2, 'job_id': 48, 'state': 'failure', 'retained': False},
        ]})
        for invalid in ([first], [first, dict(second, id=46)], [first, second, second]):
            with self.subTest(jobs=invalid):
                with self.assertRaises(ValueError):
                    collection.terminal_proof(context, REPO,
                                              read=Mock(side_effect=[run, [{'jobs': invalid}]]))

    def test_wrong_attempt_or_context_fails_before_claiming(self):
        client = Mock(contract={'managed_actions_collection_version': 1})
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / 'context.json'
            context = self.context()
            path.write_text(json.dumps(context))
            with patch.dict(os.environ, dict(ORIGIN, GITHUB_RUN_ATTEMPT='2'), clear=True):
                with self.assertRaisesRegex(ValueError, 'original master workflow attempt'):
                    collection.load_context(client, path)
            client.call.assert_not_called()
            client.call.return_value = {'context': dict(context, revision='c' * 40),
                                        'context_sha256': '\\x' + sha(context)}
            with patch.dict(os.environ, ORIGIN, clear=True):
                with self.assertRaisesRegex(ValueError, 'registered preparation'):
                    collection.load_context(client, path)
            self.assertEqual(client.call.call_args.args, ('collection', 'parent'))

    def test_failed_worker_does_not_enable_sealing(self):
        client = Mock(contract={'managed_actions_collection_version': 1})
        context = self.context()
        client.call.side_effect = [{'context': context}, {'state': 'failed', 'budget': {'disposition': 'terminal'}}]
        with tempfile.TemporaryDirectory() as directory:
            args = SimpleNamespace(repository='repo', producer='owner', repository_name=REPO,
                                   output=Path(directory) / 'state.json',
                                   context_output=Path(directory) / 'context.json',
                                   github_output=Path(directory) / 'output')
            with patch.dict(os.environ, ORIGIN, clear=True), patch.object(
                collection, 'terminal_proof', return_value={'job_id': 46, 'state': 'failure', 'retained': False}
            ):
                collection.finish_collection(client, args)
            self.assertEqual(args.github_output.read_text(), 'complete=false\n')
            self.assertEqual(json.loads(args.output.read_text())['state'], 'failed')

    def test_retained_snapshot_contains_committed_wal(self):
        with tempfile.TemporaryDirectory() as directory:
            source = Path(directory) / 'source.sqlite'
            destination = Path(directory) / 'retained.sqlite'
            db = sqlite3.connect(source)
            try:
                db.execute('PRAGMA journal_mode=WAL')
                db.execute('CREATE TABLE result (value TEXT)')
                db.execute("INSERT INTO result VALUES ('retained')")
                db.commit()
                collection.snapshot_outbox(source, destination)
                copy = sqlite3.connect(destination)
                try:
                    self.assertEqual(copy.execute('SELECT value FROM result').fetchone(), ('retained',))
                finally:
                    copy.close()
            finally:
                db.close()

    def test_bundled_identity_matches_clean_worker_without_build_intermediates(self):
        with tempfile.TemporaryDirectory() as directory:
            root = Path(directory)
            pin = root / 'tests/test262/test262.pin.json'
            pin.parent.mkdir(parents=True)
            pin.write_text(json.dumps({'upstream': {'commit': REVISION}}))
            host = root / 'scripts/test262/NativeScreeningHost'
            generated = host / 'obj/Release/net10.0/AssemblyInfo.cs'
            generated.parent.mkdir(parents=True)
            generated.write_text('// generated')
            (host / 'Program.cs').write_text('// tracked source')
            jroc = root / 'src/Cli/bin/Release/net10.0/Jroc.dll'
            jroc.parent.mkdir(parents=True)
            jroc.write_bytes(b'binary')
            args = SimpleNamespace(root=root, jroc=jroc, kind='mvp-composite',
                                   runtime_timeout=30, compile_timeout=60, cap_ms=120000)
            with patch.object(execution_identity, 'REPO', root), patch.object(
                execution_identity.catalog, 'environment', return_value={'identity': {'os': 'Linux'}}
            ):
                prepared = execution_identity.execution_identity(args, 'corpus', bundled=True)
                legacy = execution_identity.execution_identity(args, 'corpus')
                self.assertIn(generated.relative_to(root).as_posix(), legacy['tooling'])
                generated.unlink()
                worker = execution_identity.execution_identity(args, 'corpus', bundled=True)
                self.assertEqual(prepared, worker)
                self.assertIn('Jroc.dll', worker['binaries'])
                self.assertIn((host / 'Program.cs').relative_to(root).as_posix(), worker['tooling'])

    def test_bundle_rejects_digest_and_path_mismatches(self):
        with tempfile.TemporaryDirectory() as directory:
            base = Path(directory)
            runtime = base / 'runtime.tar.gz'
            image = base / 'fixture-image.tar.gz'
            image.write_bytes(b'image')
            context = base / 'central-context.json'

            def bundle(member):
                with tarfile.open(runtime, 'w:gz') as archive:
                    entry = tarfile.TarInfo(member)
                    entry.size = 1
                    archive.addfile(entry, io.BytesIO(b'x'))
                context.write_text(json.dumps({'runtime_digest': digest(runtime), 'image_digest': digest(image)}))
                return digest(context)

            allowed = 'src/Cli/bin/Release/net10.0/Jroc.dll'
            install(base, bundle(allowed), base / 'checkout')
            self.assertEqual((base / 'checkout' / allowed).read_bytes(), b'x')
            with self.assertRaisesRegex(ValueError, 'Unexpected runtime archive member'):
                install(base, bundle('../untrusted'), base / 'checkout')
            expected = bundle(allowed)
            image.write_bytes(b'changed')
            with self.assertRaisesRegex(ValueError, 'image digest mismatch'):
                install(base, expected, base / 'checkout')

    def test_workflow_keeps_execution_credentials_and_publication_separate(self):
        path = Path(__file__).resolve().parents[3] / collection.WORKFLOW
        workflow = path.read_text()
        execute = workflow.split('\n  execute:\n')[1].split('\n  finalize:\n')[0]
        finalize = workflow.split('\n  finalize:\n')[1].split('\n  validate:\n')[0]
        validation = workflow.split('\n  validate:\n')[1].split('\n  publish:\n')[0]
        publication = workflow.split('\n  publish:\n')[1]
        self.assertIn('needs: prepare', execute)
        self.assertIn('fail-fast: false', execute)
        self.assertIn('environment: test262-catalogue-worker-' + chr(36) + '{{ matrix.slot }}', execute)
        self.assertIn('--slot "' + chr(36) + '{{ matrix.slot }}"', execute)
        self.assertIn('github.run_attempt }}-' + chr(36) + '{{ matrix.slot }}', execute)
        self.assertIn('TEST262_SPLIT_WORKER_DATABASE_URL', execute)
        self.assertNotIn('TEST262_COORDINATOR_DATABASE_URL', execute)
        self.assertIn('needs: [prepare, execute]', finalize)
        self.assertIn("steps.lifecycle.outputs.complete == 'true'", finalize)
        self.assertIn('digest-mismatch: error', execute)
        self.assertNotIn(chr(36) + '{{ secrets.', validation)
        self.assertIn("needs.validate.result == 'success'", publication)
        self.assertIn("github.event_name == 'workflow_dispatch'", publication)
        self.assertIn("vars.TEST262_SPLIT_WORKFLOW != 'true'", workflow)
