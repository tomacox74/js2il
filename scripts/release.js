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

function sleep(ms) {
  // Synchronous sleep without additional deps.
  const sab = new SharedArrayBuffer(4);
  const i32 = new Int32Array(sab);
  Atomics.wait(i32, 0, 0, ms);
}

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

function getEffectiveArgv(processArgv) {
  // When invoked via `npm run ... -- <args>`, npm does not always forward
  // dash-prefixed args reliably (notably `--merge`), because npm itself
  // parses some flags. However, npm exposes the original argv via the
  // `npm_config_argv` env var. Use it as a fallback so `npm run release:cut -- patch --merge`
  // behaves the same as `node scripts/release.js patch --merge`.
  const raw = process.env.npm_config_argv;
  if (!raw) return processArgv;

  try {
    const parsed = JSON.parse(raw);
    const remain = Array.isArray(parsed?.remain) ? parsed.remain : null;
    if (!remain || remain.length === 0) return processArgv;

    // Only use npm's remain if it includes a release kind.
    if (!remain.some((x) => /^(patch|minor|major)$/.test(x))) return processArgv;

    // Reconstruct argv in the same shape as process.argv: [node, script, ...args]
    return [processArgv[0], processArgv[1], ...remain];
  } catch {
    return processArgv;
  }
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

function waitForRequiredCheck(prNumber, repo, checkName, { timeoutMs = 10 * 60 * 1000, pollMs = 4000 } = {}, args) {
  const deadline = Date.now() + timeoutMs;
  while (Date.now() < deadline) {
    // We rely on statusCheckRollup so we can detect when checks start existing at all.
    const q = `.statusCheckRollup[] | select(.name=="${checkName}") | .conclusion`;
    const conclusion = run(`gh pr view ${prNumber} --repo ${repo} --json statusCheckRollup -q '${q}'`, { ...args, allowFailure: true })
      .trim();

    if (!conclusion) {
      sleep(pollMs);
      continue;
    }

    if (/^SUCCESS$/i.test(conclusion)) return;

    // Any terminal non-success conclusion should fail the release.
    throw new Error(`Required status check "${checkName}" did not succeed (conclusion: ${conclusion}).`);
  }

  throw new Error(`Timed out waiting for required status check "${checkName}" to complete.`);
}

function main() {
  const effectiveArgv = getEffectiveArgv(process.argv);
  const args = parseArgs(effectiveArgv);

  // npm may consume dash-prefixed args (even after `--`) by turning them into
  // npm config values (available as npm_config_* env vars) instead of
  // forwarding them to the script. Support the common ones so the UX is:
  //   npm run release:cut -- patch --merge
  // and it still behaves like:
  //   node scripts/release.js patch --merge
  const truthy = (v) => v === 'true' || v === '1' || v === 'yes';

  if (!args.merge && truthy(process.env.npm_config_merge)) {
    args.merge = true;
  }

  if (!args.skipEmpty && truthy(process.env.npm_config_skip_empty)) {
    args.skipEmpty = true;
  }

  if (!args.dryRun && truthy(process.env.npm_config_dry_run)) {
    args.dryRun = true;
  }

  if (!args.repo && typeof process.env.npm_config_repo === 'string' && process.env.npm_config_repo.trim()) {
    args.repo = process.env.npm_config_repo.trim();
  }

  if (args.base === 'master' && typeof process.env.npm_config_base === 'string' && process.env.npm_config_base.trim()) {
    args.base = process.env.npm_config_base.trim();
  }

  // Convenience: if the user ran `npm run ... --verbose`, npm will typically
  // consume `--verbose` for itself (setting npm_config_loglevel=verbose).
  // Treat that as release-script verbosity when our own `--verbose` wasn't forwarded.
  if (!args.verbose && (process.env.npm_config_loglevel === 'verbose' || truthy(process.env.npm_config_verbose))) {
    args.verbose = true;
  }

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

  // Some repos/branches may not have status checks configured. In that case
  // `gh pr checks --watch` can exit non-zero with "no checks reported".
  // Treat that as a warning and rely on GitHub auto-merge to wait for checks.
  const checksOut = run(`gh pr checks ${prNumber} --repo ${repo} --watch`, { ...args, allowFailure: true });
  if (checksOut && /no checks reported/i.test(checksOut)) {
    process.stdout.write(`\nNote: no CI checks reported for ${releaseBranch}; waiting for required checks to appear.\n`);
  }

  // Some repos require checks to have been reported and passed before merge.
  // If auto-merge is disabled at the repo level, we must wait and then merge.
  waitForRequiredCheck(prNumber, repo, 'build', undefined, args);
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
