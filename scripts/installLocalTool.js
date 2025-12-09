#!/usr/bin/env node
/**
 * Build a Release publish of js2il and install it as the global dotnet tool.
 * Usage: node scripts/installLocalTool.js
 */

const { execSync } = require('child_process');
const path = require('path');

const repoRoot = path.resolve(__dirname, '..');
const publishDir = path.join(repoRoot, 'out_publish');

function run(cmd, opts = {}) {
  execSync(cmd, { stdio: 'inherit', cwd: repoRoot, ...opts });
}

try {
  console.log(`Publishing js2il to ${publishDir} ...`);
  run('dotnet publish Js2IL -c Release -o "' + publishDir + '"');

  console.log('Uninstalling existing global js2il (if any)...');
  try {
    run('dotnet tool uninstall js2il -g');
  } catch (err) {
    // Ignore failures (not installed)
  }

  console.log('Installing js2il from local publish directory...');
  run('dotnet tool install --global --add-source "' + publishDir + '" js2il');

  console.log('Done. Run "js2il --version" to verify.');
} catch (err) {
  console.error('Failed to build/install local js2il:', err.message || err);
  process.exit(1);
}
