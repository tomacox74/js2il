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

SCHEMA_VERSION = "4"
COMPATIBLE_SCHEMA_VERSIONS = {"3", SCHEMA_VERSION}
OUTCOMES = {"pass", "fail", "deferred", "unsupported", "infrastructure-error", "incomplete"}
FAILURE_CLASSES = {
    "product-feature-gap",
    "semantic-defect",
    "harness-gap",
    "policy-exclusion",
    "infrastructure-error",
    "unresolved",
}
DEFAULT_AREA = "auto"
NATIVE_ELIGIBLE_BLOCKERS = {
    "async-requirement",
    "agent-requirement",
    "can-block-requirement",
    "module-flag",
}
COMPONENT_FEATURE_MAP = {
    "callable-lowering": ("async", "generator", "arrow", "function", "class"),
    "private-member-lowering": ("private", "private-elements", "private-method"),
    "property-operations": ("property", "computed-property", "proxy", "reflect"),
    "iterator-promise-runtime": ("iterator", "promise", "async-iterator"),
    "regex-runtime": ("regexp", "regex"),
    "native-capability": ("atomics", "async", "module-code", "import", "export"),
}
CHANGE_COMPONENT_MAP = (
    ("private-member-lowering", ("private", "class", "brand")),
    ("regex-runtime", ("regexp", "regex")),
    ("iterator-promise-runtime", ("iterator", "promise", "async")),
    ("property-operations", ("objectruntime", "property", "proxy", "reflect")),
    ("callable-lowering", ("compiler/ir", "callable", "function", "arrow", "class")),
)


def canonical(value: Any) -> str:
    return json.dumps(value, sort_keys=True, separators=(",", ":"))


def porting_supported_path(path: str) -> bool:
    """Areas supported by native test generation and coverage accounting."""
    return path.startswith(("test/language/", "test/built-ins/"))


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
        CREATE TABLE IF NOT EXISTS active_provenance (
          run_id TEXT PRIMARY KEY REFERENCES runs(run_id),
          compiler_identity TEXT NOT NULL,
          harness_identity TEXT NOT NULL,
          environment_identity TEXT NOT NULL,
          updated_at REAL NOT NULL
        );
        CREATE TABLE IF NOT EXISTS automation_state (
          state_id INTEGER PRIMARY KEY CHECK (state_id=1),
          last_reconciled_revision TEXT,
          fallback_cursor INTEGER NOT NULL DEFAULT 0,
          updated_at REAL NOT NULL
        );
        CREATE TABLE IF NOT EXISTS reconciliations (
          run_id TEXT PRIMARY KEY REFERENCES runs(run_id),
          target_revision TEXT NOT NULL,
          base_revision TEXT NOT NULL,
          attribution TEXT NOT NULL,
          changed_files TEXT NOT NULL,
          components TEXT NOT NULL,
          fallback_advance INTEGER NOT NULL DEFAULT 0,
          state TEXT NOT NULL,
          created_at REAL NOT NULL,
          updated_at REAL NOT NULL
        );
        CREATE TABLE IF NOT EXISTS pending_work (
          path TEXT PRIMARY KEY,
          sha256 TEXT NOT NULL,
          variants TEXT NOT NULL,
          selection_reason TEXT NOT NULL,
          evidence_kind TEXT NOT NULL,
          catalog_provenance TEXT NOT NULL,
          capability_identity TEXT NOT NULL,
          status TEXT NOT NULL,
          updated_at REAL NOT NULL
        );
        CREATE TABLE IF NOT EXISTS stage_metrics (
          run_id TEXT NOT NULL REFERENCES runs(run_id),
          stage TEXT NOT NULL,
          seconds REAL NOT NULL,
          PRIMARY KEY(run_id,stage)
        );
        """
    )
    existing = db.execute("SELECT value FROM meta WHERE key='schema_version'").fetchone()
    if existing and existing[0] not in COMPATIBLE_SCHEMA_VERSIONS:
        raise ValueError(
            f"Incompatible native evidence schema {existing[0]}; "
            f"start a documented cold state for schema {SCHEMA_VERSION}")
    db.execute(
        """INSERT OR IGNORE INTO automation_state
           (state_id,last_reconciled_revision,fallback_cursor,updated_at)
           VALUES(1,NULL,0,?)""",
        (time.time(),),
    )
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


def classify_changed_components(paths: list[str]) -> list[str]:
    components: set[str] = set()
    for path in paths:
        if not (
            path.startswith(("src/", "tests/Jroc.Testing/", "scripts/test262/"))
            or path.startswith(".github/workflows/test262-")
            or path == "tests/test262/test262.pin.json"
        ):
            continue
        lowered = path.lower()
        before = len(components)
        if path == "tests/test262/test262.pin.json" or path.startswith(
            ("scripts/test262/", "tests/Jroc.Testing/", ".github/workflows/test262-")
        ):
            components.add("native-capability")
        for component, terms in CHANGE_COMPONENT_MAP:
            if any(term in lowered for term in terms):
                components.add(component)
        if path.startswith("src/Compiler/") and len(components) == before:
            components.add("callable-lowering")
        if path.startswith("src/JavaScriptRuntime/") and len(components) == before:
            components.update(
                {"property-operations", "iterator-promise-runtime", "regex-runtime"}
            )
    return sorted(components)


def is_generated_only_change(paths: list[str]) -> bool:
    if not paths:
        return True
    return all(
        path == "CHANGELOG.md"
        or path == "docs/ECMA262/Index.md"
        or path == "docs/ECMA262/Test262Conformance.md"
        or path.startswith("tests/Jroc.Test262.Tests/")
        for path in paths
    )


def git_output(repository: Path, *args: str) -> str:
    return subprocess.check_output(
        ["git", "-C", str(repository), *args], text=True
    ).strip()


def resolve_range(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    repository = Path(args.repository).resolve()
    target = git_output(repository, "rev-parse", f"{args.target}^{{commit}}")
    state = db.execute(
        "SELECT last_reconciled_revision FROM automation_state WHERE state_id=1"
    ).fetchone()
    prior = state["last_reconciled_revision"] if state else None
    base = None
    attribution = "validated-range"
    if prior:
        ancestor = subprocess.run(
            ["git", "-C", str(repository), "merge-base", "--is-ancestor", prior, target],
            check=False,
        ).returncode == 0
        if ancestor:
            base = prior
        else:
            attribution = "bounded-baseline-missing-ancestor"
    if base is None:
        attribution = (
            attribution
            if prior
            else "bounded-baseline-first-run"
        )
        try:
            base = git_output(
                repository, "rev-parse", f"{target}~{args.baseline_commits}"
            )
        except subprocess.CalledProcessError:
            base = target
    changed = (
        git_output(repository, "diff", "--name-only", base, target).splitlines()
        if base != target
        else []
    )
    components = classify_changed_components(changed)
    generated_only = is_generated_only_change(changed)
    pending_count = db.execute(
        "SELECT COUNT(*) FROM pending_work WHERE status='pending'"
    ).fetchone()[0]
    result = {
        "target_revision": target,
        "base_revision": base,
        "last_reconciled_revision": prior,
        "attribution": attribution,
        "changed_files": changed,
        "components": components,
        "screen": (
            (bool(components) and not generated_only)
            or (args.resume_pending and pending_count > 0)
        ),
        "generated_only": generated_only,
        "pending_count": pending_count,
    }
    if args.output:
        write_json(Path(args.output), result)
    return result


def coherent_area(path: str) -> str:
    parts = Path(path).parts
    if len(parts) < 3 or parts[0] != "test":
        return "unknown"
    return "/".join(parts[1:3])


def candidate_components(path: str) -> list[str]:
    lowered = path.lower()
    return sorted(
        component
        for component, terms in COMPONENT_FEATURE_MAP.items()
        if any(term in lowered for term in terms)
    ) or ["unmapped"]


def native_eligible_state(row: sqlite3.Row) -> bool:
    if row["state"] == "runnable":
        return True
    if row["state"] != "blocked":
        return False
    reasons = json.loads(row["reasons"])
    codes = {reason.get("code") for reason in reasons}
    return bool(codes) and codes.issubset(NATIVE_ELIGIBLE_BLOCKERS)


def catalog_candidates(
    catalog_path: Path,
    area: str,
    limit: int,
    components: set[str],
    fallback_cursor: int,
) -> tuple[str, list[dict[str, Any]]]:
    normalized = area.strip("/")
    if normalized != DEFAULT_AREA and not normalized.startswith(("language/", "built-ins/")):
        raise ValueError(
            "Native automation area must be 'auto', language/<area>, or built-ins/<area>"
        )
    catalog = open_catalog(catalog_path)
    try:
        current = catalog.execute(
            "SELECT value FROM settings WHERE key='current'"
        ).fetchone()
        if not current:
            raise ValueError("Catalog checkpoint has no current provenance")
        provenance = current[0]
        area_filter = "" if normalized == DEFAULT_AREA else "AND (path=? OR path LIKE ?)"
        parameters: tuple[Any, ...] = (provenance,)
        if normalized != DEFAULT_AREA:
            parameters += (f"test/{normalized}", f"test/{normalized}/%")
        rows = catalog.execute(
            f"""SELECT path,sha256,variants,state,reasons
               FROM fixtures
               WHERE provenance=?
                 {area_filter}
                 AND NOT EXISTS (
                   SELECT 1 FROM registrations r WHERE r.path=fixtures.path
                 )
               ORDER BY path""",
            parameters,
        ).fetchall()
        native_root = Path(__file__).resolve().parents[2] / "tests/Jroc.Test262.Tests"
        legacy_hashes: dict[str, set[str]] = {}
        for existing in native_root.rglob("*.js"):
            legacy_hashes.setdefault(existing.name, set()).add(file_sha256(existing))
        candidates: list[dict[str, Any]] = []
        retry_candidates: list[dict[str, Any]] = []
        fallback_candidates: list[dict[str, Any]] = []
        for row in rows:
            if not porting_supported_path(row["path"]) or not native_eligible_state(row):
                continue
            variants = json.loads(row["variants"])
            if not variants:
                continue
            current_evidence = {
                result["variant"]: result["verdict"]
                for result in catalog.execute(
                    """SELECT variant,verdict FROM results
                       WHERE provenance=? AND path=?""",
                    (provenance, row["path"]),
                )
            }
            evidence_kind = None
            failure_kind = None
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
            if evidence_kind is None:
                observed = [
                    current_evidence.get(variant)
                    for variant in variants
                    if current_evidence.get(variant) is not None
                ]
                if any(verdict != "matched" for verdict in observed):
                    failure_kind = "current-provenance MVP failure hint"
                else:
                    historical_failures = catalog.execute(
                        """SELECT 1 FROM results
                           WHERE path=? AND provenance!=? AND verdict!='matched'
                           LIMIT 1""",
                        (row["path"], provenance),
                    ).fetchone()
                    if historical_failures:
                        failure_kind = "historical MVP failure hint"
            if evidence_kind is None and failure_kind is None and row["state"] == "blocked":
                failure_kind = "MVP-blocked native-capability discovery hint"
            filename = Path(row["path"]).name
            legacy_duplicate = row["sha256"] in legacy_hashes.get(filename, set())
            if not legacy_duplicate and (evidence_kind or failure_kind):
                likely_components = candidate_components(row["path"])
                relevant = bool(components.intersection(likely_components))
                candidate = {
                    "path": row["path"],
                    "sha256": row["sha256"],
                    "variants": variants,
                    "evidence_kind": evidence_kind or failure_kind,
                    "reason": (
                        f"{evidence_kind or failure_kind}; likely component "
                        f"{','.join(likely_components)}; fresh native acceptance required"
                    ),
                    "retry_priority": 0 if failure_kind and relevant else (
                        1 if failure_kind else 2
                    ),
                    "area": coherent_area(row["path"]),
                }
                (retry_candidates if failure_kind and relevant else fallback_candidates).append(
                    candidate
                )
        if normalized == DEFAULT_AREA and (retry_candidates or fallback_candidates):
            counts: dict[str, int] = {}
            area_source = retry_candidates or fallback_candidates
            for candidate in area_source:
                counts[candidate["area"]] = counts.get(candidate["area"], 0) + 1
            selected_area = sorted(counts, key=lambda key: (-counts[key], key))[0]
            retry_candidates = [
                candidate for candidate in retry_candidates
                if candidate["area"] == selected_area
            ]
            fallback_candidates = [
                candidate for candidate in fallback_candidates
                if candidate["area"] == selected_area
            ]
        retry_limit = min(limit, (limit * 80) // 100)
        selected_retry = retry_candidates[:retry_limit]
        remaining = limit - len(selected_retry)
        if fallback_candidates:
            offset = fallback_cursor % len(fallback_candidates)
            rotated = fallback_candidates[offset:] + fallback_candidates[:offset]
        else:
            rotated = []
        selected_fallback = rotated[:remaining]
        candidates = selected_retry + selected_fallback
        if len(candidates) < limit:
            candidates.extend(
                retry_candidates[retry_limit:retry_limit + (limit - len(candidates))]
            )
        for candidate in candidates:
            candidate["fallback_selected"] = candidate in selected_fallback
        return provenance, candidates[:limit]
    finally:
        catalog.close()


def plan(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    active = db.execute(
        "SELECT * FROM active_provenance WHERE run_id=?", (args.run_id,)
    ).fetchone()
    budgets = json.loads(run["budgets"])
    components = {
        value for value in (args.components or "").split(",") if value
    }
    automation = db.execute(
        "SELECT fallback_cursor FROM automation_state WHERE state_id=1"
    ).fetchone()
    fallback_cursor = automation["fallback_cursor"] if automation else 0
    if args.skip_selection:
        provenance, candidates = "none", []
    else:
        if not args.catalog:
            raise ValueError("--catalog is required unless --skip-selection is used")
        provenance, candidates = catalog_candidates(
            Path(args.catalog),
            args.area,
            budgets["candidate_limit"],
            components,
            fallback_cursor,
        )
    pending = db.execute(
        """SELECT * FROM pending_work
           WHERE status='pending' ORDER BY updated_at,path"""
    ).fetchall()
    if args.skip_selection:
        pending = []
    elif pending:
        if args.area == DEFAULT_AREA:
            pending_area = coherent_area(pending[0]["path"])
            pending = [
                row for row in pending if coherent_area(row["path"]) == pending_area
            ]
        else:
            prefix = f"test/{args.area.strip('/')}/"
            pending = [row for row in pending if row["path"].startswith(prefix)]
        pending = pending[: max(1, budgets["candidate_limit"] // 2)]
    selected: list[dict[str, Any]] = [
        {
            "path": row["path"],
            "sha256": row["sha256"],
            "variants": json.loads(row["variants"]),
            "reason": row["selection_reason"] + "; resumed durable pending work",
            "evidence_kind": row["evidence_kind"],
            "fallback_selected": False,
        }
        for row in pending
    ]
    if selected:
        selected_area = coherent_area(selected[0]["path"])
        candidates = [
            candidate for candidate in candidates
            if coherent_area(candidate["path"]) == selected_area
        ]
    selected_paths = {candidate["path"] for candidate in selected}
    selected.extend(
        candidate for candidate in candidates
        if candidate["path"] not in selected_paths
    )
    selected = selected[:budgets["candidate_limit"]]
    now = time.time()
    with db:
        for candidate in selected:
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
                    candidate["reason"] if not capability_deferred else
                    candidate["reason"] + "; unchanged native capability remains unsupported",
                    provenance,
                    args.capability_identity,
                    "capability-deferred" if capability_deferred else "pending",
                ),
            )
            db.execute(
                """INSERT OR REPLACE INTO pending_work
                   (path,sha256,variants,selection_reason,evidence_kind,
                    catalog_provenance,capability_identity,status,updated_at)
                   VALUES(?,?,?,?,?,?,?,? ,?)""",
                (
                    candidate["path"],
                    candidate["sha256"],
                    canonical(candidate["variants"]),
                    candidate["reason"],
                    candidate["evidence_kind"],
                    provenance,
                    args.capability_identity,
                    "deferred" if capability_deferred else "pending",
                    now,
                ),
            )
            db.execute(
                "UPDATE candidates SET evidence_kind=? WHERE run_id=? AND path=?",
                (candidate["evidence_kind"], args.run_id, candidate["path"]),
            )
        range_document = (
            json.loads(Path(args.range).read_text(encoding="utf-8"))
            if args.range
            else {
                "target_revision": run["trigger_revision"],
                "base_revision": run["base_revision"],
                "attribution": "manual",
                "changed_files": [],
                "components": sorted(components),
            }
        )
        db.execute(
            """INSERT OR REPLACE INTO reconciliations
               (run_id,target_revision,base_revision,attribution,changed_files,
                components,fallback_advance,state,created_at,updated_at)
               VALUES(?,?,?,?,?,?,?,'planned',?,?)""",
            (
                args.run_id,
                range_document["target_revision"],
                range_document["base_revision"],
                range_document["attribution"],
                canonical(range_document["changed_files"]),
                canonical(range_document["components"]),
                len(
                    [
                        candidate for candidate in selected
                        if candidate.get("fallback_selected", False)
                    ]
                ),
                now,
                now,
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
        "components": sorted(components),
        "candidate_count": len(all_candidates),
        "new_candidate_count": len(
            [candidate for candidate in selected if candidate["path"] not in selected_paths]
        ),
        "resumed_pending_count": len(
            [candidate for candidate in selected if candidate["path"] in selected_paths]
        ),
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
    active = db.execute(
        "SELECT * FROM active_provenance WHERE run_id=?", (args.run_id,)
    ).fetchone()
    latest = {
        (row["path"], row["variant"])
        for row in db.execute(
            """SELECT a.path,a.variant
               FROM attempts a
               JOIN (
                 SELECT path,variant,MAX(attempt_id) attempt_id
                 FROM attempts WHERE run_id=? GROUP BY path,variant
               ) latest ON latest.attempt_id=a.attempt_id
               WHERE a.run_id=? AND a.outcome='pass'
                 AND ? IS NOT NULL
                 AND a.compiler_identity=?
                 AND a.harness_identity=?
                 AND a.environment_identity=?""",
            (
                args.run_id,
                args.run_id,
                1 if active else None,
                active["compiler_identity"] if active else "",
                active["harness_identity"] if active else "",
                active["environment_identity"] if active else "",
            ),
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
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    if args.pin != run["pin"]:
        raise ValueError("Outcome pin does not match the run")
    active = db.execute(
        "SELECT * FROM active_provenance WHERE run_id=?", (args.run_id,)
    ).fetchone()
    if active is None:
        raise ValueError("Active native provenance has not been established")
    if (
        args.compiler_identity != active["compiler_identity"]
        or args.harness_identity != active["harness_identity"]
        or args.environment_identity != active["environment_identity"]
    ):
        raise ValueError("Outcome provenance does not match the active native build")
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
    active = db.execute(
        "SELECT * FROM active_provenance WHERE run_id=?", (args.run_id,)
    ).fetchone()
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
                active["compiler_identity"] if active else "",
                active["harness_identity"] if active else "",
                active["environment_identity"] if active else "",
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
        "active_provenance": (
            {
                "compiler_identity": active["compiler_identity"],
                "harness_identity": active["harness_identity"],
                "environment_identity": active["environment_identity"],
            }
            if active
            else None
        ),
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
    stages = {
        row["stage"]: row["seconds"]
        for row in db.execute(
            "SELECT stage,seconds FROM stage_metrics WHERE run_id=? ORDER BY stage",
            (args.run_id,),
        )
    }
    active_seconds = float(run["active_seconds"])
    result["metrics"] = {
        "stage_seconds": stages,
        "screening_active_seconds": active_seconds,
        "accepted_per_active_hour": (
            len(batch_accepted) * 3600 / active_seconds
            if active_seconds > 0
            else None
        ),
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
    keywords = {
        "abstract", "as", "base", "bool", "break", "byte", "case", "catch",
        "char", "checked", "class", "const", "continue", "decimal", "default",
        "delegate", "do", "double", "else", "enum", "event", "explicit",
        "extern", "false", "finally", "fixed", "float", "for", "foreach",
        "goto", "if", "implicit", "in", "int", "interface", "internal", "is",
        "lock", "long", "namespace", "new", "null", "object", "operator",
        "out", "override", "params", "private", "protected", "public",
        "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof",
        "stackalloc", "static", "string", "struct", "switch", "this", "throw",
        "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe",
        "ushort", "using", "virtual", "void", "volatile", "while",
    }
    if not result[0].isalpha() or result in keywords:
        return f"Test_{result}"
    return result


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


def update_table_row(
    text: str, label: str, increment: int, section: str | None = None
) -> str:
    start = 0
    end = len(text)
    if section:
        heading = f"## {section}"
        start = text.find(heading)
        if start < 0:
            raise ValueError(f"Coverage section not found: {section}")
        next_heading = text.find("\n## ", start + len(heading))
        end = next_heading if next_heading >= 0 else len(text)
    segment = text[start:end]
    pattern = re.compile(
        rf"^\| {re.escape(label)} \| (?P<passed>[\d,]+) \| "
        rf"(?P<unsupported>[\d,]+) \| (?P<unverified>[\d,]+) \| "
        rf"(?P<total>[\d,]+) \| \*\*(?P<percent>[\d.]+)%\*\* \|$",
        re.M,
    )
    match = pattern.search(segment)
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
    absolute_start = start + match.start()
    absolute_end = start + match.end()
    return text[:absolute_start] + replacement + text[absolute_end:]


def coverage_labels(paths: list[str]) -> dict[tuple[str | None, str], int]:
    labels: dict[tuple[str | None, str], int] = {(None, "**Total**"): len(paths)}
    for path in paths:
        parts = Path(path).parts
        if len(parts) < 3:
            raise ValueError(f"Cannot classify coverage path: {path}")
        if parts[1] == "language":
            key = (None, "Language syntax and semantics")
            labels[key] = (
                labels.get(key, 0) + 1
            )
        elif parts[1] == "built-ins":
            key = (None, "Built-in objects and APIs")
            labels[key] = (
                labels.get(key, 0) + 1
            )
        else:
            raise ValueError(f"Unsupported coverage area: {path}")
        feature = (None, f"`{parts[2]}`")
        labels[feature] = labels.get(feature, 0) + 1
        if len(parts) > 3 and parts[1] == "language":
            section = {
                "expressions": "Expression Features",
                "statements": "Statement and Declaration Features",
            }.get(parts[2])
            if section:
                subfeature = (section, f"`{parts[3]}`")
                labels[subfeature] = labels.get(subfeature, 0) + 1
    return labels


def update_coverage_docs(root: Path, accepted: list[str], batch_id: str) -> list[str]:
    conformance = root / "docs/ECMA262/Test262Conformance.md"
    text = conformance.read_text(encoding="utf-8")
    for (section, label), increment in coverage_labels(accepted).items():
        text = update_table_row(text, label, increment, section)
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
    accepted_count = len(accepted)
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
            f"- test262: verify {accepted_count} additional pinned fixtures "
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


def fixture_dependencies(upstream: Path, relative: str) -> list[str]:
    result: list[str] = []
    pending = [relative]
    seen = {relative}
    pattern = re.compile(
        r"""(?:
            (?:import|export)\s+(?:[^'"]*?\s+from\s+)?|
            import\s*\(
        )['"](?P<specifier>\.[^'"]+)['"]""",
        re.X,
    )
    while pending:
        current = pending.pop()
        source = upstream / current
        text = source.read_text(encoding="utf-8-sig")
        for match in pattern.finditer(text):
            specifier = match.group("specifier")
            dependency = (Path(current).parent / specifier).as_posix()
            if not Path(dependency).suffix:
                dependency += ".js"
            resolved = upstream / dependency
            if dependency in seen or not resolved.is_file():
                continue
            if not resolved.resolve().is_relative_to(upstream):
                raise ValueError(f"Fixture dependency escapes pinned root: {specifier}")
            seen.add(dependency)
            result.append(dependency)
            pending.append(dependency)
    return sorted(result)


def generate_batch(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    summary = report(db, argparse.Namespace(run_id=args.run_id, output=None))
    if not summary["accepted"]:
        raise ValueError("Batch generation requires at least one freshly accepted fixture")
    # Validate the entire batch before copying any files into the checkout.
    for relative in summary["accepted"]:
        if not porting_supported_path(relative):
            raise ValueError(f"Accepted fixture is outside supported areas: {relative}")
    upstream = Path(args.upstream).resolve()
    root = Path(args.destination).resolve()
    test_root = root / "tests/Jroc.Test262.Tests"
    copied: list[str] = []
    grouped: dict[Path, list[tuple[str, bool]]] = {}
    method_names: dict[Path, set[str]] = {}
    for relative in summary["accepted"]:
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
        for dependency in fixture_dependencies(upstream, relative):
            dependency_path = Path(dependency).relative_to("test")
            dependency_target = (
                test_root / dependency_path.parent / "JavaScript" / dependency_path.name
            )
            if not dependency_target.exists():
                dependency_target.parent.mkdir(parents=True, exist_ok=True)
                dependency_target.write_bytes((upstream / dependency).read_bytes())
                copied.append(str(dependency_target.relative_to(root)))
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

    copied.extend(update_coverage_docs(root, summary["accepted"], run["batch_id"]))
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


def finalize_reconciliation(
    db: sqlite3.Connection, args: argparse.Namespace
) -> dict[str, Any]:
    reconciliation = db.execute(
        "SELECT * FROM reconciliations WHERE run_id=?", (args.run_id,)
    ).fetchone()
    if not reconciliation:
        raise ValueError(f"Run has no planned reconciliation: {args.run_id}")
    summary = report(db, argparse.Namespace(run_id=args.run_id, output=None))
    now = time.time()
    with db:
        for path in summary["accepted"]:
            db.execute("DELETE FROM pending_work WHERE path=?", (path,))
        accepted = set(summary["accepted"])
        for row in db.execute(
            "SELECT path FROM candidates WHERE run_id=?", (args.run_id,)
        ):
            if row["path"] not in accepted:
                db.execute(
                    """UPDATE pending_work SET status='deferred',updated_at=?
                       WHERE path=?""",
                    (now, row["path"]),
                )
        db.execute(
            """UPDATE automation_state
               SET last_reconciled_revision=?,
                   fallback_cursor=fallback_cursor+?,
                   updated_at=?
               WHERE state_id=1""",
            (
                reconciliation["target_revision"],
                reconciliation["fallback_advance"],
                now,
            ),
        )
        db.execute(
            """UPDATE reconciliations SET state='reconciled',updated_at=?
               WHERE run_id=?""",
            (now, args.run_id),
        )
    return {
        "run_id": args.run_id,
        "last_reconciled_revision": reconciliation["target_revision"],
        "pending_count": db.execute(
            "SELECT COUNT(*) FROM pending_work WHERE status='pending'"
        ).fetchone()[0],
        "fallback_advance": reconciliation["fallback_advance"],
    }


def record_stage(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    if args.seconds < 0:
        raise ValueError("Stage duration cannot be negative")
    if not db.execute(
        "SELECT 1 FROM runs WHERE run_id=?", (args.run_id,)
    ).fetchone():
        raise ValueError(f"Unknown run: {args.run_id}")
    with db:
        db.execute(
            """INSERT OR REPLACE INTO stage_metrics(run_id,stage,seconds)
               VALUES(?,?,?)""",
            (args.run_id, args.stage, args.seconds),
        )
    return {"run_id": args.run_id, "stage": args.stage, "seconds": args.seconds}


def set_provenance(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    if not db.execute(
        "SELECT 1 FROM runs WHERE run_id=?", (args.run_id,)
    ).fetchone():
        raise ValueError(f"Unknown run: {args.run_id}")
    with db:
        db.execute(
            """INSERT INTO active_provenance
               (run_id,compiler_identity,harness_identity,environment_identity,updated_at)
               VALUES(?,?,?,?,?)
               ON CONFLICT(run_id) DO UPDATE SET
                 compiler_identity=excluded.compiler_identity,
                 harness_identity=excluded.harness_identity,
                 environment_identity=excluded.environment_identity,
                 updated_at=excluded.updated_at""",
            (
                args.run_id,
                args.compiler_identity,
                args.harness_identity,
                args.environment_identity,
                time.time(),
            ),
        )
    return {
        "run_id": args.run_id,
        "compiler_identity": args.compiler_identity,
        "harness_identity": args.harness_identity,
        "environment_identity": args.environment_identity,
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


def validate_state(path: Path) -> dict[str, Any]:
    if not path.is_file() or path.stat().st_size == 0:
        raise ValueError(f"Native checkpoint is missing or empty: {path}")
    db = sqlite3.connect(f"file:{path.resolve()}?mode=ro", uri=True)
    try:
        integrity = db.execute("PRAGMA integrity_check").fetchone()
        if not integrity or integrity[0] != "ok":
            raise ValueError(f"Native checkpoint failed integrity validation: {path}")
        tables = {
            row[0]
            for row in db.execute(
                "SELECT name FROM sqlite_master WHERE type='table'"
            )
        }
        required = {
            "meta",
            "runs",
            "candidates",
            "attempts",
            "checkpoints",
            "publications",
            "active_provenance",
        }
        if not required.issubset(tables):
            raise ValueError("Native checkpoint has an incompatible table inventory")
        schema = db.execute(
            "SELECT value FROM meta WHERE key='schema_version'"
        ).fetchone()
        if not schema or schema[0] not in COMPATIBLE_SCHEMA_VERSIONS:
            raise ValueError(
                f"Native checkpoint schema is not compatible with {SCHEMA_VERSION}"
            )
        if schema[0] == SCHEMA_VERSION:
            version_four = {
                "automation_state",
                "reconciliations",
                "pending_work",
                "stage_metrics",
            }
            if not version_four.issubset(tables):
                raise ValueError("Native checkpoint is missing schema 4 tables")
        return {
            "valid": True,
            "schema_version": schema[0],
            "runs": db.execute("SELECT COUNT(*) FROM runs").fetchone()[0],
        }
    finally:
        db.close()


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--db", type=Path, default=Path("artifacts/test262/native.sqlite"))
    sub = parser.add_subparsers(dest="command", required=True)
    init = sub.add_parser("init")
    init.set_defaults(action=lambda args, db: {"schema_version": SCHEMA_VERSION})
    sub.add_parser("validate-state")

    resolve = sub.add_parser("resolve-range")
    resolve.add_argument("--repository", type=Path, default=Path("."))
    resolve.add_argument("--target", required=True)
    resolve.add_argument("--baseline-commits", type=int, default=1)
    resolve.add_argument("--resume-pending", action="store_true")
    resolve.add_argument("--output", type=Path)
    resolve.set_defaults(action=lambda args, db: resolve_range(db, args))

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
    plan_parser.add_argument("--catalog", type=Path)
    plan_parser.add_argument("--area", default=DEFAULT_AREA)
    plan_parser.add_argument("--components", default="")
    plan_parser.add_argument("--range", type=Path)
    plan_parser.add_argument("--skip-selection", action="store_true")
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

    finalize = sub.add_parser("finalize-reconciliation")
    finalize.add_argument("--run-id", required=True)
    finalize.set_defaults(action=lambda args, db: finalize_reconciliation(db, args))

    stage = sub.add_parser("record-stage")
    stage.add_argument("--run-id", required=True)
    stage.add_argument("--stage", required=True)
    stage.add_argument("--seconds", type=float, required=True)
    stage.set_defaults(action=lambda args, db: record_stage(db, args))

    provenance = sub.add_parser("set-provenance")
    provenance.add_argument("--run-id", required=True)
    provenance.add_argument("--compiler-identity", required=True)
    provenance.add_argument("--harness-identity", required=True)
    provenance.add_argument("--environment-identity", required=True)
    provenance.set_defaults(action=lambda args, db: set_provenance(db, args))

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
    if args.command == "validate-state":
        try:
            print(json.dumps(validate_state(args.db), indent=2, sort_keys=True))
            return 0
        except (OSError, sqlite3.Error, ValueError) as error:
            print(f"nativePorting: {error}", file=sys.stderr)
            return 2
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

