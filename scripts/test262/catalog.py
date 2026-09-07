#!/usr/bin/env python3
"""Artifact-only, resumable Test262 MVP evidence catalog (stdlib only)."""

import argparse
import csv
import hashlib
import json
import os
from pathlib import Path
import platform
import re
import shutil
import sqlite3
import subprocess
import sys
import time

REPO = Path(__file__).resolve().parents[2]
DEFAULT_DB = REPO / "artifacts/test262/catalog.sqlite"
SCHEMA_VERSION = "1"


def canonical(value):
    return json.dumps(value, sort_keys=True, separators=(",", ":"))


def digest(value):
    return hashlib.sha256(value).hexdigest()


def command(*args):
    return subprocess.check_output(args, cwd=REPO, text=True).strip()


def bridge(request, timeout=None):
    result = subprocess.run(
        ["node", str(REPO / "scripts/test262/catalogBridge.js")],
        input=json.dumps(request), capture_output=True, text=True, cwd=REPO, timeout=timeout)
    if result.returncode:
        raise RuntimeError(result.stderr[-8000:])
    return json.loads(result.stdout)


def connect(filename):
    filename = Path(filename)
    filename.parent.mkdir(parents=True, exist_ok=True)
    db = sqlite3.connect(filename)
    db.row_factory = sqlite3.Row
    db.executescript("""
        CREATE TABLE IF NOT EXISTS settings(key TEXT PRIMARY KEY, value TEXT NOT NULL);
        CREATE TABLE IF NOT EXISTS provenance(id TEXT PRIMARY KEY, document TEXT NOT NULL);
        CREATE TABLE IF NOT EXISTS fixtures(
          provenance TEXT, path TEXT, sha256 TEXT, variants TEXT, state TEXT, reasons TEXT,
          PRIMARY KEY(provenance,path));
        CREATE TABLE IF NOT EXISTS results(
          provenance TEXT, path TEXT, variant TEXT, verdict TEXT, kind TEXT,
          document TEXT, finished REAL, PRIMARY KEY(provenance,path,variant));
        CREATE TABLE IF NOT EXISTS registrations(
          path TEXT PRIMARY KEY, sources TEXT NOT NULL);
    """)
    existing = db.execute("SELECT value FROM settings WHERE key='schema'").fetchone()
    if existing and existing[0] != SCHEMA_VERSION:
        raise ValueError("Incompatible catalog schema")
    db.execute("INSERT OR IGNORE INTO settings VALUES('schema',?)", (SCHEMA_VERSION,))
    db.commit()
    return db


def current(db):
    row = db.execute("SELECT value FROM settings WHERE key='current'").fetchone()
    if not row:
        raise ValueError("Run init first")
    return row[0]


def registration_inventory(root):
    """Resolve literal native registrations relative to their caller, not by basename."""
    registrations = {}
    warnings = []
    calls = re.compile(r'\b(?:ExecutionTestFromFile|ExecutionTest|CompilationFailureTest)\s*\(\s*"([^"]+)"')
    for source in sorted(root.rglob("*.cs")):
        if {"bin", "obj"}.intersection(source.relative_to(root).parts):
            continue
        text = source.read_text(encoding="utf-8-sig")
        text = re.sub(r'/\*.*?\*/|//[^\n]*', '', text, flags=re.S)
        for name in calls.findall(text):
            name = name.replace("\\\\", "/")
            fixture = source.parent / "JavaScript" / (name + ".js")
            logical = "test/" + (source.parent.relative_to(root) / (name + ".js")).as_posix()
            if fixture.is_file():
                registrations.setdefault(logical, []).append(source.relative_to(root).as_posix())
            else:
                warnings.append(f"{source.relative_to(root)}: missing fixture {name}")
    return registrations, warnings


def refresh_registrations(db, root=REPO / "tests/Jroc.Test262.Tests"):
    registrations, warnings = registration_inventory(root)
    db.execute("DELETE FROM registrations")
    db.executemany("INSERT INTO registrations VALUES(?,?)",
                   [(p, canonical(sorted(set(s)))) for p, s in sorted(registrations.items())])
    db.execute("INSERT OR REPLACE INTO settings VALUES('registration_warnings',?)",
               (canonical(warnings),))
    db.commit()


def hash_files(root, patterns):
    paths = set()
    for pattern in patterns:
        paths.update(p for p in root.glob(pattern) if p.is_file())
    return {p.relative_to(root).as_posix(): digest(p.read_bytes()) for p in sorted(paths)}


def environment():
    return {"os": platform.system(), "machine": platform.machine(),
            "distribution": platform.freedesktop_os_release() if sys.platform == "linux" else platform.version(),
            "node": command("node", "--version"), "dotnet_runtimes": command("dotnet", "--list-runtimes")
            .replace(str(Path(shutil.which("dotnet")).resolve().parent), "<dotnet>")}


def initialize(db, args):
    pin = json.loads((REPO / "tests/test262/test262.pin.json").read_text())
    root = Path(args.root or command("node", "scripts/test262/bootstrap.js", "--print-root")).resolve()
    if args.expand:
        managed = REPO / "artifacts/test262/cache" / pin["upstream"]["commit"]
        if root != managed:
            raise ValueError("--expand only modifies the managed pinned cache")
        command("git", "-C", str(root), "sparse-checkout", "add", "/test/")
    # Reject tracked upstream edits; actual source and harness hashes also enter provenance.
    if command("git", "-C", str(root), "status", "--porcelain", "--untracked-files=no"):
        raise ValueError("Pinned checkout has tracked modifications")
    fixtures = bridge({"command": "inventory", "root": str(root)})
    jroc = Path(args.jroc).resolve()
    if not jroc.is_file() or jroc.suffix != ".dll":
        raise ValueError("--jroc must name a built Jroc.dll (build Release first)")
    binaries = hash_files(jroc.parent, ["*.dll", "*.deps.json", "*.runtimeconfig.json"])
    tools = hash_files(REPO, ["scripts/test262/catalog.py", "scripts/test262/catalogBridge.js",
                              "scripts/test262/runMvp.js", "scripts/test262/metadataParser.js",
                              "scripts/test262/bootstrap.js", "tests/test262/test262.pin.json"])
    document = {
        "schema": SCHEMA_VERSION, "runner": "mvp-composite-js-not-native",
        "upstream": pin["upstream"], "binaries": binaries, "tooling": tools,
        "compiler_entry": jroc.name,
        "harness": hash_files(root, ["harness/**/*"]),
        "inventory_sha256": digest(canonical(fixtures).encode()),
        "environment": environment(),
        "timeouts": {"runtime": args.timeout, "compile": args.compile_timeout},
    }
    provenance = digest(canonical(document).encode())
    with db:
        db.execute("INSERT OR IGNORE INTO provenance VALUES(?,?)", (provenance, canonical(document)))
        db.executemany("INSERT OR IGNORE INTO fixtures VALUES(?,?,?,?,?,?)", [
            (provenance, f["path"], f["sha256"], canonical(f["variants"]), f["state"],
             canonical(f["reasons"])) for f in fixtures])
        for key, value in {"current": provenance, "root": str(root), "jroc": str(jroc),
                           "compiler_commit": command("git", "rev-parse", "HEAD")}.items():
            db.execute("INSERT OR REPLACE INTO settings VALUES(?,?)", (key, value))
    refresh_registrations(db)
    print(f"Initialized {len(fixtures)} fixtures; provenance {provenance}", flush=True)


def record(db, provenance, fixture, variant, result, finished=None):
    classification = result["classification"]
    with db:  # Commit every variant before proceeding to the next fixture.
        db.execute("INSERT OR REPLACE INTO results VALUES(?,?,?,?,?,?,?)",
                   (provenance, fixture, variant, classification["verdict"],
                    classification["kind"], canonical(result), finished or time.time()))


def shard_for(path, count):
    return int(hashlib.sha256(path.encode()).hexdigest(), 16) % count


def pending(db, provenance, shard, shards, filter_text="", retry=False):
    done = set() if retry else {(r[0], r[1]) for r in db.execute(
        "SELECT path,variant FROM results WHERE provenance=?", (provenance,))}
    for fixture in db.execute(
            "SELECT * FROM fixtures WHERE provenance=? AND state='runnable' ORDER BY path", (provenance,)):
        if filter_text not in fixture["path"] or shard_for(fixture["path"], shards) != shard:
            continue
        for variant in json.loads(fixture["variants"]):
            if (fixture["path"], variant) not in done:
                yield fixture, variant


def scan(db, args):
    provenance = current(db)
    settings = dict(db.execute("SELECT key,value FROM settings"))
    info = json.loads(db.execute("SELECT document FROM provenance WHERE id=?", (provenance,)).fetchone()[0])
    jroc = Path(args.jroc or settings["jroc"]).resolve()
    root = Path(args.root or settings["root"]).resolve()
    if not jroc.is_file() or jroc.suffix != ".dll":
        raise ValueError("--jroc must name an existing compiler DLL")
    if jroc.name != info.get("compiler_entry"):
        raise ValueError("Compiler entry point changed; run init again")
    if hash_files(jroc.parent, ["*.dll", "*.deps.json", "*.runtimeconfig.json"]) != info["binaries"]:
        raise ValueError("Compiler/runtime fingerprint changed; run init again")
    if hash_files(root, ["harness/**/*"]) != info["harness"]:
        raise ValueError("Harness fingerprint changed; run init again")
    if any(digest((REPO / p).read_bytes()) != sha for p, sha in info["tooling"].items()):
        raise ValueError("Catalog/runner fingerprint changed; run init again")
    if environment() != info["environment"]:
        raise ValueError("Execution environment changed; run init again")
    start = time.monotonic()
    completed = 0
    work = Path(args.db).resolve().parent / f"catalog-work-{os.getpid()}"
    # An interrupted process can leave at most its in-flight case; never reuse another worker's directory.
    work.mkdir(exist_ok=False)
    try:
        for fixture, variant in pending(db, provenance, args.shard, args.shards, args.filter, args.retry):
            if completed >= args.limit or time.monotonic() - start >= args.seconds:
                break
            if digest((root / fixture["path"]).read_bytes()) != fixture["sha256"]:
                raise ValueError(f"Fixture changed: {fixture['path']}; run init again")
            request = {"root": str(root), "output": str(work / "case"), "fixture": fixture["path"],
                       "variant": variant, "jroc": str(jroc),
                       "timeout": info["timeouts"]["runtime"], "compileTimeout": info["timeouts"]["compile"]}
            try:
                result = bridge(request, sum(info["timeouts"].values()) + 30)
            except (RuntimeError, subprocess.TimeoutExpired, ValueError) as error:
                result = {"classification": {"verdict": "unexpected", "kind": "runner-error"},
                          "detail": str(error)[-8000:]}
            record(db, provenance, fixture["path"], variant, result)
            shutil.rmtree(work / "case", ignore_errors=True)
            completed += 1
            print(f"{completed}: {fixture['path']} [{variant}] {result['classification']['kind']}", flush=True)
    finally:
        shutil.rmtree(work, ignore_errors=True)
    print(f"Checkpointed {completed} variants", flush=True)


def passing_sets(db):
    evidence = {}
    for row in db.execute("SELECT provenance,path,variant,verdict FROM results"):
        evidence.setdefault((row[0], row[1]), {})[row[2]] = row[3]
    passing = {}
    for row in db.execute("SELECT provenance,path,variants,state FROM fixtures"):
        variants = json.loads(row[2])
        if row[3] == "runnable" and variants and all(
                evidence.get((row[0], row[1]), {}).get(v) == "matched" for v in variants):
            passing.setdefault(row[0], set()).add(row[1])
    return evidence, passing


def export(db, output):
    output = Path(output)
    output.mkdir(parents=True, exist_ok=True)
    provenance = current(db)
    registered = {r[0] for r in db.execute("SELECT path FROM registrations")}
    evidence, passing = passing_sets(db)
    current_pass = passing.get(provenance, set())
    historical = set().union(*(s for p, s in passing.items() if p != provenance))
    fixtures = list(db.execute("SELECT * FROM fixtures WHERE provenance=? ORDER BY path", (provenance,)))
    paths = {f["path"] for f in fixtures}
    incomplete = []
    unrun = []
    states = {}
    for fixture in fixtures:
        states[fixture["state"]] = states.get(fixture["state"], 0) + 1
        variants = json.loads(fixture["variants"])
        observed = evidence.get((provenance, fixture["path"]), {})
        if fixture["state"] != "runnable" or not variants or any(v not in observed for v in variants):
            incomplete.append(fixture["path"])
        if fixture["state"] == "runnable" and not observed:
            unrun.append(fixture["path"])
    lists = {"passing-unported.txt": sorted(current_pass - registered),
             "historical-passing-unported.txt": sorted((historical & paths) - registered - current_pass),
             "incomplete.txt": incomplete, "unrun.txt": unrun,
             "registered.txt": sorted(registered & paths)}
    for name, entries in lists.items():
        (output / name).write_text("".join(p + "\n" for p in entries), encoding="utf8")
    with (output / "failures.csv").open("w", newline="", encoding="utf8") as stream:
        writer = csv.writer(stream)
        writer.writerow(["path", "variant", "kind", "result"])
        for row in db.execute("""SELECT path,variant,kind,document FROM results
                                 WHERE provenance=? AND verdict!='matched' ORDER BY path,variant""", (provenance,)):
            writer.writerow(row)
        for f in fixtures:
            if f["state"] != "runnable":
                writer.writerow([f["path"], "", f["state"], f["reasons"]])
    required = sum(len(json.loads(f["variants"])) for f in fixtures if f["state"] == "runnable")
    observed = db.execute("SELECT count(*) FROM results WHERE provenance=?", (provenance,)).fetchone()[0]
    summary = {"schema": SCHEMA_VERSION, "provenance": provenance,
               "compiler_source_commit_at_init": dict(db.execute("SELECT key,value FROM settings"))
               .get("compiler_commit"),
               "runner": "mvp-composite-js-not-native", "inventory_complete": True,
               "inventory_files": len(fixtures), "states": states,
               "runnable_required_variants": required, "recorded_variants": observed,
               "runnable_scan_complete": not (set(incomplete) &
                                             {f["path"] for f in fixtures if f["state"] == "runnable"}),
               "complete_passing_unported_list": not incomplete,
               "passing_files": len(current_pass), "passing_unported": len(lists["passing-unported.txt"]),
               "historical_passing_unported": len(lists["historical-passing-unported.txt"]),
               "incomplete_files": len(incomplete), "unrun_files": len(unrun),
               "registered_files": len(registered & paths),
               "registration_warnings": json.loads(dict(db.execute("SELECT key,value FROM settings"))
                                                  .get("registration_warnings", "[]")),
               "provenance_document": json.loads(db.execute(
                   "SELECT document FROM provenance WHERE id=?", (provenance,)).fetchone()[0])}
    (output / "summary.json").write_text(json.dumps(summary, indent=2, sort_keys=True) + "\n", encoding="utf8")
    print(json.dumps({k: v for k, v in summary.items() if k not in
                      ("provenance_document", "registration_warnings")}, sort_keys=True))
    return summary


def merge(db, sources):
    provenance = current(db)
    for source in sorted(sources):
        other = sqlite3.connect(f"file:{Path(source).resolve()}?mode=ro", uri=True)
        try:
            if current(other) != provenance:
                raise ValueError(f"Incompatible shard provenance: {source}")
            with db:
                for table in ("provenance", "fixtures"):
                    for row in other.execute(f"SELECT * FROM {table}"):
                        db.execute(f"INSERT OR IGNORE INTO {table} VALUES({','.join('?' for _ in row)})", row)
                for row in other.execute("SELECT * FROM results ORDER BY provenance,path,variant"):
                    old = db.execute("SELECT finished,document FROM results WHERE provenance=? AND path=? AND variant=?",
                                     row[:3]).fetchone()
                    if old is None or (row[6], row[5]) > tuple(old):
                        db.execute("INSERT OR REPLACE INTO results VALUES(?,?,?,?,?,?,?)", row)
        finally:
            other.close()


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--db", default=str(DEFAULT_DB))
    commands = parser.add_subparsers(dest="command", required=True)
    init = commands.add_parser("init", help="Inventory every pinned test/**/*.js; no execution")
    init.add_argument("--root")
    init.add_argument("--expand", action="store_true", help="Expand managed sparse checkout to all test/")
    init.add_argument("--jroc", default=str(REPO / "src/Cli/bin/Release/net10.0/Jroc.dll"))
    init.add_argument("--timeout", type=int, default=20)
    init.add_argument("--compile-timeout", type=int, default=60)
    run = commands.add_parser("scan", help="Resume missing variants, bounded by count AND time")
    run.add_argument("--root")
    run.add_argument("--jroc")
    run.add_argument("--shard", type=int, default=0)
    run.add_argument("--shards", type=int, default=1)
    run.add_argument("--limit", type=int, default=100)
    run.add_argument("--seconds", type=int, default=600)
    run.add_argument("--filter", default="")
    run.add_argument("--retry", action="store_true", help="Explicitly rerun already recorded variants")
    out = commands.add_parser("export")
    out.add_argument("--output", default=str(DEFAULT_DB.parent))
    out.add_argument("--refresh-registrations", action="store_true")
    combine = commands.add_parser("merge")
    combine.add_argument("sources", nargs="+")
    args = parser.parse_args()
    if args.command == "scan" and (args.shards < 1 or not 0 <= args.shard < args.shards
                                   or args.limit < 0 or args.seconds < 1):
        parser.error("Require 0 <= shard < shards, limit >= 0 and seconds >= 1")
    if args.command == "init" and (args.timeout < 1 or args.compile_timeout < 1):
        parser.error("Timeouts must be positive")
    db = connect(args.db)
    try:
        if args.command == "init":
            initialize(db, args)
        elif args.command == "scan":
            scan(db, args)
        elif args.command == "merge":
            merge(db, args.sources)
        elif args.command == "export":
            if args.refresh_registrations:
                refresh_registrations(db)
            export(db, args.output)
    finally:
        db.close()


if __name__ == "__main__":
    main()
