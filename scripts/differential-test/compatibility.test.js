'use strict';

const assert = require('node:assert/strict');
const { randomUUID } = require('node:crypto');
const { spawn } = require('node:child_process');
const { once } = require('node:events');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { test } = require('node:test');
const {
  ArtifactStore, resolveWithin, normalize, decodeObservable, classifyAttempt, compilerReport,
  aggregateOutcome, parseOptions, verifyBundle
} = require('./compatibility');
const { limits, fixtureEnvironment, runBounded } = require('./boundedProcess');
const { createCompileInvocation } = require('./run');
const { hashJson, sha256, validateArtifact } = require('../nodeCompatibility/artifacts');
const baseline = require('../nodeCompatibility/baseline.json');

function temporary(t) {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-evidence-test-'));
  t.after(() => fs.rmSync(directory, { recursive: true, force: true }));
  return directory;
}

function scenario() {
  return {
    id: 'node:scenario/runner/test', targets: ['node:workflow/runner/test'],
    applicability: { state: 'applicable', reason: 'Controlled unit fixture.' },
    entry: 'fixture.js', files: ['fixture.js'], expectedExitCode: 0,
    expectedStdout: 'ok\n', expectedStderr: '', timeoutMs: 1000, compileTimeoutMs: 1000,
    timeoutRationale: 'Bounded deterministic test.', normalization: 'exact', observables: []
  };
}

function processResult(changes = {}) {
  return {
    state: 'completed', exitCode: 0, signal: null, error: null,
    startedAt: '2026-10-23T20:00:00.000Z', finishedAt: '2026-10-23T20:00:00.001Z',
    durationMs: 1, command: '/unit-test/tool', args: [],
    stdout: Buffer.from('ok\n'), stderr: Buffer.alloc(0), ...changes
  };
}

test('compiler invocation is shared with legacy corpus/canary runners', () => {
  const invocation = createCompileInvocation('/fixtures/sample.cjs', '/out', { type: 'dll', path: '/compiler/Jroc.dll' });
  assert.equal(invocation.command, 'dotnet');
  assert.deepEqual(invocation.args, ['/compiler/Jroc.dll', '/fixtures/sample.cjs', '-o', '/out/sample']);
  assert.equal(invocation.dllPath, '/out/sample/sample.dll');
});

test('result classification distinguishes reference, product, compilation and harness outcomes', () => {
  const original = { scenario: scenario(), reference: processResult(), compilation: processResult(), execution: processResult() };
  const cases = [
    [{}, 'pass'],
    [{ execution: processResult({ stdout: Buffer.from('wrong\n') }) }, 'mismatch'],
    [{ execution: processResult({ stderr: Buffer.from('extra\n') }) }, 'mismatch'],
    [{ execution: processResult({ exitCode: 1 }) }, 'mismatch'],
    [{ compilation: processResult({ exitCode: 1 }) }, 'compile-failure'],
    [{ compilation: processResult({ exitCode: 1 }), unsupported: true }, 'unsupported'],
    [{ compilation: processResult({ state: 'timeout' }) }, 'inconclusive'],
    [{ compilerReportValid: false }, 'infrastructure-failure'],
    [{ compilerArtifactPresent: false }, 'infrastructure-failure'],
    [{ execution: processResult({ state: 'timeout' }) }, 'execution-timeout'],
    [{ reference: processResult({ state: 'timeout' }) }, 'reference-failure'],
    [{ reference: processResult({ exitCode: 1 }) }, 'reference-failure'],
    [{ reference: processResult({ stdout: Buffer.from('wrong') }) }, 'reference-failure'],
    [{ reference: processResult({ state: 'spawn-error', error: 'ENOENT' }) }, 'infrastructure-failure'],
    [{ execution: processResult({ state: 'output-limit' }) }, 'infrastructure-failure'],
    [{ compilation: processResult({ state: 'resource-limit' }) }, 'infrastructure-failure'],
    [{ reference: null, compilation: null, execution: null }, 'not-run'],
    [{ compilation: null, execution: null }, 'not-run']
  ];
  for (const [changes, expected] of cases) {
    assert.equal(classifyAttempt({ ...original, ...changes }).outcome, expected, JSON.stringify(changes));
  }
});

test('missing/invalid observables never become passes or ordinary reference mismatches', () => {
  const value = scenario();
  value.observables = [{ path: 'value.json', format: 'json' }];
  const input = { scenario: value, reference: processResult(), compilation: processResult(), execution: processResult() };
  assert.equal(classifyAttempt(input).outcome, 'reference-failure');
  const referenceObservables = [{ error: null, value: { number: 1 } }];
  assert.equal(classifyAttempt({ ...input, referenceObservables }).outcome, 'mismatch');
  assert.equal(classifyAttempt({
    ...input, referenceObservables, jrocObservables: [{ error: null, value: { number: '1' } }]
  }).outcome, 'mismatch');
  assert.equal(classifyAttempt({
    ...input, referenceObservables, jrocObservables: [{ error: 'OBSERVABLE_LIMIT' }]
  }).outcome, 'infrastructure-failure');
});

test('normalization preserves meaningful whitespace, binary data and error/event semantics', () => {
  assert.equal(normalize(Buffer.from('x \r\n'), 'crlf-to-lf').toString(), 'x \n');
  assert.equal(normalize(Buffer.from('x \r\n'), 'exact').toString(), 'x \r\n');
  assert.deepEqual(normalize(Buffer.from([255, 13, 10]), 'crlf-to-lf'), Buffer.from([255, 10]));
  assert.throws(() => normalize(Buffer.alloc(0), 'trim-everything'), /Unknown normalization/);
  const bytes = Buffer.from([0, 255, 13, 10]);
  assert.deepEqual(decodeObservable(bytes, { format: 'bytes' }), bytes);
  assert.throws(() => decodeObservable(Buffer.from([255]), { format: 'json' }));
  assert.notDeepEqual(decodeObservable(Buffer.from('1'), { format: 'json' }), decodeObservable(Buffer.from('"1"'), { format: 'json' }));
  const observable = { format: 'event-order', order: [['start', 'a'], ['start', 'b'], ['a', 'end'], ['b', 'end']] };
  assert.deepEqual(decodeObservable(Buffer.from('["start","a","b","end"]'), observable),
    decodeObservable(Buffer.from('["start","b","a","end"]'), observable));
  assert.throws(() => decodeObservable(Buffer.from('["end","a","b","start"]'), observable), /ordering/);
  assert.throws(() => decodeObservable(Buffer.from('["start","a","a","end"]'), observable), /unique/);
});

test('unsupported classification requires a consistent explicit compiler report', () => {
  const report = {
    schemaVersion: 1, measurement: 'statement-source-sites-v1', compilationSucceeded: false,
    counts: { unsupported: 1 }, sites: [{ mode: 'unsupported' }]
  };
  assert.deepEqual(compilerReport(Buffer.from(JSON.stringify(report)), processResult({ exitCode: 1 })), { valid: true, unsupported: true });
  assert.equal(compilerReport(Buffer.from('NotSupportedException: arbitrary stdout'), processResult()).valid, false);
  report.counts.unsupported = 0;
  assert.equal(compilerReport(Buffer.from(JSON.stringify(report)), processResult({ exitCode: 1 })).valid, false);
});

test('retries retain conflicting outcomes instead of selecting a success', () => {
  const attempts = [{ outcome: 'mismatch' }, { outcome: 'pass' }];
  assert.equal(aggregateOutcome(attempts), 'flaky');
  assert.equal(attempts.length, 2);
  assert.equal(aggregateOutcome([{ outcome: 'unsupported' }, { outcome: 'unsupported' }]), 'unsupported');
  assert.throws(() => aggregateOutcome([]), /empty/);
});

test('CLI and manifest validation reject ambiguous inputs', () => {
  assert.equal(parseOptions(['--attempts', '2', '--case', scenario().id]).attempts, 2);
  for (const args of [['--attempts', '0'], ['--attempts', 'NaN'], ['--attempts', '1.5'], ['--unknown'], ['--manifest']]) {
    assert.throws(() => parseOptions(args));
  }
  const manifest = { schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'scenario-manifest', scopeId: baseline.scopeId, corpusId: 'unit', referenceProfileSha256: hashJson(baseline.reference), cases: [scenario()] };
  validateArtifact(manifest, 'scenario-manifest');
  assert.throws(() => validateArtifact({ ...manifest, cases: [scenario(), scenario()] }, 'scenario-manifest'), /Duplicate/);
  const changed = scenario();
  changed.files = ['different.js'];
  assert.throws(() => validateArtifact({ ...manifest, cases: [changed] }, 'scenario-manifest'), /Entry/);
  changed.files = ['fixture.js'];
  changed.observables = [{ path: 'fixture.js', format: 'json' }];
  assert.throws(() => validateArtifact({ ...manifest, cases: [changed] }, 'scenario-manifest'), /overwrite/);
});

test('evidence writes are bounded, append-only and reject escaping/symlink paths', t => {
  const directory = temporary(t);
  const store = new ArtifactStore(directory);
  const reference = store.write('raw/data.bin', Buffer.from([0, 255]));
  assert.equal(reference.sha256, sha256(Buffer.from([0, 255])));
  assert.throws(() => store.write('raw/data.bin', 'replacement'), /EEXIST/);
  for (const file of ['../outside', '/absolute', 'a/../b', 'a//b', 'a\\b', '.']) {
    assert.throws(() => resolveWithin(directory, file));
  }
  if (process.platform === 'linux') {
    fs.symlinkSync(os.tmpdir(), path.join(directory, 'link'));
    assert.throws(() => resolveWithin(directory, 'link/outside'), /Symlink/);
  }
  store.bytes = limits.bundleBytes;
  assert.throws(() => store.write('over-budget', 'x'), /budget/);
});

const linux = { skip: process.platform !== 'linux' };

test('fixture processes get isolated homes, exact bytes and no parent credentials', linux, async t => {
  const cwd = temporary(t);
  const env = fixtureEnvironment(cwd, [process.execPath]);
  for (const name of ['GH_TOKEN', 'SUPABASE_URL', 'SUPABASE_SERVICE_ROLE_KEY', 'NODE_OPTIONS', 'NUGET_AUTH_TOKEN']) {
    assert(!Object.hasOwn(env, name));
  }
  const result = await runBounded(process.execPath, ['-e', 'process.stdout.write(Buffer.from([0,255])); process.stderr.write("err");'], { cwd, env, timeoutMs: 2000 });
  assert.equal(result.state, 'completed');
  assert.equal(result.exitCode, 0);
  assert.deepEqual(result.stdout, Buffer.from([0, 255]));
  assert.equal(result.stderr.toString(), 'err');
  assert.equal(env.HOME, path.join(cwd, '.home'));
});

test('timeouts, output/resource caps and spawn failures are explicit with partial evidence', linux, async t => {
  const cwd = temporary(t);
  const env = fixtureEnvironment(cwd, [process.execPath]);
  const timedOut = await runBounded(process.execPath, ['-e', 'console.log("partial"); setInterval(()=>{}, 1000);'], { cwd, env, timeoutMs: 1000 });
  assert.equal(timedOut.state, 'timeout');
  assert.equal(timedOut.stdout.toString(), 'partial\n');
  const output = await runBounded(process.execPath, ['-e', 'process.stdout.write("x".repeat(10000));'], {
    cwd, env, timeoutMs: 2000, budget: { ...limits, outputBytes: 512 }
  });
  assert.equal(output.state, 'output-limit');
  assert.equal(output.stdout.length, 512);
  const resource = await runBounded(process.execPath, ['-e', 'require("fs").writeFileSync("large", "x".repeat(4096));'], {
    cwd, env, timeoutMs: 2000, budget: { ...limits, workspaceBytes: 1024 }
  });
  assert.equal(resource.state, 'resource-limit');
  const missing = await runBounded('/no-such-jroc-test-executable', [], { cwd, env, timeoutMs: 2000, monitor: false });
  assert.equal(missing.state, 'spawn-error');
  assert.equal(missing.error, 'ENOENT');
});

test('normal completion and timeout clean up descendant process groups', linux, async t => {
  const cwd = temporary(t);
  const env = fixtureEnvironment(cwd, [process.execPath]);
  for (const timeout of [false, true]) {
    const code = `
      const child = require('child_process').spawn(process.execPath, ['-e', 'setInterval(()=>{},1000)'], {stdio:'ignore'});
      console.log(child.pid);
      ${timeout ? 'setInterval(()=>{},1000);' : 'setTimeout(()=>process.exit(0),100);'}
    `;
    const result = await runBounded(process.execPath, ['-e', code], { cwd, env, timeoutMs: 1000 });
    assert.equal(result.state, timeout ? 'timeout' : 'completed');
    const pid = Number(result.stdout.toString().trim());
    assert(Number.isSafeInteger(pid) && pid > 1);
    const running = () => {
      try { return fs.readFileSync(`/proc/${pid}/stat`, 'utf8').split(' ')[2] !== 'Z'; }
      catch (failure) { if (failure.code === 'ENOENT') return false; throw failure; }
    };
    for (let i = 0; i < 20 && running(); i++) await new Promise(resolve => setTimeout(resolve, 25));
    assert.equal(running(), false, `Descendant ${pid} survived its owning process.`);
  }
});

test('supervisor SIGTERM kills its active fixture process group', linux, async t => {
  const cwd = temporary(t);
  const fixture = 'require("fs").writeFileSync("fixture.pid", String(process.pid)); setInterval(()=>{},1000);';
  const code = `
    const {runBounded,fixtureEnvironment} = require(${JSON.stringify(path.join(__dirname, 'boundedProcess.js'))});
    runBounded(process.execPath, ['-e', ${JSON.stringify(fixture)}], {
      cwd: ${JSON.stringify(cwd)}, env: fixtureEnvironment(${JSON.stringify(cwd)}, [process.execPath]), timeoutMs: 10000
    });
  `;
  const supervisor = spawn(process.execPath, ['-e', code], { stdio: ['ignore', 'ignore', 'pipe'] });
  supervisor.stderr.resume();
  t.after(() => { if (supervisor.exitCode === null && supervisor.signalCode === null) supervisor.kill('SIGTERM'); });
  const completion = once(supervisor, 'close');
  const pidFile = path.join(cwd, 'fixture.pid');
  for (let i = 0; i < 100 && !fs.existsSync(pidFile); i++) await new Promise(resolve => setTimeout(resolve, 20));
  assert(fs.existsSync(pidFile));
  const pid = Number(fs.readFileSync(pidFile, 'utf8'));
  supervisor.kill('SIGTERM');
  const [codeValue] = await completion;
  assert.equal(codeValue, 143);
  let running = true;
  for (let i = 0; i < 20 && running; i++) {
    try { running = fs.readFileSync(`/proc/${pid}/stat`, 'utf8').split(' ')[2] !== 'Z'; }
    catch (failure) { if (failure.code !== 'ENOENT') throw failure; running = false; }
    if (running) await new Promise(resolve => setTimeout(resolve, 25));
  }
  assert.equal(running, false);
});

function syntheticBundle(t) {
  const directory = temporary(t);
  const store = new ArtifactStore(directory);
  const value = scenario();
  const manifest = { schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'scenario-manifest', scopeId: baseline.scopeId, corpusId: 'synthetic-unit-corpus', referenceProfileSha256: hashJson(baseline.reference), cases: [value] };
  const corpusManifest = store.json('inputs/corpus/manifest.json', manifest);
  const corpusFiles = [store.write('inputs/corpus/files/fixture.js', 'console.log("ok");')];
  const inventoryBytes = fs.readFileSync(path.join(__dirname, '../../docs/nodejs/compatibility/inventory.v1.json'));
  const inventory = JSON.parse(inventoryBytes);
  const inventoryRef = store.write('inputs/inventory.json', inventoryBytes);
  const environment = {
    schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'reference-environment', scopeId: baseline.scopeId,
    recordedAt: '2026-10-23T20:00:00.000Z', nodeVersion: baseline.reference.nodeVersion,
    nodeExecutable: '/unit/node', nodeArguments: [], runtimeOverridesPresent: [],
    nodeVersions: { node: baseline.reference.nodeVersion }, platform: 'linux', architecture: 'x64', kernelVersion: 'unit',
    dotnetSdk: baseline.reference.dotnetSdk, dotnetRuntimes: [baseline.reference.dotnetRuntime],
    jrocRevision: 'a'.repeat(40), worktreeDirty: true, referenceEnvironmentEligible: false,
    referenceSourceRevision: baseline.reference.sourceRevision, inventorySha256: inventoryRef.sha256,
    referenceProfileSha256: inventory.referenceProfileSha256, targetInventorySha256: inventory.targetInventorySha256,
    implementationEvidenceSha256: inventory.implementationEvidenceSha256, behaviorRegistrationSha256: hashJson(inventory.behaviors),
    executableBehaviorCases: 0, baselineConfigSha256: 'b'.repeat(64),
    lockfiles: [{ path: 'package-lock.json', sha256: 'c'.repeat(64) }], builtinModules: baseline.builtinModules
  };
  const compilerFiles = [store.write('inputs/compiler/unit.dll', 'synthetic unit bytes, not a compiler')];
  const provenance = {
    checkoutRevision: environment.jrocRevision, worktreeDirty: true, runnerRevision: null,
    runnerFiles: [store.write('inputs/runner/unit.js', 'synthetic runner bytes')],
    corpusId: manifest.corpusId, corpusRevision: hashJson({ manifestSha256: corpusManifest.sha256, files: corpusFiles }),
    corpusRevisionAlgorithm: 'sha256', corpusSourceRevision: null, corpusManifest, corpusFiles,
    environment: store.json('inputs/environment.json', environment), inventory: inventoryRef,
    referenceProfileSha256: inventory.referenceProfileSha256, targetInventorySha256: inventory.targetInventorySha256,
    implementationEvidenceSha256: inventory.implementationEvidenceSha256, nodeExecutable: '/unit/node', dotnetExecutable: '/unit/dotnet',
    compilerSha256: compilerFiles[0].sha256, compilerSourceRevision: null, compilerFiles, trustClass: 'local-unverified'
  };
  const runId = randomUUID();
  const policy = { id: 'node-observables-v1', scenario: value, limits, environmentPolicy: 'isolated-linux-allowlist-v1' };
  const processDirectory = temporary(t);
  const processEnvironment = fixtureEnvironment(processDirectory, [process.execPath]);
  function observation(label) {
    const { stdout, stderr, ...metadata } = processResult();
    return {
      ...metadata, cwd: processDirectory, environment: processEnvironment,
      stdout: store.write(`${label}.stdout.bin`, stdout), stderr: store.write(`${label}.stderr.bin`, stderr)
    };
  }
  const result = {
    schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'execution-result', scopeId: baseline.scopeId,
    runId, attemptId: randomUUID(), attemptNumber: 1, scenarioId: value.id, targets: value.targets,
    phase: 'comparison', outcome: 'pass', detail: 'Synthetic schema/readback test; no acceptance claim.',
    policy, policySha256: hashJson(policy), provenance, reference: observation('reference'), compilation: observation('compile'),
    execution: observation('execution'), referenceObservables: [], jrocObservables: [],
    compiledArtifacts: [store.write('compiled/unit.dll', 'synthetic compiled bytes')],
    coverageReport: store.json('coverage.json', {
      schemaVersion: 1, measurement: 'statement-source-sites-v1', compilationSucceeded: true,
      counts: { unsupported: 0 }, sites: []
    })
  };
  validateArtifact(result, 'execution-result');
  const bundle = {
    schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'execution-bundle', scopeId: baseline.scopeId, runId,
    startedAt: '2026-10-23T20:00:00.000Z', finishedAt: '2026-10-23T20:00:01.000Z', attemptsPerCase: 1, provenance,
    results: [store.json('result.json', result)], cases: [{ id: value.id, outcome: 'pass' }], files: [...store.files]
  };
  const bundleRef = store.json('bundle.json', bundle);
  store.write('bundle.sha256', `${bundleRef.sha256}\n`);
  return { directory, bundle, result };
}

test('typed evidence round-trips offline with independent source/validation provenance', t => {
  const { directory, bundle, result } = syntheticBundle(t);
  assert.deepEqual(verifyBundle(directory), bundle);
  assert.deepEqual(JSON.parse(JSON.stringify(result)), result);
  assert.equal(bundle.provenance.trustClass, 'local-unverified');
  for (const changed of [
    { schemaVersion: 99 }, { artifactKind: 'api-inventory' }, { reference: null },
    { compilation: null }, { compiledArtifacts: [] }, { coverageReport: null }, { policySha256: '0'.repeat(64) }
  ]) assert.throws(() => validateArtifact({ ...result, ...changed }, 'execution-result'));
  for (const state of ['unknown', 'excluded']) {
    const notRun = structuredClone(result);
    notRun.outcome = 'not-run';
    notRun.phase = 'reference';
    notRun.policy.scenario.applicability = { state, reason: 'Not applicable to this reference profile.' };
    notRun.policySha256 = hashJson(notRun.policy);
    assert.throws(() => validateArtifact(notRun, 'execution-result'), /must not execute/);
    notRun.reference = notRun.compilation = notRun.execution = null;
    notRun.compiledArtifacts = [];
    notRun.coverageReport = null;
    validateArtifact(notRun, 'execution-result');
  }
});

test('offline readback rejects altered bytes, invented passes, missing history and unknown files', t => {
  for (const mutation of ['bytes', 'pass', 'history', 'extra', 'attempt-number']) {
    const { directory, bundle, result } = syntheticBundle(t);
    if (mutation === 'bytes') fs.writeFileSync(path.join(directory, result.reference.stdout.path), 'tampered');
    if (mutation === 'extra') fs.writeFileSync(path.join(directory, 'unregistered'), 'unexpected');
    if (mutation === 'history') {
      bundle.attemptsPerCase = 2;
      const bytes = Buffer.from(`${JSON.stringify(bundle)}\n`);
      fs.writeFileSync(path.join(directory, 'bundle.json'), bytes);
      fs.writeFileSync(path.join(directory, 'bundle.sha256'), `${sha256(bytes)}\n`);
    }
    if (mutation === 'pass') {
      const raw = Buffer.from('different product output\n');
      fs.writeFileSync(path.join(directory, result.execution.stdout.path), raw);
      const reference = { ...result.execution.stdout, sha256: sha256(raw), bytes: raw.length };
      Object.assign(result.execution.stdout, reference);
      Object.assign(bundle.files.find(file => file.path === reference.path), reference);
      const resultBytes = Buffer.from(`${JSON.stringify(result)}\n`);
      fs.writeFileSync(path.join(directory, bundle.results[0].path), resultBytes);
      const resultRef = { ...bundle.results[0], sha256: sha256(resultBytes), bytes: resultBytes.length };
      Object.assign(bundle.results[0], resultRef);
      Object.assign(bundle.files.find(file => file.path === resultRef.path), resultRef);
      const bytes = Buffer.from(`${JSON.stringify(bundle)}\n`);
      fs.writeFileSync(path.join(directory, 'bundle.json'), bytes);
      fs.writeFileSync(path.join(directory, 'bundle.sha256'), `${sha256(bytes)}\n`);
    }
    if (mutation === 'attempt-number') {
      result.attemptNumber = 5;
      const resultBytes = Buffer.from(`${JSON.stringify(result)}\n`);
      fs.writeFileSync(path.join(directory, bundle.results[0].path), resultBytes);
      const resultRef = { ...bundle.results[0], sha256: sha256(resultBytes), bytes: resultBytes.length };
      Object.assign(bundle.results[0], resultRef);
      Object.assign(bundle.files.find(file => file.path === resultRef.path), resultRef);
      const bytes = Buffer.from(`${JSON.stringify(bundle)}\n`);
      fs.writeFileSync(path.join(directory, 'bundle.json'), bytes);
      fs.writeFileSync(path.join(directory, 'bundle.sha256'), `${sha256(bytes)}\n`);
    }
    assert.throws(() => verifyBundle(directory), undefined, mutation);
  }
});
