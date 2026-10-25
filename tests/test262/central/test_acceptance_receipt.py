import copy
import hashlib
import json
from pathlib import Path
import tempfile
import unittest
import zipfile

from scripts.test262.central import acceptance_receipt
from scripts.test262.central.reconciliation import logical_json


ROOT = Path(__file__).resolve().parents[3]
EVIDENCE = ROOT / "docs/ECMA262/evidence"


class AcceptanceReceiptTests(unittest.TestCase):
    def setUp(self):
        self.temporary = tempfile.TemporaryDirectory()
        self.addCleanup(self.temporary.cleanup)
        self.directory = Path(self.temporary.name)
        self.pins = json.loads((EVIDENCE / "reconciliation-40181091117-pins.json").read_text())
        public = json.loads((EVIDENCE / "reconciliation-40181091117.json").read_text())
        self.report = {k: copy.deepcopy(public[k]) for k in (
            "export", "tables", "relationship_checks", "semantic_checks", "history", "evidence",
            "verified", "logical_digest_version", "logical_sha256")}
        self.report["report_schema"] = 1
        self.report["export"]["repository_id"] = "00000000-0000-0000-0000-000000000001"
        root = hashlib.sha256((logical_json({k: self.report["export"][k] for k in
                                            ("repository_id", "epoch", "schema", "api")}) + "\n").encode())
        for table, row in sorted(self.report["tables"].items()):
            root.update((logical_json([table, row["rows"], row["logical_sha256"]]) + "\n").encode())
        self.report["logical_sha256"] = self.pins["expected"]["logical_sha256"] = root.hexdigest()
        self.report["evidence"]["imports"] = [
            dict(parity=True, history_complete=False, expected=1, mapped=1, uploaded=1, reported=1)
            for _ in range(61)]
        self.coordinator = {"verified": True, "source_revision": self.pins["source_revision"],
                            "project_reference": "PRIVATE-PROJECT", "permission_audit": "PRIVATE-AUDIT",
                            "reconciliation": self.report,
                            "export": {k: v for k, v in self.report["export"].items() if k != "file_sha256"}}
        self.reporter = {"verified": True, "source_revision": self.pins["source_revision"],
                         "project_reference": "PRIVATE-PROJECT", "permission_audit": "PRIVATE-AUDIT"}

    def write_inputs(self):
        for identity in ("coordinator", "reporter"):
            path = self.directory / f"{identity}.zip"
            with zipfile.ZipFile(path, "w") as archive:
                archive.writestr(f"test262-verification-{identity}.json", json.dumps(getattr(self, identity)))
            self.pins["artifacts"][identity]["zip_sha256"] = hashlib.sha256(path.read_bytes()).hexdigest()
        return self.directory / "coordinator.zip", self.directory / "reporter.zip"

    def build(self):
        return acceptance_receipt.build(self.pins, *self.write_inputs())

    def test_allowlists_private_fields_and_preserves_distinct_digest_names(self):
        self.report["tables"]["fixtures"]["project_reference"] = "PRIVATE-PROJECT"
        self.report["history"]["entries"] = [{"import_id": "PRIVATE-IMPORT"}]
        self.pins["artifacts"]["coordinator"]["permission_audit"] = "PRIVATE-AUDIT"
        result = self.build()
        text = json.dumps(result)
        for forbidden in ("PRIVATE-", "00000000-0000-0000-0000-000000000001"):
            self.assertNotIn(forbidden, text)
        self.assertEqual(result["export"]["sha256"], self.pins["expected"]["sha256"])
        self.assertEqual(result["export"]["file_sha256"], self.pins["expected"]["file_sha256"])
        self.assertEqual(result["logical_sha256"], self.pins["expected"]["logical_sha256"])
        self.assertEqual(self.build(), result)
        self.assertEqual(result["evidence"]["publication_attention"], 1)
        self.assertFalse(result["history"]["archive_preservation_verified"])

    def test_rejects_changed_zip_source_revision_root_checks_and_history(self):
        mutations = [
            lambda: self.pins["artifacts"]["coordinator"].update(zip_sha256="0" * 64),
            lambda: self.reporter.update(source_revision="0" * 40),
            lambda: self.report.update(logical_sha256="0" * 64),
            lambda: self.report["semantic_checks"].update({"annotations.fixture_identity": 1}),
            lambda: self.report["history"].update(history_complete=True),
            lambda: self.report["history"].update(unmatched_entries=1),
            lambda: self.report["relationship_checks"].pop(next(iter(self.report["relationship_checks"]))),
        ]
        for mutate in mutations:
            with self.subTest(mutation=mutate):
                self.setUp()
                paths = self.write_inputs()
                mutate()
                if self.pins["artifacts"]["coordinator"]["zip_sha256"] != "0" * 64:
                    paths = self.write_inputs()
                with self.assertRaises(ValueError):
                    acceptance_receipt.build(self.pins, *paths)

    def test_failed_invocation_removes_stale_success_but_never_an_input(self):
        paths = self.write_inputs()
        pin_path = self.directory / "pins.json"
        pin_path.write_text(json.dumps(self.pins))
        output = self.directory / "result.json"
        output.write_text('{"verified":true}')
        paths[0].write_bytes(b"changed")
        args = ["--pins", str(pin_path), "--coordinator", str(paths[0]), "--reporter", str(paths[1])]
        with self.assertRaises(ValueError):
            acceptance_receipt.main([*args, "--output", str(output)])
        self.assertFalse(output.exists())
        with self.assertRaises(ValueError):
            acceptance_receipt.main([*args, "--output", str(pin_path)])
        self.assertTrue(pin_path.exists())


if __name__ == "__main__":
    unittest.main()
