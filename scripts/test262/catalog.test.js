'use strict';

const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');
const { test } = require('node:test');
const { classify, inventory } = require('./catalogBridge');

const artifacts = path.resolve(__dirname, '../../artifacts/test262/catalog-tests');
fs.mkdirSync(artifacts, { recursive: true });

function pythonTest(name, body) {
  test(name, () => {
    const directory = fs.mkdtempSync(path.join(artifacts, 'case-'));
    try {
      execFileSync('python3', ['-B', '-c', `
import sys, json, shutil
from pathlib import Path
from types import SimpleNamespace
sys.path.insert(0, str(Path("scripts/test262").resolve()))
import catalog as c
root = Path(sys.argv[1])
db = c.connect(root / "catalog.sqlite")
def fixture(p, name="test/built-ins/A/name.js", variants=("non-strict", "strict"), state="runnable"):
    db.execute("INSERT OR IGNORE INTO provenance VALUES(?,?)", (p, "{}"))
    db.execute("INSERT OR REPLACE INTO fixtures VALUES(?,?,?,?,?,?)",
               (p, name, "hash", c.canonical(variants), state, "[]"))
    db.execute("INSERT OR REPLACE INTO settings VALUES('current',?)", (p,))
    db.commit()
def result(p, v, verdict="matched", name="test/built-ins/A/name.js", finished=1):
    c.record(db, p, name, v, {"classification":{"verdict":verdict,"kind":"pass" if verdict=="matched" else "runtime-mismatch"}}, finished)
${body}
db.close()
`, directory], { cwd: path.resolve(__dirname, '../..'), stdio: 'pipe' });
    } finally {
      fs.rmSync(directory, { recursive: true, force: true });
    }
  });
}

pythonTest('requires every variant; missing and failed variants never pass', `
fixture("current")
result("current", "non-strict")
s = c.export(db, root / "export")
assert s["passing_unported"] == 0 and s["incomplete_files"] == 1
result("current", "strict", "unexpected")
s = c.export(db, root / "export")
assert s["passing_unported"] == 0 and s["incomplete_files"] == 0
assert s["runnable_scan_complete"]
result("current", "strict")
assert c.export(db, root / "export")["passing_unported"] == 1
`);

pythonTest('scan rejects nonexistent and changed compiler entry points before execution', `
fixture("current")
compiler = root / "Jroc.dll"
compiler.write_bytes(b"compiler")
db.execute("UPDATE provenance SET document=? WHERE id='current'",
           (c.canonical({"compiler_entry": "Jroc.dll"}),))
db.execute("INSERT INTO settings VALUES('root',?)", (str(root),))
db.commit()
for name, message in (("missing.dll", "existing compiler DLL"),
                      ("other.dll", "entry point changed")):
    candidate = root / name
    if name == "other.dll":
        candidate.write_bytes(b"compiler")
    try:
        c.scan(db, SimpleNamespace(jroc=str(candidate), root=str(root)))
    except ValueError as error:
        assert message in str(error), str(error)
    else:
        raise AssertionError("Invalid compiler accepted")
assert db.execute("SELECT COUNT(*) FROM results").fetchone()[0] == 0
`);

pythonTest('never combines variants across provenance; historical passes stay historical', `
fixture("old")
result("old", "non-strict")
fixture("current")
result("current", "strict")
s = c.export(db, root / "export")
assert s["passing_unported"] == s["historical_passing_unported"] == 0
result("old", "strict")
s = c.export(db, root / "export")
assert s["passing_unported"] == 0 and s["historical_passing_unported"] == 1
assert not s["complete_passing_unported_list"]
`);

pythonTest('blocked and metadata-error fixtures remain incomplete even with misleading evidence', `
fixture("current", state="blocked")
result("current", "strict")
result("current", "non-strict")
fixture("current", name="test/bad.js", variants=(), state="metadata-error")
s = c.export(db, root / "export")
assert s["incomplete_files"] == 2 and s["passing_files"] == 0
assert "metadata-error" in (root / "export/failures.csv").read_text()
`);

pythonTest('canonical and legacy registrations resolve full caller-relative paths', `
suite = root / "native/built-ins/A"
(suite / "JavaScript/nested").mkdir(parents=True)
(suite / "JavaScript/name.js").write_text("ok")
(suite / "JavaScript/nested/legacy.js").write_text("ok")
(suite / "ExecutionTests.cs").write_text('''
ExecutionTestFromFile("name");
ExecutionTest("nested/legacy");
CompilationFailureTest("name");
// ExecutionTest("not-registered");
ExecutionTest("missing");
''')
registered, warnings = c.registration_inventory(root / "native")
assert set(registered) == {"test/built-ins/A/name.js", "test/built-ins/A/nested/legacy.js"}
assert len(warnings) == 1
fixture("current")
fixture("current", name="test/built-ins/B/name.js")
for name in ("test/built-ins/A/name.js", "test/built-ins/B/name.js"):
    for v in ("strict", "non-strict"): result("current", v, name=name)
c.refresh_registrations(db, root / "native")
assert c.export(db, root / "export")["passing_unported"] == 1
assert (root / "export/passing-unported.txt").read_text() == "test/built-ins/B/name.js\\n"
`);

pythonTest('checkpoints survive reopen, deterministic sharding and exports', `
fixture("current")
result("current", "non-strict")
db.close()
db = c.connect(root / "catalog.sqlite")
assert [v for f,v in c.pending(db, "current", 0, 1)] == ["strict"]
for i in range(40): fixture("current", name=f"test/example/{i:02}.js")
shards = [{(f["path"],v) for f,v in c.pending(db, "current", i, 4)} for i in range(4)]
assert len(set.union(*shards)) == sum(map(len, shards)) == 81
c.export(db, root / "one")
c.export(db, root / "two")
assert {p.name:p.read_bytes() for p in (root / "one").iterdir()} == {p.name:p.read_bytes() for p in (root / "two").iterdir()}
`);

pythonTest('merges checkpointed shards idempotently and rejects incompatible provenance', `
fixture("current")
result("current", "non-strict")
db.commit()
shutil.copyfile(root / "catalog.sqlite", root / "shard.sqlite")
other = c.connect(root / "shard.sqlite")
c.record(other, "current", "test/built-ins/A/name.js", "strict",
         {"classification":{"verdict":"matched","kind":"pass"}}, 2)
other.close()
c.merge(db, [root / "shard.sqlite"])
c.merge(db, [root / "shard.sqlite"])
assert c.export(db, root / "export")["passing_unported"] == 1
other = c.connect(root / "shard.sqlite")
other.execute("UPDATE settings SET value='wrong' WHERE key='current'")
other.commit()
other.close()
try: c.merge(db, [root / "shard.sqlite"])
except ValueError: pass
else: raise AssertionError("incompatible merge accepted")
`);

pythonTest('bounded scan resumes and cleans per-case outputs; runner errors are checkpointed', `
fixture("current")
(root / "test/built-ins/A").mkdir(parents=True)
(root / "test/built-ins/A/name.js").write_text("source")
(root / "Jroc.dll").write_text("binary")
db.execute("UPDATE fixtures SET sha256=?", (c.digest(b"source"),))
info = {"compiler_entry": "Jroc.dll", "binaries": c.hash_files(root, ["*.dll"]), "harness": {}, "tooling": {},
        "environment": {}, "timeouts": {"runtime":1, "compile":1}}
db.execute("UPDATE provenance SET document=?", (c.canonical(info),))
for k,v in {"root":str(root), "jroc":str(root / "Jroc.dll")}.items():
    db.execute("INSERT INTO settings VALUES(?,?)", (k,v))
db.commit()
c.environment = lambda: {}
calls = []
def fake(request, timeout):
    calls.append(request["variant"])
    output = Path(request["output"])
    output.mkdir()
    (output / "large.dll").write_text("compiled")
    if request["variant"] == "strict": raise RuntimeError("synthetic failure")
    return {"classification":{"verdict":"matched","kind":"pass"}}
c.bridge = fake
args = SimpleNamespace(db=str(root / "catalog.sqlite"), root=None, jroc=None,
                       shard=0, shards=1, limit=1, seconds=30, filter="", retry=False)
c.scan(db,args)
c.scan(db,args)
c.scan(db,args)
assert calls == ["non-strict", "strict"]
assert db.execute("SELECT count(*) FROM results").fetchone()[0] == 2
assert not list(root.glob("catalog-work-*"))
assert c.export(db, root / "export")["passing_unported"] == 0
`);

test('metadata parser errors are per fixture, without changing source semantics', () => {
  const directory = fs.mkdtempSync(path.join(artifacts, 'metadata-'));
  try {
    const bad = '/*---\nnot yaml\n---*/\n';
    const good = '/*---\ndescription: valid\n---*/\nassert.sameValue(1, 1);\n';
    fs.writeFileSync(path.join(directory, 'bad.js'), bad);
    fs.writeFileSync(path.join(directory, 'good.js'), good);
    assert.equal(classify(directory, 'bad.js', {}).state, 'metadata-error');
    assert.deepEqual(classify(directory, 'good.js', {}).variants, ['non-strict', 'strict']);
    assert.equal(fs.readFileSync(path.join(directory, 'good.js'), 'utf8'), good);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test('full tracked inventory includes excluded areas and refuses missing sparse files', () => {
  const directory = fs.mkdtempSync(path.join(artifacts, 'inventory-'));
  const git = (...args) => execFileSync('git', ['-C', directory, ...args], { encoding: 'utf8' }).trim();
  try {
    for (const area of ['built-ins', 'annexB', 'intl402']) {
      fs.mkdirSync(path.join(directory, 'test', area), { recursive: true });
      fs.writeFileSync(path.join(directory, 'test', area, 'example.js'), '/*---\ndescription: inventory\n---*/');
    }
    git('init', '--quiet');
    git('add', 'test');
    git('-c', 'user.name=Catalog test', '-c', 'user.email=catalog@example.invalid',
      '-c', 'commit.gpgsign=false', 'commit', '--quiet', '-m', 'Synthetic pinned inventory');
    const pin = { upstream: { commit: git('rev-parse', 'HEAD') }, excludedFromMvp: ['test/annexB/**'] };
    const rows = inventory(directory, pin);
    assert.equal(rows.length, 3);
    assert.equal(rows.find(r => r.path.includes('annexB')).state, 'blocked');
    assert.throws(() => inventory(directory, { ...pin, upstream: { commit: 'wrong' } }), /pinned/);
    fs.unlinkSync(path.join(directory, 'test/intl402/example.js'));
    assert.throws(() => inventory(directory, pin), /ENOENT/);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});
