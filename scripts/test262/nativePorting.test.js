'use strict';

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { test } = require('node:test');

const script = path.resolve(__dirname, 'nativePorting.py');

function run(args, cwd) {
  return JSON.parse(execFileSync(
    'python3',
    ['-B', script, '--db', path.join(cwd, 'native.sqlite'), ...args],
    { cwd, encoding: 'utf8' },
  ));
}

function createRun(cwd, overrides = []) {
  const runId = run([
    'create-run', '--run-id', 'run-1', '--batch-id', 'batch-1',
    '--trigger-revision', 'head', '--base-revision', 'base', '--pin', 'pin',
    ...overrides,
  ], cwd).run_id;
  run([
    'set-provenance', '--run-id', runId,
    '--compiler-identity', 'compiler',
    '--harness-identity', 'harness',
    '--environment-identity', 'environment',
  ], cwd);
  return runId;
}

function createCatalog(cwd, fixtures) {
  const catalog = path.join(cwd, 'catalog.sqlite');
  execFileSync('python3', ['-c', `
import json, sqlite3
db=sqlite3.connect(${JSON.stringify(catalog)})
db.executescript("""
CREATE TABLE settings(key TEXT PRIMARY KEY,value TEXT);
CREATE TABLE fixtures(provenance TEXT,path TEXT,sha256 TEXT,variants TEXT,state TEXT,reasons TEXT);
CREATE TABLE results(provenance TEXT,path TEXT,variant TEXT,verdict TEXT,kind TEXT,document TEXT,finished REAL);
CREATE TABLE registrations(path TEXT PRIMARY KEY,sources TEXT);
INSERT INTO settings VALUES('current','current');
""")
fixtures=json.loads(${JSON.stringify(JSON.stringify(fixtures))})
for fixture in fixtures:
    db.execute("INSERT INTO fixtures VALUES(?,?,?,?,?,?)",
      (fixture.get("provenance","current"),fixture["path"],fixture["sha256"],
       json.dumps(fixture["variants"]),fixture.get("state","runnable"),"[]"))
    for variant, verdict in fixture.get("results",{}).items():
        db.execute("INSERT INTO results VALUES(?,?,?,?,?,?,?)",
          (fixture.get("provenance","current"),fixture["path"],variant,verdict,"kind","{}",1))
    if fixture.get("registered"):
        db.execute("INSERT INTO registrations VALUES(?,?)",(fixture["path"],"[]"))
db.commit()
`], { cwd });
  return catalog;
}

function record(cwd, runId, values = {}) {
  return run([
    'record', '--run-id', runId,
    '--path', values.path || 'test/language/computed-property-names/basics/a.js',
    '--variant', values.variant || 'strict',
    '--fixture-sha256', values.sha256 || 'hash',
    '--pin', values.pin || 'pin',
    '--compiler-identity', values.compiler || 'compiler',
    '--harness-identity', values.harness || 'harness',
    '--environment-identity', values.environment || 'environment',
    '--phase', values.phase || 'execution',
    '--outcome', values.outcome || 'pass',
    ...(values.failureClass ? ['--failure-class', values.failureClass] : []),
  ], cwd);
}

test('planning uses complete single-provenance unregistered pass hints', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = createRun(cwd, [
      '--candidate-limit', '20', '--accepted-limit', '20',
    ]);
    const catalog = createCatalog(cwd, [
      {
        path: 'test/language/computed-property-names/basics/a.js', sha256: 'a',
        variants: ['strict', 'non-strict'],
        results: { strict: 'matched', 'non-strict': 'matched' },
      },
      {
        path: 'test/language/computed-property-names/basics/incomplete.js', sha256: 'b',
        variants: ['strict', 'non-strict'], results: { strict: 'matched' },
      },
      {
        path: 'test/language/computed-property-names/basics/failed.js', sha256: 'c',
        variants: ['strict'], results: { strict: 'unexpected' },
      },
      {
        path: 'test/language/computed-property-names/basics/registered.js', sha256: 'd',
        variants: ['strict'], results: { strict: 'matched' }, registered: true,
      },
      {
        path: 'test/language/computed-property-names/basics/historical.js', sha256: 'h',
        variants: ['strict'],
      },
      {
        provenance: 'old',
        path: 'test/language/computed-property-names/basics/historical.js', sha256: 'h',
        variants: ['strict'], results: { strict: 'matched' },
      },
      {
        path: 'test/language/expressions/call/other.js', sha256: 'e',
        variants: ['strict'], results: { strict: 'matched' },
      },
    ]);
    const planned = run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
    ], cwd);
    assert.deepEqual(
      planned.paths,
      [
        'test/language/computed-property-names/basics/a.js',
        'test/language/computed-property-names/basics/historical.js',
      ],
    );
    assert.equal(planned.catalog_provenance, 'current');
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('planning excludes byte-identical legacy fixture locations', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const legacy = path.resolve(
      __dirname,
      '../../tests/Jroc.Test262.Tests/language/expressions/assignment/destructuring/JavaScript/array-empty-val-array.js',
    );
    const sha256 = crypto.createHash('sha256').update(fs.readFileSync(legacy)).digest('hex');
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, [{
      path: 'test/language/computed-property-names/basics/array-empty-val-array.js',
      sha256, variants: ['strict'], results: { strict: 'matched' },
    }]);
    const planned = run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
    ], cwd);
    assert.deepEqual(planned.paths, []);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('acceptance requires every variant under one native provenance', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, [{
      path: 'test/language/computed-property-names/basics/a.js', sha256: 'hash',
      variants: ['strict', 'non-strict'],
      results: { strict: 'matched', 'non-strict': 'matched' },
    }]);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
    ], cwd);
    record(cwd, runId, { variant: 'strict' });
    assert.equal(run(['report', '--run-id', runId], cwd).counts.incomplete, 1);
    assert.throws(
      () => record(cwd, runId, { variant: 'non-strict', compiler: 'other-compiler' }),
      /active native build/,
    );
    let report = run(['report', '--run-id', runId], cwd);
    assert.deepEqual(report.accepted, []);
    assert.equal(report.counts.incomplete, 1);
    record(cwd, runId, { variant: 'non-strict' });
    report = run(['report', '--run-id', runId], cwd);
    assert.deepEqual(
      report.accepted,
      ['test/language/computed-property-names/basics/a.js'],
    );
    assert.equal(report.complete_native_acceptance, true);
    assert.equal(report.counts.attempts, 2);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('stale active provenance cannot be accepted after a build identity change', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, [{
      path: 'test/language/computed-property-names/basics/a.js', sha256: 'hash',
      variants: ['strict'], results: { strict: 'matched' },
    }]);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
    ], cwd);
    assert.throws(
      () => record(cwd, runId, { compiler: 'changed' }),
      /active native build/,
    );
    assert.deepEqual(run(['report', '--run-id', runId], cwd).accepted, []);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('incremental result import survives an interrupted screening batch', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, [{
      path: 'test/language/computed-property-names/basics/a.js', sha256: 'hash',
      variants: ['strict', 'non-strict'],
      results: { strict: 'matched', 'non-strict': 'matched' },
    }]);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
    ], cwd);
    const result = path.join(cwd, 'result.json');
    fs.writeFileSync(result, JSON.stringify([{
      path: 'test/language/computed-property-names/basics/a.js',
      variant: 'strict',
      fixture_sha256: 'hash',
      outcome: 'pass',
      phase: 'execution',
      diagnostic: '',
      started_at: 10,
      finished_at: 11,
    }]));
    run([
      'import-results', '--run-id', runId, '--results', result, '--pin', 'pin',
      '--compiler-identity', 'compiler', '--harness-identity', 'harness',
      '--environment-identity', 'environment',
    ], cwd);
    const screenPlan = path.join(cwd, 'screen-plan.json');
    run([
      'screen-plan', '--run-id', runId, '--upstream', cwd, '--output', screenPlan,
    ], cwd);
    const pending = JSON.parse(fs.readFileSync(screenPlan));
    assert.deepEqual(pending.candidates[0].variants, ['non-strict']);
    const report = run(['report', '--run-id', runId], cwd);
    assert.equal(report.counts.attempts, 1);
    assert.equal(report.counts.incomplete, 1);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('failure-only batches remain reportable and cannot generate', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, [{
      path: 'test/language/computed-property-names/basics/a.js', sha256: 'hash',
      variants: ['strict'], results: { strict: 'matched' },
    }]);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
    ], cwd);
    record(cwd, runId, {
      outcome: 'unsupported', failureClass: 'harness-gap', phase: 'metadata',
    });
    const report = run(['report', '--run-id', runId], cwd);
    assert.equal(report.complete_native_acceptance, false);
    assert.equal(
      report.failure_clusters['harness-gap'][0].path,
      'test/language/computed-property-names/basics/a.js',
    );
    assert.throws(() => run([
      'generate', '--run-id', runId, '--upstream', cwd, '--destination', cwd,
    ], cwd), /freshly accepted fixture/);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('generation preserves bytes, runtime-negative registration, and coverage totals', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const upstream = path.join(cwd, 'upstream');
    const fixture = path.join(
      upstream,
      'test/language/computed-property-names/basics/string.js',
    );
    fs.mkdirSync(path.dirname(fixture), { recursive: true });
    const source = '/*---\nnegative:\n  phase: runtime\n  type: TypeError\n---*/\nthrow new TypeError();\n';
    fs.writeFileSync(fixture, source);
    const sha256 = crypto.createHash('sha256').update(fs.readFileSync(fixture)).digest('hex');
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, [{
      path: 'test/language/computed-property-names/basics/string.js', sha256,
      variants: ['strict'], results: { strict: 'matched' },
    }]);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
    ], cwd);
    record(cwd, runId, {
      path: 'test/language/computed-property-names/basics/string.js', sha256,
      phase: 'runtime',
    });
    fs.mkdirSync(path.join(cwd, 'docs/ECMA262'), { recursive: true });
    fs.mkdirSync(path.join(cwd, 'tests/Jroc.Test262.Tests'), { recursive: true });
    fs.writeFileSync(path.join(cwd, 'docs/ECMA262/Test262Conformance.md'), [
      '| Language syntax and semantics | 10 | 1 | 9 | 20 | **50.00%** |',
      '| **Total** | 20 | 1 | 19 | 40 | **50.00%** |',
      '| `computed-property-names` | 0 | 0 | 10 | 10 | **0.00%** |',
      '',
    ].join('\n'));
    fs.writeFileSync(path.join(cwd, 'docs/ECMA262/Index.md'), [
      '| Verified passing | 20 | **50.00%** |',
      '| Explicitly excluded due to known unsupported behavior | 1 | 2.50% |',
      '| Not yet verified | 19 | 47.50% |',
      '| **Total applicable ECMA-262 tests** | **40** | **100.00%** |',
      '',
    ].join('\n'));
    fs.writeFileSync(path.join(cwd, 'CHANGELOG.md'), '# Changelog\n\n## Unreleased\n\n');
    const output = path.join(cwd, 'generated.json');
    const generated = run([
      'generate', '--run-id', runId, '--upstream', upstream,
      '--destination', cwd, '--output', output,
    ], cwd);
    const copied = path.join(
      cwd,
      'tests/Jroc.Test262.Tests/language/computed-property-names/basics/JavaScript/string.js',
    );
    assert.deepEqual(fs.readFileSync(copied), fs.readFileSync(fixture));
    const registration = fs.readFileSync(path.join(
      cwd,
      'tests/Jroc.Test262.Tests/language/computed-property-names/basics/NativePortBatch_batch_1.cs',
    ), 'utf8');
    assert.match(registration, /allowUnhandledException: true/);
    assert.match(registration, /public Task Test_string\(\)/);
    assert.match(
      fs.readFileSync(path.join(cwd, 'docs/ECMA262/Test262Conformance.md'), 'utf8'),
      /Language syntax and semantics \| 11 \| 1 \| 8 \| 20 \| \*\*55\.00%\*\*/,
    );
    assert.equal(generated.source_fidelity, true);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('manifest verification rejects changed publication artifacts', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    createRun(cwd);
    const artifact = path.join(cwd, 'report.json');
    const manifest = path.join(cwd, 'manifest.json');
    fs.writeFileSync(artifact, '{}\n');
    run([
      'manifest', '--workflow', 'test262-native-port.yml', '--run-id', '123',
      '--target-revision', 'head', '--output', manifest, artifact,
    ], cwd);
    assert.equal(run([
      'verify-manifest', '--manifest', manifest, '--workflow', 'test262-native-port.yml',
      '--run-id', '123', '--target-revision', 'head',
    ], cwd).valid, true);
    fs.appendFileSync(artifact, 'changed');
    assert.throws(() => run([
      'verify-manifest', '--manifest', manifest, '--workflow', 'test262-native-port.yml',
      '--run-id', '123', '--target-revision', 'head',
    ], cwd), /digest mismatch/);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('checkpoint validation rejects corrupt and incompatible state', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    createRun(cwd);
    assert.equal(run(['validate-state'], cwd).valid, true);
    const corrupt = path.join(cwd, 'corrupt.sqlite');
    fs.writeFileSync(corrupt, 'not sqlite');
    assert.throws(
      () => JSON.parse(execFileSync(
        'python3',
        ['-B', script, '--db', corrupt, 'validate-state'],
        { cwd, encoding: 'utf8' },
      )),
      /Command failed/,
    );
    const incompatible = path.join(cwd, 'incompatible.sqlite');
    execFileSync('python3', ['-c', `
import sqlite3
db=sqlite3.connect(${JSON.stringify(incompatible)})
db.execute("CREATE TABLE meta(key TEXT PRIMARY KEY,value TEXT NOT NULL)")
db.execute("INSERT INTO meta VALUES('schema_version','2')")
db.commit()
`], { cwd });
    assert.throws(
      () => execFileSync(
        'python3',
        ['-B', script, '--db', incompatible, 'validate-state'],
        { cwd, encoding: 'utf8' },
      ),
      /Command failed/,
    );
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('generation rejects identifier collisions before publication', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const upstream = path.join(cwd, 'upstream');
    const fixtures = ['a-b.js', 'a_b.js'].map((name) => {
      const fixture = path.join(
        upstream, 'test/language/computed-property-names/basics', name,
      );
      fs.mkdirSync(path.dirname(fixture), { recursive: true });
      fs.writeFileSync(fixture, '/*---\n---*/\n');
      return {
        path: `test/language/computed-property-names/basics/${name}`,
        sha256: crypto.createHash('sha256').update(fs.readFileSync(fixture)).digest('hex'),
        variants: ['strict'],
        results: { strict: 'matched' },
      };
    });
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, fixtures);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
    ], cwd);
    for (const fixture of fixtures) {
      record(cwd, runId, { path: fixture.path, sha256: fixture.sha256 });
    }
    fs.mkdirSync(path.join(cwd, 'docs/ECMA262'), { recursive: true });
    fs.mkdirSync(path.join(cwd, 'tests/Jroc.Test262.Tests'), { recursive: true });
    fs.writeFileSync(path.join(cwd, 'docs/ECMA262/Test262Conformance.md'), '');
    fs.writeFileSync(path.join(cwd, 'docs/ECMA262/Index.md'), '');
    fs.writeFileSync(path.join(cwd, 'CHANGELOG.md'), '## Unreleased\n\n');
    assert.throws(() => run([
      'generate', '--run-id', runId, '--upstream', upstream, '--destination', cwd,
    ], cwd), /Identifier collision/);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('unchanged harness capabilities do not retry known harness gaps', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const catalog = createCatalog(cwd, [{
      path: 'test/language/computed-property-names/basics/a.js', sha256: 'hash',
      variants: ['strict'], results: { strict: 'matched' },
    }]);
    const first = createRun(cwd);
    run([
      'plan', '--run-id', first, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
      '--capability-identity', 'capability-1',
    ], cwd);
    record(cwd, first, {
      outcome: 'unsupported', failureClass: 'harness-gap', phase: 'metadata',
    });
    const second = run([
      'create-run', '--run-id', 'run-2', '--batch-id', 'batch-2',
      '--trigger-revision', 'head-2', '--base-revision', 'head', '--pin', 'pin',
    ], cwd).run_id;
    run([
      'plan', '--run-id', second, '--catalog', catalog,
      '--area', 'language/computed-property-names/basics',
      '--capability-identity', 'capability-1',
    ], cwd);
    const screenPlan = path.join(cwd, 'screen-plan.json');
    run([
      'screen-plan', '--run-id', second, '--upstream', cwd, '--output', screenPlan,
    ], cwd);
    assert.deepEqual(JSON.parse(fs.readFileSync(screenPlan)).candidates, []);
    const report = run(['report', '--run-id', second], cwd);
    assert.equal(report.failure_clusters['harness-gap'][0].phase, 'capability');
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});
