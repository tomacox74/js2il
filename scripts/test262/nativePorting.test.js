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
  return run([
    'create-run', '--run-id', 'run-1', '--batch-id', 'batch-1',
    '--trigger-revision', 'head', '--base-revision', 'base', '--pin', 'pin',
    ...overrides,
  ], cwd).run_id;
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
    '--path', values.path || 'test/language/expressions/assignment/dstr/a.js',
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

test('planning uses only complete current-provenance unregistered passes', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = createRun(cwd, [
      '--candidate-limit', '20', '--accepted-limit', '20',
    ]);
    const catalog = createCatalog(cwd, [
      {
        path: 'test/language/expressions/assignment/dstr/a.js', sha256: 'a',
        variants: ['strict', 'non-strict'],
        results: { strict: 'matched', 'non-strict': 'matched' },
      },
      {
        path: 'test/language/expressions/assignment/dstr/incomplete.js', sha256: 'b',
        variants: ['strict', 'non-strict'], results: { strict: 'matched' },
      },
      {
        path: 'test/language/expressions/assignment/dstr/failed.js', sha256: 'c',
        variants: ['strict'], results: { strict: 'unexpected' },
      },
      {
        path: 'test/language/expressions/assignment/dstr/registered.js', sha256: 'd',
        variants: ['strict'], results: { strict: 'matched' }, registered: true,
      },
      {
        path: 'test/language/expressions/call/other.js', sha256: 'e',
        variants: ['strict'], results: { strict: 'matched' },
      },
    ]);
    const planned = run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/expressions/assignment/dstr',
    ], cwd);
    assert.deepEqual(
      planned.paths,
      ['test/language/expressions/assignment/dstr/a.js'],
    );
    assert.equal(planned.catalog_provenance, 'current');
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('acceptance requires every variant under one native provenance', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, [{
      path: 'test/language/expressions/assignment/dstr/a.js', sha256: 'hash',
      variants: ['strict', 'non-strict'],
      results: { strict: 'matched', 'non-strict': 'matched' },
    }]);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/expressions/assignment/dstr',
    ], cwd);
    record(cwd, runId, { variant: 'strict' });
    assert.equal(run(['report', '--run-id', runId], cwd).counts.incomplete, 1);
    record(cwd, runId, { variant: 'non-strict', compiler: 'other-compiler' });
    let report = run(['report', '--run-id', runId], cwd);
    assert.deepEqual(report.accepted, []);
    assert.equal(report.failure_clusters['infrastructure-error'][0].phase, 'provenance');
    record(cwd, runId, { variant: 'non-strict' });
    report = run(['report', '--run-id', runId], cwd);
    assert.deepEqual(
      report.accepted,
      ['test/language/expressions/assignment/dstr/a.js'],
    );
    assert.equal(report.complete_native_acceptance, true);
    assert.equal(report.counts.attempts, 3);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('failure-only batches remain reportable and cannot generate', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, [{
      path: 'test/language/expressions/assignment/dstr/a.js', sha256: 'hash',
      variants: ['strict'], results: { strict: 'matched' },
    }]);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/expressions/assignment/dstr',
    ], cwd);
    record(cwd, runId, {
      outcome: 'unsupported', failureClass: 'harness-gap', phase: 'metadata',
    });
    const report = run(['report', '--run-id', runId], cwd);
    assert.equal(report.complete_native_acceptance, false);
    assert.equal(
      report.failure_clusters['harness-gap'][0].path,
      'test/language/expressions/assignment/dstr/a.js',
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
      'test/language/expressions/assignment/dstr/runtime-negative.js',
    );
    fs.mkdirSync(path.dirname(fixture), { recursive: true });
    const source = '/*---\nnegative:\n  phase: runtime\n  type: TypeError\n---*/\nthrow new TypeError();\n';
    fs.writeFileSync(fixture, source);
    const sha256 = crypto.createHash('sha256').update(fs.readFileSync(fixture)).digest('hex');
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, [{
      path: 'test/language/expressions/assignment/dstr/runtime-negative.js', sha256,
      variants: ['strict'], results: { strict: 'matched' },
    }]);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/expressions/assignment/dstr',
    ], cwd);
    record(cwd, runId, {
      path: 'test/language/expressions/assignment/dstr/runtime-negative.js', sha256,
      phase: 'runtime',
    });
    fs.mkdirSync(path.join(cwd, 'docs/ECMA262'), { recursive: true });
    fs.mkdirSync(path.join(cwd, 'tests/Jroc.Test262.Tests'), { recursive: true });
    fs.writeFileSync(path.join(cwd, 'docs/ECMA262/Test262Conformance.md'), [
      '| Language syntax and semantics | 10 | 1 | 9 | 20 | **50.00%** |',
      '| **Total** | 20 | 1 | 19 | 40 | **50.00%** |',
      '| `expressions` | 5 | 0 | 5 | 10 | **50.00%** |',
      '| `assignment` | 4 | 0 | 6 | 10 | **40.00%** |',
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
      'tests/Jroc.Test262.Tests/language/expressions/assignment/dstr/JavaScript/runtime-negative.js',
    );
    assert.deepEqual(fs.readFileSync(copied), fs.readFileSync(fixture));
    const registration = fs.readFileSync(path.join(
      cwd,
      'tests/Jroc.Test262.Tests/language/expressions/assignment/dstr/NativePortBatch_batch_1.cs',
    ), 'utf8');
    assert.match(registration, /allowUnhandledException: true/);
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

test('generation rejects identifier collisions before publication', () => {
  const cwd = fs.mkdtempSync(path.join(os.tmpdir(), 'native-porting-'));
  try {
    const upstream = path.join(cwd, 'upstream');
    const fixtures = ['a-b.js', 'a_b.js'].map((name) => {
      const fixture = path.join(
        upstream, 'test/language/expressions/assignment/dstr', name,
      );
      fs.mkdirSync(path.dirname(fixture), { recursive: true });
      fs.writeFileSync(fixture, '/*---\n---*/\n');
      return {
        path: `test/language/expressions/assignment/dstr/${name}`,
        sha256: crypto.createHash('sha256').update(fs.readFileSync(fixture)).digest('hex'),
        variants: ['strict'],
        results: { strict: 'matched' },
      };
    });
    const runId = createRun(cwd);
    const catalog = createCatalog(cwd, fixtures);
    run([
      'plan', '--run-id', runId, '--catalog', catalog,
      '--area', 'language/expressions/assignment/dstr',
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
      path: 'test/language/expressions/assignment/dstr/a.js', sha256: 'hash',
      variants: ['strict'], results: { strict: 'matched' },
    }]);
    const first = createRun(cwd);
    run([
      'plan', '--run-id', first, '--catalog', catalog,
      '--area', 'language/expressions/assignment/dstr',
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
      '--area', 'language/expressions/assignment/dstr',
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
