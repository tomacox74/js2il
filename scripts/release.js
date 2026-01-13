#!/usr/bin/env node
/*
 * release.js
 *
 * Automates the JS2IL release process:
 *  1) Ensure clean, up-to-date master
 *  2) Compute next version (patch/minor/major)
 *  3) Create release/<version> branch
 *  4) Run version bump (CHANGELOG + csproj versions)
 *  5) Commit
 *  6) Push + open PR
 *  7) Optionally wait for checks, merge PR, and create GitHub release/tag
 *
 * Usage:
 *   node scripts/release.js patch
 *   node scripts/release.js minor
 *   node scripts/release.js major
 *   node scripts/release.js patch --merge
 *
 * Requirements:
 *  - git
 *  - gh (GitHub CLI) authenticated
 */

const fs = require('fs');
const path = require('path');
const cp = require('child_process');

const ROOT = path.resolve(__dirname, '..');
const CHANGELOG_PATH = path.join(ROOT, 'CHANGELOG.md');
const CSPROJ_PATH = path.join(ROOT, 'Js2IL', 'Js2IL.csproj');
const RUNTIME_CSPROJ_PATH = path.join(ROOT, 'JavaScriptRuntime', 'JavaScriptRuntime.csproj');

function usageAndExit(code = 1) {
  console.error(`\nUsage: node scripts/release.js <patch|minor|major> [--merge] [--skip-empty] [--dry-run] [--repo owner/name] [--base master]\n`);
  process.exit(code);
}

function parseArgs(argv) {
  const args = {
    kind: undefined,
    merge: false,
    skipEmpty: false,
    dryRun: false,
    repo: undefined,
    base: 'master',
    verbose: false,
  };

  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];
    if (!args.kind && !a.startsWith('--')) {
      args.kind = a;
      continue;
    }
    switch (a) {
      case '--merge':
        args.merge = true;
        break;
      case '--skip-empty':
        args.skipEmpty = true;
        break;
      case '--dry-run':
        args.dryRun = true;
        break;
      case '--verbose':
        args.verbose = true;
        break;
      case '--repo':
        args.repo = argv[++i];
        break;
      case '--base':
        args.base = argv[++i];
        break;
      case '--help':
      case '-h':
        usageAndExit(0);
        break;
      default:
        console.error(`Unknown arg: ${a}`);
        usageAndExit(1);
    }
  }

  if (!args.kind || !/^(patch|minor|major)$/.test(args.kind)) {
    console.error('Missing or invalid release kind (patch|minor|major).');
    usageAndExit(1);
  }

  if (!args.base) {
    console.error('Missing --base value.');
    usageAndExit(1);
  }

  return args;
}

function run(command, { dryRun = false, verbose = false, allowFailure = false } = {}) {
  if (verbose || dryRun) {
    process.stdout.write(`> ${command}\n`);
  }
  if (dryRun) return '';

  try {
    return cp.execSync(command, {
      cwd: ROOT,
      stdio: ['ignore', 'pipe', 'pipe'],
      encoding: 'utf8',
    }).trim();
  } catch (err) {
    if (allowFailure) {
      const out = (err.stdout ? err.stdout.toString() : '').trim();
      const eout = (err.stderr ? err.stderr.toString() : '').trim();
      return [out, eout].filter(Boolean).join('\n');
    }

    const out = err.stdout ? err.stdout.toString() : '';
    const eout = err.stderr ? err.stderr.toString() : '';
    console.error(out);
    console.error(eout);
    throw new Error(`Command failed: ${command}`);
  }
}

function readText(p) {
  return fs.readFileSync(p, 'utf8');
}

function parseCsprojVersion(csprojText) {
  const m = csprojText.match(/<Version>([^<]+)<\/Version>/);
  if (!m) throw new Error('Could not find <Version> in Js2IL.csproj');
  return m[1].trim();
}

function incVersion(version, kind) {
  const pre = version.split('-')[0];
  const parts = pre.split('.').map((n) => parseInt(n, 10));
  if (parts.length !== 3 || parts.some((n) => Number.isNaN(n))) {
    throw new Error(`Invalid current version: ${version}`);
  }
  let [maj, min, pat] = parts;
  switch (kind) {
    case 'major':
      maj++;
      min = 0;
      pat = 0;
      break;
    case 'minor':
      min++;
      pat = 0;
      break;
    case 'patch':
      pat++;
      break;
    default:
      throw new Error(`Unknown bump kind: ${kind}`);
  }
  return `${maj}.${min}.${pat}`;
}

function inferRepoFromGitRemote() {
  const url = run('git remote get-url origin', { allowFailure: true });
  // https://github.com/owner/repo(.git)
  let m = url.match(/^https?:\/\/github\.com\/(.+?)\/(.+?)(?:\.git)?$/);
  if (m) return `${m[1]}/${m[2]}`;
  // git@github.com:owner/repo(.git)
  m = url.match(/^git@github\.com:(.+?)\/(.+?)(?:\.git)?$/);
  if (m) return `${m[1]}/${m[2]}`;
  return undefined;
}

function requireCleanWorkingTree(args) {
  const status = run('git status --porcelain', args);
  if (status) {
    throw new Error('Working tree is not clean. Commit/stash changes before releasing.');
  }
}

function checkoutAndSyncBase(base, args) {
  run(`git checkout ${base}`, args);
  run('git fetch origin --tags', args);
  run(`git pull --ff-only origin ${base}`, args);

  const local = run(`git rev-parse ${base}`, args);
  const remote = run(`git rev-parse origin/${base}`, args);
  if (local !== remote) {
    throw new Error(`${base} is not in sync with origin/${base}. Resolve before releasing.`);
  }
}

function ensureTools(args) {
  run('git --version', args);
  run('node --version', args);
  run('npm --version', args);
  run('gh --version', args);
  // Throws if not authenticated
  run('gh auth status', args);
}

function changelogSectionFor(version) {
  const changelog = readText(CHANGELOG_PATH);
  const header = `## v${version}`;
  const idx = changelog.indexOf(header);
  if (idx === -1) {
    throw new Error(`Could not find ${header} section in CHANGELOG.md`);
  }

  const after = changelog.slice(idx);
  const lines = after.split(/\r?\n/);

  let endLine = lines.length;
  for (let i = 1; i < lines.length; i++) {
    if (/^##\s+v\d+\.\d+\.\d+\b/.test(lines[i])) {
      endLine = i;
      break;
    }
  }

  return lines.slice(0, endLine).join('\n').trim() + '\n';
}

function main() {
  const args = parseArgs(process.argv);

  ensureTools(args);

  // Must start from clean master
  checkoutAndSyncBase(args.base, args);
  requireCleanWorkingTree(args);

  const currentVersion = parseCsprojVersion(readText(CSPROJ_PATH));
  const nextVersion = incVersion(currentVersion, args.kind);
  const releaseBranch = `release/${nextVersion}`;

  // Guard against reruns
  const existingTag = run(`git tag -l v${nextVersion}`, { ...args, allowFailure: true });
  if (existingTag) {
    throw new Error(`Tag v${nextVersion} already exists. Aborting.`);
  }

  const existingBranch = run(`git branch --list ${releaseBranch}`, { ...args, allowFailure: true });
  if (existingBranch) {
    throw new Error(`Local branch ${releaseBranch} already exists. Aborting.`);
  }

  // Create release branch first (per repo conventions)
  run(`git checkout -b ${releaseBranch}`, args);

  // Bump versions + changelog
  const bumpArgs = [args.kind];
  if (args.skipEmpty) bumpArgs.push('--skip-empty');
  run(`node scripts/bumpVersion.js ${bumpArgs.join(' ')}`, args);

  // Commit
  run(`git add "${path.relative(ROOT, CHANGELOG_PATH)}" "${path.relative(ROOT, CSPROJ_PATH)}" "${path.relative(ROOT, RUNTIME_CSPROJ_PATH)}"`, args);
  run(`git commit -m "chore(release): cut v${nextVersion}"`, args);

  // Push + PR
  run(`git push -u origin ${releaseBranch}`, args);

  const repo = args.repo || inferRepoFromGitRemote();
  if (!repo) {
    throw new Error('Could not infer GitHub repo from origin remote. Provide --repo owner/name');
  }

  const prTitle = `chore(release): Release v${nextVersion}`;
  const prBody = `Patch release v${nextVersion}.\n\n- Version bump + changelog updates.`;
  const prBodyPath = path.join(ROOT, 'pr-body.md');
  fs.writeFileSync(prBodyPath, prBody, 'utf8');
  let prUrl = '';
  try {
    prUrl = run(
      `gh pr create --repo ${repo} --title "${prTitle}" --body-file "${path.basename(prBodyPath)}" --base ${args.base} --head ${releaseBranch}`,
      args
    );
  } finally {
    try { fs.unlinkSync(prBodyPath); } catch { /* ignore */ }
  }

  // gh prints the PR URL to stdout on success
  const urlMatch = prUrl.match(/https:\/\/github\.com\/.+/);
  const prLink = urlMatch ? urlMatch[0].trim() : prUrl.trim();

  if (!args.merge) {
    process.stdout.write(`\nCreated PR: ${prLink}\n`);
    process.stdout.write(`\nNext: merge the PR, then create the GitHub release/tag with:\n`);
    process.stdout.write(`  node scripts/release.js ${args.kind} --merge\n`);
    return;
  }

  // If merge requested, wait for checks then merge, then create GitHub release.
  const prNumber = run(`gh pr view "${prLink}" --repo ${repo} --json number -q .number`, args);
  run(`gh pr checks ${prNumber} --repo ${repo} --watch`, args);
  run(`gh pr merge ${prNumber} --repo ${repo} --merge --delete-branch`, args);

  // Update local master to the merged commit
  checkoutAndSyncBase(args.base, args);

  // Create release notes from CHANGELOG section
  const notesPath = path.join(ROOT, 'release-notes.md');
  fs.writeFileSync(notesPath, changelogSectionFor(nextVersion), 'utf8');
  try {
    run(
      `gh release create v${nextVersion} --repo ${repo} --title "v${nextVersion}" --notes-file "${path.basename(notesPath)}" --target ${args.base}`,
      args
    );
  } finally {
    try { fs.unlinkSync(notesPath); } catch { /* ignore */ }
  }

  // Cleanup local release branch
  run(`git branch -D ${releaseBranch}`, { ...args, allowFailure: true });

  process.stdout.write(`\nRelease v${nextVersion} created successfully.\n`);
}

try {
  main();
} catch (e) {
  console.error(`\nERROR: ${e.message}`);
  process.exitCode = 1;
}
