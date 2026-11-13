'use strict';

const assert = require('node:assert/strict');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const { test } = require('node:test');
const vm = require('node:vm');
const { createHash } = require('node:crypto');
const YAML = require('yaml');
const { verifyReleaseCandidate } = require('./verifyReleaseCandidate');

const root = path.resolve(__dirname, '../..');
const workflow = name => YAML.parse(fs.readFileSync(path.join(root, '.github/workflows', name), 'utf8'));
const ci = workflow('dotnet.yml');
const validation = workflow('release-validation.yml');
const publication = workflow('publish-tool.yml');
const checkout = job => job.steps.find(step => step.uses?.startsWith('actions/checkout@'));

test('SDK 11 candidate preserves exact-source producer artifacts across consumers and retries', () => {
  const trial = workflow('sdk11-package-validation.yml');
  assert(trial.on.workflow_dispatch !== undefined);
  assert(trial.on.workflow_call);
  assert.equal(trial.on.workflow_call.outputs.artifact_id.value, '${{ jobs.package.outputs.artifact_id }}');
  assert.deepEqual(trial.permissions, { contents: 'read' });
  const pack = trial.jobs.package;
  assert.equal(pack.steps.find(step => step.uses?.startsWith('actions/setup-dotnet@')).with['dotnet-version'], '11.0.x');
  assert.equal(checkout(pack).with.ref, '${{ inputs.ref || github.sha }}');
  const buildStep = pack.steps.find(step => step.id === 'package');
  assert.equal(buildStep.env.REQUESTED_PACKAGE_VERSION, '${{ inputs.package_version }}');
  assert(buildStep.run.includes('${REQUESTED_PACKAGE_VERSION:-'));
  const build = pack.steps.find(step => step.id === 'package').run;
  for (const project of ['src/Cli/Jroc.csproj', 'src/Jroc.Core/Jroc.Core.csproj',
    'src/JavaScriptRuntime/JavaScriptRuntime.csproj', 'src/Jroc.SDK/Jroc.SDK.csproj']) {
    assert(build.includes(project));
  }
  assert.match(build, /dotnet build jroc\.sln/);
  assert.match(build, /--no-build --no-restore/);
  assert.match(build, /git rev-parse HEAD/);
  const artifact = pack.steps.find(step => step.uses?.startsWith('actions/upload-artifact@'));
  assert.equal(artifact.id, 'candidate');
  assert.equal(pack.outputs.artifact_id, '${{ steps.candidate.outputs.artifact-id }}');
  for (const name of ['cli', 'sdk']) {
    const consumer = trial.jobs[name];
    assert.equal(consumer.needs, 'package');
    assert.equal(checkout(consumer).with.ref, '${{ inputs.ref || github.sha }}');
    const download = consumer.steps.find(step => step.uses?.startsWith('actions/download-artifact@'));
    assert.equal(download.with['artifact-ids'], '${{ needs.package.outputs.artifact_id }}');
    assert.equal(download.with.name, undefined);
    assert.equal(consumer['continue-on-error'], undefined);
  }
  assert.deepEqual(trial.jobs.cli.strategy.matrix, {
    runner: ['ubuntu-latest', 'windows-latest'], major: [10, 11],
  });
  const cli = trial.jobs.cli.steps.find(step => step.run?.includes('smokePackagedCliRuntime.ps1')).run;
  assert(cli.includes('${{ needs.package.outputs.package_version }}'));
  const sdk = trial.jobs.sdk.steps.find(step => step.env?.JROC_SDK_TEST_FEED);
  assert.equal(sdk.env.JROC_SDK_TEST_PACKAGE_VERSION, '${{ needs.package.outputs.package_version }}');
  assert.equal(sdk.env.JROC_SDK_TEST_SOURCE_SHA, '${{ needs.package.outputs.source_sha }}');
  assert.match(sdk.run, /FullyQualifiedName=Jroc.Tests.JrocSdkPackageTests.Build_ExtractedBasicSample_WithLocalJrocSdkPackage_CompilesAndRuns/);
  const pin = spawnSync('git', ['show', 'HEAD:global.json'], { cwd: root, encoding: 'utf8' });
  assert.equal(pin.status, 0, pin.stderr);
  assert.equal(JSON.parse(pin.stdout).sdk.version, '10.0.100');
  assert(pack.steps.some(step => step.run?.includes('bash scripts/ci/selectReleaseSdk.sh')));
});

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
  assert.equal(validation.jobs.candidate.uses, './.github/workflows/sdk11-package-validation.yml');
  assert.equal(validation.jobs.candidate.with.ref, '${{ inputs.ref || github.sha }}');
  assert.equal(validation.jobs.candidate.with.package_version, "${{ inputs.package_version || '' }}");
  assert.equal(validation.on.workflow_call.outputs.artifact_id.value, '${{ jobs.candidate.outputs.artifact_id }}');
  const setup = validation.jobs.validate.steps.find(step => step.uses?.startsWith('actions/setup-dotnet@'));
  assert.deepEqual(setup.with['dotnet-version'].trim().split('\n'), ['10.0.x', '11.0.x']);
  assert(validation.jobs.validate.steps.some(step => step.run === 'bash scripts/ci/selectReleaseSdk.sh'));
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
  assert.equal(gate.with.package_version, '${{ needs.resolve-release.outputs.version }}');
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
  assert.equal(publish.env.RELEASE_SOURCE_SHA, commit);
  assert.equal(publish.steps.find(step => step.uses?.startsWith('actions/setup-dotnet@')).with['dotnet-version'], '11.0.x');
  assert(publish.steps.some(step => step.run === 'bash scripts/ci/selectReleaseSdk.sh'));
  assert.equal(publish.steps.find(step => step.uses?.startsWith('actions/download-artifact@')).with['artifact-ids'],
    '${{ needs.validate-release.outputs.artifact_id }}');
  const verification = publish.steps.findIndex(step => step.run === 'node scripts/ci/verifyReleaseCandidate.js candidate');
  assert(verification >= 0);
  assert(publish.steps.findIndex(step => step.uses === 'NuGet/login@v1') > verification);
  assert(!publish.steps.some(step => /dotnet (build|pack)\b/.test(step.run || '')));
  assert.match(publish.steps.at(-1).run, /Join-Path 'candidate\/feed'/);
  assert(ci.jobs.test.steps.find(step => step.uses?.startsWith('actions/upload-artifact@')).with.name.includes('matrix.os'));
});

test('release SDK selection pins stable SDK 11 despite the SDK 10 repository pin and newer installed SDKs', t => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-release-sdk-'));
  t.after(() => fs.rmSync(directory, { recursive: true, force: true }));
  const tools = path.join(directory, 'tools');
  fs.mkdirSync(tools);
  fs.writeFileSync(path.join(tools, 'dotnet'), `#!/usr/bin/env node
if (process.argv[2] === '--list-sdks') process.stdout.write(process.env.SDK_LIST);
else if (process.argv[2] === '--version') console.log(process.env.RESOLVED_SDK || JSON.parse(require('node:fs').readFileSync('global.json', 'utf8')).sdk.version);
else process.exit(1);
`, { mode: 0o755 });
  const bash = process.platform === 'win32' ? path.join(process.env.ProgramFiles, 'Git', 'bin', 'bash.exe') : 'bash';
  const script = fs.readFileSync(path.join(__dirname, 'selectReleaseSdk.sh'), 'utf8');
  const globalPath = path.join(directory, 'global.json');
  for (const [name, sdks, resolved] of [
    ['stable', '10.0.404 [sdk]\n11.0.100 [sdk]\n11.0.102 [sdk]\n12.0.100 [sdk]\n11.0.200-preview.1 [sdk]\n', ''],
    ['missing', '10.0.404 [sdk]\n11.0.100-preview.1 [sdk]\n', ''],
    ['resolver-mismatch', '11.0.102 [sdk]\n', '10.0.404'],
  ]) {
    fs.writeFileSync(globalPath, '{"sdk":{"version":"10.0.100"}}\n');
    const child = spawnSync(bash, ['-c', script], {
      cwd: directory, encoding: 'utf8',
      env: { ...process.env, PATH: `${tools}${path.delimiter}${process.env.PATH}`, SDK_LIST: sdks, RESOLVED_SDK: resolved },
    });
    assert.equal(child.error, undefined);
    if (name === 'stable') {
      assert.equal(child.status, 0, child.stderr);
      assert.deepEqual(JSON.parse(fs.readFileSync(globalPath, 'utf8')).sdk,
        { version: '11.0.102', rollForward: 'disable', allowPrerelease: false });
    } else {
      assert.notEqual(child.status, 0);
      assert.match(child.stderr, name === 'missing' ? /require a stable .NET 11 SDK/ : /Expected release SDK 11.0.102, resolved 10.0.404/);
      if (name === 'missing') assert.equal(JSON.parse(fs.readFileSync(globalPath, 'utf8')).sdk.version, '10.0.100');
    }
  }
});

function releaseCandidate(t) {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-release-candidate-'));
  t.after(() => fs.rmSync(directory, { recursive: true, force: true }));
  fs.mkdirSync(path.join(directory, 'feed'));
  const source = 'a'.repeat(40);
  const version = '1.2.3-rc.1';
  const packages = ['jroc', 'Jroc.Core', 'Jroc.Runtime', 'Jroc.SDK'].map(id => `${id}.${version}.nupkg`);
  const hashes = packages.map(name => {
    const contents = Buffer.from(`validated package ${name}`);
    fs.writeFileSync(path.join(directory, 'feed', name), contents);
    return `${createHash('sha256').update(contents).digest('hex')}  ${name}`;
  });
  for (const [name, value] of [['source-sha.txt', source], ['package-version.txt', version],
    ['build-sdk.txt', '11.0.100'], ['packages.sha256', hashes.join('\n')]]) {
    fs.writeFileSync(path.join(directory, name), `${value}\n`);
  }
  return { directory, source, version, packages };
}

test('publication candidate verification accepts all four unchanged packages from the exact SDK 11 source and version', t => {
  const candidate = releaseCandidate(t);
  assert.deepEqual(verifyReleaseCandidate(candidate.directory, candidate.source, candidate.version),
    { source: candidate.source, version: candidate.version, sdk: '11.0.100', packages: candidate.packages });
});

test('publication candidate verification rejects wrong source, release version, and unstable or old SDKs', t => {
  const candidate = releaseCandidate(t);
  assert.throws(() => verifyReleaseCandidate(candidate.directory, undefined, candidate.version), /requires an artifact directory, exact source SHA/);
  assert.throws(() => verifyReleaseCandidate(candidate.directory, 'b'.repeat(40), candidate.version), /does not match release source/);
  assert.throws(() => verifyReleaseCandidate(candidate.directory, candidate.source, '1.2.4'), /does not match release version/);
  for (const sdk of ['10.0.404', '11.0.100-preview.1', '12.0.100']) {
    fs.writeFileSync(path.join(candidate.directory, 'build-sdk.txt'), sdk);
    assert.throws(() => verifyReleaseCandidate(candidate.directory, candidate.source, candidate.version), /must be built with stable SDK 11/);
  }
});

test('publication candidate verification rejects modified, missing, or unmanifested packages', t => {
  const candidate = releaseCandidate(t);
  const packagePath = path.join(candidate.directory, 'feed', candidate.packages[3]);
  fs.appendFileSync(packagePath, 'modified after validation');
  assert.throws(() => verifyReleaseCandidate(candidate.directory, candidate.source, candidate.version), /does not match its validated SHA-256 manifest/);
  fs.unlinkSync(packagePath);
  assert.throws(() => verifyReleaseCandidate(candidate.directory, candidate.source, candidate.version), /ENOENT/);
  const manifestPath = path.join(candidate.directory, 'packages.sha256');
  const lines = fs.readFileSync(manifestPath, 'utf8').trim().split('\n');
  fs.writeFileSync(manifestPath, lines.slice(1).join('\n'));
  assert.throws(() => verifyReleaseCandidate(candidate.directory, candidate.source, candidate.version), /jroc.*does not match/);
  fs.writeFileSync(manifestPath, [...lines, lines[0]].join('\n'));
  assert.throws(() => verifyReleaseCandidate(candidate.directory, candidate.source, candidate.version), /invalid or duplicate entry/);
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

const releaseConsumers = [
  ['linux-smoke.yml', 'smoke'],
  ['windows-smoke.yml', 'smoke'],
  ['benchmarkdotnet-suite.yml', 'benchmarkdotnet-suite'],
  ['mitata-suite.yml', 'mitata-suite'],
];

function publicationRun(id, version, status = 'completed', conclusion = 'success', event = 'push') {
  return {
    id,
    path: '.github/workflows/publish-tool.yml',
    head_branch: event === 'push' ? `v${version}` : 'master',
    display_title: `Publish coordinated packages - ${event} - v${version}`,
    status,
    conclusion,
  };
}

function executePublicationWait(filename, job, runsAt, repositoryRunsAt = runsAt) {
  const step = workflow(filename).jobs[job].steps.find(item =>
    item.name.toLowerCase() === 'wait for published jroc packages');
  const messages = [];
  let elapsedMs = 0;
  let polls = 0;
  const getRuns = source => {
    polls++;
    return source(elapsedMs, polls);
  };
  const completion = vm.runInNewContext(`(async () => { ${step.with.script} })()`, {
    github: {
      request: async () => ({ data: { workflow_runs: getRuns(repositoryRunsAt).slice(0, 50) } }),
      paginate: async (route, parameters) => {
        assert.equal(route, 'GET /repos/{owner}/{repo}/actions/workflows/{workflow_id}/runs');
        assert.equal(parameters.owner, 'test-owner');
        assert.equal(parameters.repo, 'test-repo');
        assert.equal(parameters.workflow_id, 'publish-tool.yml');
        return getRuns(runsAt);
      },
    },
    context: { repo: { owner: 'test-owner', repo: 'test-repo' } },
    core: { info: message => messages.push(message) },
    process: { env: { TOOL_VERSION: '0.12.35', JROC_PACKAGE_VERSION: '0.12.35' } },
    Date: { now: () => elapsedMs },
    setTimeout: (resolve, milliseconds) => {
      elapsedMs += milliseconds;
      resolve();
    },
  });
  return { completion, messages, get elapsedMs() { return elapsedMs; }, get polls() { return polls; } };
}

for (const [filename, job] of releaseConsumers) {
  test(`${filename}: finds an older publication despite a busy repository and paginated history`, async () => {
    const olderPublication = publicationRun(41785830082, '0.12.35');
    const recentPublications = Array.from({ length: 100 }, (_, index) =>
      publicationRun(index, `0.13.${index}`));
    const recentRepositoryRuns = Array.from({ length: 50 }, (_, index) => ({
      id: index,
      path: '.github/workflows/dotnet.yml',
      display_title: 'CI',
      status: 'completed',
      conclusion: 'success',
    }));
    const execution = executePublicationWait(filename, job,
      () => [...recentPublications, olderPublication], () => recentRepositoryRuns);

    await execution.completion;
    assert.equal(execution.polls, 1);
    assert(execution.messages.some(message => message.includes('run 41785830082; proceeding')));
  });

  test(`${filename}: waits for cross-platform validation and subsequent publication`, async () => {
    const publishAfterMs = 95 * 60 * 1000;
    const execution = executePublicationWait(filename, job, elapsedMs => [
      elapsedMs >= publishAfterMs
        ? publicationRun(42, '0.12.35')
        : publicationRun(42, '0.12.35', 'in_progress', null),
    ]);

    await execution.completion;
    assert(execution.elapsedMs >= publishAfterMs);
    assert(execution.messages.some(message => message.includes('run 42 to complete')));
    assert(execution.messages.some(message => message.includes('run 42; proceeding')));
  });

  test(`${filename}: requires the exact release and accepts a successful manual publication retry`, async () => {
    const execution = executePublicationWait(filename, job, (_, poll) => [
      publicationRun(43, '0.12.350'),
      ...(poll > 1 ? [publicationRun(44, '0.12.35', 'completed', 'success', 'workflow_dispatch')] : []),
      publicationRun(42, '0.12.35', 'completed', 'failure'),
    ]);

    await execution.completion;
    assert.equal(execution.polls, 2);
    assert(execution.messages.some(message => message.includes('run 42 concluded failure')));
    assert(execution.messages.some(message => message.includes('run 44; proceeding')));
    assert(!execution.messages.some(message => message.includes('run 43; proceeding')));
  });

  test(`${filename}: fails explicitly if publication never succeeds`, async () => {
    const execution = executePublicationWait(filename, job, () => [
      publicationRun(42, '0.12.35', 'completed', 'failure'),
    ]);

    await assert.rejects(execution.completion,
      /Timed out waiting for a successful publish-tool\.yml run for v0\.12\.35/);
    assert(execution.messages.some(message => message.includes('run 42 concluded failure')));
    assert(!execution.messages.some(message => message.includes('; proceeding')));
  });
}
