#!/usr/bin/env node
'use strict';

const { execFileSync } = require('node:child_process');
const fs = require('node:fs');
const os = require('node:os');
const path = require('node:path');
const { builtinModules } = require('node:module');
const { sha256, readContractEvidence } = require('./buildInventory');
const config = require('./baseline.json');

const repoRoot = path.resolve(__dirname, '../..');
const inventoryRelativePath = 'docs/nodejs/compatibility/inventory.v1.json';

function validateEnvironment(environment, baseline, { allowDirty = false } = {}) {
  if (environment.nodeArguments.length || environment.runtimeOverridesPresent.length) {
    throw new Error('Reference runtime flags or environment overrides are present; use the declared default-options profile.');
  }
  for (const [key, expected] of Object.entries({
    nodeVersion: baseline.reference.nodeVersion,
    platform: baseline.reference.platform,
    architecture: baseline.reference.architecture,
    dotnetSdk: baseline.reference.dotnetSdk
  })) {
    if (environment[key] !== expected) {
      throw new Error(`Reference ${key} mismatch: expected ${expected}, received ${environment[key]}. Select the pinned environment; rebaseline deliberately, never silently.`);
    }
  }
  const runtimeLine = baseline.reference.dotnetRuntime.split('.').slice(0, 2).join('.') + '.';
  const runtime = environment.dotnetRuntimes.filter(x => x.startsWith(runtimeLine))
    .sort((a, b) => a.localeCompare(b, 'en', { numeric: true })).at(-1);
  if (runtime !== baseline.reference.dotnetRuntime) {
    throw new Error(`Reference .NET runtime mismatch: expected ${baseline.reference.dotnetRuntime}, default patch selection is ${runtime ?? 'unavailable'}.`);
  }
  if (!/^[a-f0-9]{40}$/.test(environment.jrocRevision ?? '')) throw new Error('Missing exact JROC revision.');
  for (const key of ['inventorySha256', 'corpusRevision']) {
    if (!/^[a-f0-9]{64}$/.test(environment[key] ?? '')) throw new Error(`Missing ${key}.`);
  }
  if (!environment.lockfiles.length || environment.lockfiles.some(x => !/^[a-f0-9]{64}$/.test(x.sha256))) {
    throw new Error('Missing dependency lockfile hashes.');
  }
  const normalize = values => [...values].map(x => x.replace(/^node:/, '')).sort();
  if (JSON.stringify(normalize(environment.builtinModules)) !== JSON.stringify(normalize(baseline.builtinModules))) {
    throw new Error('Reference builtin module roster mismatch.');
  }
  if (environment.worktreeDirty && !allowDirty) {
    throw new Error('JROC working tree is dirty; commit changes or use --allow-dirty for explicitly non-baseline diagnostics.');
  }
}

function collectEnvironment() {
  const run = (file, args) => execFileSync(file, args, {
    cwd: repoRoot, encoding: 'utf8', timeout: 30000,
    stdio: ['ignore', 'pipe', 'pipe'], maxBuffer: 8 * 1024 * 1024
  }).trim();
  const bytes = fs.readFileSync(path.join(repoRoot, inventoryRelativePath));
  const inventory = JSON.parse(bytes);
  if (JSON.stringify(inventory.reference) !== JSON.stringify(config.reference)
    || inventory.scopeId !== config.scopeId || inventory.schemaVersion !== config.schemaVersion) {
    throw new Error('Inventory reference metadata does not match baseline configuration.');
  }
  if (JSON.stringify(inventory.contracts) !== JSON.stringify(readContractEvidence(repoRoot))) {
    throw new Error('Inventory contract evidence is stale. Regenerate the compatibility inventory.');
  }
  const lockfiles = run('git', ['ls-files', '-z']).split('\0')
    .filter(file => /(^|\/)(package-lock\.json|packages\.lock\.json)$/.test(file)).sort()
    .map(file => ({ path: file, sha256: sha256(fs.readFileSync(path.join(repoRoot, file))) }));
  return {
    schemaVersion: 1, scopeId: config.scopeId, recordedAt: new Date().toISOString(),
    nodeVersion: process.versions.node, nodeExecutable: process.execPath,
    nodeArguments: process.execArgv,
    runtimeOverridesPresent: [
      'NODE_OPTIONS', 'NODE_PATH', 'DOTNET_ROLL_FORWARD',
      'DOTNET_ROLL_FORWARD_TO_PRERELEASE', 'DOTNET_ADDITIONAL_DEPS',
      'DOTNET_SHARED_STORE', 'DOTNET_STARTUP_HOOKS'
    ].filter(key => process.env[key]),
    nodeVersions: process.versions, platform: process.platform, architecture: process.arch,
    kernelVersion: os.release(),
    dotnetSdk: run('dotnet', ['--version']),
    dotnetRuntimes: run('dotnet', ['--list-runtimes']).split('\n')
      .filter(x => x.startsWith('Microsoft.NETCore.App ')).map(x => x.split(' ')[1]),
    jrocRevision: run('git', ['rev-parse', 'HEAD']),
    worktreeDirty: run('git', ['status', '--porcelain', '--untracked-files=normal']).length > 0,
    inventorySha256: sha256(bytes),
    corpusRevision: sha256(JSON.stringify(inventory.behaviors)),
    executableBehaviorCases: 0,
    baselineConfigSha256: sha256(fs.readFileSync(path.join(__dirname, 'baseline.json'))),
    lockfiles,
    builtinModules: builtinModules.filter(x => !x.startsWith('_'))
  };
}

function main(args) {
  if (args.some(x => x !== '--allow-dirty')) throw new Error('Usage: node checkReference.js [--allow-dirty]');
  if (process.versions.node !== config.reference.nodeVersion) {
    throw new Error(`Reference nodeVersion mismatch: expected ${config.reference.nodeVersion}, received ${process.versions.node}.`);
  }
  const environment = collectEnvironment();
  validateEnvironment(environment, config, { allowDirty: args.includes('--allow-dirty') });
  console.log(JSON.stringify({
    ...environment, referenceEnvironmentEligible: !environment.worktreeDirty,
    referenceSourceRevision: config.reference.sourceRevision
  }, null, 2));
}

if (require.main === module) {
  try {
    main(process.argv.slice(2));
  } catch (error) {
    console.error(error.message);
    process.exitCode = 1;
  }
}

module.exports = { validateEnvironment, collectEnvironment };
