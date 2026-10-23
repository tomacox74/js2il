'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const { test } = require('node:test');
const { createReport, markdown } = require('./reportBehaviorCorpus');
const { directory } = require('./buildBehaviorCorpus');

test('public corpus validates every reference and retains gaps without inflating target/attempt counts', { timeout: 180000 }, t => {
  const root = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-public-api-integration-'));
  t.after(() => fs.rmSync(root, { recursive: true, force: true }));
  const output = path.join(root, 'runs');
  const child = spawnSync(process.execPath, [
    path.resolve(__dirname, '../differential-test/run.js'), '--node-compatibility',
    '--manifest', path.join(directory, 'manifest.json'), '--output', output, '--attempts', '2', '--allow-dirty'
  ], { encoding: 'utf8', timeout: 160000, maxBuffer: 4 * 1024 * 1024 });
  assert.equal(child.error, undefined);
  assert([0, 1].includes(child.status), child.stdout + child.stderr);
  assert(fs.existsSync(output), child.stdout + child.stderr);
  const runs = fs.readdirSync(output);
  assert.equal(runs.length, 1);
  const report = createReport(path.join(output, runs[0]));
  assert.equal(report.selectedScenarios, 19);
  assert.equal(report.attempts, 38);
  assert.equal(Object.values(report.outcomes).reduce((sum, count) => sum + count, 0), 19);
  assert.equal(report.outcomes['reference-failure'], 0);
  assert.equal(report.outcomes['infrastructure-failure'], 0);
  assert.equal(report.outcomes['inconclusive'], 0);
  assert.equal(child.status, report.outcomes.pass === 19 ? 0 : 1);
  assert.equal(report.inventory.mappedApiCandidates.length + report.inventory.unmappedApiCandidates.length,
    report.inventory.totalApiCandidates);
  assert.equal(report.inventory.mappedLoaderBehaviors.length, 8);
  assert(report.cases.reduce((sum, item) => sum + item.targets.length, 0) > 19);
  for (const item of report.cases) {
    assert.equal(item.attempts.length, 2);
    for (const attempt of item.attempts) {
      assert.equal(attempt.reference.state, 'completed');
      assert.equal(attempt.reference.exitCode, 0);
      assert.equal(attempt.reference.stdout.base64, '');
      assert.equal(attempt.reference.stderr.base64, '');
      assert(attempt.referenceObservables.every(observation => observation.error === null && observation.artifact));
    }
  }
  assert(markdown(report).includes('not an overall Node compatibility score'));
  assert.deepEqual(JSON.parse(JSON.stringify(report)), report);
  const bundlePath = path.join(output, runs[0], 'bundle.json');
  fs.appendFileSync(bundlePath, ' ');
  assert.throws(() => createReport(path.join(output, runs[0])), /checksum/);
});
