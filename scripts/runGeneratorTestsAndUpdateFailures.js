/*
Runs generator tests and updates Js2IL.Tests/TestResults/failed-tests.txt based on TRX output.

Usage:
  node scripts/runGeneratorTestsAndUpdateFailures.js

Options:
  --configuration Debug|Release   (default: Debug)
  --filter <trx filter>           (default: FullyQualifiedName~.GeneratorTests.)
  --project <path>               (default: Js2IL.Tests/Js2IL.Tests.csproj)
  --results <dir>                (default: Js2IL.Tests/TestResults)
  --trx <filename>               (default: generator.trx)

Exit code:
  - Mirrors `dotnet test` exit code when possible.
  - Still writes failed-tests.txt even if tests fail.
*/

const childProcess = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');

function parseArgs(argv) {
  const args = {
    configuration: 'Debug',
    filter: 'FullyQualifiedName~.GeneratorTests.',
    project: null,
    results: null,
    trx: 'generator.trx',
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

    if (arg === '--help' || arg === '-h') {
      return { ...args, help: true };
    }
  }

  return args;
}

function ensureDir(dirPath) {
  fs.mkdirSync(dirPath, { recursive: true });
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

  // Unique + sort for stable output
  return Array.from(new Set(failed)).sort((a, b) => a.localeCompare(b));
}

function writeFailedTestsFile(outputPath, failedTests) {
  const content = failedTests.length === 0 ? '' : failedTests.join('\n') + '\n';
  fs.writeFileSync(outputPath, content, 'utf8');
}

function main() {
  const repoRoot = path.resolve(__dirname, '..');
  const args = parseArgs(process.argv.slice(2));

  if (args.help) {
    process.stdout.write(
      [
        'node scripts/runGeneratorTestsAndUpdateFailures.js',
        '',
        'Options:',
        '  --configuration Debug|Release   (default: Debug)',
        '  --filter <trx filter>           (default: FullyQualifiedName~.GeneratorTests.)',
        '  --project <path>               (default: Js2IL.Tests/Js2IL.Tests.csproj)',
        '  --results <dir>                (default: Js2IL.Tests/TestResults)',
        '  --trx <filename>               (default: generator.trx)',
        '',
      ].join('\n')
    );
    process.exit(0);
  }

  const projectPath = path.resolve(repoRoot, args.project ?? path.join('Js2IL.Tests', 'Js2IL.Tests.csproj'));
  const resultsDir = path.resolve(repoRoot, args.results ?? path.join('Js2IL.Tests', 'TestResults'));
  const trxFileName = args.trx;

  ensureDir(resultsDir);

  const dotnetArgs = [
    'test',
    projectPath,
    '-c',
    args.configuration,
    '--filter',
    args.filter,
    '--logger',
    `trx;LogFileName=${trxFileName}`,
    '--results-directory',
    resultsDir,
  ];

  console.log('Running:', 'dotnet ' + dotnetArgs.map((a) => (a.includes(' ') ? `"${a}"` : a)).join(' '));

  const result = childProcess.spawnSync('dotnet', dotnetArgs, {
    cwd: repoRoot,
    stdio: 'inherit',
    shell: false,
  });

  const trxPath = findTrxFile(resultsDir, trxFileName);
  if (!trxPath) {
    console.error('Could not find TRX results under:', resultsDir);
    process.exit(result.status ?? 1);
  }

  const trxXml = fs.readFileSync(trxPath, 'utf8');
  const failedTests = parseFailedTestsFromTrx(trxXml);

  const failedTestsPath = path.join(resultsDir, 'failed-tests.txt');
  writeFailedTestsFile(failedTestsPath, failedTests);

  console.log(`Wrote ${failedTests.length} failing test(s) to: ${failedTestsPath}`);

  process.exit(result.status ?? 0);
}

main();
