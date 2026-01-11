/*
Runs execution tests (tests deriving from ExecutionTestsBase) and writes a list of failing tests
based on TRX output.

Usage:
  node scripts/runExecutionTestsAndReportFailures.js

Options:
  --configuration Debug|Release   (default: Debug)
  --filter <trx filter>           (default: auto-generated from ExecutionTestsBase-derived classes)
  --project <path>               (default: Js2IL.Tests/Js2IL.Tests.csproj)
  --results <dir>                (default: Js2IL.Tests/TestResults)
  --trx <filename>               (default: execution.trx)
  --list                         (print discovered test classes and exit)

Output:
  - Writes failing test names to: <results>/failed-execution-tests.txt

Exit code:
  - Mirrors `dotnet test` exit code when possible.
*/

const childProcess = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');

function parseArgs(argv) {
  const args = {
    configuration: 'Debug',
    filter: null,
    project: null,
    results: null,
    trx: 'execution.trx',
    list: false,
  };

  for (let i = 0; i < argv.length; i++) {
    const arg = argv[i];

    if (arg === '--configuration' && argv[i + 1]) {
      args.configuration = argv[++i];
      continue;
    }

    if (arg === '--filter' && argv[i + 1]) {
      args.filter = argv[++i];
      continue;
    }

    if (arg === '--project' && argv[i + 1]) {
      args.project = argv[++i];
      continue;
    }

    if (arg === '--results' && argv[i + 1]) {
      args.results = argv[++i];
      continue;
    }

    if (arg === '--trx' && argv[i + 1]) {
      args.trx = argv[++i];
      continue;
    }

    if (arg === '--list') {
      args.list = true;
      continue;
    }

    if (arg === '--help' || arg === '-h') {
      return { ...args, help: true };
    }
  }

  return args;
}

function ensureDir(dirPath) {
  fs.mkdirSync(dirPath, { recursive: true });
}

function listFilesRecursive(rootDir) {
  const results = [];

  /** @type {string[]} */
  const stack = [rootDir];

  while (stack.length > 0) {
    const current = stack.pop();
    if (!current) continue;

    const entries = fs.readdirSync(current, { withFileTypes: true });
    for (const entry of entries) {
      const fullPath = path.join(current, entry.name);
      if (entry.isDirectory()) {
        stack.push(fullPath);
      } else {
        results.push(fullPath);
      }
    }
  }

  return results;
}

function extractAttr(tagText, attrName) {
  const re = new RegExp(`\\b${attrName}="([^"]*)"`, 'i');
  const match = tagText.match(re);
  return match ? match[1] : null;
}

function parseFailedTestsFromTrx(trxXml) {
  const failed = [];
  const unitTestResultRe = /<UnitTestResult\b[^>]*\boutcome="Failed"[^>]*>/gi;

  let match;
  while ((match = unitTestResultRe.exec(trxXml)) !== null) {
    const tag = match[0];
    const testName = extractAttr(tag, 'testName');
    if (testName) failed.push(testName);
  }

  return Array.from(new Set(failed)).sort((a, b) => a.localeCompare(b));
}

function findTrxFile(resultsDir, preferredName) {
  const preferredPath = path.join(resultsDir, preferredName);
  if (fs.existsSync(preferredPath)) return preferredPath;

  const candidates = fs
    .readdirSync(resultsDir)
    .filter((name) => name.toLowerCase().endsWith('.trx'))
    .map((name) => {
      const fullPath = path.join(resultsDir, name);
      const stat = fs.statSync(fullPath);
      return { fullPath, mtimeMs: stat.mtimeMs };
    })
    .sort((a, b) => b.mtimeMs - a.mtimeMs);

  return candidates[0]?.fullPath ?? null;
}

function writeFailedTestsFile(outputPath, failedTests) {
  const content = failedTests.length === 0 ? '' : failedTests.join('\n') + '\n';
  fs.writeFileSync(outputPath, content, 'utf8');
}

function discoverExecutionTestClassNames(repoRoot) {
  const testsRoot = path.join(repoRoot, 'Js2IL.Tests');
  const csFiles = listFilesRecursive(testsRoot).filter((p) => p.toLowerCase().endsWith('.cs'));

  /** @type {string[]} */
  const classNames = [];

  const namespaceRe = /\bnamespace\s+([A-Za-z0-9_.]+)\s*\{/;
  const derivesExecutionBaseRe = /\bclass\s+([A-Za-z_][A-Za-z0-9_]*)\s*:\s*ExecutionTestsBase\b/g;

  for (const filePath of csFiles) {
    const content = fs.readFileSync(filePath, 'utf8');
    const nsMatch = content.match(namespaceRe);
    if (!nsMatch) continue;
    const ns = nsMatch[1];

    let match;
    while ((match = derivesExecutionBaseRe.exec(content)) !== null) {
      const className = match[1];
      classNames.push(`${ns}.${className}`);
    }
  }

  return Array.from(new Set(classNames)).sort((a, b) => a.localeCompare(b));
}

function buildOrFilterForClasses(classNames) {
  if (classNames.length === 0) return null;
  // Use substring match on the class name; this will match any test method within that class.
  return '(' + classNames.map((c) => `FullyQualifiedName~${c}`).join('|') + ')';
}

function main() {
  const repoRoot = path.resolve(__dirname, '..');
  const args = parseArgs(process.argv.slice(2));

  if (args.help) {
    process.stdout.write(
      [
        'node scripts/runExecutionTestsAndReportFailures.js',
        '',
        'Options:',
        '  --configuration Debug|Release   (default: Debug)',
        '  --filter <trx filter>           (default: auto-generated from ExecutionTestsBase-derived classes)',
        '  --project <path>               (default: Js2IL.Tests/Js2IL.Tests.csproj)',
        '  --results <dir>                (default: Js2IL.Tests/TestResults)',
        '  --trx <filename>               (default: execution.trx)',
        '  --list                         (print discovered test classes and exit)',
        '',
      ].join('\n')
    );
    process.exit(0);
  }

  const projectPath = path.resolve(repoRoot, args.project ?? path.join('Js2IL.Tests', 'Js2IL.Tests.csproj'));
  const resultsDir = path.resolve(repoRoot, args.results ?? path.join('Js2IL.Tests', 'TestResults'));
  const trxFileName = args.trx;

  ensureDir(resultsDir);

  const discoveredClasses = discoverExecutionTestClassNames(repoRoot);

  if (args.list) {
    if (discoveredClasses.length === 0) {
      console.log('No ExecutionTestsBase-derived classes found.');
    } else {
      console.log(`Discovered ${discoveredClasses.length} ExecutionTestsBase-derived classes:`);
      for (const c of discoveredClasses) console.log('  ' + c);
    }
    process.exit(0);
  }

  const defaultFilter = buildOrFilterForClasses(discoveredClasses);
  const filter = args.filter ?? defaultFilter;

  if (!filter) {
    console.error('Could not auto-generate a filter (no ExecutionTestsBase-derived classes found).');
    console.error('Provide an explicit --filter value.');
    process.exit(1);
  }

  const dotnetArgs = [
    'test',
    projectPath,
    '-c',
    args.configuration,
    '--filter',
    filter,
    '--logger',
    `trx;LogFileName=${trxFileName}`,
    '--results-directory',
    resultsDir,
  ];

  console.log('Running:', 'dotnet ' + dotnetArgs.map((a) => (a.includes(' ') ? `"${a}"` : a)).join(' '));

  const result = childProcess.spawnSync('dotnet', dotnetArgs, {
    cwd: repoRoot,
    // Suppress output to avoid flooding the console.
    stdio: ['ignore', 'ignore', 'ignore'],
    shell: false,
  });

  const trxPath = findTrxFile(resultsDir, trxFileName);
  if (!trxPath) {
    console.error('Could not find TRX results under:', resultsDir);
    process.exit(result.status ?? 1);
  }

  const trxXml = fs.readFileSync(trxPath, 'utf8');
  const failedTests = parseFailedTestsFromTrx(trxXml);

  const failedTestsPath = path.join(resultsDir, 'failed-execution-tests.txt');
  writeFailedTestsFile(failedTestsPath, failedTests);

  console.log(`Wrote ${failedTests.length} failing execution test(s) to: ${failedTestsPath}`);
  if (result.status !== 0) {
    console.log(`dotnet test exited with code: ${result.status}`);
  }

  process.exit(result.status ?? 0);
}

main();
