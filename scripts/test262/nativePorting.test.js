'use strict';

const assert = require('node:assert/strict');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { test } = require('node:test');

const script = path.resolve(__dirname, 'nativePorting.py');

function run(args, cwd) {
  return JSON.parse(execFileSync('python3', ['-B', script, '--db', path.join(cwd, 'native.sqlite'), ...args], {
    cwd, encoding: 'utf8',
  }));
}

test('native evidence is variant-complete and resumable', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = run([
      'create-run', '--run-id', 'run-1', '--trigger-revision', 'head',
      '--base-revision', 'base', '--pin', 'pin',
    ], cwd).run_id;
    const catalog = path.join(cwd, 'catalog.sqlite');
    execFileSync('python3', ['-c', `
import sqlite3
db=sqlite3.connect(${JSON.stringify(catalog)})
db.executescript("""
CREATE TABLE fixtures(provenance TEXT,path TEXT,sha256 TEXT,variants TEXT,state TEXT,reasons TEXT);
CREATE TABLE settings(key TEXT PRIMARY KEY,value TEXT);
CREATE TABLE registrations(path TEXT PRIMARY KEY);
INSERT INTO settings VALUES('current','p');
INSERT INTO fixtures VALUES('p','test/built-ins/Array/a.js','hash','["strict","non-strict"]','runnable','[]');
""")
db.commit()
`,], { cwd });
    run(['plan', '--run-id', runId, '--catalog', catalog, '--area', 'built-ins/Array'], cwd);
    run([
      'record', '--run-id', runId, '--path', 'test/built-ins/Array/a.js',
      '--variant', 'strict', '--pin', 'pin', '--compiler-identity', 'c',
      '--harness-identity', 'h', '--environment-identity', 'e', '--outcome', 'pass',
    ], cwd);
    let report = run(['report', '--run-id', runId], cwd);
    assert.equal(report.counts.incomplete, 1);
    run([
      'record', '--run-id', runId, '--path', 'test/built-ins/Array/a.js',
      '--variant', 'non-strict', '--pin', 'pin', '--compiler-identity', 'c',
      '--harness-identity', 'h', '--environment-identity', 'e', '--outcome', 'pass',
    ], cwd);
    report = run(['report', '--run-id', runId], cwd);
    assert.deepEqual(report.accepted, ['test/built-ins/Array/a.js']);
    assert.equal(report.complete_native_acceptance, true);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('publication guard rejects failure clusters', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = run([
      'create-run', '--run-id', 'run-2', '--trigger-revision', 'head',
      '--base-revision', 'base', '--pin', 'pin',
    ], cwd).run_id;
    const db = path.join(cwd, 'native.sqlite');
    const sqlite = `
import sqlite3
db=sqlite3.connect(${JSON.stringify(db)})
db.execute("INSERT INTO candidates VALUES('run-2','a.js','hash','[\\"default\\"]','reason','pending',0)")
db.commit()
`;
    execFileSync('python3', ['-c', sqlite], { cwd });
    assert.throws(() => run([
      'publication-guard', '--run-id', runId, '--dry-run',
    ], cwd), /complete native acceptance/);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('generation preserves accepted fixture bytes and emits deterministic registration', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = run([
      'create-run', '--run-id', 'run-3', '--trigger-revision', 'head',
      '--base-revision', 'base', '--pin', 'pin',
    ], cwd).run_id;
    const upstream = path.join(cwd, 'upstream');
    const fixture = path.join(upstream, 'test/built-ins/Array/a.js');
    fs.mkdirSync(path.dirname(fixture), { recursive: true });
    fs.writeFileSync(fixture, '/* pinned */\nconsole.log(1);\n');
    const hash = require('node:crypto').createHash('sha256')
      .update(fs.readFileSync(fixture)).digest('hex');
    const db = path.join(cwd, 'native.sqlite');
    execFileSync('python3', ['-c', `
import sqlite3
db=sqlite3.connect(${JSON.stringify(db)})
db.execute("INSERT INTO candidates VALUES('run-3','test/built-ins/Array/a.js',?,'[\\"default\\"]','reason','pending',0)", (${JSON.stringify(hash)},))
db.execute("INSERT INTO outcomes VALUES('run-3','test/built-ins/Array/a.js','default',?,'pin','c','h','e','runtime','','pass',NULL,0,1)", (${JSON.stringify(hash)},))
db.commit()
`], { cwd });
    const destination = path.join(cwd, 'generated');
    const generated = run([
      'generate', '--run-id', runId, '--upstream', upstream, '--destination', destination,
    ], cwd);
    assert.deepEqual(generated.accepted, ['test/built-ins/Array/a.js']);
    assert.equal(fs.readFileSync(
      path.join(destination, 'built-ins/Array/JavaScript/a.js'), 'utf8',
    ), '/* pinned */\nconsole.log(1);\n');
    assert.match(
      fs.readFileSync(path.join(destination, 'built-ins/Array/NativePortBatch_run_3.cs'), 'utf8'),
      /ExecutionTest\("a"\)/,
    );
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});
