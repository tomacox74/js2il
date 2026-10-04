import json
from pathlib import Path
import tempfile
import unittest

from scripts.test262.central.history_import import digest, select, verify_run


class HistoryImportTests(unittest.TestCase):
    repository = 'tomacox74/js2il'

    def archive(self, directory, ids=(11, 12)):
        artifacts = []
        for aid in ids:
            parent = directory / str(aid)
            parent.mkdir()
            archive, source = parent / 'source.zip', parent / 'catalog.sqlite'
            archive.write_bytes(b'original archive')
            source.write_bytes(b'original snapshot')
            artifacts.append({'artifact_id': aid, 'run_id': 10, 'workflow': 'test262-catalog.yml',
                              'archive_sha256': digest(archive), 'databases': [
                                  {'path': 'previous/runner/path/catalog.sqlite', 'sha256': digest(source),
                                   'source_uri': f'github-actions:{self.repository}/test262-catalog.yml/10/{aid}'}]})
        manifest = {'repository': self.repository, 'artifacts': artifacts,
                    'gaps': [{'run_id': 9, 'reason': 'expired'}], 'history_complete': False}
        (directory / 'history-manifest.json').write_text(json.dumps(manifest))
        return manifest

    def test_relocated_archive_selection_preserves_gaps_and_bounds(self):
        with tempfile.TemporaryDirectory() as temporary:
            directory = Path(temporary)
            self.archive(directory)
            manifest, selected, remaining = select(directory, self.repository, 'mvp', '', 1)
            self.assertEqual(selected[0]['source'], str(directory / '11/catalog.sqlite'))
            self.assertEqual(remaining, 1)
            self.assertEqual(manifest['gaps'][0]['reason'], 'expired')
            _, selected, remaining = select(directory, self.repository, 'mvp', '12', 1)
            self.assertEqual(selected[0]['artifact_id'], 12)
            self.assertEqual(remaining, 0)

    def test_tampered_database_or_original_archive_is_rejected(self):
        for filename in ('catalog.sqlite', 'source.zip'):
            with self.subTest(filename=filename), tempfile.TemporaryDirectory() as temporary:
                directory = Path(temporary)
                self.archive(directory)
                (directory / '11' / filename).write_bytes(b'changed')
                with self.assertRaisesRegex(ValueError, 'checksum mismatch'):
                    select(directory, self.repository, 'mvp', '', 5)

    def test_explicit_selection_does_not_silently_drop_ids(self):
        with tempfile.TemporaryDirectory() as temporary:
            directory = Path(temporary)
            self.archive(directory)
            with self.assertRaisesRegex(ValueError, 'exceeds'):
                select(directory, self.repository, 'mvp', '11,12', 1)
            with self.assertRaisesRegex(ValueError, 'no matching'):
                select(directory, self.repository, 'mvp', '99', 5)

    def test_foreign_repository_and_forged_source_uri_are_rejected(self):
        with tempfile.TemporaryDirectory() as temporary:
            directory = Path(temporary)
            manifest = self.archive(directory)
            with self.assertRaisesRegex(ValueError, 'repository mismatch'):
                select(directory, 'another/repo', 'mvp', '', 5)
            manifest['artifacts'][0]['databases'][0]['source_uri'] = 'github-actions:another/repo/10/11'
            (directory / 'history-manifest.json').write_text(json.dumps(manifest))
            with self.assertRaisesRegex(ValueError, 'attribution mismatch'):
                select(directory, self.repository, 'mvp', '', 5)

    def test_symlinked_source_is_rejected(self):
        with tempfile.TemporaryDirectory() as temporary:
            directory = Path(temporary)
            self.archive(directory)
            source = directory / '11/catalog.sqlite'
            source.unlink()
            source.symlink_to(directory / '12/catalog.sqlite')
            with self.assertRaisesRegex(ValueError, 'Unsafe'):
                select(directory, self.repository, 'mvp', '', 5)

    def test_download_accepts_only_successful_master_workflow(self):
        run = {'head_repository': {'full_name': self.repository}, 'head_branch': 'master',
               'path': '.github/workflows/test262-history-import.yml', 'event': 'workflow_dispatch',
               'status': 'completed', 'conclusion': 'success'}
        verify_run(run, self.repository)
        for field, value in (('head_branch', 'feature'), ('path', '.github/workflows/other.yml'),
                             ('conclusion', 'failure'), ('event', 'pull_request')):
            with self.subTest(field=field), self.assertRaises(ValueError):
                verify_run(dict(run, **{field: value}), self.repository)

    def test_receipt_is_written_only_after_parity_and_source_checks(self):
        import os
        from types import SimpleNamespace
        from unittest.mock import MagicMock, patch
        from scripts.test262.central.history_import import import_history
        for missing in (False, True):
            with self.subTest(missing=missing), tempfile.TemporaryDirectory() as temporary:
                directory = Path(temporary)
                self.archive(directory, ids=(11,))
                args = SimpleNamespace(directory=str(directory), repository_name=self.repository,
                    kind='mvp', artifact_ids='11', max_snapshots=1, output=str(directory/'recovery'))
                client = MagicMock()
                client.contract = {'deployment_state': 'shadow', 'repository_id': 'repo',
                                   'producer_id': 'producer', 'trust_class': 'legacy'}
                def read(table, filters=None):
                    return [{'entity_kind': 'observation', 'entity_id': 'obs'}] if table == 'import_records' else ([] if missing else [{'observation_id': 'obs'}])
                client.read.side_effect = read
                env = {'TEST262_DATABASE_URL': 'postgresql://localhost/disposable',
                       'TEST262_REPOSITORY_ID': 'repo', 'TEST262_PRODUCER_ID': 'producer'}
                with patch.dict(os.environ, env), patch('scripts.test262.central.history_import.check'), \
                     patch('scripts.test262.central.history_import.Client', return_value=client), \
                     patch('scripts.test262.central.history_import.import_snapshot', return_value={'import_id':'import','observations':1}):
                    if missing:
                        with self.assertRaisesRegex(ValueError, 'parity failed'):
                            import_history(args)
                        client.put.assert_not_called()
                    else:
                        import_history(args)
                        self.assertEqual(client.put.call_args.args[1][0]['source_table'], 'history_verified_snapshot')
                        self.assertTrue(json.loads((directory/'recovery/import-report.json').read_text())['complete'])
