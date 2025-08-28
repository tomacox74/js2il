#!/usr/bin/env node
/**
 * Finds and optionally deletes unused *.verified.txt snapshot files.
 * A snapshot is considered unused if its derived test name
 * (the portion after 'ExecutionTests.' or 'GeneratorTests.' in the filename)
 * is not referenced in any test source (*.cs) under the supplied root (default: Js2IL.Tests).
 */
const fs = require('fs');
const path = require('path');

function usage() {
  console.log(`Usage: node scripts/cleanUnusedSnapshots.js [--root Js2IL.Tests] [--delete] [--quiet]`);
}

function parseArgs(argv) {
  const args = { root: 'Js2IL.Tests', delete: false, quiet: false };
  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];
    if (a === '--help' || a === '-h') { args.help = true; }
    else if (a === '--root') { args.root = argv[++i] || args.root; }
    else if (a === '--delete' || a === '-d') { args.delete = true; }
    else if (a === '--quiet' || a === '-q') { args.quiet = true; }
    else { console.warn(`Unknown arg: ${a}`); }
  }
  return args;
}

function writeInfo(quiet, msg) { if (!quiet) console.log(msg); }

function listFiles(dir, predicate) {
  const out = [];
  (function walk(d) {
    const ents = fs.readdirSync(d, { withFileTypes: true });
    for (const e of ents) {
      const fp = path.join(d, e.name);
      if (e.isDirectory()) walk(fp);
      else if (predicate(fp)) out.push(fp);
    }
  })(dir);
  return out;
}

function readFileSafe(fp) {
  try { return fs.readFileSync(fp, 'utf8'); } catch { return ''; }
}

function main() {
  const args = parseArgs(process.argv);
  if (args.help) { usage(); process.exit(0); }
  const root = path.resolve(args.root);
  if (!fs.existsSync(root)) {
    console.error(`Root path '${root}' not found.`);
    process.exit(1);
  }

  const snapshotFiles = listFiles(root, fp => fp.endsWith('.verified.txt'));
  if (snapshotFiles.length === 0) {
    console.log(`No snapshot files found under '${root}'.`);
    return;
  }
  const testSources = listFiles(root, fp => fp.endsWith('.cs'));

  const unused = [];
  for (const snap of snapshotFiles) {
    const base = path.basename(snap, '.txt'); // e.g., ExecutionTests.Foo.verified
    let core = base;
    if (core.endsWith('.verified')) core = core.slice(0, -'.verified'.length);
    let testName = core;
    for (const prefix of ['ExecutionTests.', 'GeneratorTests.']) {
      if (core.startsWith(prefix)) { testName = core.slice(prefix.length); break; }
    }

    const pattern = new RegExp(`\\b${testName.replace(/[.*+?^${}()|[\\]\\]/g, r => `\\${r}`)}\\b`, 'm');
    const matchDirs = new Set();
    for (const src of testSources) {
      const content = readFileSafe(src);
      if (!content) continue;
      if (pattern.test(content) || content.includes(`nameof(${testName})`)) {
        matchDirs.add(path.dirname(src));
      }
    }

    const isReferenced = matchDirs.size > 0;
    const dirMatches = matchDirs.has(path.dirname(snap));
    const isEmpty = (fs.statSync(snap).size === 0);

    if (!isReferenced) {
      unused.push(snap);
      writeInfo(args.quiet, `UNUSED: ${snap} (no test referencing '${testName}')`);
      continue;
    }
    if (!dirMatches) {
      const reason = isEmpty ? 'misplaced duplicate (empty)' : 'misplaced duplicate';
      const dirs = Array.from(matchDirs).sort().join('; ');
      unused.push(snap);
      writeInfo(args.quiet, `UNUSED: ${snap} (derived test name: ${testName}, ${reason}; expected dir(s): ${dirs})`);
      continue;
    }
    // otherwise it is valid
  }

  if (unused.length === 0) {
    console.log(`No unused snapshots detected. (${snapshotFiles.length} total)`);
    return;
  }

  console.log(`Unused snapshot count: ${unused.length} of ${snapshotFiles.length} total.`);
  if (args.delete) {
    for (const f of unused) {
      try { fs.rmSync(f, { force: true }); writeInfo(args.quiet, `Deleted: ${f}`); }
      catch (err) { console.warn(`Failed to delete ${f}: ${err}`); }
    }
    console.log('Deletion complete.');
  } else {
    console.log('Run with --delete to remove these files.');
  }
}

if (require.main === module) {
  main();
}
