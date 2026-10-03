'use strict';

// Integration coverage for the screening host -> importer contract. The workflow
// consumes host stdout one line at a time, so these tests exercise the real C#
// serialization boundary instead of synthetic result documents.

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const { execFileSync, spawn } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const readline = require('node:readline');
const { test } = require('node:test');

const repository = path.resolve(__dirname, '../..');
const script = path.join(repository, 'scripts/test262/nativePorting.py');
const project = path.join(
  repository, 'scripts/test262/NativeScreeningHost/NativeScreeningHost.csproj');
const area = 'language/computed-property-names/basics';

function resolveHost() {
  if (process.env.JROC_NATIVE_SCREENING_HOST) {
    return process.env.JROC_NATIVE_SCREENING_HOST;
  }
  const candidates = ['Release', 'Debug'].map((configuration) => path.join(
    repository, 'scripts/test262/NativeScreeningHost/bin', configuration,
    'net10.0/NativeScreeningHost.dll'));
  const existing = candidates.find((candidate) => fs.existsSync(candidate));
  if (existing) {
    return existing;
  }
  execFileSync('dotnet', ['build', project, '-c', 'Release', '--nologo'], {
    cwd: repository, stdio: 'inherit',
  });
  return candidates[0];
}

const host = resolveHost();

test('screening host publishes its capability matrix', () => {
  const capabilities = JSON.parse(execFileSync(
    'dotnet', [host, '--capabilities'], { cwd: repository, encoding: 'utf8' },
  ));
  assert.equal(capabilities.flags.async, true);
  assert.equal(capabilities.flags.raw, false);
  assert.equal(capabilities.dependencies.sibling_files, true);
  assert.equal(capabilities.isolation.agent_cleanup, true);
  assert.ok(capabilities.includes.includes('atomicsHelper.js'));
  assert.ok(capabilities.includes.includes('asyncHelpers.js'));
  assert.equal(capabilities.flags.module, 'static-syntax-only');
});

function run(args, cwd) {
  return JSON.parse(execFileSync(
    'python3',
    ['-B', script, '--db', path.join(cwd, 'native.sqlite'), ...args],
    { cwd, encoding: 'utf8' },
  ));
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
for fixture in json.loads(${JSON.stringify(JSON.stringify(fixtures))}):
    db.execute("INSERT INTO fixtures VALUES(?,?,?,?,?,?)",
      ("current",fixture["path"],fixture["sha256"],
       json.dumps(fixture["variants"]),"runnable","[]"))
    for variant in fixture["variants"]:
        db.execute("INSERT INTO results VALUES(?,?,?,?,?,?,?)",
          ("current",fixture["path"],variant,"matched","kind","{}",1))
db.commit()
`], { cwd });
  return catalog;
}

// Builds fixtures that cover the terminal screening outcomes: a fixture that
// compiles and passes, metadata-driven gaps, and a load failure. Sorted first so
// the passing fixture's variants are the earliest streamed results.
function createUpstream(cwd) {
  const directory = path.join(cwd, 'test', area);
  fs.mkdirSync(directory, { recursive: true });
  const sources = [
    {
      name: 'aa-basic.js',
      source: '/*---\ndescription: compiles and runs silently\n---*/\nvar value = { ["a"]: 1 };\nif (value.a !== 1) { throw new Error("bad"); }\n',
      variants: ['strict', 'non-strict'],
    },
    {
      name: 'async-fixture.js',
      source: '/*---\nflags:\n  - async\nincludes: [asyncHelpers.js]\n---*/\nPromise.resolve(42).then(function(value) { assert.sameValue(value, 42); $DONE(); }, $DONE);\n',
      variants: ['strict', 'non-strict'],
    },
    {
      name: 'agent-fixture.js',
      source: `/*---
flags: [async]
includes:
  - atomicsHelper.js
---*/
$262.agent.start("$262.agent.report('ready'); $262.agent.leaving();");
$262.agent.getReportAsync().then(function(value) {
  assert.sameValue(value, "ready");
  $DONE();
}, $DONE);
`,
      variants: ['strict', 'non-strict'],
    },
    {
      name: 'module-fixture.js',
      source: '/*---\nflags: [module]\n---*/\nimport { value } from \"./dependency_FIXTURE.js\";\nassert.sameValue(value, 42);\n',
      variants: ['module'],
    },
    {
      name: 'module-goal-only.js',
      source: '/*---\nflags: [module]\n---*/\nassert.sameValue(this, undefined);\n',
      variants: ['module'],
    },
    {
      name: 'module-missing-dependency.js',
      source: '/*---\nflags: [module]\n---*/\nimport "./missing_FIXTURE.js";\n',
      variants: ['module'],
    },
    {
      name: 'parse-negative.js',
      source: '/*---\nnegative:\n  phase: parse\n  type: SyntaxError\n---*/\nvar = ;\n',
      variants: ['strict', 'non-strict'],
    },
  ];
  fs.writeFileSync(
    path.join(directory, 'dependency_FIXTURE.js'),
    'export const value = 42;\n',
  );
  const fixtures = sources.map(({ name, source, variants }) => {
    fs.writeFileSync(path.join(directory, name), source);
    return {
      path: `test/${area}/${name}`,
      sha256: crypto.createHash('sha256').update(source).digest('hex'),
      variants,
    };
  });
  // Absent from disk on purpose: the host must report a load failure rather
  // than aborting the batch.
  fixtures.push({
    path: `test/${area}/zz-absent-fixture.js`,
    sha256: crypto.createHash('sha256').update('absent').digest('hex'),
    variants: ['strict', 'non-strict'],
  });
  return fixtures;
}

function prepare(cwd) {
  const fixtures = createUpstream(cwd);
  const catalog = createCatalog(cwd, fixtures);
  const runId = run([
    'create-run', '--run-id', 'run-1', '--batch-id', 'batch-1',
    '--trigger-revision', 'head', '--base-revision', 'base', '--pin', 'pin',
  ], cwd).run_id;
  run([
    'set-provenance', '--run-id', runId, '--compiler-identity', 'compiler',
    '--harness-identity', 'harness', '--environment-identity', 'environment',
  ], cwd);
  run(['plan', '--run-id', runId, '--catalog', catalog, '--area', area], cwd);
  const screenPlan = path.join(cwd, 'screen-plan.json');
  const planned = run([
    'screen-plan', '--run-id', runId, '--upstream', cwd, '--output', screenPlan,
  ], cwd);
  assert.equal(planned.variant_count, 13);
  return { runId, screenPlan };
}

// Mirrors the workflow import step exactly: each physical stdout line is wrapped
// as a single-element array and imported on its own.
function importLine(cwd, runId, line) {
  const results = path.join(cwd, 'native-result.json');
  fs.writeFileSync(results, `[${line}]\n`);
  run([
    'import-results', '--run-id', runId, '--results', results, '--pin', 'pin',
    '--compiler-identity', 'compiler', '--harness-identity', 'harness',
    '--environment-identity', 'environment',
  ], cwd);
}

function startHost(cwd, screenPlan) {
  return spawn('dotnet', [host, '--plan', screenPlan], { cwd: repository });
}

test('screening host streams one importable JSON result per stdout line', async () => {
  const cwd = fs.realpathSync(fs.mkdtempSync(path.join(os.tmpdir(), 'native-host-')));
  try {
    const { runId, screenPlan } = prepare(cwd);
    const child = startHost(cwd, screenPlan);
    const lines = [];
    const reader = readline.createInterface({ input: child.stdout });
    reader.on('line', (line) => {
      if (line.trim()) {
        lines.push(line);
      }
    });
    const code = await new Promise((resolve, reject) => {
      child.on('error', reject);
      child.on('close', resolve);
    });
    assert.equal(code, 0);
    assert.equal(lines.length, 13);

    for (const line of lines) {
      // The regression: indented serialization would split a record across
      // several lines and fail to parse here.
      const value = JSON.parse(line);
      assert.ok(value.path && value.variant && value.fixture_sha256);
      importLine(cwd, runId, line);
    }

    const outcomes = lines.map((line) => JSON.parse(line));
    const outcomeFor = (name) => outcomes
      .filter((value) => value.path.endsWith(name))
      .map((value) => value.outcome);
    assert.deepEqual(outcomeFor('zz-absent-fixture.js'),
      ['infrastructure-error', 'infrastructure-error']);
    assert.deepEqual(outcomeFor('aa-basic.js'), ['pass', 'pass']);
    assert.deepEqual(outcomeFor('async-fixture.js'), ['pass', 'pass']);
    assert.deepEqual(outcomeFor('agent-fixture.js'), ['pass', 'pass']);
    assert.deepEqual(outcomeFor('module-fixture.js'), ['pass']);
    assert.deepEqual(outcomeFor('module-goal-only.js'), ['unsupported']);
    assert.deepEqual(outcomeFor('module-missing-dependency.js'), ['unsupported']);
    assert.deepEqual(outcomeFor('parse-negative.js'), ['unsupported', 'unsupported']);

    const report = run(['report', '--run-id', runId], cwd);
    assert.equal(report.counts.attempts, 13);
    // Only the fixture whose variants all passed under the active provenance is
    // accepted; the gaps and the load failure stay in failure clusters.
    assert.deepEqual(report.accepted, [
      `test/${area}/aa-basic.js`,
      `test/${area}/agent-fixture.js`,
      `test/${area}/async-fixture.js`,
      `test/${area}/module-fixture.js`,
    ]);
    assert.ok(report.failure_clusters['harness-gap'].length >= 2);
    assert.ok(report.failure_clusters['infrastructure-error'].length >= 2);
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('worker isolation cleans up agent failures and timeouts', () => {
  const cwd = fs.realpathSync(fs.mkdtempSync(path.join(os.tmpdir(), 'native-host-')));
  try {
    const directory = path.join(cwd, 'test', area);
    fs.mkdirSync(directory, { recursive: true });
    const fixtures = [
      {
        name: 'agent-failure.js',
        source: `/*--- includes: [atomicsHelper.js] ---*/
$262.agent.start("throw new Error('worker failed');");
$262.agent.getReport();
`,
      },
      {
        name: 'timeout.js',
        source: '/*--- description: worker timeout ---*/\nwhile (true) {}\n',
      },
      {
        name: 'success-after-failure.js',
        source: '/*--- description: fresh worker after cleanup ---*/\nassert.sameValue(1, 1);\n',
      },
    ];
    const candidates = fixtures.map(({ name, source }) => {
      fs.writeFileSync(path.join(directory, name), source);
      return {
        path: `test/${area}/${name}`,
        sha256: crypto.createHash('sha256').update(source).digest('hex'),
        variants: ['non-strict'],
      };
    });
    const plan = path.join(cwd, 'plan.json');
    fs.writeFileSync(plan, JSON.stringify({
      upstream_root: cwd,
      timeout_ms: 2000,
      variant_limit: 3,
      time_limit_seconds: 30,
      candidates,
    }));
    const results = execFileSync('dotnet', [host, '--plan', plan], {
      cwd: repository, encoding: 'utf8',
    }).trim().split('\n').map(JSON.parse);
    assert.deepEqual(
      results.map((result) => [path.basename(result.path), result.outcome]),
      [
        ['agent-failure.js', 'fail'],
        ['timeout.js', 'infrastructure-error'],
        ['success-after-failure.js', 'pass'],
      ],
    );
    assert.equal(results[1].phase, 'timeout');
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});

test('results imported before an interruption survive and only resume pending work', async () => {
  const cwd = fs.realpathSync(fs.mkdtempSync(path.join(os.tmpdir(), 'native-host-')));
  try {
    const { runId, screenPlan } = prepare(cwd);
    const child = startHost(cwd, screenPlan);
    const imported = [];
    const reader = readline.createInterface({ input: child.stdout });
    await new Promise((resolve, reject) => {
      child.on('error', reject);
      child.on('close', resolve);
      reader.on('line', (line) => {
        if (!line.trim() || imported.length >= 2) {
          return;
        }
        importLine(cwd, runId, line);
        imported.push(JSON.parse(line));
        if (imported.length === 2) {
          // Simulate a cancelled job after partial progress.
          child.kill('SIGKILL');
        }
      });
    });
    assert.equal(imported.length, 2);
    // The earliest streamed variants are the passing fixture's, so resume must
    // exclude exactly those.
    assert.deepEqual(imported.map((value) => value.outcome), ['pass', 'pass']);

    const report = run(['report', '--run-id', runId], cwd);
    assert.equal(report.counts.attempts, 2);

    const resumed = path.join(cwd, 'resumed-plan.json');
    const pending = run([
      'screen-plan', '--run-id', runId, '--upstream', cwd, '--output', resumed,
    ], cwd);
    assert.equal(pending.variant_count, 11);
    const remaining = JSON.parse(fs.readFileSync(resumed)).candidates
      .flatMap((candidate) => candidate.variants.map(
        (variant) => `${candidate.path}#${variant}`));
    for (const value of imported) {
      assert.ok(!remaining.includes(`${value.path}#${value.variant}`));
    }
  } finally {
    fs.rmSync(cwd, { recursive: true, force: true });
  }
});
