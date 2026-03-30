#!/usr/bin/env node
'use strict';

const fs = require('fs');
const os = require('os');
const path = require('path');
const { spawnSync } = require('child_process');
const {
  installPackagedTool,
  packToolPackage,
  repoRoot,
  resolveRepoPath,
} = require('../localToolUtils');

function extractArgs(argv) {
  let output = null;
  const forwarded = [];

  for (let i = 2; i < argv.length; i++) {
    const arg = argv[i];

    if ((arg === '--suite' || arg === '-s' || arg === '--timeout' || arg === '-t' || arg === '--compile-timeout') && argv[i + 1]) {
      forwarded.push(arg, argv[++i]);
      continue;
    }

    if (arg.startsWith('--suite=') || arg.startsWith('--timeout=') || arg.startsWith('--compile-timeout=')) {
      forwarded.push(arg);
      continue;
    }

    if ((arg === '--output' || arg === '-o') && argv[i + 1]) {
      output = argv[++i];
      continue;
    }

    if (arg.startsWith('--output=')) {
      output = arg.substring('--output='.length);
      continue;
    }

    if ((arg === '--js2il' || arg === '-j') && argv[i + 1]) {
      i++;
      continue;
    }

    if (arg.startsWith('--js2il=')) {
      continue;
    }

    if (arg === '--verbose' || arg === '-v' || arg === '--help' || arg === '-h') {
      forwarded.push(arg);
      continue;
    }

    if (!arg.startsWith('-') && !output) {
      output = arg;
      continue;
    }

    forwarded.push(arg);
  }

  return { output, forwarded };
}

function getOutputRoot(requestedOutput) {
  if (requestedOutput) {
    const resolved = resolveRepoPath(requestedOutput);
    fs.mkdirSync(resolved, { recursive: true });
    return resolved;
  }

  return fs.mkdtempSync(path.join(os.tmpdir(), 'js2il-packed-canary-'));
}

function main() {
  const { output, forwarded } = extractArgs(process.argv);
  const npmOutput = typeof process.env.npm_config_output === 'string' &&
    process.env.npm_config_output.trim() &&
    process.env.npm_config_output !== 'true' &&
    process.env.npm_config_output !== 'false'
    ? process.env.npm_config_output.trim()
    : null;
  const outputRoot = getOutputRoot(output || npmOutput);
  const packDir = path.join(outputRoot, 'packed-tool-package');
  const toolPath = path.join(outputRoot, 'packed-tool');

  console.log(`Using canary artifact root: ${outputRoot}`);

  const resolvedPackDir = packToolPackage({ packDir });
  const installResult = installPackagedTool({ packDir: resolvedPackDir, toolPath });

  console.log(`Running canary suite against packaged tool: ${installResult.executable}`);

  const runnerPath = path.join(__dirname, 'runCanarySuites.js');
  const result = spawnSync(
    process.execPath,
    [
      runnerPath,
      ...forwarded,
      '--output',
      outputRoot,
      '--js2il',
      installResult.executable,
    ],
    {
      cwd: repoRoot,
      stdio: 'inherit',
      env: process.env,
    }
  );

  if (result.error) {
    throw result.error;
  }

  if (typeof result.status === 'number') {
    process.exit(result.status);
  }

  process.exit(1);
}

try {
  main();
} catch (err) {
  console.error(`Failed to run packaged canary suite: ${err.message || err}`);
  process.exit(1);
}
