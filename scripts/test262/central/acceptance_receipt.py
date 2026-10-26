"""Verify pinned, already-produced receipts offline and emit a public allowlisted record.

No database, storage, fixture execution or publication access is used. Original
artifact bytes and private permission/identity fields must not be published.
"""
import argparse
import hashlib
import io
import json
from pathlib import Path
import re
import zipfile

from .reconciliation import CONTRACT, EXPORT_TABLES, decode, logical_json


def require(condition, message):
    if not condition:
        raise ValueError(message)


def read_receipt(path, pin, identity):
    with Path(path).open("rb") as stream:
        raw = stream.read(2_000_001)
    require(len(raw) <= 2_000_000, "Receipt ZIP exceeds bound")
    require(hashlib.sha256(raw).hexdigest() == pin["zip_sha256"], "Receipt ZIP digest differs")
    with zipfile.ZipFile(io.BytesIO(raw)) as archive:
        files = archive.infolist()
        require(len(files) == 1 and files[0].filename == f"test262-verification-{identity}.json"
                and files[0].file_size <= 262144, "Unexpected receipt member")
        document = decode(archive.read(files[0]))
    require(document.get("verified") is True, "Source verification did not pass")
    return document


def build(pins, coordinator_path, reporter_path):
    require(pins["schema_version"] == 1 and pins["repository"] == "tomacox74/js2il",
            "Unsupported acceptance scope")
    coordinator = read_receipt(coordinator_path, pins["artifacts"]["coordinator"], "coordinator")
    reporter = read_receipt(reporter_path, pins["artifacts"]["reporter"], "reporter")
    require(coordinator["source_revision"] == reporter["source_revision"] == pins["source_revision"],
            "Source revisions differ")
    require(coordinator["project_reference"] == reporter["project_reference"], "Source projects differ")
    report = coordinator["reconciliation"]
    require(report["verified"] is True and report["report_schema"] == 1
            and report["logical_digest_version"] == 1, "Reconciliation did not pass")
    exported = report["export"]
    require({k: v for k, v in exported.items() if k != "file_sha256"} == coordinator["export"],
            "Export receipts differ")
    for key in ("snapshot_token", "as_of", "sha256", "file_sha256"):
        require(exported[key] == pins["expected"][key], "Pinned export identity differs")
    require(set(exported["counts"]) == set(report["tables"]) == set(EXPORT_TABLES),
            "Incomplete export table set")
    require(sum(exported["counts"].values()) == pins["expected"]["records"], "Record count differs")
    tables = {}
    root = hashlib.sha256((logical_json({k: exported[k] for k in
                                        ("repository_id", "epoch", "schema", "api")}) + "\n").encode())
    for table in sorted(EXPORT_TABLES):
        row = report["tables"][table]
        require(type(row["rows"]) is int and row["rows"] >= 0
                and row["rows"] == exported["counts"][table]
                and row["relationship_failures"] == 0
                and re.fullmatch("[0-9a-f]{64}", row["logical_sha256"]), "Invalid table receipt")
        tables[table] = {k: row[k] for k in ("rows", "logical_sha256", "relationship_failures")}
        root.update((logical_json([table, row["rows"], row["logical_sha256"]]) + "\n").encode())
    require(root.hexdigest() == report["logical_sha256"] == pins["expected"]["logical_sha256"],
            "Version-1 logical root differs")
    refs = {f"{table}.{','.join(ref['columns'])}->{ref['parent']}"
            for table, spec in CONTRACT.items() for ref in spec["references"]
            if ref["parent"] in EXPORT_TABLES}
    require(set(report["relationship_checks"]) == refs
            and len(refs) == pins["expected"]["relationship_checks"], "Relationship coverage differs")
    checks = report["semantic_checks"]
    require(len(checks) == pins["expected"]["semantic_checks"]
            and all(re.fullmatch("[a-z_]+\\.[a-z_]+", key) for key in checks),
            "Semantic coverage differs")
    require(all(type(v) is int and v == 0 for v in
                [*report["relationship_checks"].values(), *checks.values()]), "Reconciliation failures")
    imports = report["evidence"]["imports"]
    require(all(i["parity"] is True and i["history_complete"] is False
                and i["expected"] == i["mapped"] == i["uploaded"] == i["reported"] for i in imports),
            "Historical parity differs")
    history = report["history"]
    for key in ("source_entries", "distinct_imports", "unavailable_gaps"):
        require(history[key] == pins["expected"][key], "Historical ledger totals differ")
    require(len(imports) == history["distinct_imports"]
            and history["recorded_manifest_sha256"] == [pins["expected"]["history_manifest_sha256"]]
            and history["attribution_complete"] is True
            and history["history_complete"] is False
            and history["archive_preservation_verified"] is False
            and all(history[k] == 0 for k in ("ambiguous_entries", "unmatched_entries", "orphan_receipts")),
            "Historical attribution differs")
    public_history = {k: history[k] for k in (
        "state", "source_entries", "distinct_imports", "unavailable_gaps", "ambiguous_entries",
        "unmatched_entries", "orphan_receipts", "attribution_complete", "history_complete",
        "archive_preservation_verified", "original_manifest_inspected", "ledger_sha256",
        "gap_ledger_sha256", "recorded_manifest_sha256")}
    return {
        "schema_version": 1, "repository": pins["repository"], "source_revision": pins["source_revision"],
        "run_id": pins["run_id"], "run_attempt": pins["run_attempt"],
        "artifacts": {name: {k: pin[k] for k in ("artifact_id", "zip_sha256", "expires_at")}
                      for name, pin in pins["artifacts"].items() if name in ("coordinator", "reporter")},
        "verified": True, "verification_scope": "Pinned receipt integrity and internal reconciliation consistency; no export payload reread",
        "export": {k: exported[k] for k in
                   ("snapshot_token", "as_of", "schema", "api", "epoch", "counts", "sha256", "file_sha256")},
        "logical_digest_version": 1, "logical_sha256": report["logical_sha256"],
        "tables": tables, "relationship_checks": report["relationship_checks"], "semantic_checks": checks,
        "history": public_history,
        "evidence": {k: report["evidence"][k] for k in (
            "batches_requiring_refresh_or_conflicted", "historical_imports_incomplete",
            "invalidated_observations", "missing_payloads", "observations_without_payload",
            "publication_attention", "trusted_conflicting_variants")},
        "limitations": [
            "Private project/database identities, permission audits and per-import identities are omitted.",
            "The export payload was not retained; source jobs verified it while available.",
            "Archive preservation and complete history remain unverified; #2242 owns the gap ledger.",
            "Operational warnings remain with #2332/#2344; no publication or repair authority is granted.",
            "Git/issue preservation is independent of Actions expiry, not an indefinite retention policy.",
        ],
    }


def main(argv=None):
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--pins", required=True, type=Path)
    parser.add_argument("--coordinator", required=True, type=Path)
    parser.add_argument("--reporter", required=True, type=Path)
    parser.add_argument("--output", required=True, type=Path)
    args = parser.parse_args(argv)
    require(args.output.resolve() not in {p.resolve() for p in
            (args.pins, args.coordinator, args.reporter)}, "Output must not overwrite inputs")
    args.output.unlink(missing_ok=True)
    result = build(decode(args.pins.read_bytes()), args.coordinator, args.reporter)
    args.output.write_text(json.dumps(result, indent=2, sort_keys=True) + "\n")


if __name__ == "__main__":
    main()
