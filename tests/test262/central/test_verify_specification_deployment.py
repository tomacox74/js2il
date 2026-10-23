import hashlib
import json
from pathlib import Path
import tempfile
import unittest

from scripts.test262.central.client import EXPORT_TABLES, canonical
from scripts.test262.central.verify_specification_deployment import project_reference, verify_export


class DeploymentReceiptTests(unittest.TestCase):
    def test_project_reference_direct_and_pooler(self):
        reference = 'abcdefghijklmnopqrst'
        self.assertEqual(project_reference({'host': f'db.{reference}.supabase.co'}), reference)
        self.assertEqual(project_reference({'host': 'pooler.supabase.com', 'user': f'test262_supervisor.{reference}'}), reference)
        for settings in ({}, {'host': 'db.invalid.supabase.co'},
                         {'host': f'db.{reference}.supabase.co', 'user': 'test262_agent_ro.aaaaaaaaaaaaaaaaaaaa'}):
            with self.subTest(settings=settings), self.assertRaises(ValueError):
                project_reference(settings)

    def export_bytes(self, *, empty=False):
        header = {'schema': 1, 'api': 1, 'epoch': 2, 'repository_id': 'repo',
                  'snapshot_token': '1:2:', 'as_of': '2026-10-22T00:00:00+00:00'}
        counts = dict.fromkeys(EXPORT_TABLES, 0)
        body = (canonical({'header': header}) + '\n').encode()
        if not empty:
            body += (canonical({'table': 'fixtures', 'row': {'fixture_id': 'example'}}) + '\n').encode()
            counts['fixtures'] = 1
        footer = {'counts': counts, 'sha256': hashlib.sha256(body).hexdigest()}
        return body + (canonical({'footer': footer}) + '\n').encode(), dict(header, **footer)

    def check_bytes(self, content, expected):
        with tempfile.TemporaryDirectory() as temporary:
            path = Path(temporary) / 'snapshot.ndjson'
            path.write_bytes(content)
            return verify_export(path, expected)

    def test_valid_receipt_includes_empty_annotation_table(self):
        for empty in (False, True):
            data, receipt = self.export_bytes(empty=empty)
            self.assertEqual(self.check_bytes(data, receipt), receipt)
            self.assertEqual(receipt['counts']['fixture_specification_metadata'], 0)

    def test_tampered_or_incomplete_export_rejected(self):
        data, receipt = self.export_bytes()
        malformed = [data.replace(b'example', b'changed'), b'',
                     data.splitlines(keepends=True)[0], data + b'{}\n',
                     data.replace(b'"fixtures":1', b'"fixtures":2'),
                     data.replace(b'"fixture_specification_metadata":0,', b''),
                     data.replace(b'"table":"fixtures"', b'"table":"unknown"')]
        for value in malformed:
            with self.subTest(value=value), self.assertRaises((ValueError, KeyError)):
                self.check_bytes(value, receipt)

    def test_wrong_repository_and_epoch_rejected(self):
        data, receipt = self.export_bytes()
        for key in ('repository_id', 'epoch'):
            with self.subTest(key=key), self.assertRaises(ValueError):
                self.check_bytes(data, dict(receipt, **{key: 'wrong'}))


if __name__ == '__main__':
    unittest.main()
