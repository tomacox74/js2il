#!/usr/bin/env python3
"""Deterministic native Test262 screening, generation, and publication guards."""

from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import re
import sqlite3
import subprocess
import sys
import time
from typing import Any

SCHEMA_VERSION = "2"
OUTCOMES = {"pass", "fail", "deferred", "unsupported", "infrastructure-error", "incomplete"}
FAILURE_CLASSES = {
    "product-feature-gap",
    "semantic-defect",
    "harness-gap",
    "policy-exclusion",
    "infrastructure-error",
    "unresolved",
}
SUPPORTED_AREA = "language/computed-property-names/basics"


def canonical(value: Any) -> str:
    return json.dumps(value, sort_keys=True, separators=(",", ":"))


def file_sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def write_json(path: Path, value: Any) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, sort_keys=True) + "\n", encoding="utf-8")


def connect(path: Path) -> sqlite3.Connection:
    path.parent.mkdir(parents=True, exist_ok=True)
    db = sqlite3.connect(path)
    db.row_factory = sqlite3.Row
    db.execute("PRAGMA foreign_keys = ON")
    db.executescript(
        """
        CREATE TABLE IF NOT EXISTS meta (
          key TEXT PRIMARY KEY,
          value TEXT NOT NULL
        );
        CREATE TABLE IF NOT EXISTS runs (
          run_id TEXT PRIMARY KEY,
          batch_id TEXT NOT NULL,
          trigger_revision TEXT NOT NULL,
          base_revision TEXT NOT NULL,
          pin TEXT NOT NULL,
          budgets TEXT NOT NULL,
          active_seconds REAL NOT NULL DEFAULT 0,
          state TEXT NOT NULL,
          created_at REAL NOT NULL,
          updated_at REAL NOT NULL
        );
        CREATE TABLE IF NOT EXISTS candidates (
          run_id TEXT NOT NULL REFERENCES runs(run_id),
          path TEXT NOT NULL,
          sha256 TEXT NOT NULL,
          variants TEXT NOT NULL,
          selection_reason TEXT NOT NULL,
          evidence_kind TEXT NOT NULL,
          catalog_provenance TEXT NOT NULL,
          capability_identity TEXT NOT NULL,
          status TEXT NOT NULL,
          cursor INTEGER NOT NULL DEFAULT 0,
          PRIMARY KEY (run_id, path)
        );
        CREATE TABLE IF NOT EXISTS attempts (
          attempt_id INTEGER PRIMARY KEY AUTOINCREMENT,
          run_id TEXT NOT NULL REFERENCES runs(run_id),
          path TEXT NOT NULL,
          variant TEXT NOT NULL,
          fixture_sha256 TEXT NOT NULL,
          pin TEXT NOT NULL,
          compiler_identity TEXT NOT NULL,
          harness_identity TEXT NOT NULL,
          environment_identity TEXT NOT NULL,
          phase TEXT NOT NULL,
          diagnostic TEXT NOT NULL,
          outcome TEXT NOT NULL,
          failure_class TEXT,
          started_at REAL NOT NULL,
          finished_at REAL NOT NULL,
          CHECK (outcome IN ('pass','fail','deferred','unsupported',
                             'infrastructure-error','incomplete')),
          CHECK (failure_class IS NULL OR failure_class IN
                 ('product-feature-gap','semantic-defect','harness-gap',
                  'policy-exclusion','infrastructure-error','unresolved'))
        );
        CREATE INDEX IF NOT EXISTS attempts_latest
          ON attempts(run_id,path,variant,attempt_id);
        CREATE TABLE IF NOT EXISTS checkpoints (
          run_id TEXT NOT NULL REFERENCES runs(run_id),
          checkpoint_id INTEGER PRIMARY KEY AUTOINCREMENT,
          cursor INTEGER NOT NULL,
          artifact_digest TEXT NOT NULL,
          complete INTEGER NOT NULL,
          created_at REAL NOT NULL
        );
        CREATE TABLE IF NOT EXISTS publications (
          batch_id TEXT PRIMARY KEY,
          run_id TEXT NOT NULL REFERENCES runs(run_id),
          branch TEXT NOT NULL,
          pr_number INTEGER,
          expected_head TEXT,
          state TEXT NOT NULL,
          closure_reason TEXT,
          updated_at REAL NOT NULL
        );
        """
    )
    existing = db.execute("SELECT value FROM meta WHERE key='schema_version'").fetchone()
    if existing and existing[0] != SCHEMA_VERSION:
        raise ValueError(
            f"Incompatible native evidence schema {existing[0]}; "
            f"start a documented cold state for schema {SCHEMA_VERSION}")
    db.execute(
        "INSERT OR REPLACE INTO meta(key,value) VALUES('schema_version',?)",
        (SCHEMA_VERSION,),
    )
    db.commit()
    return db


def require_budget(value: int, name: str, maximum: int) -> int:
    if value < 1 or value > maximum:
        raise ValueError(f"{name} must be between 1 and {maximum}")
    return value


def create_run(db: sqlite3.Connection, args: argparse.Namespace) -> str:
    candidate_limit = require_budget(args.candidate_limit, "candidate-limit", 500)
    accepted_limit = require_budget(args.accepted_limit, "accepted-limit", candidate_limit)
    variant_limit = require_budget(args.variant_limit, "variant-limit", 2000)
    time_limit = require_budget(args.time_limit, "time-limit", 24 * 60 * 60)
    run_id = args.run_id or f"native-{int(time.time())}-{os.getpid()}"
    batch_id = args.batch_id or run_id
    budgets = canonical(
        {
            "candidate_limit": candidate_limit,
            "accepted_limit": accepted_limit,
            "variant_limit": variant_limit,
            "time_limit": time_limit,
        }
    )
    existing = db.execute("SELECT * FROM runs WHERE run_id=?", (run_id,)).fetchone()
    if existing:
        expected = (
            batch_id,
            args.trigger_revision,
            args.base_revision,
            args.pin,
            budgets,
        )
        actual = (
            existing["batch_id"],
            existing["trigger_revision"],
            existing["base_revision"],
            existing["pin"],
            existing["budgets"],
        )
        if actual != expected:
            raise ValueError("Existing run identity does not match the requested provenance")
        return run_id
    now = time.time()
    db.execute(
        """INSERT INTO runs
           (run_id,batch_id,trigger_revision,base_revision,pin,budgets,active_seconds,
            state,created_at,updated_at)
           VALUES(?,?,?,?,? ,?,0,'planned',?,?)""",
        (
            run_id,
            batch_id,
            args.trigger_revision,
            args.base_revision,
            args.pin,
            budgets,
            now,
            now,
        ),
    )
    db.commit()
    return run_id


def open_catalog(path: Path) -> sqlite3.Connection:
    if not path.is_file():
        raise ValueError(f"Catalog checkpoint is missing (cold start): {path}")
    catalog = sqlite3.connect(f"file:{path.resolve()}?mode=ro", uri=True)
    catalog.row_factory = sqlite3.Row
    required = {"settings", "fixtures", "results", "registrations"}
    actual = {
        row[0]
        for row in catalog.execute(
            "SELECT name FROM sqlite_master WHERE type='table'"
        )
    }
    if not required.issubset(actual):
        catalog.close()
        raise ValueError("Catalog checkpoint has an incompatible schema")
    return catalog


def catalog_candidates(
    catalog_path: Path, area: str, limit: int
) -> tuple[str, list[dict[str, Any]]]:
    normalized = area.strip("/")
    if normalized != SUPPORTED_AREA and not normalized.startswith(SUPPORTED_AREA + "/"):
        raise ValueError(
            f"First-release native automation supports only {SUPPORTED_AREA} and its subfolders"
        )
    catalog = open_catalog(catalog_path)
    try:
        current = catalog.execute(
            "SELECT value FROM settings WHERE key='current'"
        ).fetchone()
        if not current:
            raise ValueError("Catalog checkpoint has no current provenance")
        provenance = current[0]
        rows = catalog.execute(
            """SELECT path,sha256,variants
               FROM fixtures
               WHERE provenance=? AND state='runnable'
                 AND (path=? OR path LIKE ?)
                 AND NOT EXISTS (
                   SELECT 1 FROM registrations r WHERE r.path=fixtures.path
                 )
               ORDER BY path""",
            (provenance, f"test/{normalized}", f"test/{normalized}/%"),
        ).fetchall()
        native_root = Path(__file__).resolve().parents[2] / "tests/Jroc.Test262.Tests"
        candidates: list[dict[str, Any]] = []
        for row in rows:
            variants = json.loads(row["variants"])
            current_evidence = {
                result["variant"]: result["verdict"]
                for result in catalog.execute(
                    """SELECT variant,verdict FROM results
                       WHERE provenance=? AND path=?""",
                    (provenance, row["path"]),
                )
            }
            evidence_kind = None
            if variants and all(
                current_evidence.get(variant) == "matched" for variant in variants
            ):
                evidence_kind = "current-provenance MVP pass"
            else:
                historical = catalog.execute(
                    """SELECT provenance,variants FROM fixtures
                       WHERE provenance!=? AND path=? AND sha256=? AND state='runnable'
                       ORDER BY provenance""",
                    (provenance, row["path"], row["sha256"]),
                ).fetchall()
                for old in historical:
                    old_variants = json.loads(old["variants"])
                    if old_variants != variants:
                        continue
                    old_evidence = {
                        result["variant"]: result["verdict"]
                        for result in catalog.execute(
                            """SELECT variant,verdict FROM results
                               WHERE provenance=? AND path=?""",
                            (old["provenance"], row["path"]),
                        )
                    }
                    if variants and all(
                        old_evidence.get(variant) == "matched"
                        for variant in variants
                    ):
                        evidence_kind = "historical single-provenance MVP pass"
                        break
            filename = Path(row["path"]).name
            legacy_duplicate = any(
                file_sha256(existing) == row["sha256"]
                for existing in native_root.rglob(filename)
            )
            if evidence_kind and not legacy_duplicate:
                candidates.append(
                    {
                        "path": row["path"],
                        "sha256": row["sha256"],
                        "variants": variants,
                        "reason": (
                            f"{evidence_kind} used only as a selection hint; "
                            "fresh native acceptance required"
                        ),
                    }
                )
            if len(candidates) >= limit:
                break
        return provenance, candidates
    finally:
        catalog.close()


def plan(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    budgets = json.loads(run["budgets"])
    provenance, candidates = catalog_candidates(
        Path(args.catalog), args.area, budgets["candidate_limit"]
    )
    with db:
        for candidate in candidates:
            prior = db.execute(
                """SELECT a.outcome,a.failure_class
                   FROM attempts a
                   JOIN candidates c
                     ON c.run_id=a.run_id AND c.path=a.path
                   WHERE a.path=? AND a.fixture_sha256=? AND a.pin=?
                     AND c.capability_identity=?
                   ORDER BY a.attempt_id DESC LIMIT 1""",
                (
                    candidate["path"],
                    candidate["sha256"],
                    run["pin"],
                    args.capability_identity,
                ),
            ).fetchone()
            capability_deferred = (
                prior
                and prior["outcome"] == "unsupported"
                and prior["failure_class"] in {"harness-gap", "policy-exclusion"}
            )
            db.execute(
                """INSERT OR IGNORE INTO candidates
                   (run_id,path,sha256,variants,selection_reason,evidence_kind,
                    catalog_provenance,capability_identity,status,cursor)
                   VALUES(?,?,?,?,?,'mvp-selection-hint',?,?,?,0)""",
                (
                    args.run_id,
                    candidate["path"],
                    candidate["sha256"],
                    canonical(candidate["variants"]),
                    (
                        candidate["reason"]
                        if not capability_deferred
                        else candidate["reason"]
                        + "; unchanged native capability remains unsupported"
                    ),
                    provenance,
                    args.capability_identity,
                    "capability-deferred" if capability_deferred else "pending",
                ),
            )
        db.execute(
            "UPDATE runs SET state='screening',updated_at=? WHERE run_id=?",
            (time.time(), args.run_id),
        )
    all_candidates = db.execute(
        "SELECT path FROM candidates WHERE run_id=? ORDER BY path", (args.run_id,)
    ).fetchall()
    result = {
        "run_id": args.run_id,
        "batch_id": run["batch_id"],
        "catalog_provenance": provenance,
        "candidate_count": len(all_candidates),
        "new_candidate_count": len(candidates),
        "paths": [row["path"] for row in all_candidates],
        "cold_start": False,
    }
    if args.output:
        write_json(Path(args.output), result)
    return result


def screening_plan(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    budgets = json.loads(run["budgets"])
    attempted = db.execute(
        "SELECT COUNT(*) FROM attempts WHERE run_id=?", (args.run_id,)
    ).fetchone()[0]
    remaining_attempts = max(0, budgets["variant_limit"] - attempted)
    elapsed = max(0, int(run["active_seconds"]))
    remaining_time = max(0, budgets["time_limit"] - elapsed)
    latest = {
        (row["path"], row["variant"])
        for row in db.execute(
            """SELECT a.path,a.variant
               FROM attempts a
               JOIN (
                 SELECT path,variant,MAX(attempt_id) attempt_id
                 FROM attempts WHERE run_id=? GROUP BY path,variant
               ) latest ON latest.attempt_id=a.attempt_id
               WHERE a.run_id=? AND a.outcome='pass'""",
            (args.run_id, args.run_id),
        )
    }
    candidates = []
    if remaining_attempts > 0 and remaining_time > 0:
        for row in db.execute(
            """SELECT path,sha256,variants FROM candidates
               WHERE run_id=? AND status!='capability-deferred' ORDER BY path""",
            (args.run_id,),
        ):
            variants = [
                variant
                for variant in json.loads(row["variants"])
                if (row["path"], variant) not in latest
            ]
            if variants:
                candidates.append(
                    {
                        "path": row["path"],
                        "sha256": row["sha256"],
                        "variants": variants,
                    }
                )
    result = {
        "upstream_root": str(Path(args.upstream).resolve()),
        "timeout_ms": args.timeout_ms,
        "variant_limit": remaining_attempts,
        "time_limit_seconds": remaining_time,
        "candidates": candidates,
    }
    write_json(Path(args.output), result)
    return {
        "candidate_count": len(candidates),
        "variant_count": sum(len(item["variants"]) for item in candidates),
        "remaining_variant_budget": remaining_attempts,
        "remaining_time_seconds": remaining_time,
    }


def record(db: sqlite3.Connection, args: argparse.Namespace) -> None:
    if args.outcome not in OUTCOMES:
        raise ValueError(f"Unknown outcome: {args.outcome}")
    if args.failure_class and args.failure_class not in FAILURE_CLASSES:
        raise ValueError(f"Unknown failure class: {args.failure_class}")
    candidate = db.execute(
        "SELECT sha256,variants FROM candidates WHERE run_id=? AND path=?",
        (args.run_id, args.path),
    ).fetchone()
    if not candidate:
        raise ValueError("Outcome path is not a planned candidate")
    if args.variant not in json.loads(candidate["variants"]):
        raise ValueError("Outcome variant is not required by the planned candidate")
    if args.fixture_sha256 != candidate["sha256"]:
        raise ValueError("Outcome fixture hash does not match the planned candidate")
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if args.pin != run["pin"]:
        raise ValueError("Outcome pin does not match the run")
    budgets = json.loads(run["budgets"])
    attempts = db.execute(
        "SELECT COUNT(*) FROM attempts WHERE run_id=?", (args.run_id,)
    ).fetchone()[0]
    if attempts >= budgets["variant_limit"]:
        raise ValueError("variant attempt budget exhausted; checkpoint and resume later")
    now = time.time()
    with db:
        db.execute(
            """INSERT INTO attempts
               (run_id,path,variant,fixture_sha256,pin,compiler_identity,
                harness_identity,environment_identity,phase,diagnostic,outcome,
                failure_class,started_at,finished_at)
               VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)""",
            (
                args.run_id,
                args.path,
                args.variant,
                args.fixture_sha256,
                args.pin,
                args.compiler_identity,
                args.harness_identity,
                args.environment_identity,
                args.phase,
                args.diagnostic,
                args.outcome,
                args.failure_class,
                args.started_at or now,
                args.finished_at or now,
            ),
        )
        db.execute(
            """UPDATE candidates SET status=?,cursor=cursor+1
               WHERE run_id=? AND path=?""",
            (
                "accepted" if args.outcome == "pass" else "attempted",
                args.run_id,
                args.path,
            ),
        )
        db.execute(
            "UPDATE runs SET updated_at=? WHERE run_id=?",
            (now, args.run_id),
        )


def import_results(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    values = json.loads(Path(args.results).read_text(encoding="utf-8"))
    if not isinstance(values, list):
        raise ValueError("Native screening results must be a JSON array")
    for value in values:
        record(
            db,
            argparse.Namespace(
                run_id=args.run_id,
                path=value["path"],
                variant=value["variant"],
                fixture_sha256=value["fixture_sha256"],
                pin=args.pin,
                compiler_identity=args.compiler_identity,
                harness_identity=args.harness_identity,
                environment_identity=args.environment_identity,
                phase=value.get("phase", "execution"),
                diagnostic=value.get("diagnostic", ""),
                outcome=value["outcome"],
                failure_class=value.get("failure_class"),
                started_at=value.get("started_at"),
                finished_at=value.get("finished_at"),
            ),
        )
    active_seconds = 0.0
    if values:
        active_seconds = max(
            float(value.get("finished_at", 0)) for value in values
        ) - min(float(value.get("started_at", 0)) for value in values)
        active_seconds = max(0.0, active_seconds)
    with db:
        db.execute(
            """UPDATE runs SET active_seconds=active_seconds+?,updated_at=?
               WHERE run_id=?""",
            (active_seconds, time.time(), args.run_id),
        )
    return {"imported": len(values)}


def latest_attempts(db: sqlite3.Connection, run_id: str) -> list[sqlite3.Row]:
    return db.execute(
        """SELECT a.*
           FROM attempts a
           JOIN (
             SELECT path,variant,MAX(attempt_id) attempt_id
             FROM attempts WHERE run_id=? GROUP BY path,variant
           ) latest ON latest.attempt_id=a.attempt_id
           WHERE a.run_id=? ORDER BY a.path,a.variant""",
        (run_id, run_id),
    ).fetchall()


def classify_outcome(row: sqlite3.Row) -> str:
    if row["outcome"] in {"infrastructure-error", "incomplete"}:
        return "infrastructure-error"
    if row["outcome"] == "unsupported":
        return row["failure_class"] or "harness-gap"
    if row["outcome"] == "deferred":
        return row["failure_class"] or "unresolved"
    if row["outcome"] == "fail":
        return row["failure_class"] or "unresolved"
    return "accepted"


def report(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    rows = latest_attempts(db, args.run_id)
    candidates = db.execute(
        """SELECT path,sha256,variants,status,selection_reason FROM candidates
           WHERE run_id=? ORDER BY path""",
        (args.run_id,),
    ).fetchall()
    grouped: dict[str, list[sqlite3.Row]] = {}
    for row in rows:
        grouped.setdefault(row["path"], []).append(row)
    accepted: list[str] = []
    incomplete: list[str] = []
    clusters: dict[str, list[dict[str, str]]] = {}
    for candidate in candidates:
        if candidate["status"] == "capability-deferred":
            clusters.setdefault("harness-gap", []).append(
                {
                    "path": candidate["path"],
                    "variant": "*",
                    "phase": "capability",
                    "diagnostic": candidate["selection_reason"],
                }
            )
            continue
        path_rows = grouped.get(candidate["path"], [])
        expected = json.loads(candidate["variants"])
        by_variant = {row["variant"]: row for row in path_rows}
        if not all(variant in by_variant for variant in expected):
            incomplete.append(candidate["path"])
            continue
        selected = [by_variant[variant] for variant in expected]
        provenance = {
            (
                row["fixture_sha256"],
                row["pin"],
                row["compiler_identity"],
                row["harness_identity"],
                row["environment_identity"],
            )
            for row in selected
        }
        valid_provenance = provenance == {
            (
                candidate["sha256"],
                run["pin"],
                selected[0]["compiler_identity"],
                selected[0]["harness_identity"],
                selected[0]["environment_identity"],
            )
        }
        if valid_provenance and all(row["outcome"] == "pass" for row in selected):
            accepted.append(candidate["path"])
            continue
        for row in selected:
            category = classify_outcome(row)
            if category != "accepted":
                clusters.setdefault(category, []).append(
                    {
                        "path": candidate["path"],
                        "variant": row["variant"],
                        "phase": row["phase"],
                        "diagnostic": row["diagnostic"],
                    }
                )
        if not valid_provenance:
            clusters.setdefault("infrastructure-error", []).append(
                {
                    "path": candidate["path"],
                    "variant": "*",
                    "phase": "provenance",
                    "diagnostic": "Required variants do not share one native provenance.",
                }
            )
    accepted_limit = json.loads(run["budgets"])["accepted_limit"]
    batch_accepted = accepted[:accepted_limit]
    deferred_accepted = accepted[accepted_limit:]
    result = {
        "schema_version": SCHEMA_VERSION,
        "run_id": args.run_id,
        "batch_id": run["batch_id"],
        "trigger_revision": run["trigger_revision"],
        "base_revision": run["base_revision"],
        "pin": run["pin"],
        "counts": {
            "candidates": len(candidates),
            "attempts": db.execute(
                "SELECT COUNT(*) FROM attempts WHERE run_id=?", (args.run_id,)
            ).fetchone()[0],
            "latest_attempts": len(rows),
            "accepted": len(batch_accepted),
            "accepted_deferred_by_limit": len(deferred_accepted),
            "incomplete": len(incomplete),
            "failed_or_unsupported": len(
                {
                    item["path"]
                    for values in clusters.values()
                    for item in values
                }
            ),
        },
        "accepted": batch_accepted,
        "accepted_deferred_by_limit": deferred_accepted,
        "incomplete": incomplete,
        "failure_clusters": {
            key: value for key, value in sorted(clusters.items())
        },
        "complete_native_acceptance": bool(batch_accepted),
    }
    budgets = json.loads(run["budgets"])
    result["budget_exceeded"] = (
        result["counts"]["attempts"] >= budgets["variant_limit"]
        or run["active_seconds"] >= budgets["time_limit"]
    )
    if args.output:
        write_json(Path(args.output), result)
    return result


def safe_identifier(value: str) -> str:
    result = "".join(character if character.isalnum() else "_" for character in value)
    result = result.strip("_") or "Test"
    return result if result[0].isalpha() else f"Test_{result}"


def runtime_negative(source: str) -> bool:
    frontmatter = re.search(r"/\*---(?P<body>.*?)---\*/", source, re.S)
    if not frontmatter:
        return False
    return bool(
        re.search(
            r"(?m)^\s*phase\s*:\s*['\"]?runtime['\"]?\s*$",
            frontmatter.group("body"),
        )
    )


def update_table_row(text: str, label: str, increment: int) -> str:
    pattern = re.compile(
        rf"^\| {re.escape(label)} \| (?P<passed>[\d,]+) \| "
        rf"(?P<unsupported>[\d,]+) \| (?P<unverified>[\d,]+) \| "
        rf"(?P<total>[\d,]+) \| \*\*(?P<percent>[\d.]+)%\*\* \|$",
        re.M,
    )
    match = pattern.search(text)
    if not match:
        raise ValueError(f"Coverage row not found: {label}")
    passed = int(match.group("passed").replace(",", "")) + increment
    unsupported = int(match.group("unsupported").replace(",", ""))
    unverified = int(match.group("unverified").replace(",", "")) - increment
    total = int(match.group("total").replace(",", ""))
    if unverified < 0 or passed + unsupported + unverified != total:
        raise ValueError(f"Coverage row would become inconsistent: {label}")
    replacement = (
        f"| {label} | {passed:,} | {unsupported:,} | {unverified:,} | "
        f"{total:,} | **{passed / total * 100:.2f}%** |"
    )
    return text[: match.start()] + replacement + text[match.end() :]


def update_coverage_docs(root: Path, accepted_count: int, batch_id: str) -> list[str]:
    conformance = root / "docs/ECMA262/Test262Conformance.md"
    text = conformance.read_text(encoding="utf-8")
    for label in (
        "Language syntax and semantics",
        "**Total**",
        "`computed-property-names`",
    ):
        text = update_table_row(text, label, accepted_count)
    conformance.write_text(text, encoding="utf-8")

    index = root / "docs/ECMA262/Index.md"
    index_text = index.read_text(encoding="utf-8")
    summary_pattern = re.compile(
        r"\| Verified passing \| (?P<passed>[\d,]+) \| \*\*(?P<percent>[\d.]+)%\*\* \|\n"
        r"\| Explicitly excluded due to known unsupported behavior \| (?P<excluded>[\d,]+) \| (?P<excluded_percent>[\d.]+)% \|\n"
        r"\| Not yet verified \| (?P<unverified>[\d,]+) \| (?P<unverified_percent>[\d.]+)% \|\n"
        r"\| \*\*Total applicable ECMA-262 tests\*\* \| \*\*(?P<total>[\d,]+)\*\* \| \*\*100.00%\*\* \|"
    )
    match = summary_pattern.search(index_text)
    if not match:
        raise ValueError("ECMA-262 index Test262 summary was not found")
    passed = int(match.group("passed").replace(",", "")) + accepted_count
    excluded = int(match.group("excluded").replace(",", ""))
    unverified = int(match.group("unverified").replace(",", "")) - accepted_count
    total = int(match.group("total").replace(",", ""))
    replacement = (
        f"| Verified passing | {passed:,} | **{passed / total * 100:.2f}%** |\n"
        f"| Explicitly excluded due to known unsupported behavior | {excluded:,} | "
        f"{excluded / total * 100:.2f}% |\n"
        f"| Not yet verified | {unverified:,} | {unverified / total * 100:.2f}% |\n"
        f"| **Total applicable ECMA-262 tests** | **{total:,}** | **100.00%** |"
    )
    index.write_text(
        index_text[: match.start()] + replacement + index_text[match.end() :],
        encoding="utf-8",
    )

    changelog = root / "CHANGELOG.md"
    changelog_text = changelog.read_text(encoding="utf-8")
    marker = f"native batch `{batch_id}`"
    if marker not in changelog_text:
        insertion = (
            f"- test262: verify {accepted_count} additional pinned computed "
            f"property-name fixtures "
            f"from native batch `{batch_id}`.\n"
        )
        changelog_text = changelog_text.replace(
            "## Unreleased\n\n", "## Unreleased\n\n" + insertion, 1
        )
        changelog.write_text(changelog_text, encoding="utf-8")
    return [
        "CHANGELOG.md",
        "docs/ECMA262/Index.md",
        "docs/ECMA262/Test262Conformance.md",
    ]


def generate_batch(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    summary = report(db, argparse.Namespace(run_id=args.run_id, output=None))
    if not summary["accepted"]:
        raise ValueError("Batch generation requires at least one freshly accepted fixture")
    upstream = Path(args.upstream).resolve()
    root = Path(args.destination).resolve()
    test_root = root / "tests/Jroc.Test262.Tests"
    copied: list[str] = []
    grouped: dict[Path, list[tuple[str, bool]]] = {}
    method_names: dict[Path, set[str]] = {}
    for relative in summary["accepted"]:
        if not relative.startswith(f"test/{SUPPORTED_AREA}/"):
            raise ValueError(f"Accepted fixture is outside the supported area: {relative}")
        source = upstream / relative
        if not source.is_file():
            raise ValueError(f"Missing pinned fixture: {relative}")
        candidate = db.execute(
            "SELECT sha256 FROM candidates WHERE run_id=? AND path=?",
            (args.run_id, relative),
        ).fetchone()
        if file_sha256(source) != candidate["sha256"]:
            raise ValueError(f"Fixture hash changed: {relative}")
        test_path = Path(relative).relative_to("test")
        target = test_root / test_path.parent / "JavaScript" / test_path.name
        if target.exists():
            raise ValueError(f"Fixture is already registered or copied: {relative}")
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(source.read_bytes())
        copied.append(str(target.relative_to(root)))
        method = safe_identifier(test_path.stem)
        names = method_names.setdefault(test_path.parent, set())
        if method in names:
            raise ValueError(
                f"Identifier collision in {test_path.parent}: {method}"
            )
        names.add(method)
        grouped.setdefault(test_path.parent, []).append(
            (test_path.stem, runtime_negative(source.read_text(encoding="utf-8-sig")))
        )

    batch_name = safe_identifier(run["batch_id"])
    for folder, tests in sorted(grouped.items()):
        namespace = ".".join(
            ["Jroc", "Test262", "Tests"]
            + [safe_identifier(part) for part in folder.parts]
        )
        class_name = f"NativePortBatch_{batch_name}"
        cs = test_root / folder / f"{class_name}.cs"
        if cs.exists():
            raise ValueError(f"Registration already exists: {cs.relative_to(root)}")
        lines = [
            "using Jroc.Test262.Tests;",
            "",
            f"namespace {namespace};",
            "",
            f"public sealed class {class_name} : DiskExecutionTestsBase",
            "{",
            f'    public {class_name}() : base("{namespace}") {{ }}',
            "",
        ]
        for name, is_runtime_negative in sorted(tests):
            method = safe_identifier(name)
            lines.append(f'    [Fact(DisplayName = "{name}")]')
            suffix = ", allowUnhandledException: true" if is_runtime_negative else ""
            lines.append(
                f'    public Task {method}() => ExecutionTestFromFile("{name}"{suffix});'
            )
            lines.append("")
        lines.append("}")
        cs.parent.mkdir(parents=True, exist_ok=True)
        cs.write_text("\n".join(lines) + "\n", encoding="utf-8")
        copied.append(str(cs.relative_to(root)))

    copied.extend(update_coverage_docs(root, len(summary["accepted"]), run["batch_id"]))
    result = {
        "run_id": args.run_id,
        "batch_id": run["batch_id"],
        "accepted": summary["accepted"],
        "copied": sorted(copied),
        "source_fidelity": True,
        "file_hashes": {
            path: file_sha256(root / path) for path in sorted(copied)
        },
    }
    if args.output:
        write_json(Path(args.output), result)
    return result


def validate_patch(args: argparse.Namespace) -> dict[str, Any]:
    root = Path(args.root).resolve()
    generated = json.loads(Path(args.generated).read_text(encoding="utf-8"))
    expected = set(generated["copied"])
    changed = set(
        subprocess.check_output(
            ["git", "-C", str(root), "diff", "--name-only", args.base_revision, "--"],
            text=True,
        ).splitlines()
    )
    rejected = sorted(changed - expected)
    missing = sorted(expected - changed)
    hash_mismatches = sorted(
        path
        for path, expected_hash in generated["file_hashes"].items()
        if not (root / path).is_file()
        or file_sha256(root / path) != expected_hash
    )
    patch = subprocess.check_output(
        ["git", "-C", str(root), "diff", "--binary", args.base_revision, "--"]
    )
    result = {
        "base_revision": args.base_revision,
        "changed": sorted(changed),
        "expected": sorted(expected),
        "rejected": rejected,
        "missing": missing,
        "hash_mismatches": hash_mismatches,
        "valid": not rejected and not missing and not hash_mismatches,
        "patch_digest": hashlib.sha256(patch).hexdigest(),
    }
    if args.output:
        write_json(Path(args.output), result)
    if not result["valid"]:
        raise ValueError("Patch does not exactly match the generated allowlist and hashes")
    return result


def checkpoint(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    summary = report(db, argparse.Namespace(run_id=args.run_id, output=None))
    artifact = Path(args.artifact)
    if not artifact.is_file():
        raise ValueError(f"Checkpoint artifact does not exist: {artifact}")
    cursor = db.execute(
        "SELECT COALESCE(SUM(cursor),0) FROM candidates WHERE run_id=?",
        (args.run_id,),
    ).fetchone()[0]
    artifact_digest = file_sha256(artifact)
    with db:
        db.execute(
            """INSERT INTO checkpoints(run_id,cursor,artifact_digest,complete,created_at)
               VALUES(?,?,?,?,?)""",
            (
                args.run_id,
                cursor,
                artifact_digest,
                int(summary["complete_native_acceptance"]),
                time.time(),
            ),
        )
        db.execute(
            "UPDATE runs SET state=?,updated_at=? WHERE run_id=?",
            (
                "accepted" if summary["complete_native_acceptance"] else "deferred",
                time.time(),
                args.run_id,
            ),
        )
    return {
        "run_id": args.run_id,
        "cursor": cursor,
        "artifact_digest": artifact_digest,
        "complete": summary["complete_native_acceptance"],
    }


def manifest(args: argparse.Namespace) -> dict[str, Any]:
    files = [Path(path) for path in args.files]
    result = {
        "workflow": args.workflow,
        "run_id": args.run_id,
        "target_revision": args.target_revision,
        "files": {
            str(path): file_sha256(path)
            for path in sorted(files)
            if path.is_file()
        },
    }
    if len(result["files"]) != len(files):
        raise ValueError("One or more manifest inputs are missing")
    write_json(Path(args.output), result)
    return result


def verify_manifest(args: argparse.Namespace) -> dict[str, Any]:
    value = json.loads(Path(args.manifest).read_text(encoding="utf-8"))
    if value["workflow"] != args.workflow:
        raise ValueError("Artifact workflow identity mismatch")
    if str(value["run_id"]) != str(args.run_id):
        raise ValueError("Artifact run identity mismatch")
    if value["target_revision"] != args.target_revision:
        raise ValueError("Artifact target revision mismatch")
    mismatches = [
        path
        for path, digest in value["files"].items()
        if not Path(path).is_file() or file_sha256(Path(path)) != digest
    ]
    if mismatches:
        raise ValueError("Artifact digest mismatch: " + ", ".join(mismatches))
    return {"valid": True, "files": sorted(value["files"])}


def publication_guard(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    summary = report(db, argparse.Namespace(run_id=args.run_id, output=None))
    if not summary["complete_native_acceptance"]:
        raise ValueError("Publication requires at least one complete native acceptance")
    validation = json.loads(Path(args.validation).read_text(encoding="utf-8"))
    if not validation.get("valid") or validation.get("patch_digest") != args.patch_digest:
        raise ValueError("Publication patch validation or digest does not match")
    if run["trigger_revision"] != args.target_revision:
        raise ValueError("Publication target does not match native acceptance")
    if args.dry_run:
        return {"publishable": True, "dry_run": True, "batch_id": run["batch_id"]}
    if not args.branch:
        raise ValueError("--branch is required for publication")
    with db:
        db.execute(
            """INSERT INTO publications(batch_id,run_id,branch,state,updated_at)
               VALUES(?,?,?,'validated',?)
               ON CONFLICT(batch_id) DO UPDATE SET
                 run_id=excluded.run_id,branch=excluded.branch,
                 state='validated',updated_at=excluded.updated_at""",
            (run["batch_id"], args.run_id, args.branch, time.time()),
        )
    return {
        "publishable": True,
        "dry_run": False,
        "batch_id": run["batch_id"],
        "branch": args.branch,
    }


def publication_state(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    publication = db.execute(
        "SELECT * FROM publications WHERE batch_id=?", (args.batch_id,)
    ).fetchone()
    if not publication:
        raise ValueError(f"Unknown publication batch: {args.batch_id}")
    with db:
        db.execute(
            """UPDATE publications
               SET pr_number=COALESCE(?,pr_number),
                   expected_head=COALESCE(?,expected_head),
                   state=?,
                   closure_reason=?,
                   updated_at=?
               WHERE batch_id=?""",
            (
                args.pr_number,
                args.expected_head,
                args.state,
                args.closure_reason,
                time.time(),
                args.batch_id,
            ),
        )
    return {
        "batch_id": args.batch_id,
        "state": args.state,
        "pr_number": args.pr_number,
        "expected_head": args.expected_head,
        "closure_reason": args.closure_reason,
    }


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--db", type=Path, default=Path("artifacts/test262/native.sqlite"))
    sub = parser.add_subparsers(dest="command", required=True)
    init = sub.add_parser("init")
    init.set_defaults(action=lambda args, db: {"schema_version": SCHEMA_VERSION})

    create = sub.add_parser("create-run")
    create.add_argument("--run-id")
    create.add_argument("--batch-id")
    create.add_argument("--trigger-revision", required=True)
    create.add_argument("--base-revision", required=True)
    create.add_argument("--pin", required=True)
    create.add_argument("--candidate-limit", type=int, default=200)
    create.add_argument("--accepted-limit", type=int, default=100)
    create.add_argument("--variant-limit", type=int, default=400)
    create.add_argument("--time-limit", type=int, default=20 * 60)
    create.set_defaults(action=lambda args, db: {"run_id": create_run(db, args)})

    plan_parser = sub.add_parser("plan")
    plan_parser.add_argument("--run-id", required=True)
    plan_parser.add_argument("--catalog", type=Path, required=True)
    plan_parser.add_argument("--area", default=SUPPORTED_AREA)
    plan_parser.add_argument("--capability-identity", default="native-host-v1")
    plan_parser.add_argument("--output", type=Path)
    plan_parser.set_defaults(action=lambda args, db: plan(db, args))

    screen = sub.add_parser("screen-plan")
    screen.add_argument("--run-id", required=True)
    screen.add_argument("--upstream", type=Path, required=True)
    screen.add_argument("--timeout-ms", type=int, default=30000)
    screen.add_argument("--output", type=Path, required=True)
    screen.set_defaults(action=lambda args, db: screening_plan(db, args))

    rec = sub.add_parser("record")
    rec.add_argument("--run-id", required=True)
    rec.add_argument("--path", required=True)
    rec.add_argument("--variant", required=True)
    rec.add_argument("--fixture-sha256", required=True)
    rec.add_argument("--pin", required=True)
    rec.add_argument("--compiler-identity", required=True)
    rec.add_argument("--harness-identity", required=True)
    rec.add_argument("--environment-identity", required=True)
    rec.add_argument("--phase", default="execution")
    rec.add_argument("--diagnostic", default="")
    rec.add_argument("--outcome", required=True)
    rec.add_argument("--failure-class")
    rec.add_argument("--started-at", type=float)
    rec.add_argument("--finished-at", type=float)
    rec.set_defaults(action=lambda args, db: (record(db, args), {"recorded": True})[1])

    imported = sub.add_parser("import-results")
    imported.add_argument("--run-id", required=True)
    imported.add_argument("--results", type=Path, required=True)
    imported.add_argument("--pin", required=True)
    imported.add_argument("--compiler-identity", required=True)
    imported.add_argument("--harness-identity", required=True)
    imported.add_argument("--environment-identity", required=True)
    imported.set_defaults(action=lambda args, db: import_results(db, args))

    rep = sub.add_parser("report")
    rep.add_argument("--run-id", required=True)
    rep.add_argument("--output", type=Path)
    rep.set_defaults(action=lambda args, db: report(db, args))

    generate = sub.add_parser("generate")
    generate.add_argument("--run-id", required=True)
    generate.add_argument("--upstream", type=Path, required=True)
    generate.add_argument("--destination", type=Path, required=True)
    generate.add_argument("--output", type=Path)
    generate.set_defaults(action=lambda args, db: generate_batch(db, args))

    validate = sub.add_parser("validate-patch")
    validate.add_argument("--root", type=Path, default=Path("."))
    validate.add_argument("--base-revision", required=True)
    validate.add_argument("--generated", type=Path, required=True)
    validate.add_argument("--output", type=Path)
    validate.set_defaults(action=lambda args, db: validate_patch(args))

    check = sub.add_parser("checkpoint")
    check.add_argument("--run-id", required=True)
    check.add_argument("--artifact", type=Path, required=True)
    check.set_defaults(action=lambda args, db: checkpoint(db, args))

    make_manifest = sub.add_parser("manifest")
    make_manifest.add_argument("--workflow", required=True)
    make_manifest.add_argument("--run-id", required=True)
    make_manifest.add_argument("--target-revision", required=True)
    make_manifest.add_argument("--output", type=Path, required=True)
    make_manifest.add_argument("files", nargs="+")
    make_manifest.set_defaults(action=lambda args, db: manifest(args))

    verify = sub.add_parser("verify-manifest")
    verify.add_argument("--manifest", type=Path, required=True)
    verify.add_argument("--workflow", required=True)
    verify.add_argument("--run-id", required=True)
    verify.add_argument("--target-revision", required=True)
    verify.set_defaults(action=lambda args, db: verify_manifest(args))

    pub = sub.add_parser("publication-guard")
    pub.add_argument("--run-id", required=True)
    pub.add_argument("--target-revision", required=True)
    pub.add_argument("--validation", type=Path, required=True)
    pub.add_argument("--patch-digest", required=True)
    pub.add_argument("--branch")
    pub.add_argument("--dry-run", action="store_true")
    pub.set_defaults(action=lambda args, db: publication_guard(db, args))

    pub_state = sub.add_parser("publication-state")
    pub_state.add_argument("--batch-id", required=True)
    pub_state.add_argument("--state", required=True)
    pub_state.add_argument("--pr-number", type=int)
    pub_state.add_argument("--expected-head")
    pub_state.add_argument("--closure-reason")
    pub_state.set_defaults(action=lambda args, db: publication_state(db, args))

    args = parser.parse_args()
    db = connect(args.db)
    try:
        result = args.action(args, db)
        print(json.dumps(result, indent=2, sort_keys=True))
        return 0
    except (OSError, sqlite3.Error, subprocess.CalledProcessError, ValueError) as error:
        print(f"nativePorting: {error}", file=sys.stderr)
        return 2
    finally:
        db.close()


if __name__ == "__main__":
    raise SystemExit(main())
