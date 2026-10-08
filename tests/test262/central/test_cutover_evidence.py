import copy
import json
from pathlib import Path
import tempfile
import unittest

from scripts.test262.central.cutover_evidence import DEFAULT_MANIFEST, load_manifest


class EvidenceManifestTests(unittest.TestCase):
    def setUp(self):
        self.config=json.loads(DEFAULT_MANIFEST.read_text())

    def load(self,config):
        with tempfile.TemporaryDirectory() as directory:
            file=Path(directory)/'manifest.json';file.write_text(json.dumps(config))
            return load_manifest(file)

    def test_required_set_and_coverage_can_grow_without_code_changes(self):
        self.config['required_artifacts'].append({'artifact_id':9001,'sha256':'a'*64})
        self.config['import_report_ids'].append(9001)
        self.config['expected_sources'].append({'artifact_id':9002,'observations':777,'database':'catalog.sqlite'})
        config,evidence,expected,digest=self.load(self.config)
        self.assertEqual(len(evidence),9);self.assertEqual(expected[9002],777)
        self.assertEqual(len(digest),64);self.assertFalse(config['history_complete'])

    def test_incomplete_required_set_duplicates_and_false_history_claim_are_rejected(self):
        for mutate in (lambda c:c.update(history_complete=True),
                       lambda c:c['required_artifacts'].pop(0),
                       lambda c:c['required_artifacts'].append(c['required_artifacts'][0]),
                       lambda c:c['expected_sources'].append(c['expected_sources'][0]),
                       lambda c:c.update(restore_report_ids=[])):
            config=copy.deepcopy(self.config);mutate(config)
            with self.assertRaises(ValueError):self.load(config)
