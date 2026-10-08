'use strict';

const assert = require('node:assert/strict');
const crypto = require('node:crypto');
const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { test } = require('node:test');
const { evaluateCase, parseArgs } = require('./runMvp');
const { parseTest262File } = require('./metadataParser');

const repository = path.resolve(__dirname, '../..');
const cliProject = path.join(repository, 'src/Cli/Jroc.csproj');
const hostProject = path.join(repository, 'scripts/test262/NativeScreeningHost/NativeScreeningHost.csproj');
function binary(project, name) {
  const filename = path.join(path.dirname(project), 'bin/Release/net10.0', name + '.dll');
  execFileSync('dotnet', ['build', project, '-c', 'Release', '--nologo', '--verbosity', 'quiet'],
    { cwd: repository, stdio: 'pipe' });
  return filename;
}
const cli = binary(cliProject, 'Jroc');
const host = binary(hostProject, 'NativeScreeningHost');

test('MVP instrumentation is opt-in and embeds the actual compilation report', () => {
  assert.equal(parseArgs([]).compilationCoverage, false);
  assert.equal(parseArgs(['--compilation-coverage']).compilationCoverage, true);
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-mvp-coverage-'));
  try {
    const file = path.join(directory, 'sample.js');
    fs.mkdirSync(path.join(directory, 'harness'));
    fs.writeFileSync(path.join(directory, 'harness/assert.js'), '');
    fs.writeFileSync(path.join(directory, 'harness/sta.js'), '');
    fs.writeFileSync(file, '/*---\ndescription: compiler-mode probe\n---*/\nvar number = 1;');
    const fixture = {
      relativePath: 'sample.js', absolutePath: file, variant: 'non-strict', metadata: parseTest262File(file),
    };
    const args = { timeoutSeconds: 5, compileTimeoutSeconds: 30 };
    const ordinary = evaluateCase(directory, path.join(directory, 'ordinary'), fixture,
      { type: 'dll', path: cli }, args);
    assert.equal(ordinary.classification.verdict, 'matched');
    assert.equal(Object.hasOwn(ordinary, 'compilation_coverage'), false);
    const measured = evaluateCase(directory, path.join(directory, 'measured'), fixture,
      { type: 'dll', path: cli }, { ...args, compilationCoverage: true });
    assert.equal(measured.classification.verdict, ordinary.classification.verdict);
    assert.equal(measured.compilation_coverage.complete, true);
    assert.ok(measured.compilation_coverage.counts.directIl > 0);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});

test('native host retains complete, partial and unavailable reports without changing correctness', () => {
  const directory = fs.mkdtempSync(path.join(os.tmpdir(), 'jroc-native-coverage-'));
  try {
    const sources = [
      ['passing.js', '/*---\ndescription: direct compilation probe\n---*/\nvar number = 1;'],
      ['unsupported.js', '/*---\ndescription: unsupported compiler feature\n---*/\neval("1");'],
      ['raw.js', '/*---\nflags: [raw]\n---*/\nvar number = 1;'],
    ];
    const candidates = sources.map(([name, source]) => {
      fs.writeFileSync(path.join(directory, name), source);
      return { path: name, sha256: crypto.createHash('sha256').update(source).digest('hex'), variants: ['non-strict'] };
    });
    const plan = path.join(directory, 'plan.json');
    fs.writeFileSync(plan, JSON.stringify({
      upstream_root: directory, timeout_ms: 5000, variant_limit: 3, time_limit_seconds: 60,
      compilation_coverage: true, candidates,
    }));
    const results = execFileSync('dotnet', [host, '--plan', plan],
      { cwd: repository, encoding: 'utf8', timeout: 90000 }).trim().split('\n').map(JSON.parse);
    assert.deepEqual(results.map(result => result.outcome), ['pass', 'fail', 'unsupported']);
    assert.equal(results[0].compilation_coverage.complete, true);
    assert.equal(results[1].compilation_coverage.complete, false);
    assert.equal(results[1].compilation_coverage.counts.unsupported, 1);
    assert.equal(Object.hasOwn(results[2], 'compilation_coverage'), false);
  } finally {
    fs.rmSync(directory, { recursive: true, force: true });
  }
});
