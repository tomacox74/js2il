import argparse
import copy
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

from scripts.test262.central.client import bytea, sha
from scripts.test262.central.inventory import enrich_specification_metadata


class Catalogue:
    def __init__(self):
        self.corpus = {'corpus_id': 'corpus', 'revision': 'a' * 40, 'inventory_state': 'sealed'}
        self.fixture = {'fixture_id': 'fixture', 'corpus_id': 'corpus',
                        'upstream_path': 'test/example.js', 'content_sha256': bytea('b' * 64)}
        self.writes = []

    def one(self, table, **filters):
        if table != 'corpora' or filters != {'corpus_id': 'corpus'}:
            raise AssertionError((table, filters))
        return self.corpus

    def read(self, table, filters):
        if table != 'fixtures' or filters != {'corpus_id': 'corpus'}:
            raise AssertionError((table, filters))
        yield self.fixture

    def put(self, table, rows):
        self.writes.append((table, copy.deepcopy(rows)))


class SpecificationMetadataTests(unittest.TestCase):
    def setUp(self):
        self.directory = tempfile.TemporaryDirectory()
        self.addCleanup(self.directory.cleanup)
        self.args = argparse.Namespace(corpus='corpus', root=self.directory.name)
        self.client = Catalogue()
        self.metadata = {
            'esid': 'sec-example', 'es5id': None, 'es6id': None,
            'description': 'Description', 'info': None, 'author': None,
            'specification': {'schema_version': 1, 'raw_identifiers': {'esid': 'sec-example'},
                              'raw_frontmatter': '\nesid: sec-example\n', 'reference_state': 'unresolved'},
        }
        self.row = {'path': 'test/example.js', 'sha256': 'b' * 64, 'metadata': self.metadata}
        self.commands = patch('scripts.test262.central.inventory.subprocess.check_output',
                              side_effect=lambda command, **kwargs:
                              'a' * 40 + '\n' if command[-2:] == ['rev-parse', 'HEAD'] else '')
        self.commands.start()
        self.addCleanup(self.commands.stop)
        bridge = patch('scripts.test262.catalog.bridge', return_value=[self.row])
        self.bridge = bridge.start()
        self.addCleanup(bridge.stop)

    def test_enrichment_is_bound_to_original_fixture_and_content(self):
        result = enrich_specification_metadata(self.client, self.args)
        self.assertEqual(result, {'corpus': 'corpus', 'revision': 'a' * 40, 'enriched_fixtures': 1})
        table, rows = self.client.writes[0]
        self.assertEqual(table, 'fixture_specification_metadata')
        self.assertEqual(rows, [{
            'fixture_id': 'fixture', 'upstream_revision': 'a' * 40,
            'upstream_path': 'test/example.js', 'content_sha256': bytea('b' * 64),
            'metadata': self.metadata, 'metadata_sha256': bytea(sha(self.metadata)),
        }])
        self.bridge.assert_called_once_with({
            'command': 'metadata', 'root': str(Path(self.directory.name).resolve()),
            'paths': ['test/example.js'],
        })

    def test_rejects_wrong_revision_dirty_checkout_and_unknown_corpus(self):
        self.client.corpus['revision'] = 'c' * 40
        with self.assertRaisesRegex(ValueError, 'revision'):
            enrich_specification_metadata(self.client, self.args)
        self.client.corpus['revision'] = 'a' * 40
        with patch('scripts.test262.central.inventory.subprocess.check_output', return_value='dirty'):
            with self.assertRaises(ValueError):
                enrich_specification_metadata(self.client, self.args)
        self.client.corpus = None
        with self.assertRaisesRegex(ValueError, 'corpus'):
            enrich_specification_metadata(self.client, self.args)
        self.assertEqual(self.client.writes, [])

    def test_rejects_mutable_inventory(self):
        self.client.corpus['inventory_state'] = 'staging'
        with self.assertRaisesRegex(ValueError, 'sealed corpus'):
            enrich_specification_metadata(self.client, self.args)
        self.assertEqual(self.client.writes, [])

    def test_rejects_changed_bytes_missing_rows_and_path_substitution(self):
        for rows in ([dict(self.row, sha256='c' * 64)], [], [dict(self.row, path='test/other.js')]):
            with self.subTest(rows=rows):
                self.bridge.return_value = rows
                with self.assertRaises(ValueError):
                    enrich_specification_metadata(self.client, self.args)
        self.assertEqual(self.client.writes, [])

    def test_enrichment_batches_are_bounded_and_keep_complete_source_identities(self):
        fixtures = [dict(self.client.fixture, fixture_id=str(i), upstream_path=f'test/{i}.js') for i in range(251)]
        self.client.read = lambda table, filters: iter(fixtures)
        self.bridge.side_effect = lambda request: [
            dict(self.row, path=path) for path in request['paths']]
        self.assertEqual(enrich_specification_metadata(self.client, self.args)['enriched_fixtures'], 251)
        self.assertEqual([len(rows) for _, rows in self.client.writes], [250, 1])
        self.assertEqual([len(call.args[0]['paths']) for call in self.bridge.call_args_list], [250, 1])
        self.assertEqual(self.client.writes[-1][1][0]['upstream_path'], 'test/250.js')

    def test_cli_exposes_explicit_enrichment_command(self):
        from scripts.test262.central.cli import parser
        args = parser().parse_args(['enrich-specification-metadata', '--corpus', 'corpus', '--root', 'upstream'])
        self.assertEqual((args.command, args.corpus, args.root),
                         ('enrich-specification-metadata', 'corpus', 'upstream'))


if __name__ == '__main__':
    unittest.main()
