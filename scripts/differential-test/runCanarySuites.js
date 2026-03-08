#!/usr/bin/env node
'use strict';

const fs = require('fs');
const os = require('os');
const path = require('path');

const {
  compileWithJs2IL,
  findJs2IL,
  normaliseOutput,
  runDotnet,
  runWithNode,
} = require('./run');

const COLORS = {
  green: '\x1b[32m',
  red: '\x1b[31m',
  cyan: '\x1b[36m',
  gray: '\x1b[90m',
  reset: '\x1b[0m',
};

const c = (color, text) => `${COLORS[color] || ''}${text}${COLORS.reset}`;

function parseArgs(argv) {
  const args = {
    suite: 'pr',
    timeout: 20,
    compileTimeout: null,
    output: null,
    js2il: null,
    verbose: false,
    help: false,
  };

  for (let i = 2; i < argv.length; i++) {
    const a = argv[i];

    if (a === '--help' || a === '-h') {
      args.help = true;
      continue;
    }

    if ((a === '--suite' || a === '-s') && argv[i + 1]) {
      args.suite = argv[++i];
      continue;
    }

    if (a.startsWith('--suite=')) {
      args.suite = a.substring('--suite='.length);
      continue;
    }

    if ((a === '--timeout' || a === '-t') && argv[i + 1]) {
      args.timeout = Number(argv[++i]);
      continue;
    }

    if (a === '--compile-timeout' && argv[i + 1]) {
      args.compileTimeout = Number(argv[++i]);
      continue;
    }

    if ((a === '--output' || a === '-o') && argv[i + 1]) {
      args.output = argv[++i];
      continue;
    }

    if ((a === '--js2il' || a === '-j') && argv[i + 1]) {
      args.js2il = argv[++i];
      continue;
    }

    if (a === '--verbose' || a === '-v') {
      args.verbose = true;
    }
  }

  if (args.compileTimeout === null) {
    args.compileTimeout = args.timeout * 2;
  }

  return args;
}

function usage() {
  console.log('Run bounded real-world canary smoke suites.');
  console.log('');
  console.log('Usage:');
  console.log('  node scripts/differential-test/runCanarySuites.js --suite pr');
  console.log('  node scripts/differential-test/runCanarySuites.js --suite nightly --verbose');
  console.log('');
  console.log('Options:');
  console.log('  --suite, -s            pr (default) or nightly');
  console.log('  --timeout, -t          Per-execution timeout in seconds (default: 20)');
  console.log('  --compile-timeout      Compilation timeout in seconds (default: 2x timeout)');
  console.log('  --output, -o           Output/artifact directory (default: OS temp)');
  console.log('  --js2il, -j            Path to Js2IL.dll or js2il executable');
  console.log('  --verbose, -v          Print expected output for passing canaries');
}

function getSuiteDirectories(suiteName) {
  const canaryRoot = path.join(__dirname, 'corpus', 'canary');
  const prDir = path.join(canaryRoot, 'pr');
  const expandedDir = path.join(canaryRoot, 'expanded');

  switch (suiteName) {
    case 'pr':
      return [
        { suite: 'pr', dir: prDir },
      ];
    case 'nightly':
    case 'all':
      return [
        { suite: 'pr', dir: prDir },
        { suite: 'expanded', dir: expandedDir },
      ];
    default:
      throw new Error(`Unknown suite '${suiteName}'. Expected 'pr' or 'nightly'.`);
  }
}

function discoverCases(suiteDirectories) {
  const cases = [];

  for (const suiteDirectory of suiteDirectories) {
    if (!fs.existsSync(suiteDirectory.dir)) {
      throw new Error(`Canary directory not found: ${suiteDirectory.dir}`);
    }

    const jsFiles = fs.readdirSync(suiteDirectory.dir)
      .filter(file => file.endsWith('.js'))
      .sort((a, b) => a.localeCompare(b, undefined, { numeric: true }));

    for (const jsFile of jsFiles) {
      const jsPath = path.join(suiteDirectory.dir, jsFile);
      const expectedPath = path.join(
        suiteDirectory.dir,
        `${path.basename(jsFile, '.js')}.expected.txt`
      );

      if (!fs.existsSync(expectedPath)) {
        throw new Error(`Missing expected output file for ${jsPath}: ${expectedPath}`);
      }

      cases.push({
        name: `${suiteDirectory.suite}/${path.basename(jsFile, '.js')}`,
        jsFile: jsPath,
        expectedFile: expectedPath,
        suite: suiteDirectory.suite,
      });
    }
  }

  return cases;
}

function buildArtifactDirectory(outputRoot, canaryCase) {
  const safeName = path.basename(canaryCase.jsFile, '.js');
  return path.join(outputRoot, canaryCase.suite, safeName);
}

function buildCompileOutputDirectory(outputRoot, canaryCase) {
  return path.join(outputRoot, canaryCase.suite);
}

function formatExpectedMismatch(expected, actual, sourceLabel) {
  return [
    `${sourceLabel} stdout did not match the committed canary expectation.`,
    'Expected:',
    expected || '(no output)',
    '',
    `Actual (${sourceLabel}):`,
    actual || '(no output)',
  ].join('\n');
}

function formatProcessFailure(sourceLabel, result) {
  return [
    `${sourceLabel} exited with code ${result.exitCode}.`,
    '',
    `${sourceLabel} stdout:`,
    normaliseOutput(result.stdout) || '(no output)',
    '',
    `${sourceLabel} stderr:`,
    normaliseOutput(result.stderr) || '(no output)',
  ].join('\n');
}

function formatCompileFailure(compileResult) {
  const rawStderr = compileResult && typeof compileResult.stderr === 'string'
    ? compileResult.stderr
    : '';
  const normalisedStderr = normaliseOutput(rawStderr);
  const isTimeout = rawStderr.includes('COMPILE_TIMEOUT');

  if (isTimeout) {
    const durationMs = typeof compileResult.durationMs === 'number'
      ? compileResult.durationMs
      : typeof compileResult.timeoutMs === 'number'
        ? compileResult.timeoutMs
        : undefined;
    const durationSuffix = typeof durationMs === 'number'
      ? ` after ${Math.round(durationMs / 1000)}s`
      : '';

    return [
      `JS2IL compilation timed out${durationSuffix}.`,
      '',
      'Compiler stderr/stdout:',
      normalisedStderr || '(no output)',
    ].join('\n');
  }

  return [
    'JS2IL compilation failed.',
    '',
    'Compiler stderr/stdout:',
    normalisedStderr || '(no output)',
  ].join('\n');
}

function runCanaryCase(canaryCase, js2il, timeoutSec, compileTimeoutSec, outputRoot) {
  const expected = normaliseOutput(fs.readFileSync(canaryCase.expectedFile, 'utf8'));
  const artifactDir = buildArtifactDirectory(outputRoot, canaryCase);
  const compileOutputDir = buildCompileOutputDirectory(outputRoot, canaryCase);

  fs.mkdirSync(artifactDir, { recursive: true });

  const nodeResult = runWithNode(canaryCase.jsFile, timeoutSec * 1000);
  if (nodeResult.timedOut) {
    return {
      name: canaryCase.name,
      status: 'node-timeout',
      detail: `Node timed out after ${timeoutSec}s.`,
      artifactDir,
      expected,
    };
  }

  if (nodeResult.exitCode !== 0) {
    return {
      name: canaryCase.name,
      status: 'node-failure',
      detail: formatProcessFailure('Node', nodeResult),
      artifactDir,
      expected,
    };
  }

  const nodeStdout = normaliseOutput(nodeResult.stdout);
  if (nodeStdout !== expected) {
    return {
      name: canaryCase.name,
      status: 'node-output-mismatch',
      detail: formatExpectedMismatch(expected, nodeStdout, 'Node'),
      artifactDir,
      expected,
    };
  }

  const compileResult = compileWithJs2IL(
    canaryCase.jsFile,
    compileOutputDir,
    js2il,
    compileTimeoutSec * 1000
  );
  if (!compileResult.success) {
    return {
      name: canaryCase.name,
      status: 'compile-error',
      detail: formatCompileFailure(compileResult),
      artifactDir,
      expected,
    };
  }

  const js2ilResult = runDotnet(compileResult.dllPath, timeoutSec * 1000);
  if (js2ilResult.timedOut) {
    return {
      name: canaryCase.name,
      status: 'js2il-timeout',
      detail: `JS2IL execution timed out after ${timeoutSec}s.`,
      artifactDir,
      expected,
    };
  }

  if (js2ilResult.exitCode !== 0) {
    return {
      name: canaryCase.name,
      status: 'js2il-failure',
      detail: formatProcessFailure('JS2IL', js2ilResult),
      artifactDir,
      expected,
    };
  }

  const js2ilStdout = normaliseOutput(js2ilResult.stdout);
  if (js2ilStdout !== expected) {
    return {
      name: canaryCase.name,
      status: 'js2il-output-mismatch',
      detail: formatExpectedMismatch(expected, js2ilStdout, 'JS2IL'),
      artifactDir,
      expected,
    };
  }

  return {
    name: canaryCase.name,
    status: 'pass',
    detail: null,
    artifactDir,
    expected,
  };
}

function printResult(result, verbose) {
  if (result.status === 'pass') {
    console.log(c('green', `  PASS  ${result.name}`));
    if (verbose) {
      const lines = result.expected ? result.expected.split('\n') : ['(no output)'];
      for (const line of lines) {
        console.log(c('gray', `         ${line}`));
      }
    }
    return;
  }

  console.log(c('red', `  FAIL  ${result.name}  [${result.status}]`));
  if (result.detail) {
    for (const line of result.detail.split('\n')) {
      console.log(c('gray', `         ${line}`));
    }
  }
  console.log(c('gray', `         artifacts: ${result.artifactDir}`));
}

async function main() {
  const args = parseArgs(process.argv);
  if (args.help) {
    usage();
    return;
  }

  const js2il = findJs2IL(args.js2il);
  const outputRoot = args.output || fs.mkdtempSync(path.join(os.tmpdir(), 'canary-smoke-'));
  const suiteDirectories = getSuiteDirectories(args.suite);
  const cases = discoverCases(suiteDirectories);

  console.log(c('cyan', '\nCanary smoke suites'));
  console.log(c('gray', `  suite   : ${args.suite}`));
  console.log(c('gray', `  timeout : ${args.timeout}s per execution, ${args.compileTimeout}s per compilation`));
  console.log(c('gray', `  output  : ${outputRoot}`));
  console.log(c('gray', `  js2il   : [${js2il.type}] ${js2il.path}`));
  console.log();
  console.log(c('cyan', `Running ${cases.length} real-world canaries...\n`));

  const results = [];
  for (const canaryCase of cases) {
    const result = runCanaryCase(
      canaryCase,
      js2il,
      args.timeout,
      args.compileTimeout,
      outputRoot
    );
    results.push(result);
    printResult(result, args.verbose);
  }

  const passed = results.filter(result => result.status === 'pass').length;
  const failed = results.length - passed;

  console.log();
  console.log(c('cyan', '─'.repeat(60)));
  console.log(
    c('green', `  ${passed} passed`) + '  ' +
    (failed > 0 ? c('red', `${failed} failed`) : c('gray', `${failed} failed`))
  );

  if (failed > 0) {
    console.log();
    console.log(c('red', 'Failing canaries:'));
    for (const result of results.filter(entry => entry.status !== 'pass')) {
      console.log(c('red', `  ${result.name}`));
      console.log(c('gray', `    artifacts: ${result.artifactDir}`));
    }
    process.exitCode = 1;
  }
}

if (require.main === module) {
  main().catch(err => {
    console.error('Canary harness error:', err);
    process.exitCode = 1;
  });
}
