'use strict';

const assert = require('node:assert/strict');
const { randomUUID } = require('node:crypto');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { findJroc, createCompileInvocation } = require('./run');
const { limits, workspaceFiles, fixtureEnvironment, runBounded } = require('./boundedProcess');
const { sha256, hashJson, validateArtifact } = require('../nodeCompatibility/artifacts');
const { collectEnvironment, validateEnvironment } = require('../nodeCompatibility/checkReference');
const baseline = require('../nodeCompatibility/baseline.json');

const repoRoot = path.resolve(__dirname, '../..');
const defaultManifest = path.join(__dirname, 'corpus/node-compatibility/manifest.json');

function resolveWithin(root, relative) {
  if (!relative || path.isAbsolute(relative) || relative.includes('\\') || relative.includes('\0')
    || path.posix.normalize(relative) !== relative || relative === '.' || relative.split('/').includes('..')) {
    throw new Error(`Non-canonical relative artifact/input path: ${relative}`);
  }
  const resolved = path.join(root, relative);
  let current = root;
  for (const segment of relative.split('/')) {
    current = path.join(current, segment);
    if (fs.existsSync(current) && fs.lstatSync(current).isSymbolicLink()) throw new Error('Symlinks are not supported in evidence paths.');
  }
  return resolved;
}

function readBounded(root, relative, maximum = limits.observableBytes) {
  const file = resolveWithin(root, relative);
  const stat = fs.lstatSync(file);
  if (!stat.isFile()) throw new Error('Expected a regular evidence/input file.');
  if (stat.size > maximum) throw Object.assign(new Error('Evidence/input byte budget exceeded.'), { code: 'OBSERVABLE_LIMIT' });
  return fs.readFileSync(file);
}

class ArtifactStore {
  constructor(directory) {
    fs.mkdirSync(directory, { recursive: true });
    this.root = fs.realpathSync(directory);
    this.files = [];
    this.bytes = 0;
  }

  write(relative, bytes) {
    const buffer = Buffer.isBuffer(bytes) ? bytes : Buffer.from(bytes);
    if (this.bytes + buffer.length > limits.bundleBytes || this.files.length >= 4096) throw new Error('Bundle artifact budget exceeded.');
    const file = resolveWithin(this.root, relative);
    fs.mkdirSync(path.dirname(file), { recursive: true });
    fs.writeFileSync(file, buffer, { flag: 'wx' });
    const reference = { path: relative, sha256: sha256(buffer), bytes: buffer.length };
    this.files.push(reference);
    this.bytes += buffer.length;
    return reference;
  }

  json(relative, value) {
    return this.write(relative, `${JSON.stringify(value, null, 2)}\n`);
  }
}

function normalize(bytes, policy) {
  if (policy === 'exact') return bytes;
  if (policy !== 'crlf-to-lf') throw new Error(`Unknown normalization policy ${policy}.`);
  const output = [];
  for (let i = 0; i < bytes.length; i++) {
    if (bytes[i] !== 13 || bytes[i + 1] !== 10) output.push(bytes[i]);
  }
  return Buffer.from(output);
}

function decodeObservable(bytes, observable) {
  if (!['bytes', 'json', 'event-order'].includes(observable.format)) throw new Error(`Unknown observable format ${observable.format}.`);
  if (observable.format === 'bytes') return bytes;
  const value = JSON.parse(new TextDecoder('utf-8', { fatal: true }).decode(bytes));
  if (observable.format === 'event-order') {
    if (!Array.isArray(value) || value.some(item => typeof item !== 'string') || new Set(value).size !== value.length) {
      throw new Error('Event log must contain unique string event IDs.');
    }
    for (const [before, after] of observable.order) {
      if (!value.includes(before) || !value.includes(after) || value.indexOf(before) >= value.indexOf(after)) {
        throw new Error(`Required event ordering was not observed: ${before} before ${after}.`);
      }
    }
    return [...value].sort();
  }
  return value;
}

function observe(directory, observables, store, prefix) {
  return observables.map(observable => {
    let artifact = null;
    let value;
    let error = null;
    let bytes;
    try {
      bytes = readBounded(directory, observable.path);
    } catch (failure) {
      error = failure.code ?? failure.message;
    }
    if (bytes) {
      artifact = store.write(`${prefix}/${observable.path}`, bytes);
      try {
        value = decodeObservable(bytes, observable);
      } catch (failure) {
        error = failure.code ?? failure.message;
      }
    }
    return { path: observable.path, format: observable.format, artifact, error, value };
  });
}

function classifyAttempt({
  reference, compilation, execution, scenario, referenceObservables = [], jrocObservables = [],
  unsupported = false, compilerReportValid = true, compilerArtifactPresent = true
}) {
  const result = (outcome, phase, detail) => ({ outcome, phase, detail });
  const infrastructure = process => process && !['completed', 'timeout'].includes(process.state);
  if (!reference) return result('not-run', 'reference', 'No reference execution.');
  if (infrastructure(reference)) return result('infrastructure-failure', 'reference', reference.error ?? reference.state);
  if (reference.state === 'timeout' || reference.exitCode !== scenario.expectedExitCode) {
    return result('reference-failure', 'reference', 'Reference did not satisfy its completion/exit expectation.');
  }
  for (const [field, expected] of [['stdout', scenario.expectedStdout], ['stderr', scenario.expectedStderr]]) {
    if (expected !== undefined && !normalize(reference[field], scenario.normalization).equals(normalize(Buffer.from(expected), scenario.normalization))) {
      return result('reference-failure', 'reference', `Reference ${field} did not satisfy its committed expectation.`);
    }
  }
  if (referenceObservables.length !== scenario.observables.length || referenceObservables.some(item => item.error)) {
    const bounded = referenceObservables.some(item => item.error === 'OBSERVABLE_LIMIT');
    return result(bounded ? 'infrastructure-failure' : 'reference-failure', 'reference', 'Reference observables are incomplete or invalid.');
  }
  if (!compilation) return result('not-run', 'compile', 'Reference valid; compilation not run.');
  if (infrastructure(compilation)) return result('infrastructure-failure', 'compile', compilation.error ?? compilation.state);
  if (compilation.state === 'timeout') return result('inconclusive', 'compile', 'Compilation exceeded its budget; no product execution result.');
  if (!compilerReportValid) return result('infrastructure-failure', 'compile', 'Missing, malformed or contradictory compiler coverage report.');
  if (compilation.exitCode !== 0) return result(unsupported ? 'unsupported' : 'compile-failure', 'compile', 'Compiler rejected the valid reference scenario.');
  if (!compilerArtifactPresent) return result('infrastructure-failure', 'compile', 'Compiler reported success without a generated DLL.');
  if (!execution) return result('not-run', 'execution', 'Compilation valid; product execution not run.');
  if (infrastructure(execution)) return result('infrastructure-failure', 'execution', execution.error ?? execution.state);
  if (execution.state === 'timeout') return result('execution-timeout', 'execution', 'Product exceeded the declared budget after a valid reference and compilation.');
  if (jrocObservables.some(item => item.error === 'OBSERVABLE_LIMIT')) {
    return result('infrastructure-failure', 'execution', 'Product observable exceeded the harness byte budget.');
  }
  if (execution.exitCode !== reference.exitCode || execution.signal !== reference.signal
    || !normalize(execution.stdout, scenario.normalization).equals(normalize(reference.stdout, scenario.normalization))
    || !normalize(execution.stderr, scenario.normalization).equals(normalize(reference.stderr, scenario.normalization))) {
    return result('mismatch', 'comparison', 'Exit status, signal, stdout or stderr differs.');
  }
  if (jrocObservables.length !== referenceObservables.length || jrocObservables.some(item => item.error)) {
    return result('mismatch', 'comparison', 'Product observables are incomplete or invalid.');
  }
  try {
    assert.deepStrictEqual(jrocObservables.map(item => item.value), referenceObservables.map(item => item.value));
  } catch (failure) {
    if (failure.code !== 'ERR_ASSERTION') throw failure;
    return result('mismatch', 'comparison', 'Structured values, bytes, errors, effects or event constraints differ.');
  }
  return result('pass', 'comparison', 'Paired executions satisfy the declared comparison policy.');
}

function publishProcess(store, prefix, observation) {
  if (!observation) return null;
  const { stdout, stderr, ...metadata } = observation;
  return {
    ...metadata,
    stdout: store.write(`${prefix}.stdout.bin`, stdout),
    stderr: store.write(`${prefix}.stderr.bin`, stderr)
  };
}

function copyInputs(files, directory) {
  for (const [relative, bytes] of files) {
    if (['.home', '.tmp', 'build', 'coverage.json'].includes(relative.split('/')[0])) throw new Error(`Reserved workspace input path: ${relative}`);
    const destination = resolveWithin(directory, relative);
    fs.mkdirSync(path.dirname(destination), { recursive: true });
    fs.writeFileSync(destination, bytes, { flag: 'wx' });
  }
}

function compilerReport(bytes, compilation) {
  let report;
  try {
    report = JSON.parse(new TextDecoder('utf-8', { fatal: true }).decode(bytes));
  } catch (failure) {
    if (!(failure instanceof SyntaxError || failure instanceof TypeError)) throw failure;
    return { valid: false, unsupported: false };
  }
  const valid = !!report && report.schemaVersion === 1
    && report.measurement === 'statement-source-sites-v1'
    && report.compilationSucceeded === (compilation.exitCode === 0)
    && Number.isSafeInteger(report.counts?.unsupported) && report.counts.unsupported >= 0
    && Array.isArray(report.sites) && report.sites.every(site => site
      && ['directIl', 'runtimeIntrinsic', 'runtimeDispatch', 'unsupported'].includes(site.mode))
    && report.counts.unsupported === report.sites.filter(site => site.mode === 'unsupported').length;
  return { valid, unsupported: valid && report.counts.unsupported > 0 };
}

async function runAttempt(scenario, files, compiler, dotnet, store, context, attemptNumber) {
  const attemptId = randomUUID();
  const prefix = `attempts/${attemptId}`;
  const workspace = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-node-evidence-'));
  let reference = null;
  let compilation = null;
  let execution = null;
  let referenceObservables = [];
  let jrocObservables = [];
  let compiledArtifacts = [];
  let coverageReport = null;
  let classification;
  const policy = { id: 'node-observables-v1', scenario, limits, environmentPolicy: 'isolated-linux-allowlist-v1' };
  const workspaces = {};
  try {
    for (const phase of ['reference', 'compile', 'execution']) {
      const directory = path.join(workspace, phase);
      fs.mkdirSync(directory);
      copyInputs(files, directory);
      workspaces[phase] = { directory, env: fixtureEnvironment(directory, [process.execPath, dotnet]) };
      workspaceFiles(directory);
    }
    const execute = (command, args, phase, timeoutMs) => runBounded(command, args, {
      cwd: workspaces[phase].directory, env: workspaces[phase].env, timeoutMs
    });
    if (scenario.applicability.state === 'applicable') {
      reference = await execute(process.execPath, [resolveWithin(workspaces.reference.directory, scenario.entry)], 'reference', scenario.timeoutMs);
    }
    if (reference?.state === 'completed' && reference.exitCode === scenario.expectedExitCode) {
      referenceObservables = observe(workspaces.reference.directory, scenario.observables, store, `${prefix}/reference-observables`);
    }
    const inputs = () => ({ reference, compilation, execution, scenario, referenceObservables, jrocObservables });
    classification = classifyAttempt(inputs());
    if (classification.outcome === 'not-run' && classification.phase === 'compile') {
      const invocation = createCompileInvocation(resolveWithin(workspaces.compile.directory, scenario.entry),
        path.join(workspaces.compile.directory, 'build'), compiler);
      fs.mkdirSync(invocation.outputDirectory, { recursive: true });
      const reportPath = path.join(workspaces.compile.directory, 'coverage.json');
      compilation = await execute(dotnet, [...invocation.args, '--coverage-report', reportPath], 'compile', scenario.compileTimeoutMs);
      let report = { valid: false, unsupported: false };
      if (fs.existsSync(reportPath)) {
        const bytes = readBounded(workspaces.compile.directory, 'coverage.json');
        coverageReport = store.write(`${prefix}/coverage.json`, bytes);
        report = compilerReport(bytes, compilation);
      }
      const { valid: compilerReportValid, unsupported } = report;
      const compilerArtifactPresent = fs.existsSync(invocation.dllPath);
      classification = classifyAttempt({ ...inputs(), compilerReportValid, unsupported, compilerArtifactPresent });
      if (classification.outcome === 'not-run' && classification.phase === 'execution') {
        compiledArtifacts = workspaceFiles(invocation.outputDirectory).map(file =>
          store.write(`${prefix}/compiled/${file.path}`, readBounded(invocation.outputDirectory, file.path, limits.workspaceBytes)));
        execution = await execute(dotnet, [invocation.dllPath], 'execution', scenario.timeoutMs);
        if (execution.state === 'completed') {
          jrocObservables = observe(workspaces.execution.directory, scenario.observables, store, `${prefix}/jroc-observables`);
        }
        classification = classifyAttempt({ ...inputs(), compilerReportValid, unsupported, compilerArtifactPresent });
      }
    }
  } catch (failure) {
    classification = { outcome: 'infrastructure-failure', phase: 'infrastructure', detail: `${failure.code ?? failure.name}: ${failure.message}` };
  } finally {
    try {
      fs.rmSync(workspace, { recursive: true, force: true });
    } catch (failure) {
      classification = { outcome: 'infrastructure-failure', phase: 'infrastructure', detail: `Workspace cleanup failed: ${failure.message}` };
    }
  }
  const publicObservables = values => values.map(({ value, ...item }) => item);
  const result = {
    schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'execution-result', scopeId: baseline.scopeId,
    runId: context.runId, attemptId, attemptNumber, scenarioId: scenario.id, targets: scenario.targets,
    ...classification, policy, policySha256: hashJson(policy), provenance: context.provenance,
    reference: publishProcess(store, `${prefix}/reference`, reference),
    compilation: publishProcess(store, `${prefix}/compile`, compilation),
    execution: publishProcess(store, `${prefix}/execution`, execution),
    referenceObservables: publicObservables(referenceObservables), jrocObservables: publicObservables(jrocObservables),
    compiledArtifacts, coverageReport
  };
  validateArtifact(result, 'execution-result');
  return result;
}

function aggregateOutcome(results) {
  if (!results.length) throw new Error('Cannot summarize an empty attempt history.');
  const outcomes = new Set(results.map(result => result.outcome));
  return outcomes.size === 1 ? results[0].outcome : 'flaky';
}

function findDotnet() {
  for (const directory of (process.env.PATH ?? '').split(path.delimiter)) {
    const candidate = path.resolve(directory, 'dotnet');
    try {
      fs.accessSync(candidate, fs.constants.X_OK);
      return fs.realpathSync(candidate);
    } catch (failure) {
      if (!['ENOENT', 'EACCES'].includes(failure.code)) throw failure;
    }
  }
  throw new Error('Cannot locate dotnet on PATH.');
}

function parseOptions(args) {
  const options = { manifest: defaultManifest, output: path.join(repoRoot, 'artifacts/nodeCompatibility/runs'), attempts: 1, cases: [], allowDirty: false, jroc: null };
  for (let i = 0; i < args.length; i++) {
    const key = args[i];
    if (key === '--help') { options.help = true; continue; }
    if (key === '--allow-dirty') { options.allowDirty = true; continue; }
    if (!['--manifest', '--output', '--attempts', '--case', '--jroc'].includes(key) || !args[i + 1] || args[i + 1].startsWith('--')) {
      throw new Error(`Unknown or incomplete option ${key}.`);
    }
    const value = args[++i];
    if (key === '--case') options.cases.push(value);
    else options[key.slice(2)] = key === '--attempts' ? Number(value) : value;
  }
  if (!Number.isSafeInteger(options.attempts) || options.attempts < 1 || options.attempts > 5) throw new Error('--attempts must be an integer from 1 to 5.');
  options.manifest = path.resolve(options.manifest);
  options.output = path.resolve(options.output);
  return options;
}

async function runSuite(options) {
  const environment = collectEnvironment();
  validateEnvironment(environment, baseline, { allowDirty: options.allowDirty });
  const manifestBytes = readBounded(path.dirname(options.manifest), path.basename(options.manifest));
  const manifest = JSON.parse(new TextDecoder('utf-8', { fatal: true }).decode(manifestBytes));
  validateArtifact(manifest, 'scenario-manifest');
  if (manifest.scopeId !== baseline.scopeId) throw new Error('Scenario scope does not match the reference inventory.');
  if (manifest.referenceProfileSha256 !== environment.referenceProfileSha256) throw new Error('Scenario applicability was not reviewed for this reference profile.');
  const inventoryBytes = fs.readFileSync(path.join(repoRoot, 'docs/nodejs/compatibility/inventory.v1.json'));
  const inventory = JSON.parse(inventoryBytes);
  const targets = new Set([...inventory.entries, ...inventory.behaviors].map(entry => entry.id));
  const fixtureRoot = path.dirname(options.manifest);
  const allInputs = new Map();
  for (const scenario of manifest.cases) {
    for (const target of scenario.targets) {
      if (!targets.has(target) && !target.startsWith('node:workflow/')) throw new Error(`Unknown inventory target ${target}.`);
    }
    for (const file of scenario.files) {
      if (!allInputs.has(file)) allInputs.set(file, readBounded(fixtureRoot, file));
    }
  }
  for (const id of options.cases) {
    if (!manifest.cases.some(scenario => scenario.id === id)) throw new Error(`Unknown scenario ${id}.`);
  }
  const selected = manifest.cases.filter(scenario => !options.cases.length || options.cases.includes(scenario.id));
  const jroc = findJroc(options.jroc);
  if (jroc.type !== 'dll') throw new Error('Evidence mode requires a built Jroc.dll. Build src/Cli or pass --jroc <dll>.');
  const compilerPath = fs.realpathSync(jroc.path);
  const dotnet = findDotnet();
  const runId = randomUUID();
  fs.mkdirSync(options.output, { recursive: true });
  const directory = path.join(options.output, runId);
  fs.mkdirSync(directory);
  const store = new ArtifactStore(directory);
  console.log(`Evidence run (in progress): ${directory}`);
  const startedAt = new Date().toISOString();
  const environmentRef = store.json('inputs/environment.json', environment);
  const inventoryRef = store.write('inputs/inventory.json', inventoryBytes);
  const corpusManifest = store.write('inputs/corpus/manifest.json', manifestBytes);
  const corpusFiles = [...allInputs].sort(([a], [b]) => a.localeCompare(b, 'en'))
    .map(([file, bytes]) => store.write(`inputs/corpus/files/${file}`, bytes));
  const tracked = new Set(execFileSync('git', ['ls-files', '-z'], {
    cwd: repoRoot, encoding: 'utf8', maxBuffer: 8 * 1024 * 1024, timeout: 30000
  }).split('\0'));
  const pinned = file => !environment.worktreeDirty && tracked.has(path.relative(repoRoot, file).split(path.sep).join('/'));
  const runnerPaths = [
    'scripts/differential-test/run.js', 'scripts/differential-test/compatibility.js', 'scripts/differential-test/boundedProcess.js',
    'scripts/nodeCompatibility/artifacts.js', 'scripts/nodeCompatibility/artifact.schema.json',
    'scripts/nodeCompatibility/execution.schema.json', 'scripts/nodeCompatibility/baseline.json',
    'scripts/nodeCompatibility/checkReference.js', 'scripts/nodeCompatibility/buildInventory.js', 'package-lock.json'
  ];
  const runnerFiles = runnerPaths.map(file => store.write(`inputs/runner/${file}`, fs.readFileSync(path.join(repoRoot, file))));
  const compilerFiles = fs.readdirSync(path.dirname(compilerPath)).sort()
    .filter(file => /\.(dll|deps\.json|runtimeconfig\.json)$/.test(file))
    .map(file => store.write(`inputs/compiler/${file}`, readBounded(path.dirname(compilerPath), file, limits.workspaceBytes)));
  const frozenCompiler = path.join(store.root, 'inputs/compiler', path.basename(compilerPath));
  const provenance = {
    checkoutRevision: environment.jrocRevision, worktreeDirty: environment.worktreeDirty,
    runnerRevision: runnerPaths.every(file => pinned(path.join(repoRoot, file))) ? environment.jrocRevision : null,
    runnerFiles, corpusId: manifest.corpusId,
    corpusRevision: hashJson({ manifestSha256: corpusManifest.sha256, files: corpusFiles }),
    corpusRevisionAlgorithm: 'sha256',
    corpusSourceRevision: pinned(options.manifest) && [...allInputs.keys()].every(file => pinned(path.join(fixtureRoot, file))) ? environment.jrocRevision : null,
    corpusManifest, corpusFiles, environment: environmentRef, inventory: inventoryRef,
    referenceProfileSha256: environment.referenceProfileSha256,
    targetInventorySha256: environment.targetInventorySha256, implementationEvidenceSha256: environment.implementationEvidenceSha256,
    nodeExecutable: process.execPath, dotnetExecutable: dotnet,
    compilerSha256: sha256(fs.readFileSync(frozenCompiler)), compilerSourceRevision: null, compilerFiles,
    trustClass: 'local-unverified'
  };
  const results = [];
  const cases = [];
  for (const scenario of selected) {
    const attempts = [];
    for (let attempt = 1; attempt <= options.attempts; attempt++) {
      const result = await runAttempt(scenario, new Map(scenario.files.map(file => [file, allInputs.get(file)])),
        { type: 'dll', path: frozenCompiler }, dotnet, store, { runId, provenance }, attempt);
      results.push(store.json(`attempts/${result.attemptId}/result.json`, result));
      attempts.push(result);
      console.log(`${result.outcome}: ${scenario.id} (attempt ${attempt})`);
    }
    cases.push({ id: scenario.id, outcome: aggregateOutcome(attempts) });
  }
  const bundle = {
    schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'execution-bundle', scopeId: baseline.scopeId,
    runId, startedAt, finishedAt: new Date().toISOString(), attemptsPerCase: options.attempts, provenance, results, cases,
    files: [...store.files].sort((a, b) => a.path.localeCompare(b.path, 'en'))
  };
  validateArtifact(bundle, 'execution-bundle');
  const bundleRef = store.json('bundle.json', bundle);
  store.write('bundle.sha256', `${bundleRef.sha256}\n`);
  verifyBundle(directory);
  return { directory, bundle };
}

function verifyBundle(directory) {
  const bytes = readBounded(directory, 'bundle.json', limits.bundleBytes);
  if (readBounded(directory, 'bundle.sha256').toString('utf8') !== `${sha256(bytes)}\n`) throw new Error('Bundle checksum mismatch.');
  const bundle = JSON.parse(new TextDecoder('utf-8', { fatal: true }).decode(bytes));
  validateArtifact(bundle, 'execution-bundle');
  const known = new Map(bundle.files.map(file => [file.path, file]));
  function verifyReference(reference) {
    const declared = known.get(reference.path);
    if (!declared || hashJson(declared) !== hashJson(reference)) throw new Error(`Unregistered/conflicting artifact ${reference.path}.`);
    const content = readBounded(directory, reference.path, limits.bundleBytes);
    if (content.length !== reference.bytes || sha256(content) !== reference.sha256) throw new Error(`Artifact digest/size mismatch: ${reference.path}.`);
    return content;
  }
  for (const file of bundle.files) verifyReference(file);
  for (const file of [...bundle.provenance.runnerFiles, ...bundle.provenance.corpusFiles, ...bundle.provenance.compilerFiles]) {
    verifyReference(file);
  }
  const manifest = JSON.parse(verifyReference(bundle.provenance.corpusManifest));
  validateArtifact(manifest, 'scenario-manifest');
  const environment = JSON.parse(verifyReference(bundle.provenance.environment));
  validateArtifact(environment, 'reference-environment');
  const inventory = JSON.parse(verifyReference(bundle.provenance.inventory));
  validateArtifact(inventory, 'api-inventory');
  if (manifest.corpusId !== bundle.provenance.corpusId || manifest.scopeId !== bundle.scopeId
    || manifest.referenceProfileSha256 !== environment.referenceProfileSha256
    || environment.jrocRevision !== bundle.provenance.checkoutRevision
    || environment.worktreeDirty !== bundle.provenance.worktreeDirty
    || environment.nodeExecutable !== bundle.provenance.nodeExecutable
    || environment.inventorySha256 !== bundle.provenance.inventory.sha256
    || !bundle.provenance.compilerFiles.some(file => file.sha256 === bundle.provenance.compilerSha256)) {
    throw new Error('Bundle source/environment/compiler identities do not reconcile.');
  }
  for (const key of ['referenceProfileSha256', 'targetInventorySha256', 'implementationEvidenceSha256']) {
    if (environment[key] !== inventory[key] || bundle.provenance[key] !== inventory[key]) throw new Error(`Bundle ${key} mismatch.`);
  }
  const expectedInputs = [...new Set(manifest.cases.flatMap(scenario => scenario.files))]
    .map(file => `inputs/corpus/files/${file}`).sort();
  if (hashJson(expectedInputs) !== hashJson(bundle.provenance.corpusFiles.map(file => file.path).sort())) {
    throw new Error('Executable corpus input inventory does not reconcile.');
  }
  if (bundle.provenance.corpusRevision !== hashJson({
    manifestSha256: bundle.provenance.corpusManifest.sha256, files: bundle.provenance.corpusFiles
  })) throw new Error('Executable corpus digest mismatch.');
  const attempts = bundle.results.map(reference => {
    const result = JSON.parse(verifyReference(reference));
    validateArtifact(result, 'execution-result');
    if (result.runId !== bundle.runId || hashJson(result.provenance) !== hashJson(bundle.provenance)
      || !manifest.cases.some(scenario => hashJson(scenario) === hashJson(result.policy.scenario))) {
      throw new Error('Result identity/provenance does not match bundle/corpus.');
    }
    for (const observation of [result.reference, result.compilation, result.execution]) {
      if (observation) { verifyReference(observation.stdout); verifyReference(observation.stderr); }
    }
    for (const observation of [...result.referenceObservables, ...result.jrocObservables]) {
      if (observation.artifact) verifyReference(observation.artifact);
    }
    for (const artifact of result.compiledArtifacts) verifyReference(artifact);
    if (result.coverageReport) verifyReference(result.coverageReport);
    const restoreProcess = observation => observation ? {
      ...observation, stdout: verifyReference(observation.stdout), stderr: verifyReference(observation.stderr)
    } : null;
    const restoreObservables = observations => observations.map((observation, index) => ({
      ...observation,
      value: observation.error || !observation.artifact ? undefined
        : decodeObservable(verifyReference(observation.artifact), result.policy.scenario.observables[index])
    }));
    if (!(result.outcome === 'infrastructure-failure' && result.phase === 'infrastructure')) {
      const compilation = restoreProcess(result.compilation);
      const report = result.coverageReport && compilation
        ? compilerReport(verifyReference(result.coverageReport), compilation) : { valid: false, unsupported: false };
      const expected = classifyAttempt({
        reference: restoreProcess(result.reference), compilation, execution: restoreProcess(result.execution),
        scenario: result.policy.scenario,
        referenceObservables: restoreObservables(result.referenceObservables),
        jrocObservables: restoreObservables(result.jrocObservables),
        compilerReportValid: report.valid, unsupported: report.unsupported,
        compilerArtifactPresent: result.compiledArtifacts.some(file => file.path.endsWith('.dll'))
      });
      if (expected.outcome !== result.outcome || expected.phase !== result.phase) {
        throw new Error('Result classification contradicts retained paired observations.');
      }
    }
    return result;
  });
  if (new Set(attempts.map(result => result.attemptId)).size !== attempts.length) throw new Error('Duplicate attempt identity.');
  if (attempts.length !== bundle.cases.length * bundle.attemptsPerCase) throw new Error('Missing or extra execution attempts.');
  for (const summary of bundle.cases) {
    const selected = attempts.filter(result => result.scenarioId === summary.id);
    if (selected.length !== bundle.attemptsPerCase
      || new Set(selected.map(result => result.attemptNumber)).size !== bundle.attemptsPerCase
      || selected.some(result => result.attemptNumber > bundle.attemptsPerCase)
      || summary.outcome !== aggregateOutcome(selected)) throw new Error('Attempt history and case summary do not reconcile.');
  }
  const actual = workspaceFiles(directory, { workspaceFiles: 8192, workspaceBytes: limits.bundleBytes })
    .map(file => file.path).filter(file => !['bundle.json', 'bundle.sha256'].includes(file)).sort();
  if (hashJson(actual) !== hashJson([...known.keys()].sort())) throw new Error('Unregistered or missing files in bundle.');
  return bundle;
}

async function main(args) {
  const options = parseOptions(args);
  if (options.help) {
    console.log('node scripts/differential-test/run.js --node-compatibility [--manifest <json>] [--case <id>] [--attempts 1..5] [--jroc <dll>] [--output <directory>] [--allow-dirty]');
    return;
  }
  const { directory, bundle } = await runSuite(options);
  console.log(`Evidence bundle: ${directory}`);
  if (bundle.cases.some(scenario => scenario.outcome !== 'pass')) process.exitCode = 1;
}

module.exports = {
  resolveWithin, readBounded, ArtifactStore, normalize, decodeObservable, classifyAttempt,
  compilerReport, runAttempt, aggregateOutcome, parseOptions, runSuite, verifyBundle, main
};
