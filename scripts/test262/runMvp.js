'use strict';

const fs = require('node:fs');
const path = require('node:path');
const { spawnSync } = require('node:child_process');

const bootstrap = require('./bootstrap');
const { parseTest262File } = require('./metadataParser');

const REPO_ROOT = path.resolve(__dirname, '..', '..');
const DEFAULT_TIMEOUT_SECONDS = 20;
const DEFAULT_COMPILE_TIMEOUT_SECONDS = 60;
const VALID_VARIANTS = new Set(['non-strict', 'strict']);

function printHelp() {
  console.log([
    'Usage: node scripts/test262/runMvp.js [options]',
    '',
    'Run the plain synchronous script MVP slice of test262 through js2il.',
    '',
    'Options:',
    '  --pin <path>               Override the pin file used to resolve test262.',
    '  --root <path>              Use an explicit test262 checkout instead of the managed cache.',
    '  --force                    Reuse an out-of-date managed checkout instead of failing validation.',
    '  --js2il <path>             Override the js2il executable or Js2IL.dll path.',
    '  --output <path>            Directory for generated case inputs and compiled assemblies.',
    '  --timeout <seconds>        Per-test execution timeout (default: 20).',
    '  --compile-timeout <secs>   Per-test compile timeout (default: 60).',
    '  --file <relative-path>     Run one specific test262 file.',
    '  --filter <substring>       Keep only test files whose relative path contains the substring.',
    '  --limit <count>            Limit the number of selected test files before variant expansion.',
    '  --variant <name>           Restrict execution to non-strict or strict for the selected file(s).',
    '  --list                     List the selected MVP cases without compiling or running them.',
    '  --help                     Show this help.',
  ].join('\n'));
}

function parseArgs(argv) {
  const args = {
    pin: bootstrap.defaultPinPath(),
    root: null,
    force: false,
    js2il: null,
    output: null,
    timeoutSeconds: DEFAULT_TIMEOUT_SECONDS,
    compileTimeoutSeconds: DEFAULT_COMPILE_TIMEOUT_SECONDS,
    file: null,
    filter: null,
    limit: null,
    variant: null,
    list: false,
    help: false,
  };

  for (let i = 0; i < argv.length; i++) {
    const argument = argv[i];
    switch (argument) {
      case '--help':
      case '-h':
        args.help = true;
        break;
      case '--pin':
        args.pin = requireValue(argv, ++i, '--pin');
        break;
      case '--root':
        args.root = requireValue(argv, ++i, '--root');
        break;
      case '--force':
        args.force = true;
        break;
      case '--js2il':
        args.js2il = requireValue(argv, ++i, '--js2il');
        break;
      case '--output':
        args.output = requireValue(argv, ++i, '--output');
        break;
      case '--timeout':
        args.timeoutSeconds = parsePositiveInteger(requireValue(argv, ++i, '--timeout'), '--timeout');
        break;
      case '--compile-timeout':
        args.compileTimeoutSeconds = parsePositiveInteger(requireValue(argv, ++i, '--compile-timeout'), '--compile-timeout');
        break;
      case '--file':
        args.file = normalizeRelativeTestPath(requireValue(argv, ++i, '--file'));
        break;
      case '--filter':
        args.filter = requireValue(argv, ++i, '--filter');
        break;
      case '--limit':
        args.limit = parsePositiveInteger(requireValue(argv, ++i, '--limit'), '--limit');
        break;
      case '--variant':
        args.variant = requireValue(argv, ++i, '--variant');
        break;
      case '--list':
        args.list = true;
        break;
      default:
        throw new Error(`Unknown argument: ${argument}`);
    }
  }

  if (args.file && args.filter) {
    throw new Error('--file and --filter cannot be used together.');
  }

  if (args.variant !== null && !VALID_VARIANTS.has(args.variant)) {
    throw new Error(`Unsupported variant '${args.variant}'. Expected 'non-strict' or 'strict'.`);
  }

  return args;
}

function requireValue(argv, index, optionName) {
  if (index >= argv.length || !argv[index]) {
    throw new Error(`${optionName} requires a value.`);
  }

  return argv[index];
}

function parsePositiveInteger(value, optionName) {
  const parsed = Number.parseInt(value, 10);
  if (!Number.isInteger(parsed) || parsed <= 0) {
    throw new Error(`${optionName} must be a positive integer.`);
  }

  return parsed;
}

function normalizePortablePath(value) {
  return value.replace(/\\/g, '/');
}

function normalizeRelativeTestPath(value) {
  let normalized = normalizePortablePath(value.trim());
  while (normalized.startsWith('./')) {
    normalized = normalized.slice(2);
  }

  while (normalized.startsWith('/')) {
    normalized = normalized.slice(1);
  }

  return normalized;
}

function defaultOutputRoot() {
  const stamp = new Date().toISOString().replace(/[:.]/g, '-');
  return path.join(REPO_ROOT, 'artifacts', 'test262', 'runs', `${stamp}-${process.pid}`);
}

function collectCandidateFiles(rootPath, pin) {
  const files = [];
  const includeDirectories = Array.isArray(pin.includeDirectories) ? pin.includeDirectories : [];

  for (let i = 0; i < includeDirectories.length; i++) {
    const includeDirectory = normalizeRelativeTestPath(includeDirectories[i]);
    if (!includeDirectory.startsWith('test/')) {
      continue;
    }

    const absoluteDirectory = path.join(rootPath, ...includeDirectory.split('/'));
    collectJavaScriptFiles(absoluteDirectory, rootPath, files);
  }

  files.sort();
  return files;
}

function collectJavaScriptFiles(directoryPath, rootPath, files) {
  if (!fs.existsSync(directoryPath)) {
    return;
  }

  const entries = fs.readdirSync(directoryPath, { withFileTypes: true })
    .sort((left, right) => left.name.localeCompare(right.name));

  for (let i = 0; i < entries.length; i++) {
    const entry = entries[i];
    const absolutePath = path.join(directoryPath, entry.name);

    if (entry.isDirectory()) {
      collectJavaScriptFiles(absolutePath, rootPath, files);
      continue;
    }

    if (!entry.isFile() || !entry.name.endsWith('.js')) {
      continue;
    }

    files.push(normalizeRelativeTestPath(path.relative(rootPath, absolutePath)));
  }
}

function matchPathExclusion(relativePath, pin) {
  const exclusions = Array.isArray(pin.excludedFromMvp) ? pin.excludedFromMvp : [];
  const normalizedRelativePath = normalizeRelativeTestPath(relativePath);

  for (let i = 0; i < exclusions.length; i++) {
    const exclusion = exclusions[i];
    if (typeof exclusion !== 'string' || exclusion.startsWith('frontmatter:')) {
      continue;
    }

    const normalizedExclusion = normalizeRelativeTestPath(exclusion);
    if (normalizedExclusion.endsWith('/**')) {
      const prefix = normalizedExclusion.slice(0, -3);
      if (normalizedRelativePath === prefix || normalizedRelativePath.startsWith(prefix + '/')) {
        return exclusion;
      }
      continue;
    }

    if (normalizedRelativePath === normalizedExclusion) {
      return exclusion;
    }
  }

  return null;
}

function determineVariants(execution, requestedVariant, relativePath) {
  let variants;
  switch (execution.strictMode) {
    case 'strict-only':
      variants = ['strict'];
      break;
    case 'non-strict-only':
      variants = ['non-strict'];
      break;
    case 'strict-and-non-strict':
      variants = ['non-strict', 'strict'];
      break;
    default:
      throw new Error(`Cannot determine runnable variants for ${relativePath}: strict mode '${execution.strictMode}' is not executable in the MVP runner.`);
  }

  if (requestedVariant === null) {
    return variants;
  }

  if (variants.indexOf(requestedVariant) >= 0) {
    return [requestedVariant];
  }

  throw new Error(`Variant '${requestedVariant}' is not valid for ${relativePath}; supported variants: ${variants.join(', ')}.`);
}

function issueCodes(issues) {
  if (!Array.isArray(issues) || issues.length === 0) {
    return [];
  }

  return issues
    .map(issue => issue && issue.code ? issue.code : 'unknown-issue')
    .filter((value, index, array) => array.indexOf(value) === index)
    .sort();
}

function createExecutionPlan(rootPath, pin, args) {
  const discoveredFiles = collectCandidateFiles(rootPath, pin);
  let selectedFiles = discoveredFiles.slice();

  if (args.file) {
    selectedFiles = selectedFiles.filter(relativePath => relativePath === args.file);
    if (selectedFiles.length === 0) {
      throw new Error(`Requested test file was not found in the pinned intake: ${args.file}`);
    }
  }

  if (args.filter) {
    selectedFiles = selectedFiles.filter(relativePath => relativePath.indexOf(args.filter) >= 0);
  }

  if (args.limit !== null) {
    selectedFiles = selectedFiles.slice(0, args.limit);
  }

  const cases = [];
  const skipped = [];

  for (let i = 0; i < selectedFiles.length; i++) {
    const relativePath = selectedFiles[i];
    const exclusion = matchPathExclusion(relativePath, pin);
    if (exclusion !== null) {
      skipped.push({
        relativePath,
        reasons: [`pin-excluded:${exclusion}`],
      });
      continue;
    }

    const absolutePath = path.join(rootPath, ...relativePath.split('/'));
    const parsed = parseTest262File(absolutePath);
    const reasons = issueCodes(parsed.unsupported).concat(issueCodes(parsed.mvpBlockers));
    if (reasons.length > 0) {
      skipped.push({ relativePath, reasons });
      continue;
    }

    const variants = determineVariants(parsed.execution, args.variant, relativePath);
    for (let variantIndex = 0; variantIndex < variants.length; variantIndex++) {
      cases.push({
        relativePath,
        absolutePath,
        variant: variants[variantIndex],
        metadata: parsed,
      });
    }
  }

  return {
    discoveredCount: discoveredFiles.length,
    selectedFileCount: selectedFiles.length,
    cases,
    skipped,
  };
}

function resolveHarnessIncludePath(rootPath, includeName) {
  let normalized = normalizeRelativeTestPath(includeName);
  if (normalized.startsWith('harness/')) {
    normalized = normalized.slice('harness/'.length);
  }

  return path.join(rootPath, 'harness', ...normalized.split('/'));
}

function buildCompositeSource(rootPath, testCase) {
  const parts = [];
  if (testCase.variant === 'strict') {
    parts.push('"use strict";');
  }

  const harnessIncludes = testCase.metadata.execution.harnessIncludes || [];
  for (let i = 0; i < harnessIncludes.length; i++) {
    const includeName = harnessIncludes[i];
    const includePath = resolveHarnessIncludePath(rootPath, includeName);
    if (!fs.existsSync(includePath)) {
      throw new Error(`Harness include '${includeName}' was not found for ${testCase.relativePath}.`);
    }

    parts.push(fs.readFileSync(includePath, 'utf8'));
  }

  parts.push(fs.readFileSync(testCase.absolutePath, 'utf8'));
  return parts.join('\n\n');
}

function findJs2IL(override) {
  if (override) {
    if (override.endsWith('.dll')) {
      return { type: 'dll', path: override };
    }

    return { type: 'exec', path: override };
  }

  if (process.env.JS2IL_DLL) {
    return { type: 'dll', path: process.env.JS2IL_DLL };
  }

  for (const configuration of ['Release', 'Debug']) {
    const dllPath = path.join(REPO_ROOT, 'src', 'Cli', 'bin', configuration, 'net10.0', 'Js2IL.dll');
    if (fs.existsSync(dllPath)) {
      return { type: 'dll', path: dllPath };
    }
  }

  const lookup = spawnSync(process.platform === 'win32' ? 'where' : 'which', ['js2il'], { encoding: 'utf8' });
  if (lookup.status === 0 && lookup.stdout.trim()) {
    return { type: 'exec', path: 'js2il' };
  }

  return { type: 'run', path: path.join(REPO_ROOT, 'src', 'Cli', 'Js2IL.csproj') };
}

function compileWithJs2IL(jsFilePath, outputDirectory, js2il, timeoutMs, extraArgs) {
  const fileName = path.basename(jsFilePath, '.js');
  const compileOutDirectory = path.join(outputDirectory, fileName);
  fs.mkdirSync(compileOutDirectory, { recursive: true });
  const compilerArgs = Array.isArray(extraArgs) ? extraArgs : [];

  let command;
  let args;
  if (js2il.type === 'dll') {
    command = 'dotnet';
    args = [js2il.path, jsFilePath, '-o', compileOutDirectory].concat(compilerArgs);
  } else if (js2il.type === 'exec') {
    command = js2il.path;
    args = [jsFilePath, '-o', compileOutDirectory].concat(compilerArgs);
  } else {
    command = 'dotnet';
    args = ['run', '--project', js2il.path, '--', jsFilePath, '-o', compileOutDirectory].concat(compilerArgs);
  }

  const startedAt = Date.now();
  const result = spawnSync(command, args, {
    encoding: 'utf8',
    timeout: timeoutMs,
    maxBuffer: 4 * 1024 * 1024,
  });
  const durationMs = Date.now() - startedAt;

  if (result.error && result.error.code === 'ETIMEDOUT') {
    return { success: false, dllPath: null, stderr: 'COMPILE_TIMEOUT', durationMs };
  }

  if (result.status !== 0) {
    return {
      success: false,
      dllPath: null,
      stderr: (result.stderr || '') + (result.stdout || ''),
      durationMs,
    };
  }

  const dllPath = path.join(compileOutDirectory, fileName + '.dll');
  if (!fs.existsSync(dllPath)) {
    return {
      success: false,
      dllPath: null,
      stderr: 'DLL not found after compilation',
      durationMs,
    };
  }

  return {
    success: true,
    dllPath,
    stderr: result.stderr || '',
    durationMs,
  };
}

function runProcess(command, args, timeoutMs) {
  const startedAt = Date.now();
  const result = spawnSync(command, args, {
    encoding: 'utf8',
    timeout: timeoutMs,
    maxBuffer: 4 * 1024 * 1024,
  });
  const durationMs = Date.now() - startedAt;

  if (result.error && result.error.code === 'ETIMEDOUT') {
    return {
      stdout: '',
      stderr: 'TIMEOUT',
      exitCode: -1,
      timedOut: true,
      durationMs,
    };
  }

  return {
    stdout: result.stdout || '',
    stderr: result.stderr || '',
    exitCode: result.status !== null ? result.status : -1,
    timedOut: false,
    durationMs,
  };
}

function runDotnet(dllPath, timeoutMs) {
  return runProcess('dotnet', [dllPath], timeoutMs);
}

function extractJsError(stderr) {
  const dotnetMatch = stderr.match(/JavaScriptException:\s*(.+?)(?:\r?\n|$)/);
  if (dotnetMatch) {
    return dotnetMatch[1].trim();
  }

  const nodeMatch = stderr.match(/^([A-Za-z]+Error|Error|EvalError|RangeError|ReferenceError|SyntaxError|TypeError|URIError):\s*(.+)/m);
  if (nodeMatch) {
    return `${nodeMatch[1]}: ${nodeMatch[2].trim()}`;
  }

  const genericMatch = stderr.match(/^([A-Za-z]+Error|Error):\s*(.+)/m);
  if (genericMatch) {
    return `${genericMatch[1]}: ${genericMatch[2].trim()}`;
  }

  return '';
}

function summarizeFailureText(text) {
  const normalized = text.replace(/\r\n/g, '\n').replace(/\r/g, '\n');
  const jsError = extractJsError(normalized);
  if (jsError) {
    return jsError;
  }

  const lines = normalized
    .split('\n')
    .map(line => line.trim())
    .filter(line => line.length > 0);

  if (lines.length === 0) {
    return 'no error output';
  }

  return lines[0];
}

function containsExpectedError(text, expectedType) {
  if (!expectedType) {
    return true;
  }

  return text.toLowerCase().indexOf(String(expectedType).toLowerCase()) >= 0;
}

function expectedOutcomeLabel(testCase) {
  const negative = testCase.metadata.negative;
  if (!negative) {
    return 'positive';
  }

  return `negative(${negative.phase}:${negative.type})`;
}

function createCaseDirectory(outputRoot, testCase) {
  const relativeWithoutExtension = testCase.relativePath.replace(/\.js$/i, '');
  return path.join(outputRoot, 'cases', ...relativeWithoutExtension.split('/'), testCase.variant);
}

function quoteForDisplay(value) {
  return `"${String(value).replace(/"/g, '\\"')}"`;
}

function createReproCommand(rootPath, testCase, args) {
  const command = [
    'node',
    'scripts/test262/runMvp.js',
    '--root',
    quoteForDisplay(rootPath),
    '--file',
    quoteForDisplay(testCase.relativePath),
    '--variant',
    testCase.variant,
  ];

  if (args.js2il) {
    command.push('--js2il', quoteForDisplay(args.js2il));
  }

  return command.join(' ');
}

function evaluateCase(rootPath, outputRoot, testCase, js2il, args) {
  const caseDirectory = createCaseDirectory(outputRoot, testCase);
  fs.mkdirSync(caseDirectory, { recursive: true });

  const compositeSourcePath = path.join(caseDirectory, 'input.js');
  fs.writeFileSync(compositeSourcePath, buildCompositeSource(rootPath, testCase), 'utf8');
  const compilerArgs = ['--strictMode', 'Ignore'];

  const compileResult = compileWithJs2IL(
    compositeSourcePath,
    path.join(caseDirectory, 'compile'),
    js2il,
    args.compileTimeoutSeconds * 1000,
    compilerArgs,
  );

  const negative = testCase.metadata.negative;
  if (negative && negative.phase === 'parse') {
    // JS2IL's front-end currently reports parse/early failures with compiler diagnostics rather
    // than a normalized JS error object, so the MVP parse-negative contract is compile-time failure.
    if (!compileResult.success && compileResult.stderr !== 'COMPILE_TIMEOUT' && compileResult.stderr !== 'DLL not found after compilation') {
      return {
        status: 'pass',
        testCase,
        detail: expectedOutcomeLabel(testCase),
      };
    }

    if (!compileResult.success) {
      return {
        status: 'fail',
        testCase,
        detail: `expected ${expectedOutcomeLabel(testCase)} but compilation failed with ${summarizeFailureText(compileResult.stderr)}`,
        repro: createReproCommand(rootPath, testCase, args),
      };
    }

    return {
      status: 'fail',
      testCase,
      detail: `expected ${expectedOutcomeLabel(testCase)} but compilation succeeded`,
      repro: createReproCommand(rootPath, testCase, args),
    };
  }

  if (!compileResult.success) {
    return {
      status: 'fail',
      testCase,
      detail: `compilation failed: ${summarizeFailureText(compileResult.stderr)}`,
      repro: createReproCommand(rootPath, testCase, args),
    };
  }

  const executionResult = runDotnet(compileResult.dllPath, args.timeoutSeconds * 1000);
  const combinedExecutionOutput = `${executionResult.stderr}\n${executionResult.stdout}`;

  if (negative && negative.phase === 'runtime') {
    if (!executionResult.timedOut && executionResult.exitCode !== 0 && containsExpectedError(combinedExecutionOutput, negative.type)) {
      return {
        status: 'pass',
        testCase,
        detail: expectedOutcomeLabel(testCase),
      };
    }

    const runtimeDetail = executionResult.timedOut
      ? 'runtime timed out'
      : executionResult.exitCode === 0
        ? 'runtime completed successfully'
        : summarizeFailureText(combinedExecutionOutput);

    return {
      status: 'fail',
      testCase,
      detail: `expected ${expectedOutcomeLabel(testCase)} but observed ${runtimeDetail}`,
      repro: createReproCommand(rootPath, testCase, args),
    };
  }

  if (executionResult.timedOut) {
    return {
      status: 'fail',
      testCase,
      detail: 'runtime timed out',
      repro: createReproCommand(rootPath, testCase, args),
    };
  }

  if (executionResult.exitCode !== 0) {
    return {
      status: 'fail',
      testCase,
      detail: `runtime failed: ${summarizeFailureText(combinedExecutionOutput)}`,
      repro: createReproCommand(rootPath, testCase, args),
    };
  }

  return {
    status: 'pass',
    testCase,
    detail: expectedOutcomeLabel(testCase),
  };
}

function formatCaseStatus(result) {
  return `${result.status.toUpperCase()} ${result.testCase.relativePath} [${result.testCase.variant}] ${result.detail}`;
}

function formatSkipStatus(skip) {
  return `SKIP ${skip.relativePath} [${skip.reasons.join(', ')}]`;
}

function printPlanHeader(rootPath, outputRoot, js2il, plan, args) {
  console.log(`root ${rootPath}`);
  console.log(`js2il ${js2il.path}`);
  if (!args.list) {
    console.log(`output ${outputRoot}`);
  }
  console.log(`discovered ${plan.discoveredCount}`);
  console.log(`selected-files ${plan.selectedFileCount}`);
  console.log(`selected-cases ${plan.cases.length}`);
}

function runMvp(argv) {
  const args = parseArgs(argv);
  if (args.help) {
    printHelp();
    return 0;
  }

  const pinPath = path.resolve(args.pin);
  const pin = bootstrap.loadPin(pinPath);
  const resolvedRoot = bootstrap.resolveBootstrapRoot(pinPath, pin, args);
  const rootPath = resolvedRoot.rootPath;
  const outputRoot = path.resolve(args.output || defaultOutputRoot());
  const js2il = findJs2IL(args.js2il);
  const plan = createExecutionPlan(rootPath, pin, args);

  printPlanHeader(rootPath, outputRoot, js2il, plan, args);

  for (let i = 0; i < plan.skipped.length; i++) {
    console.log(formatSkipStatus(plan.skipped[i]));
  }

  if (args.list) {
    for (let i = 0; i < plan.cases.length; i++) {
      const testCase = plan.cases[i];
      console.log(`LIST ${testCase.relativePath} [${testCase.variant}] ${expectedOutcomeLabel(testCase)}`);
    }

    console.log(`SUMMARY passed=0 failed=0 skipped=${plan.skipped.length} listed=${plan.cases.length}`);
    return 0;
  }

  fs.mkdirSync(outputRoot, { recursive: true });

  let passed = 0;
  let failed = 0;
  for (let i = 0; i < plan.cases.length; i++) {
    const result = evaluateCase(rootPath, outputRoot, plan.cases[i], js2il, args);
    console.log(formatCaseStatus(result));
    if (result.status === 'pass') {
      passed++;
      continue;
    }

    failed++;
    if (result.repro) {
      console.log(`REPRO ${result.repro}`);
    }
  }

  console.log(`SUMMARY passed=${passed} failed=${failed} skipped=${plan.skipped.length} selected=${plan.cases.length}`);
  return failed === 0 ? 0 : 1;
}

module.exports = {
  createExecutionPlan,
  determineVariants,
  findJs2IL,
  parseArgs,
  runMvp,
};

if (require.main === module) {
  try {
    process.exitCode = runMvp(process.argv.slice(2));
  } catch (error) {
    const message = error && error.message ? error.message : String(error);
    console.error(message);
    process.exitCode = 1;
  }
}
