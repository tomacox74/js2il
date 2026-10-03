#!/usr/bin/env python3
"""Deterministic, resumable native Test262 porting automation.

This tool deliberately keeps native evidence separate from the MVP catalog.
Screening commands accept a runner command supplied by the trusted workflow;
fixture source is never interpreted by the publication command.
"""

from __future__ import annotations

import argparse
import hashlib
import json
import os
from pathlib import Path
import sqlite3
import subprocess
import sys
import time
from typing import Any

SCHEMA_VERSION = "1"
OUTCOMES = {
    "pass",
    "fail",
    "deferred",
    "unsupported",
    "infrastructure-error",
    "incomplete",
}
FAILURE_CLASSES = {
    "product-feature-gap",
    "semantic-defect",
    "harness-gap",
    "policy-exclusion",
    "infrastructure-error",
    "unresolved",
}


def canonical(value: Any) -> str:
    return json.dumps(value, sort_keys=True, separators=(",", ":"))


def file_sha256(path: Path) -> str:
    h = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            h.update(chunk)
    return h.hexdigest()


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
          status TEXT NOT NULL,
          cursor INTEGER NOT NULL DEFAULT 0,
          PRIMARY KEY (run_id, path)
        );
        CREATE TABLE IF NOT EXISTS outcomes (
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
          PRIMARY KEY (run_id, path, variant),
          CHECK (outcome IN ('pass','fail','deferred','unsupported',
                             'infrastructure-error','incomplete')),
          CHECK (failure_class IS NULL OR failure_class IN
                 ('product-feature-gap','semantic-defect','harness-gap',
                  'policy-exclusion','infrastructure-error','unresolved'))
        );
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
    db.execute("INSERT OR REPLACE INTO meta(key,value) VALUES('schema_version',?)", (SCHEMA_VERSION,))
    db.commit()
    return db


def require_budget(value: int, name: str, maximum: int) -> int:
    if value < 1 or value > maximum:
        raise ValueError(f"{name} must be between 1 and {maximum}")
    return value


def write_json(path: Path, value: Any) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(json.dumps(value, indent=2, sort_keys=True) + "\n", encoding="utf-8")


def create_run(db: sqlite3.Connection, args: argparse.Namespace) -> str:
    candidate_limit = require_budget(args.candidate_limit, "candidate-limit", 500)
    accepted_limit = require_budget(args.accepted_limit, "accepted-limit", candidate_limit)
    variant_limit = require_budget(args.variant_limit, "variant-limit", 2000)
    time_limit = require_budget(args.time_limit, "time-limit", 24 * 60)
    run_id = args.run_id or f"native-{int(time.time())}-{os.getpid()}"
    batch_id = args.batch_id or run_id
    now = time.time()
    budgets = {
        "candidate_limit": candidate_limit,
        "accepted_limit": accepted_limit,
        "variant_limit": variant_limit,
        "time_limit": time_limit,
    }
    db.execute(
        """INSERT INTO runs
           (run_id,batch_id,trigger_revision,base_revision,pin,budgets,state,created_at,updated_at)
           VALUES(?,?,?,?,?,?,?, ?,?)""",
        (
            run_id,
            batch_id,
            args.trigger_revision,
            args.base_revision,
            args.pin,
            canonical(budgets),
            "planned",
            now,
            now,
        ),
    )
    db.commit()
    return run_id


def catalog_candidates(catalog: Path, area: str, limit: int) -> list[dict[str, Any]]:
    db = sqlite3.connect(catalog)
    db.row_factory = sqlite3.Row
    try:
        rows = db.execute(
            """SELECT f.path, f.sha256, f.variants
               FROM fixtures f
               JOIN settings s ON s.key='current' AND s.value=f.provenance
               WHERE f.state='runnable' AND f.path LIKE ?
                 AND NOT EXISTS (
                   SELECT 1 FROM registrations r WHERE r.path=f.path
                 )
               ORDER BY f.path
               LIMIT ?""",
            (f"%{area}%", limit),
        ).fetchall()
    finally:
        db.close()
    return [
        {
            "path": row["path"],
            "sha256": row["sha256"],
            "variants": json.loads(row["variants"]),
            "reason": "current-provenance MVP pass; native acceptance required",
        }
        for row in rows
    ]


def plan(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    candidates = catalog_candidates(Path(args.catalog), args.area, int(json.loads(run["budgets"])["candidate_limit"]))
    for candidate in candidates:
        db.execute(
            """INSERT OR IGNORE INTO candidates
               (run_id,path,sha256,variants,selection_reason,status,cursor)
               VALUES(?,?,?,?,?,'pending',0)""",
            (
                args.run_id,
                candidate["path"],
                candidate["sha256"],
                canonical(candidate["variants"]),
                candidate["reason"],
            ),
        )
    db.execute("UPDATE runs SET state='screening',updated_at=? WHERE run_id=?", (time.time(), args.run_id))
    db.commit()
    return {
        "run_id": args.run_id,
        "batch_id": run["batch_id"],
        "candidate_count": len(candidates),
        "paths": [candidate["path"] for candidate in candidates],
        "cold_start": not Path(args.catalog).exists(),
    }


def record(db: sqlite3.Connection, args: argparse.Namespace) -> None:
    if args.outcome not in OUTCOMES:
        raise ValueError(f"Unknown outcome: {args.outcome}")
    if args.failure_class and args.failure_class not in FAILURE_CLASSES:
        raise ValueError(f"Unknown failure class: {args.failure_class}")
    candidate = db.execute(
        "SELECT sha256 FROM candidates WHERE run_id=? AND path=?",
        (args.run_id, args.path),
    ).fetchone()
    if not candidate:
        raise ValueError("Outcome path is not a planned candidate")
    run = db.execute("SELECT budgets,created_at FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    budgets = json.loads(run["budgets"])
    attempts = db.execute(
        "SELECT COUNT(*) FROM outcomes WHERE run_id=?", (args.run_id,)
    ).fetchone()[0]
    if attempts >= budgets["variant_limit"]:
        raise ValueError("variant attempt budget exhausted; checkpoint and resume later")
    if time.time() - run["created_at"] > budgets["time_limit"]:
        raise ValueError("time budget exhausted; checkpoint and resume later")
    now = time.time()
    with db:
        db.execute(
            """INSERT OR REPLACE INTO outcomes
               (run_id,path,variant,fixture_sha256,pin,compiler_identity,
                harness_identity,environment_identity,phase,diagnostic,outcome,
                failure_class,started_at,finished_at)
               VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?)""",
            (
                args.run_id,
                args.path,
                args.variant,
                candidate["sha256"],
                args.pin,
                args.compiler_identity,
                args.harness_identity,
                args.environment_identity,
                args.phase,
                args.diagnostic,
                args.outcome,
                args.failure_class,
                args.started_at or now,
                now,
            ),
        )
        db.execute(
            "UPDATE candidates SET status='attempted',cursor=cursor+1 WHERE run_id=? AND path=?",
            (args.run_id, args.path),
        )
        db.execute("UPDATE runs SET updated_at=? WHERE run_id=?", (now, args.run_id))


def classify_outcome(row: sqlite3.Row) -> str:
    if row["outcome"] == "infrastructure-error":
        return "infrastructure-error"
    if row["outcome"] == "unsupported":
        return "harness-gap"
    if row["outcome"] == "fail":
        return row["failure_class"] or "unresolved"
    return "accepted"


def report(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    rows = db.execute(
        "SELECT * FROM outcomes WHERE run_id=? ORDER BY path,variant", (args.run_id,)
    ).fetchall()
    candidates = db.execute(
        "SELECT path,variants FROM candidates WHERE run_id=? ORDER BY path", (args.run_id,)
    ).fetchall()
    grouped: dict[str, list[sqlite3.Row]] = {}
    for row in rows:
        grouped.setdefault(row["path"], []).append(row)
    accepted: list[str] = []
    incomplete: list[str] = []
    clusters: dict[str, list[str]] = {}
    for candidate in candidates:
        path = candidate["path"]
        path_rows = grouped.get(path, [])
        expected = json.loads(candidate["variants"])
        by_variant = {row["variant"]: row for row in path_rows}
        if not all(variant in by_variant for variant in expected):
            incomplete.append(path)
            continue
        if all(row["outcome"] == "pass" for row in by_variant.values()):
            accepted.append(path)
        else:
            for row in path_rows:
                category = classify_outcome(row)
                if category != "accepted":
                    clusters.setdefault(category, []).append(path)
    result = {
        "schema_version": SCHEMA_VERSION,
        "run_id": args.run_id,
        "batch_id": run["batch_id"],
        "trigger_revision": run["trigger_revision"],
        "base_revision": run["base_revision"],
        "pin": run["pin"],
        "counts": {
            "candidates": db.execute(
                "SELECT COUNT(*) FROM candidates WHERE run_id=?", (args.run_id,)
            ).fetchone()[0],
            "attempts": len(rows),
            "accepted": len(accepted),
            "incomplete": len(incomplete),
            "deferred": sum(1 for row in rows if row["outcome"] == "deferred"),
        },
        "accepted": sorted(set(accepted)),
        "incomplete": sorted(set(incomplete)),
        "failure_clusters": {key: sorted(set(value)) for key, value in sorted(clusters.items())},
        "complete_native_acceptance": bool(accepted) and not incomplete and not clusters,
    }
    budgets = json.loads(run["budgets"])
    result["budget_exceeded"] = (
        result["counts"]["attempts"] >= budgets["variant_limit"]
        or time.time() - run["created_at"] > budgets["time_limit"]
    )
    if args.output:
        write_json(Path(args.output), result)
    return result


def validate_patch(args: argparse.Namespace) -> dict[str, Any]:
    root = Path(args.root).resolve()
    allowed = (
        "tests/Jroc.Test262.Tests/",
        "docs/ECMA262/",
        "CHANGELOG.md",
    )
    changed = subprocess.check_output(
        ["git", "-C", str(root), "diff", "--name-only", args.base_revision, "--"],
        text=True,
    ).splitlines()
    rejected = [path for path in changed if not any(path == item or path.startswith(item) for item in allowed)]
    result = {
        "base_revision": args.base_revision,
        "changed": sorted(changed),
        "rejected": sorted(rejected),
        "valid": not rejected,
        "patch_digest": hashlib.sha256(
            subprocess.check_output(["git", "-C", str(root), "diff", args.base_revision, "--"])
        ).hexdigest(),
    }
    if args.output:
        write_json(Path(args.output), result)
    if rejected:
        raise ValueError("Patch contains paths outside the native-port allowlist: " + ", ".join(rejected))
    return result


def publication_guard(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    summary = report(db, argparse.Namespace(run_id=args.run_id, output=None))
    if not summary["complete_native_acceptance"]:
        raise ValueError("Publication requires complete native acceptance and no failure clusters")
    if args.dry_run:
        return {"publishable": True, "dry_run": True, "batch_id": run["batch_id"]}
    if not args.branch:
        raise ValueError("--branch is required for publication")
    db.execute(
        """INSERT INTO publications(batch_id,run_id,branch,state,updated_at)
           VALUES(?,?,?,'validated',?)
           ON CONFLICT(batch_id) DO UPDATE SET run_id=excluded.run_id,
             branch=excluded.branch,state='validated',updated_at=excluded.updated_at""",
        (run["batch_id"], args.run_id, args.branch, time.time()),
    )
    db.commit()
    return {"publishable": True, "dry_run": False, "batch_id": run["batch_id"], "branch": args.branch}


def checkpoint(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    summary = report(db, argparse.Namespace(run_id=args.run_id, output=None))
    artifact = Path(args.artifact)
    if not artifact.is_file():
        raise ValueError(f"Checkpoint artifact does not exist: {artifact}")
    cursor = db.execute(
        "SELECT COALESCE(MAX(cursor),0) FROM candidates WHERE run_id=?", (args.run_id,)
    ).fetchone()[0]
    artifact_digest = file_sha256(artifact)
    db.execute(
        """INSERT INTO checkpoints(run_id,cursor,artifact_digest,complete,created_at)
           VALUES(?,?,?,?,?)""",
        (args.run_id, cursor, artifact_digest, int(summary["complete_native_acceptance"]), time.time()),
    )
    db.execute(
        "UPDATE runs SET state=?,updated_at=? WHERE run_id=?",
        ("complete" if summary["complete_native_acceptance"] else "deferred", time.time(), args.run_id),
    )
    db.commit()
    return {
        "run_id": args.run_id,
        "cursor": cursor,
        "artifact_digest": artifact_digest,
        "complete": summary["complete_native_acceptance"],
    }


def safe_identifier(value: str) -> str:
    result = "".join(character if character.isalnum() else "_" for character in value)
    result = result.strip("_") or "Test"
    return result if result[0].isalpha() else f"Test_{result}"


def generate_batch(db: sqlite3.Connection, args: argparse.Namespace) -> dict[str, Any]:
    run = db.execute("SELECT * FROM runs WHERE run_id=?", (args.run_id,)).fetchone()
    if not run:
        raise ValueError(f"Unknown run: {args.run_id}")
    summary = report(db, argparse.Namespace(run_id=args.run_id, output=None))
    if not summary["complete_native_acceptance"]:
        raise ValueError("Batch generation requires complete native acceptance")
    upstream = Path(args.upstream).resolve()
    destination = Path(args.destination).resolve()
    accepted = summary["accepted"][: json.loads(run["budgets"])["accepted_limit"]]
    copied: list[str] = []
    for relative in accepted:
        source = upstream / relative
        if not source.is_file():
            raise ValueError(f"Missing pinned fixture: {relative}")
        candidate = db.execute(
            "SELECT sha256 FROM candidates WHERE run_id=? AND path=?", (args.run_id, relative)
        ).fetchone()
        if file_sha256(source) != candidate["sha256"]:
            raise ValueError(f"Fixture hash changed: {relative}")
        target_relative = Path(relative).relative_to("test")
        target = destination / target_relative.parent / "JavaScript" / target_relative.name
        target.parent.mkdir(parents=True, exist_ok=True)
        target.write_bytes(source.read_bytes())
        copied.append(str(target.relative_to(destination)))
    if copied:
        batch_name = safe_identifier(run["batch_id"])
        grouped: dict[Path, list[str]] = {}
        for relative in accepted:
            test_path = Path(relative).relative_to("test")
            grouped.setdefault(test_path.parent, []).append(test_path.stem)
        for folder, names in sorted(grouped.items()):
            namespace = ".".join(["Jroc", "Test262", "Tests"] + [
                safe_identifier(part) for part in folder.parts
            ])
            class_name = f"NativePortBatch_{batch_name}"
            lines = [
                "using Jroc.Test262.Tests;",
                "",
                f"namespace {namespace};",
                "",
                f"public sealed class {class_name} : DiskExecutionTestsBase",
                "{",
                f"    public {class_name}() : base(\"{'.'.join(folder.parts)}\") {{ }}",
                "",
            ]
            for name in sorted(names):
                method = safe_identifier(name)
                lines.extend([
                    f"    [Fact(DisplayName = \"{name}\")]",
                    f"    public Task {method}() => ExecutionTest(\"{name}\");",
                    "",
                ])
            lines.append("}")
            cs = destination / folder / f"{class_name}.cs"
            cs.parent.mkdir(parents=True, exist_ok=True)
            cs.write_text("\n".join(lines) + "\n", encoding="utf-8")
            copied.append(str(cs.relative_to(destination)))
    result = {
        "run_id": args.run_id,
        "batch_id": run["batch_id"],
        "accepted": accepted,
        "copied": sorted(copied),
        "source_fidelity": True,
    }
    if args.output:
        write_json(Path(args.output), result)
    return result


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
    plan_parser.add_argument("--area", default="")
    plan_parser.set_defaults(action=lambda args, db: plan(db, args))
    rec = sub.add_parser("record")
    rec.add_argument("--run-id", required=True)
    rec.add_argument("--path", required=True)
    rec.add_argument("--variant", required=True)
    rec.add_argument("--pin", required=True)
    rec.add_argument("--compiler-identity", required=True)
    rec.add_argument("--harness-identity", required=True)
    rec.add_argument("--environment-identity", required=True)
    rec.add_argument("--phase", default="runtime")
    rec.add_argument("--diagnostic", default="")
    rec.add_argument("--outcome", required=True)
    rec.add_argument("--failure-class")
    rec.add_argument("--started-at", type=float)
    rec.set_defaults(action=lambda args, db: (record(db, args), {"recorded": True})[1])
    rep = sub.add_parser("report")
    rep.add_argument("--run-id", required=True)
    rep.add_argument("--output", type=Path)
    rep.set_defaults(action=lambda args, db: report(db, args))
    validate = sub.add_parser("validate-patch")
    validate.add_argument("--root", type=Path, default=Path("."))
    validate.add_argument("--base-revision", required=True)
    validate.add_argument("--output", type=Path)
    validate.set_defaults(action=lambda args, db: validate_patch(args))
    pub = sub.add_parser("publication-guard")
    pub.add_argument("--run-id", required=True)
    pub.add_argument("--branch")
    pub.add_argument("--dry-run", action="store_true")
    pub.set_defaults(action=lambda args, db: publication_guard(db, args))
    check = sub.add_parser("checkpoint")
    check.add_argument("--run-id", required=True)
    check.add_argument("--artifact", type=Path, required=True)
    check.set_defaults(action=lambda args, db: checkpoint(db, args))
    generate = sub.add_parser("generate")
    generate.add_argument("--run-id", required=True)
    generate.add_argument("--upstream", type=Path, required=True)
    generate.add_argument("--destination", type=Path, required=True)
    generate.add_argument("--output", type=Path)
    generate.set_defaults(action=lambda args, db: generate_batch(db, args))
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
