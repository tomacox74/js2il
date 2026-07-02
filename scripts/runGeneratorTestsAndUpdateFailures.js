/*
Runs generator tests and updates per-project failed-tests.txt files based on TRX output.

Usage:
  node scripts/runGeneratorTestsAndUpdateFailures.js

Options:
  --configuration Debug|Release   (default: Debug)
  --filter <trx filter>           (default: FullyQualifiedName~.GeneratorTests.)
  --project <path>               (default: runs both Jroc.Tests and Jroc.Test262.Tests)
  --results <dir>                (default: sibling TestResults directory for the selected project)
  --trx <filename>               (default: generator.trx)

Exit code:
  - Mirrors the first non-zero `dotnet test` exit code when possible.
  - Still writes failed-tests.txt for each project even if tests fail.
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

function getDefaultProjectConfigs(repoRoot, trxFileName) {
  return [
    {
      projectPath: path.resolve(repoRoot, path.join('tests', 'Jroc.Tests', 'Jroc.Tests.csproj')),
      resultsDir: path.resolve(repoRoot, path.join('tests', 'Jroc.Tests', 'TestResults')),
      trxFileName,
    },
    {
      projectPath: path.resolve(repoRoot, path.join('tests', 'Jroc.Test262.Tests', 'Jroc.Test262.Tests.csproj')),
      resultsDir: path.resolve(repoRoot, path.join('tests', 'Jroc.Test262.Tests', 'TestResults')),
      trxFileName,
    },
  ];
}

function getProjectConfigs(repoRoot, args) {
  if (!args.project && !args.results) {
    return getDefaultProjectConfigs(repoRoot, args.trx);
  }

  const projectPath = path.resolve(repoRoot, args.project ?? path.join('tests', 'Jroc.Tests', 'Jroc.Tests.csproj'));
  const resultsDir = path.resolve(
    repoRoot,
    args.results ?? path.join(path.dirname(path.relative(repoRoot, projectPath)), 'TestResults')
  );

  return [{ projectPath, resultsDir, trxFileName: args.trx }];
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
        '  --project <path>               (default: tests/Jroc.Tests/Jroc.Tests.csproj)',
        '  --results <dir>                (default: tests/Jroc.Tests/TestResults)',
        '  --trx <filename>               (default: generator.trx)',
        '',
      ].join('\n')
    );
    process.exit(0);
  }

  const projectConfigs = getProjectConfigs(repoRoot, args);
  let exitCode = 0;

  for (const config of projectConfigs) {
    ensureDir(config.resultsDir);

    const dotnetArgs = [
      'test',
      config.projectPath,
      '-c',
      args.configuration,
      '--filter',
      args.filter,
      '--logger',
      `trx;LogFileName=${config.trxFileName}`,
      '--results-directory',
      config.resultsDir,
    ];

    console.log('Running:', 'dotnet ' + dotnetArgs.map((a) => (a.includes(' ') ? `"${a}"` : a)).join(' '));

    const result = childProcess.spawnSync('dotnet', dotnetArgs, {
      cwd: repoRoot,
      // Suppress dotnet test output to avoid overwhelming the console (and hanging chat sessions).
      // We rely on TRX parsing below to report failures.
      stdio: ['ignore', 'ignore', 'ignore'],
      shell: false,
    });

    const trxPath = findTrxFile(config.resultsDir, config.trxFileName);
    if (!trxPath) {
      console.error('Could not find TRX results under:', config.resultsDir);
      process.exit(result.status ?? 1);
    }

    const trxXml = fs.readFileSync(trxPath, 'utf8');
    const failedTests = parseFailedTestsFromTrx(trxXml);

    const failedTestsPath = path.join(config.resultsDir, 'failed-tests.txt');
    writeFailedTestsFile(failedTestsPath, failedTests);

    console.log(`Wrote ${failedTests.length} failing test(s) to: ${failedTestsPath}`);
    if (result.status !== 0) {
      console.log(`dotnet test exited with code: ${result.status}`);
      if (exitCode === 0) {
        exitCode = result.status ?? 1;
      }
    }
  }

  process.exit(exitCode);
}

main();
