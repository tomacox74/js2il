#!/usr/bin/env node
/*
  updateVerifiedFiles.js
  Port of scripts/Update-VerifiedFiles.ps1

  Behavior:
  - Recursively find all files matching *.received.* under a root (default: CWD)
  - For each, create/update the corresponding *.verified.* file alongside it
    using first occurrence replacement of ".received." -> ".verified."
    with a fallback for names ending with .received.<ext>
  - Creates target directories as needed
*/

const fs = require('fs');
const fsp = require('fs/promises');
const path = require('path');

function parseArgs(argv) {
  const args = { root: process.cwd(), quiet: false, help: false };
  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];
    if (a === '--root' || a === '--verifyRoot' || a === '-r') {
      args.root = argv[++i] || args.root;
    } else if (a === '--quiet' || a === '-q') {
      args.quiet = true;
    } else if (a === '--help' || a === '-h') {
      args.help = true;
    } else {
      // Allow passing a positional root
      if (!a.startsWith('-') && !args._posRoot) {
        args._posRoot = a;
        args.root = a;
      }
    }
  }
  return args;
}

function printHelp() {
  const msg = `Usage: node scripts/updateVerifiedFiles.js [options]

Options:
  --root, -r <dir>         Root directory to search (default: CWD)
  --verifyRoot <dir>       Alias for --root
  --quiet, -q              Minimal output
  --help, -h               Show help
`;
  process.stdout.write(msg);
}

async function exists(p) {
  try {
    await fsp.access(p, fs.constants.F_OK);
    return true;
  } catch {
    return false;
  }
}

async function* walk(dir) {
  const entries = await fsp.readdir(dir, { withFileTypes: true });
  for (const entry of entries) {
    const fullPath = path.join(dir, entry.name);
    if (entry.isDirectory()) {
      // Skip common directories we don't need to scan
      if (entry.name === '.git' || entry.name === 'bin' || entry.name === 'obj' || entry.name === 'node_modules') {
        continue;
      }
      yield* walk(fullPath);
    } else if (entry.isFile()) {
      yield fullPath;
    }
  }
}

function toVerifiedPath(fullPath) {
  const fileName = path.basename(fullPath);
  if (fileName.includes('.received.')) {
    // Replace only the first occurrence in the full path to keep directory parts intact
    return fullPath.replace('.received.', '.verified.');
  }
  // Fallback: ends with .received.<ext>
  const m = fileName.match(/\.received(\.[A-Za-z0-9]+)$/);
  if (m) {
    // Replace at the end of the file name; reconstruct full path
    const dir = path.dirname(fullPath);
    const baseNoSuffix = fileName.replace(/\.received(\.[A-Za-z0-9]+)$/, '.verified$1');
    return path.join(dir, baseNoSuffix);
  }
  return null;
}

async function main() {
  const args = parseArgs(process.argv);
  if (args.help) {
    printHelp();
    return 0;
  }

  const root = path.resolve(args.root);
  if (!(await exists(root))) {
    console.error(`Root path not found: ${root}`);
    return 1;
  }

  // Collect candidate files first
  const receivedFiles = [];
  for await (const file of walk(root)) {
    const base = path.basename(file);
    if (base.includes('.received.') || /\.received\.[A-Za-z0-9]+$/.test(base)) {
      receivedFiles.push(file);
    }
  }

  if (receivedFiles.length === 0) {
    if (!args.quiet) console.log(`No received files found in ${root}`);
    return 0;
  }

  for (const src of receivedFiles) {
    const verified = toVerifiedPath(src);
    if (!verified || verified === src) {
      if (!args.quiet) console.log(`Skipping (no transform): ${path.basename(src)}`);
      continue;
    }
    await fsp.mkdir(path.dirname(verified), { recursive: true });
    await fsp.copyFile(src, verified);
    if (!args.quiet) console.log(`Updated: ${path.basename(verified)}`);
  }

  if (!args.quiet) console.log('✅ All verified files updated from received files.');
  return 0;
}

main().then((code) => {
  if (typeof code === 'number' && code !== 0) process.exit(code);
}).catch((err) => {
  console.error(err?.stack || String(err));
  process.exit(1);
});
