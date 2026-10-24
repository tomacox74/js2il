'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const { test } = require('node:test');
const YAML = require('yaml');

const root = path.resolve(__dirname, '../..');
const workflow = name => YAML.parse(fs.readFileSync(path.join(root, '.github/workflows', name), 'utf8'));
const ci = workflow('dotnet.yml');
const validation = workflow('release-validation.yml');
const publication = workflow('publish-tool.yml');
const checkout = job => job.steps.find(step => step.uses?.startsWith('actions/checkout@'));

test('release validation reuses all CI shards on both operating systems', () => {
  const runners = ['windows-2025', 'macos-15'];
  assert(ci.on.workflow_call);
  assert(validation.on.workflow_call);
  assert(validation.on.workflow_dispatch !== undefined);
  assert(validation.on.pull_request.paths.includes('.github/workflows/publish-tool.yml'));
  assert.equal(validation.jobs.tests.uses, './.github/workflows/dotnet.yml');
  assert.deepEqual(JSON.parse(validation.jobs.tests.with.runners), runners);
  assert.deepEqual(validation.jobs.validate.strategy.matrix.os, ['ubuntu-latest', ...runners]);
  const matrix = ci.jobs.test.strategy.matrix;
  assert.equal(matrix.name.length, 11);
  assert.equal(new Set(matrix.name).size, 11);
  assert.deepEqual(matrix.name, matrix.include.map(shard => shard.name));
  for (const name of matrix.name) {
    const shard = matrix.include.find(item => item.name === name);
    assert(shard.project);
    assert(shard.filter || shard.shard);
  }
  assert.equal(JSON.parse(ci.on.workflow_call.inputs.runners.default)[0], 'ubuntu-latest');
  assert.match(matrix.os, /fromJSON\(inputs.runners/);
  assert.equal(ci.jobs.test.defaults.run.shell, 'bash');
  assert.equal(ci.jobs.test.env.DOTNET_Thread_DefaultStackSize, '800000');
  assert.equal(ci.jobs.test.strategy['fail-fast'], false);
  assert.equal(validation.jobs.validate.strategy['fail-fast'], false);
  assert.equal(validation.jobs.validate.steps.at(-1).run, 'npm run release:validate');
});

test('publication requires successful exact-commit OS validation without credentials in test jobs', () => {
  const resolve = publication.jobs['resolve-release'];
  const gate = publication.jobs['validate-release'];
  const publish = publication.jobs['build-and-publish'];
  assert.deepEqual(publish.needs, ['resolve-release', 'validate-release']);
  assert.equal(publish.if, undefined);
  assert.equal(publish['continue-on-error'], undefined);
  assert.equal(gate.needs, 'resolve-release');
  assert.equal(gate.uses, './.github/workflows/release-validation.yml');
  const commit = '${{ needs.resolve-release.outputs.commit }}';
  assert.equal(gate.with.ref, commit);
  assert.equal(checkout(publish).with.ref, commit);
  assert.equal(checkout(resolve).with.ref, 'refs/tags/${{ steps.tag.outputs.tag }}');
  assert.match(resolve.steps.find(step => step.id === 'commit').run, /git rev-parse HEAD/);
  assert.equal(checkout(ci.jobs.test).with.ref, '${{ inputs.ref || github.sha }}');
  assert.equal(validation.jobs.tests.with.ref, '${{ inputs.ref || github.sha }}');
  assert.equal(checkout(validation.jobs.validate).with.ref, '${{ inputs.ref || github.sha }}');
  assert.equal(gate.secrets, undefined);
  assert.deepEqual(gate.permissions, { contents: 'read' });
  assert.deepEqual(validation.permissions, { contents: 'read' });
  assert.equal(publish.permissions['id-token'], 'write');
  assert.equal(publish.env.VERSION, '${{ needs.resolve-release.outputs.version }}');
  assert(publish.steps.findIndex(step => step.uses === 'NuGet/login@v1') >
    publish.steps.findIndex(step => step.name === 'Validate coordinated release package set'));
  assert(ci.jobs.test.steps.find(step => step.uses?.startsWith('actions/upload-artifact@')).with.name.includes('matrix.os'));
});

test('release tag parsing accepts tags and rejects branches and injected output lines', t => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-release-tag-'));
  t.after(() => fs.rmSync(directory, { recursive: true, force: true }));
  const script = publication.jobs['resolve-release'].steps.find(step => step.id === 'tag').run;
  const bash = process.platform === 'win32'
    ? path.join(process.env.ProgramFiles, 'Git', 'bin', 'bash.exe')
    : 'bash';
  for (const [index, ref] of [
    'v0.9.17', 'refs/tags/v0.9.17', 'v1.2.3-rc.1', '0.9.17',
    'master', 'refs/heads/master', 'v1.2.3\nversion=injected', 'v1.2.3; echo injected'
  ].entries()) {
    const output = path.join(directory, `${index}.txt`);
    const child = spawnSync(bash, ['-e', '-o', 'pipefail', '-c', script], {
      env: { ...process.env, REQUESTED_REF: ref, GITHUB_OUTPUT: output }, encoding: 'utf8'
    });
    assert.equal(child.error, undefined);
    if (index < 4) {
      assert.equal(child.status, 0, child.stderr);
      const tag = ref.replace(/^refs\/tags\//, '');
      assert.equal(fs.readFileSync(output, 'utf8'), `tag=${tag}\nversion=${tag.replace(/^v/, '')}\n`);
    } else {
      assert.notEqual(child.status, 0);
      assert.equal(fs.existsSync(output), false);
    }
  }
});
