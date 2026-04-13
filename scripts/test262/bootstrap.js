#!/usr/bin/env node
'use strict';

const childProcess = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');

function parseArgs(argv) {
  const args = {
    pin: null,
    root: null,
    describe: false,
    force: false,
    printRoot: false,
    help: false,
  };

  for (let i = 0; i < argv.length; i++) {
    const arg = argv[i];

    if (arg === '--pin' && argv[i + 1]) {
      args.pin = argv[++i];
      continue;
    }

    if (arg === '--root' && argv[i + 1]) {
      args.root = argv[++i];
      continue;
    }

    if (arg === '--describe') {
      args.describe = true;
      continue;
    }

    if (arg === '--force') {
      args.force = true;
      continue;
    }

    if (arg === '--print-root') {
      args.printRoot = true;
      continue;
    }

    if (arg === '--help' || arg === '-h') {
      args.help = true;
      continue;
    }
  }

  return args;
}

function printHelp() {
  console.log(
    [
      'node scripts/test262/bootstrap.js',
      '',
      'Materializes the pinned upstream test262 MVP slice into a managed local cache,',
      'or reuses a developer-provided checkout via JS2IL_TEST262_ROOT / --root.',
      '',
      'Options:',
      '  --pin <path>      Pin/config file (default: tests/test262/test262.pin.json)',
      '  --root <path>     Developer override root (same effect as JS2IL_TEST262_ROOT)',
      '  --describe        Print the configured intake/update policy without fetching',
      '  --print-root      Print only the resolved test262 root after validation/bootstrap',
      '  --force           Recreate the managed checkout even if it already looks valid',
      '  --help, -h        Show this help text',
      '',
    ].join('\n')
  );
}

function defaultPinPath() {
  return path.resolve(__dirname, '..', '..', 'tests', 'test262', 'test262.pin.json');
}

function resolvePathFromCwd(value) {
  if (!value) {
    return '';
  }

  return path.isAbsolute(value) ? value : path.resolve(process.cwd(), value);
}

function toPortablePath(value) {
  return value.replace(/\\/g, '/');
}

function normalizeSpecPath(value) {
  return toPortablePath(value).replace(/^\/+/, '').replace(/\/+$/, '');
}

function ensureString(value, label) {
  if (typeof value !== 'string' || value.trim() === '') {
    throw new Error(`Invalid pin file: expected non-empty string for '${label}'.`);
  }
}

function ensureStringArray(value, label) {
  if (!Array.isArray(value) || value.length === 0) {
    throw new Error(`Invalid pin file: expected non-empty string array for '${label}'.`);
  }

  for (const entry of value) {
    if (typeof entry !== 'string' || entry.trim() === '') {
      throw new Error(`Invalid pin file: '${label}' entries must be non-empty strings.`);
    }
  }
}

function validateExcludedFromMvp(value) {
  ensureStringArray(value, 'excludedFromMvp');

  for (const entry of value) {
    if (entry.startsWith('frontmatter:')) {
      throw new Error("Invalid pin file: 'excludedFromMvp' now accepts only path-based exclusions; frontmatter rules belong in metadata-driven unsupported classification.");
    }
  }
}

function validatePin(pin) {
  if (!pin || typeof pin !== 'object') {
    throw new Error('Invalid pin file: expected a JSON object.');
  }

  if (!pin.upstream || typeof pin.upstream !== 'object') {
    throw new Error("Invalid pin file: expected object for 'upstream'.");
  }

  ensureString(pin.upstream.owner, 'upstream.owner');
  ensureString(pin.upstream.repo, 'upstream.repo');
  ensureString(pin.upstream.cloneUrl, 'upstream.cloneUrl');
  ensureString(pin.upstream.commit, 'upstream.commit');
  ensureString(pin.upstream.packageVersion, 'upstream.packageVersion');
  ensureString(pin.localOverrideEnvVar, 'localOverrideEnvVar');
  ensureString(pin.managedRoot, 'managedRoot');
  ensureString(pin.lineEndings, 'lineEndings');
  ensureString(pin.updateStrategy, 'updateStrategy');
  ensureStringArray(pin.includeFiles, 'includeFiles');
  ensureStringArray(pin.includeDirectories, 'includeDirectories');
  ensureStringArray(pin.requiredFiles, 'requiredFiles');
  ensureStringArray(pin.requiredDirectories, 'requiredDirectories');
  ensureStringArray(pin.defaultHarnessFiles, 'defaultHarnessFiles');
  validateExcludedFromMvp(pin.excludedFromMvp);
  ensureStringArray(pin.attributionFiles, 'attributionFiles');

  if (!/^[0-9a-f]{40}$/i.test(pin.upstream.commit)) {
    throw new Error("Invalid pin file: 'upstream.commit' must be a 40-character git SHA.");
  }
}

function loadPin(pinPath) {
  const raw = fs.readFileSync(pinPath, 'utf8');
  const pin = JSON.parse(raw);
  validatePin(pin);
  return pin;
}

function resolveManagedCheckoutRoot(pinPath, pin) {
  const pinDirectory = path.dirname(pinPath);
  return path.resolve(pinDirectory, pin.managedRoot, pin.upstream.commit);
}

function resolveManagedCheckoutLabel(pin) {
  return toPortablePath(path.posix.join(normalizeSpecPath(pin.managedRoot), pin.upstream.commit));
}

function resolveOverrideRoot(args, pin) {
  const overrideValue = args.root || process.env[pin.localOverrideEnvVar] || '';
  if (!overrideValue) {
    return null;
  }

  return {
    source: args.root ? '--root' : pin.localOverrideEnvVar,
    rootPath: resolvePathFromCwd(overrideValue),
  };
}

function describePlan(pin, override) {
  const lines = [
    `source ${override ? 'override' : 'managed'}`,
    `cloneUrl ${pin.upstream.cloneUrl}`,
    `commit ${pin.upstream.commit}`,
    `packageVersion ${pin.upstream.packageVersion}`,
    `managedRoot ${toPortablePath(pin.managedRoot)}`,
    `checkoutDir ${resolveManagedCheckoutLabel(pin)}`,
    `overrideEnv ${pin.localOverrideEnvVar}`,
    `lineEndings ${pin.lineEndings}`,
    `updateStrategy ${pin.updateStrategy}`,
    `includeFiles ${pin.includeFiles.join(', ')}`,
    `includeDirectories ${pin.includeDirectories.join(', ')}`,
    `requiredFiles ${pin.requiredFiles.join(', ')}`,
    `requiredDirectories ${pin.requiredDirectories.join(', ')}`,
    `defaultHarnessFiles ${pin.defaultHarnessFiles.join(', ')}`,
    `excludedFromMvp ${pin.excludedFromMvp.join('; ')}`,
    `attributionFiles ${pin.attributionFiles.join(', ')}`,
  ];

  if (override) {
    lines.push(`overrideSource ${override.source}`);
    lines.push(`overrideRoot ${toPortablePath(override.rootPath)}`);
  }

  return lines.join('\n');
}

function ensureDir(directoryPath) {
  fs.mkdirSync(directoryPath, { recursive: true });
}

function removeDir(directoryPath) {
  if (!fs.existsSync(directoryPath)) {
    return;
  }

  fs.rmSync(directoryPath, { recursive: true, force: true });
}

function runGit(args, cwd) {
  const result = childProcess.spawnSync('git', args, {
    cwd,
    encoding: 'utf8',
    maxBuffer: 4 * 1024 * 1024,
    shell: false,
  });

  if (result.error) {
    throw result.error;
  }

  if (result.status !== 0) {
    const stderr = (result.stderr || '').trim();
    const stdout = (result.stdout || '').trim();
    const details = [stderr, stdout].filter(Boolean).join('\n');
    throw new Error(`git ${args.join(' ')} failed${details ? `:\n${details}` : '.'}`);
  }

  return result.stdout || '';
}

function buildSparsePatterns(pin) {
  const patterns = [];

  for (const filePath of pin.includeFiles) {
    patterns.push(`/${normalizeSpecPath(filePath)}`);
  }

  for (const directoryPath of pin.includeDirectories) {
    const normalized = normalizeSpecPath(directoryPath);
    patterns.push(`/${normalized}/`);
    patterns.push(`/${normalized}/**`);
  }

  return patterns;
}

function validateResolvedRoot(rootPath, pin) {
  const missingFiles = [];
  const missingDirectories = [];

  for (const filePath of pin.requiredFiles) {
    const fullPath = path.join(rootPath, ...normalizeSpecPath(filePath).split('/'));
    if (!fs.existsSync(fullPath) || !fs.statSync(fullPath).isFile()) {
      missingFiles.push(filePath);
    }
  }

  for (const directoryPath of pin.requiredDirectories) {
    const fullPath = path.join(rootPath, ...normalizeSpecPath(directoryPath).split('/'));
    if (!fs.existsSync(fullPath) || !fs.statSync(fullPath).isDirectory()) {
      missingDirectories.push(directoryPath);
    }
  }

  if (missingFiles.length > 0 || missingDirectories.length > 0) {
    const parts = [];
    if (missingFiles.length > 0) {
      parts.push(`missing files: ${missingFiles.join(', ')}`);
    }
    if (missingDirectories.length > 0) {
      parts.push(`missing directories: ${missingDirectories.join(', ')}`);
    }
    return { ok: false, message: parts.join('; ') };
  }

  const packageJsonPath = path.join(rootPath, 'package.json');
  const packageJson = JSON.parse(fs.readFileSync(packageJsonPath, 'utf8'));
  if (!packageJson || packageJson.version !== pin.upstream.packageVersion) {
    return {
      ok: false,
      message: `package.json version mismatch: expected ${pin.upstream.packageVersion}, got ${packageJson && packageJson.version ? packageJson.version : '<missing>'}`,
    };
  }

  return { ok: true, message: '' };
}

function ensureManagedCheckout(rootPath, pin, force) {
  if (!force) {
    const current = validateResolvedRoot(rootPath, pin);
    if (current.ok) {
      return { source: 'managed-cache', rootPath, reused: true };
    }
  }

  removeDir(rootPath);
  ensureDir(rootPath);

  runGit(['init'], rootPath);
  runGit(['config', 'core.autocrlf', 'false'], rootPath);
  runGit(['config', 'core.eol', 'lf'], rootPath);
  runGit(['remote', 'add', 'origin', pin.upstream.cloneUrl], rootPath);
  runGit(['sparse-checkout', 'init', '--no-cone'], rootPath);
  runGit(['sparse-checkout', 'set', '--no-cone'].concat(buildSparsePatterns(pin)), rootPath);
  runGit(['fetch', '--depth', '1', 'origin', pin.upstream.commit], rootPath);
  runGit(['checkout', '--detach', 'FETCH_HEAD'], rootPath);

  const validation = validateResolvedRoot(rootPath, pin);
  if (!validation.ok) {
    throw new Error(`Pinned test262 checkout is incomplete after bootstrap: ${validation.message}`);
  }

  return { source: 'managed-cache', rootPath, reused: false };
}

function resolveBootstrapRoot(pinPath, pin, args) {
  const override = resolveOverrideRoot(args, pin);
  if (override) {
    const validation = validateResolvedRoot(override.rootPath, pin);
    if (!validation.ok) {
      throw new Error(`Override test262 root '${override.rootPath}' is invalid: ${validation.message}`);
    }

    return { source: override.source, rootPath: override.rootPath, reused: true };
  }

  return ensureManagedCheckout(resolveManagedCheckoutRoot(pinPath, pin), pin, args.force);
}

function printResolvedRoot(result, pin, printRootOnly) {
  if (printRootOnly) {
    console.log(result.rootPath);
    return;
  }

  const reusedText = result.reused ? 'reused' : 'fetched';
  console.log(
    [
      `source ${result.source}`,
      `root ${toPortablePath(result.rootPath)}`,
      `commit ${pin.upstream.commit}`,
      `packageVersion ${pin.upstream.packageVersion}`,
      `status ${reusedText}`,
    ].join('\n')
  );
}

function main() {
  const args = parseArgs(process.argv.slice(2));
  if (args.help) {
    printHelp();
    return;
  }

  const pinPath = resolvePathFromCwd(args.pin || defaultPinPath());
  const pin = loadPin(pinPath);
  const override = resolveOverrideRoot(args, pin);

  if (args.describe) {
    console.log(describePlan(pin, override));
    return;
  }

  const result = resolveBootstrapRoot(pinPath, pin, args);
  printResolvedRoot(result, pin, args.printRoot);
}

module.exports = {
  buildSparsePatterns,
  defaultPinPath,
  describePlan,
  ensureManagedCheckout,
  loadPin,
  parseArgs,
  resolveBootstrapRoot,
  resolveManagedCheckoutLabel,
  resolveManagedCheckoutRoot,
  validateResolvedRoot,
};

if (require.main === module) {
  try {
    main();
  } catch (error) {
    const message = error && error.message ? error.message : String(error);
    console.error(message);
    process.exitCode = 1;
  }
}
