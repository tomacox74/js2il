#!/usr/bin/env node
"use strict";

/**
 * Build a Release nupkg of js2il and install it as either:
 * 1. The global dotnet tool (default)
 * 2. A local isolated tool path (`--tool-path`)
 *
 * This script ensures the newest version is installed by:
 * 1. Using dotnet pack (not publish) to create a proper nupkg
 * 2. Removing the target install location before installing
 * 3. Removing the global tool store cache before global installs
 *
 * Usage:
 *   node scripts/installLocalTool.js
 *   node scripts/installLocalTool.js --tool-path artifacts/local-js2il
 */
const {
  defaultPackDir,
  installPackagedTool,
  packToolPackage,
  resolveRepoPath,
} = require('./localToolUtils');

function parseArgs(argv) {
  const args = {
    packDir: defaultPackDir,
    toolPath: null,
  };

  for (let i = 2; i < argv.length; i++) {
    const arg = argv[i];

    if (arg === '--pack-dir' && argv[i + 1]) {
      args.packDir = resolveRepoPath(argv[++i]);
      continue;
    }

    if (arg === '--tool-path' && argv[i + 1]) {
      args.toolPath = resolveRepoPath(argv[++i]);
      continue;
    }

    if (arg === '--help' || arg === '-h') {
      console.log('Usage: node scripts/installLocalTool.js [--pack-dir <dir>] [--tool-path <dir>]');
      process.exit(0);
    }

    throw new Error(`Unknown arg: ${arg}`);
  }

  return args;
}

try {
  const args = parseArgs(process.argv);
  const packDir = packToolPackage({ packDir: args.packDir });
  const installResult = installPackagedTool({ packDir, toolPath: args.toolPath });

  console.log(`Done. Run "${installResult.executable} --version" to verify.`);
} catch (err) {
  console.error('Failed to build/install local js2il:', err.message || err);
  process.exit(1);
}
