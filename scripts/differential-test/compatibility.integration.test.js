'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const { test } = require('node:test');
const { verifyBundle } = require('./compatibility');
const { hashJson } = require('../nodeCompatibility/artifacts');
const baseline = require('../nodeCompatibility/baseline.json');

test('real Node/public JROC executions retain positive and negative classifications', { timeout: 180000 }, async t => {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-evidence-integration-'));
  t.after(() => fs.rmSync(root, { recursive: true, force: true }));
  const examples = [
    ['pass', 'console.log("ok");', 'pass'],
    ['mismatch', 'console.log(process.argv[0]);', 'mismatch'],
    ['compile-failure', 'import "data:text/javascript,console.log(42)";', 'compile-failure'],
    ['unsupported', 'console.log(eval("1 + 1"));', 'unsupported'],
    ['execution-timeout', `if (process.argv[0] !== ${JSON.stringify(process.execPath)}) { while (true) {} } console.log("ok");`, 'execution-timeout'],
    ['reference-timeout', 'while (true) {}', 'reference-failure'],
    ['reference-failure', 'throw new Error("deliberate reference failure");', 'reference-failure'],
    ['output-limit', 'process.stdout.write("x".repeat(1048577));', 'infrastructure-failure'],
    ['applicability-unknown', 'throw new Error("must not execute");', 'not-run'],
    ['applicability-excluded', 'throw new Error("must not execute");', 'not-run']
  ];
  const cases = examples.map(([name, source]) => {
    const entry = `${name}.${name === 'compile-failure' ? 'mjs' : 'js'}`;
    fs.writeFileSync(path.join(root, entry), source);
    return {
      id: `node:scenario/runner/integration-${name}`, targets: [`node:workflow/runner/${name}`],
      applicability: {
        state: name === 'applicability-unknown' ? 'unknown' : name === 'applicability-excluded' ? 'excluded' : 'applicable',
        reason: 'Controlled harness validation, not a product-support exclusion.'
      },
      entry, files: [entry], expectedExitCode: 0,
      timeoutMs: name === 'reference-timeout' ? 500 : name === 'execution-timeout' ? 1500 : 10000,
      compileTimeoutMs: 60000,
      timeoutRationale: 'Controlled validation: host argv distinguishes the intentional product loop; not a conformance claim.',
      normalization: 'exact', observables: []
    };
  });
  const manifest = { schemaVersion: 1, suiteId: baseline.suiteId, artifactKind: 'scenario-manifest', scopeId: baseline.scopeId, corpusId: 'controlled-runner-integration-v1', referenceProfileSha256: hashJson(baseline.reference), cases };
  const manifestPath = path.join(root, 'manifest.json');
  fs.writeFileSync(manifestPath, JSON.stringify(manifest));
  const output = path.join(root, 'runs');
  const child = spawnSync(process.execPath, [
    path.join(__dirname, 'run.js'), '--node-compatibility', '--manifest', manifestPath, '--output', output, '--allow-dirty'
  ], { encoding: 'utf8', timeout: 160000, maxBuffer: 4 * 1024 * 1024 });
  assert.equal(child.error, undefined);
  assert.equal(child.status, 1, child.stdout + child.stderr);
  assert(fs.existsSync(output), child.stdout + child.stderr);
  const directories = fs.readdirSync(output);
  assert.equal(directories.length, 1);
  const directory = path.join(output, directories[0]);
  assert(fs.existsSync(path.join(directory, 'bundle.json')), child.stdout + child.stderr);
  const bundle = verifyBundle(directory);
  assert.deepEqual(bundle.cases.map(value => value.outcome), examples.map(([, , outcome]) => outcome));
  assert.deepEqual(verifyBundle(directory), bundle);
  for (const reference of bundle.results) {
    const result = JSON.parse(fs.readFileSync(path.join(directory, reference.path)));
    if (result.scenarioId.includes('reference-') || result.scenarioId.endsWith('output-limit') || result.scenarioId.includes('applicability-')) {
      assert.equal(result.compilation, null);
      assert.equal(result.execution, null);
    }
    if (result.scenarioId.includes('applicability-')) assert.equal(result.reference, null);
    assert.equal(result.provenance.trustClass, 'local-unverified');
    assert.equal(result.provenance.corpusSourceRevision, null);
  }
});
