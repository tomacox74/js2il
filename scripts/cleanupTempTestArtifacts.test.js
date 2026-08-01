#!/usr/bin/env node
"use strict";

const assert = require('node:assert/strict');
const fs = require('node:fs/promises');
const os = require('node:os');
const path = require('node:path');
const { spawnSync } = require('node:child_process');
const test = require('node:test');

const scriptPath = path.join(__dirname, 'cleanupTempTestArtifacts.js');
const oneDayAgo = new Date(Date.now() - 48 * 60 * 60 * 1000);

async function createArtifact(root, parts, modifiedAt) {
  const artifactPath = path.join(root, ...parts);
  await fs.mkdir(artifactPath, { recursive: true });
  await fs.writeFile(path.join(artifactPath, 'artifact.dll'), 'test artifact');
  await fs.utimes(artifactPath, modifiedAt, modifiedAt);
  return artifactPath;
}

test('removes old individual test artifacts while preserving recent runs', async () => {
  const tempRoot = await fs.mkdtemp(path.join(os.tmpdir(), 'jroc-cleanup-test-'));

  try {
    const oldJrocRun = await createArtifact(
      tempRoot,
      ['Jroc.Tests', 'Array.ExecutionTests', 'old-run'],
      oneDayAgo);
    const recentJrocRun = await createArtifact(
      tempRoot,
      ['Jroc.Tests', 'Array.ExecutionTests', 'recent-run'],
      new Date());
    const oldTest262Run = await createArtifact(
      tempRoot,
      ['Jroc.Test262.Tests', 'CompilationFailure', 'old-run'],
      oneDayAgo);

    const result = spawnSync(
      process.execPath,
      [scriptPath, '--apply', '--older-than-hours', '24'],
      {
        encoding: 'utf8',
        env: { ...process.env, TEMP: tempRoot, TMP: tempRoot },
      });

    assert.equal(result.status, 0, result.stderr);
    await assert.rejects(fs.access(oldJrocRun));
    await assert.rejects(fs.access(oldTest262Run));
    await fs.access(recentJrocRun);
  } finally {
    await fs.rm(tempRoot, { recursive: true, force: true });
  }
});
