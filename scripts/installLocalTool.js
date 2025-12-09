#!/usr/bin/env node
/**
 * Build a Release nupkg of js2il and install it as the global dotnet tool.
 * This script ensures the newest version is installed by:
 * 1. Using dotnet pack (not publish) to create a proper nupkg
 * 2. Removing the tool store cache to force fresh extraction
 * Usage: node scripts/installLocalTool.js
 */

const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');
const os = require('os');

const repoRoot = path.resolve(__dirname, '..');
const packDir = path.join(repoRoot, 'out_publish');
const toolStorePath = path.join(os.homedir(), '.dotnet', 'tools', '.store', 'js2il');

function run(cmd, opts = {}) {
  execSync(cmd, { stdio: 'inherit', cwd: repoRoot, ...opts });
}

function removeDir(dirPath) {
  if (fs.existsSync(dirPath)) {
    fs.rmSync(dirPath, { recursive: true, force: true });
    console.log(`Removed: ${dirPath}`);
  }
}

try {
  console.log(`Packing js2il to ${packDir} ...`);
  run('dotnet pack Js2IL -c Release -o "' + packDir + '"');

  console.log('Uninstalling existing global js2il (if any)...');
  try {
    run('dotnet tool uninstall js2il -g');
  } catch (err) {
    // Ignore failures (not installed)
  }

  // Remove tool store cache to ensure fresh install
  console.log('Clearing tool store cache...');
  removeDir(toolStorePath);

  console.log('Installing js2il from local pack directory...');
  run('dotnet tool install --global --add-source "' + packDir + '" js2il');

  console.log('Done. Run "js2il --version" to verify.');
} catch (err) {
  console.error('Failed to build/install local js2il:', err.message || err);
  process.exit(1);
}
