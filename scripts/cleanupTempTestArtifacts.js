#!/usr/bin/env node
"use strict";

/*
  cleanupTempTestArtifacts.js

  Cleans up JS2IL test artifacts left behind in the OS temp directory.

  Safety goals:
  - Only touches entries under os.tmpdir()
  - Only considers known JS2IL test prefixes and directories used by the test suite
  - Dry-run by default; pass --apply to actually delete
  - Skips items modified recently (default: last 24 hours)

  Examples:
    node scripts/cleanupTempTestArtifacts.js                 # dry-run
    node scripts/cleanupTempTestArtifacts.js --apply         # delete
    node scripts/cleanupTempTestArtifacts.js --apply --older-than-hours 4
*/

const fs = require('fs');
const fsp = require('fs/promises');
const os = require('os');
const path = require('path');

function usage() {
  process.stdout.write(`Usage: node scripts/cleanupTempTestArtifacts.js [options]

Options:
  --apply                 Actually delete files/dirs (default: dry-run)
  --older-than-hours <n>  Only delete items not modified in the last N hours (default: 24)
  --quiet, -q             Minimal output
  --help, -h              Show help
`);
}

function parseArgs(argv) {
  const args = {
    apply: false,
    olderThanHours: 24,
    quiet: false,
    help: false,
  };

  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];
    if (a === '--help' || a === '-h') args.help = true;
    else if (a === '--apply') args.apply = true;
    else if (a === '--older-than-hours') {
      const v = Number(argv[++i]);
      if (!Number.isFinite(v) || v < 0) throw new Error(`Invalid --older-than-hours: ${v}`);
      args.olderThanHours = v;
    } else if (a === '--quiet' || a === '-q') args.quiet = true;
    else throw new Error(`Unknown arg: ${a}`);
  }

  return args;
}

function logInfo(args, msg) {
  if (!args.quiet) process.stdout.write(msg + '\n');
}

function formatBytes(bytes) {
  const units = ['B', 'KiB', 'MiB', 'GiB', 'TiB'];
  let b = bytes;
  let u = 0;
  while (b >= 1024 && u < units.length - 1) {
    b /= 1024;
    u++;
  }
  return `${b.toFixed(u === 0 ? 0 : 2)} ${units[u]}`;
}

async function exists(p) {
  try {
    await fsp.access(p, fs.constants.F_OK);
    return true;
  } catch {
    return false;
  }
}

async function getMtimeMs(p) {
  try {
    const st = await fsp.lstat(p);
    return st.mtimeMs;
  } catch {
    return null;
  }
}

async function getSizeBytes(root) {
  let total = 0;
  const stack = [root];

  while (stack.length > 0) {
    const p = stack.pop();
    let st;
    try {
      st = await fsp.lstat(p);
    } catch {
      continue;
    }

    if (st.isFile()) {
      total += st.size;
      continue;
    }

    if (st.isDirectory()) {
      let entries;
      try {
        entries = await fsp.readdir(p, { withFileTypes: true });
      } catch {
        continue;
      }
      for (const e of entries) {
        stack.push(path.join(p, e.name));
      }
    }
  }

  return total;
}

function isTopLevelCandidateName(name) {
  // Roots created by .NET tests
  if (name === 'Js2IL.Tests') return true;
  if (name.startsWith('Js2IL.Tests.')) return true; // e.g. Js2IL.Tests.ILVerification
  if (name === 'js2il-tests') return true;

  // Per-test roots created by CLI tests
  if (name.startsWith('js2il_cli_test_')) return true;

  // JS execution fixtures that create their own tmp artifacts
  const prefixes = [
    'js2il-',
    'test-readfile-callback-',
    'test-writefile-callback-',
    'test-stat-callback-',
    'test-realpath-callback-',
    'test-readdir-callback-',
    'test-copyfile-src-',
    'test-copyfile-dst-',
    'test-copyfile-callback-src-',
    'test-copyfile-callback-dst-',
  ];
  return prefixes.some(p => name.startsWith(p));
}

function isManagedRootName(name) {
  // For these roots we clean their children (not just the root itself)
  return name === 'Js2IL.Tests' || name.startsWith('Js2IL.Tests.') || name === 'js2il-tests';
}

async function listTopLevelCandidates(tempRoot) {
  const entries = await fsp.readdir(tempRoot, { withFileTypes: true });
  const out = [];
  for (const e of entries) {
    if (!isTopLevelCandidateName(e.name)) continue;
    out.push({ name: e.name, fullPath: path.join(tempRoot, e.name), dirent: e });
  }
  return out;
}

async function listManagedRootChildren(rootPath) {
  try {
    const entries = await fsp.readdir(rootPath, { withFileTypes: true });
    return entries.map(e => ({ name: e.name, fullPath: path.join(rootPath, e.name), dirent: e }));
  } catch {
    return [];
  }
}

async function main() {
  let args;
  try {
    args = parseArgs(process.argv);
  } catch (e) {
    console.error(String(e && e.message ? e.message : e));
    usage();
    process.exit(1);
  }

  if (args.help) {
    usage();
    process.exit(0);
  }

  const tempRoot = os.tmpdir();
  if (!(await exists(tempRoot))) {
    console.error(`Temp directory not found: ${tempRoot}`);
    process.exit(1);
  }

  const cutoffMs = Date.now() - args.olderThanHours * 60 * 60 * 1000;
  const modeLabel = args.apply ? 'DELETE' : 'DRY-RUN';
  logInfo(args, `[${modeLabel}] tempRoot=${tempRoot}`);
  logInfo(args, `[${modeLabel}] olderThanHours=${args.olderThanHours}`);

  const top = await listTopLevelCandidates(tempRoot);

  // Expand managed roots to their children for finer-grained cleanup
  const expanded = [];
  for (const item of top) {
    if (isManagedRootName(item.name) && item.dirent.isDirectory()) {
      expanded.push(...(await listManagedRootChildren(item.fullPath)));
      continue;
    }
    expanded.push(item);
  }

  // Deduplicate paths
  const seen = new Set();
  const candidates = [];
  for (const c of expanded) {
    if (seen.has(c.fullPath)) continue;
    seen.add(c.fullPath);
    candidates.push(c);
  }

  const toDelete = [];
  for (const c of candidates) {
    const mtimeMs = await getMtimeMs(c.fullPath);
    if (mtimeMs == null) continue;
    if (mtimeMs > cutoffMs) continue;
    toDelete.push(c);
  }

  if (toDelete.length === 0) {
    logInfo(args, `[${modeLabel}] No matching items older than cutoff.`);
    return;
  }

  let bytes = 0;
  for (const c of toDelete) {
    bytes += await getSizeBytes(c.fullPath);
  }

  logInfo(args, `[${modeLabel}] Candidate count: ${toDelete.length}`);
  logInfo(args, `[${modeLabel}] Estimated bytes: ${formatBytes(bytes)}`);

  let deleted = 0;
  let failed = 0;

  for (const c of toDelete) {
    if (!args.apply) {
      logInfo(args, `[DRY-RUN] Would remove: ${c.fullPath}`);
      continue;
    }

    try {
      await fsp.rm(c.fullPath, { recursive: true, force: true, maxRetries: 2 });
      deleted++;
      logInfo(args, `[DELETE] Removed: ${c.fullPath}`);
    } catch (e) {
      failed++;
      if (!args.quiet) console.warn(`[DELETE] Failed: ${c.fullPath} (${e && e.message ? e.message : e})`);
    }
  }

  if (args.apply) {
    logInfo(args, `[DELETE] Deleted: ${deleted}, failed: ${failed}, estimated freed: ${formatBytes(bytes)}`);
  } else {
    logInfo(args, `Run again with --apply to delete these items.`);
  }
}

if (require.main === module) {
  main().catch(err => {
    console.error(err);
    process.exit(1);
  });
}
