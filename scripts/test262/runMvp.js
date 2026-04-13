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
const SUMMARY_FILE_NAME = 'summary.json';
const SUMMARY_SCHEMA_VERSION = 1;
const RESULT_KIND_ORDER = [
  'pass',
  'compile-rejection',
  'runtime-rejection',
  'wrong-phase',
  'wrong-error-kind',
  'runtime-mismatch',
  'timeout',
  'runner-error',
  'unsupported-requirement',
  'skipped-by-policy',
];
const VERDICT_ORDER = ['matched', 'unexpected', 'not-run'];
const RESULT_LABELS = {
  pass: 'PASS',
  'compile-rejection': 'COMPILE-REJECTION',
  'runtime-rejection': 'RUNTIME-REJECTION',
  'wrong-phase': 'WRONG-PHASE',
  'wrong-error-kind': 'WRONG-ERROR-KIND',
  'runtime-mismatch': 'RUNTIME-MISMATCH',
  timeout: 'TIMEOUT',
  'runner-error': 'RUNNER-ERROR',
  'unsupported-requirement': 'UNSUPPORTED',
  'skipped-by-policy': 'SKIP-POLICY',
};

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

function defaultSummaryPath(outputRoot) {
  return path.join(outputRoot, SUMMARY_FILE_NAME);
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

function createResultId(relativePath, variant) {
  return variant ? `${relativePath}#${variant}` : `${relativePath}#all`;
}

function createReproData(relativePath, variant) {
  return variant ? { file: relativePath, variant } : { file: relativePath };
}

function createClassification(kind, verdict, phase, detail) {
  return {
    kind,
    verdict,
    phase,
    detail,
  };
}

function createObservation(status, overrides) {
  return Object.assign(
    {
      status,
      exitCode: null,
      errorType: null,
      summary: null,
    },
    overrides || {}
  );
}

function createResult(relativePath, variant, metadata, classification, reasons, observed, repro) {
  return {
    id: createResultId(relativePath, variant),
    relativePath,
    variant,
    expected: buildExpectedOutcome(metadata),
    classification,
    reasons: Array.isArray(reasons) ? reasons : [],
    observed,
    repro: repro || null,
  };
}

function buildExpectedOutcome(metadata) {
  if (!metadata || !metadata.negative) {
    return {
      category: 'positive',
      phase: null,
      errorType: null,
      validatesErrorType: true,
    };
  }

  return {
    category: 'negative',
    phase: metadata.negative.phase,
    errorType: metadata.negative.type,
    validatesErrorType: metadata.negative.phase !== 'parse',
  };
}

function expectedOutcomeLabel(testCase) {
  const negative = testCase.metadata.negative;
  if (!negative) {
    return 'positive';
  }

  return `negative(${negative.phase}:${negative.type})`;
}

function createPathExclusionReason(exclusion) {
  return {
    code: 'path-excluded',
    source: 'excludedFromMvp',
    reason: `Path matched excludedFromMvp entry '${exclusion}'.`,
    pattern: exclusion,
  };
}

function createNotRunResult(relativePath, metadata, kind, reasons, detail) {
  return createResult(
    relativePath,
    null,
    metadata,
    createClassification(kind, 'not-run', 'selection', detail),
    reasons,
    {
      compile: createObservation('not-run'),
      runtime: createObservation('not-run'),
    },
    null
  );
}

function normalizeReasonEntries(reasons) {
  const entries = Array.isArray(reasons) ? reasons : [];
  const seen = new Set();
  const normalized = [];

  for (let i = 0; i < entries.length; i++) {
    const entry = entries[i];
    if (!entry || typeof entry !== 'object') {
      continue;
    }

    const normalizedEntry = {
      code: entry.code || 'unknown-issue',
      source: entry.source || '<unknown>',
      reason: entry.reason || '',
    };

    if (entry.pattern) {
      normalizedEntry.pattern = entry.pattern;
    }

    const key = JSON.stringify(normalizedEntry);
    if (seen.has(key)) {
      continue;
    }

    seen.add(key);
    normalized.push(normalizedEntry);
  }

  return normalized;
}

function createSelectionResult(relativePath, metadata, kind, reasons, detail) {
  return createNotRunResult(relativePath, metadata, kind, normalizeReasonEntries(reasons), detail);
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
      skipped.push(
        createSelectionResult(
          relativePath,
          null,
          'skipped-by-policy',
          [createPathExclusionReason(exclusion)],
          `excluded by policy (${exclusion})`
        )
      );
      continue;
    }

    const absolutePath = path.join(rootPath, ...relativePath.split('/'));
    const parsed = parseTest262File(absolutePath);
    const reasons = normalizeReasonEntries(parsed.unsupported.concat(parsed.mvpBlockers));
    if (reasons.length > 0) {
      skipped.push(
        createSelectionResult(
          relativePath,
          parsed,
          'unsupported-requirement',
          reasons,
          'unsupported requirement'
        )
      );
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

  const dotnetUnhandledMatch = stderr.match(/Unhandled exception\.\s*([A-Za-z]+Error|Error|EvalError|RangeError|ReferenceError|SyntaxError|TypeError|URIError):\s*(.+)/m);
  if (dotnetUnhandledMatch) {
    return `${dotnetUnhandledMatch[1]}: ${dotnetUnhandledMatch[2].trim()}`;
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

function normalizeDiagnosticText(text, diagnosticContext) {
  let normalized = String(text || '')
    .replace(/\r\n/g, '\n')
    .replace(/\r/g, '\n')
    .replace(/\\/g, '/')
    .trim();

  const replacements = Array.isArray(diagnosticContext) ? diagnosticContext : [];
  for (let i = 0; i < replacements.length; i++) {
    const replacement = replacements[i];
    if (!replacement || !replacement.from) {
      continue;
    }

    normalized = normalized.split(replacement.from).join(replacement.to);
  }

  return normalized.trim();
}

function createDiagnosticContext(rootPath, outputRoot) {
  return [
    { from: normalizePortablePath(outputRoot), to: '<output>' },
    { from: normalizePortablePath(rootPath), to: '<test262-root>' },
    { from: normalizePortablePath(REPO_ROOT), to: '<repo>' },
  ].sort((left, right) => right.from.length - left.from.length);
}

function extractErrorType(text) {
  const jsError = extractJsError(text);
  if (!jsError) {
    return null;
  }

  const match = /^([A-Za-z]+Error|Error)\b/.exec(jsError);
  return match ? match[1] : null;
}

function summarizeFailureText(text, diagnosticContext) {
  const normalized = normalizeDiagnosticText(text, diagnosticContext);
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

function matchesExpectedErrorType(observedType, expectedType) {
  if (!expectedType) {
    return true;
  }

  if (!observedType) {
    return false;
  }

  return String(observedType).toLowerCase() === String(expectedType).toLowerCase();
}

function createCaseDirectory(outputRoot, testCase) {
  const relativeWithoutExtension = testCase.relativePath.replace(/\.js$/i, '');
  return path.join(outputRoot, 'cases', ...relativeWithoutExtension.split('/'), testCase.variant);
}

function quoteForDisplay(value) {
  return `"${String(value).replace(/"/g, '\\"')}"`;
}

function createReproCommand(rootPath, repro, args) {
  const command = [
    'node',
    'scripts/test262/runMvp.js',
    '--root',
    quoteForDisplay(rootPath),
    '--file',
    quoteForDisplay(repro.file),
  ];

  if (repro.variant) {
    command.push('--variant', repro.variant);
  }

  if (args.js2il) {
    command.push('--js2il', quoteForDisplay(args.js2il));
  }

  return command.join(' ');
}

function createMatchedCaseResult(testCase, kind, phase, detail, observed) {
  return createResult(
    testCase.relativePath,
    testCase.variant,
    testCase.metadata,
    createClassification(kind, 'matched', phase, detail),
    [],
    observed,
    createReproData(testCase.relativePath, testCase.variant)
  );
}

function createUnexpectedCaseResult(testCase, kind, phase, detail, observed) {
  return createResult(
    testCase.relativePath,
    testCase.variant,
    testCase.metadata,
    createClassification(kind, 'unexpected', phase, detail),
    [],
    observed,
    createReproData(testCase.relativePath, testCase.variant)
  );
}

function evaluateCase(rootPath, outputRoot, testCase, js2il, args) {
  const caseDirectory = createCaseDirectory(outputRoot, testCase);
  fs.mkdirSync(caseDirectory, { recursive: true });

  const compositeSourcePath = path.join(caseDirectory, 'input.js');
  fs.writeFileSync(compositeSourcePath, buildCompositeSource(rootPath, testCase), 'utf8');
  const compilerArgs = ['--strictMode', 'Ignore'];
  const diagnosticContext = createDiagnosticContext(rootPath, outputRoot);

  const compileResult = compileWithJs2IL(
    compositeSourcePath,
    path.join(caseDirectory, 'compile'),
    js2il,
    args.compileTimeoutSeconds * 1000,
    compilerArgs,
  );

  const negative = testCase.metadata.negative;
  const expectedLabel = expectedOutcomeLabel(testCase);

  if (!compileResult.success) {
    if (compileResult.stderr === 'COMPILE_TIMEOUT') {
      return createUnexpectedCaseResult(
        testCase,
        'timeout',
        'compile',
        'compilation timed out',
        {
          compile: createObservation('timeout', { summary: 'COMPILE_TIMEOUT' }),
          runtime: createObservation('not-run'),
        }
      );
    }

    if (compileResult.stderr === 'DLL not found after compilation') {
      return createUnexpectedCaseResult(
        testCase,
        'runner-error',
        'compile',
        'compiler completed without emitting a DLL',
        {
          compile: createObservation('runner-error', { summary: 'DLL not found after compilation' }),
          runtime: createObservation('not-run'),
        }
      );
    }

    const compileSummary = summarizeFailureText(compileResult.stderr, diagnosticContext);
    const compileErrorType = extractErrorType(normalizeDiagnosticText(compileResult.stderr, diagnosticContext));
    const observed = {
      compile: createObservation('rejected', {
        errorType: compileErrorType,
        summary: compileSummary,
      }),
      runtime: createObservation('not-run'),
    };

    if (negative && negative.phase === 'parse') {
      // Parse negatives are currently phase-classified at compile time. JS2IL does not yet surface
      // normalized parser error objects, so the MVP baseline records phase match but not parse error kind.
      return createMatchedCaseResult(
        testCase,
        'compile-rejection',
        'compile',
        `matched ${expectedLabel}`,
        observed
      );
    }

    if (negative && negative.phase === 'runtime') {
      return createUnexpectedCaseResult(
        testCase,
        'wrong-phase',
        'compile',
        `expected ${expectedLabel} but rejection happened during compile`,
        observed
      );
    }

    return createUnexpectedCaseResult(
      testCase,
      'compile-rejection',
      'compile',
      `expected ${expectedLabel} but compilation rejected (${compileSummary})`,
      observed
    );
  }

  const executionResult = runDotnet(compileResult.dllPath, args.timeoutSeconds * 1000);
  const combinedExecutionOutput = `${executionResult.stderr}\n${executionResult.stdout}`;
  const runtimeSummary = summarizeFailureText(combinedExecutionOutput, diagnosticContext);
  const runtimeErrorType = extractErrorType(normalizeDiagnosticText(combinedExecutionOutput, diagnosticContext));
  const observed = {
    compile: createObservation('succeeded'),
    runtime: executionResult.timedOut
      ? createObservation('timeout', { summary: 'TIMEOUT' })
      : executionResult.exitCode === 0
        ? createObservation('passed', { exitCode: 0 })
        : createObservation('rejected', {
            exitCode: executionResult.exitCode,
            errorType: runtimeErrorType,
            summary: runtimeSummary,
          }),
  };

  if (negative && negative.phase === 'parse') {
    if (executionResult.timedOut) {
      return createUnexpectedCaseResult(
        testCase,
        'timeout',
        'runtime',
        `expected ${expectedLabel} but runtime timed out after a successful compile`,
        observed
      );
    }

    if (executionResult.exitCode !== 0) {
      const runtimeDetail = runtimeErrorType ? ` (${runtimeErrorType})` : '';
      return createUnexpectedCaseResult(
        testCase,
        'wrong-phase',
        'runtime',
        `expected ${expectedLabel} but rejection moved to runtime${runtimeDetail}`,
        observed
      );
    }

    return createUnexpectedCaseResult(
      testCase,
      'runtime-mismatch',
      'runtime',
      `expected ${expectedLabel} but the test completed successfully`,
      observed
    );
  }

  if (negative && negative.phase === 'runtime') {
    if (executionResult.timedOut) {
      return createUnexpectedCaseResult(
        testCase,
        'timeout',
        'runtime',
        'runtime timed out',
        observed
      );
    }

    if (executionResult.exitCode === 0) {
      return createUnexpectedCaseResult(
        testCase,
        'runtime-mismatch',
        'runtime',
        `expected ${expectedLabel} but the test completed successfully`,
        observed
      );
    }

    if (matchesExpectedErrorType(runtimeErrorType, negative.type)) {
      return createMatchedCaseResult(
        testCase,
        'runtime-rejection',
        'runtime',
        `matched ${expectedLabel}`,
        observed
      );
    }

    const observedType = runtimeErrorType || runtimeSummary;
    return createUnexpectedCaseResult(
      testCase,
      'wrong-error-kind',
      'runtime',
      `expected ${expectedLabel} but observed ${observedType}`,
      observed
    );
  }

  if (executionResult.timedOut) {
    return createUnexpectedCaseResult(
      testCase,
      'timeout',
      'runtime',
      'runtime timed out',
      observed
    );
  }

  if (executionResult.exitCode !== 0) {
    return createUnexpectedCaseResult(
      testCase,
      'runtime-mismatch',
      'runtime',
      `expected ${expectedLabel} but runtime rejected (${runtimeSummary})`,
      observed
    );
  }

  return createMatchedCaseResult(
    testCase,
    'pass',
    'runtime',
    expectedLabel,
    observed
  );
}

function formatReasonCodes(reasons) {
  const codes = [];
  const entries = Array.isArray(reasons) ? reasons : [];

  for (let i = 0; i < entries.length; i++) {
    const reason = entries[i];
    if (!reason) {
      continue;
    }

    if (reason.code === 'path-excluded' && reason.pattern) {
      codes.push(`path-excluded:${reason.pattern}`);
      continue;
    }

    codes.push(reason.code || 'unknown-issue');
  }

  return codes
    .filter((value, index, array) => array.indexOf(value) === index)
    .join(', ');
}

function formatVariant(variant) {
  return variant || 'all';
}

function formatResultStatus(result) {
  const label = RESULT_LABELS[result.classification.kind] || result.classification.kind.toUpperCase();
  const reasonSuffix = result.reasons.length > 0 ? ` [${formatReasonCodes(result.reasons)}]` : '';
  return `${label} ${result.relativePath} [${formatVariant(result.variant)}]${reasonSuffix} ${result.classification.detail}`;
}

function createCountMap(results, selector, orderedKeys) {
  const counts = {};
  const order = Array.isArray(orderedKeys) ? orderedKeys : [];

  for (let i = 0; i < order.length; i++) {
    counts[order[i]] = 0;
  }

  for (let i = 0; i < results.length; i++) {
    const key = selector(results[i]);
    if (!Object.prototype.hasOwnProperty.call(counts, key)) {
      counts[key] = 0;
    }

    counts[key]++;
  }

  const compact = {};
  const keys = order.concat(Object.keys(counts).filter(key => order.indexOf(key) < 0).sort());
  for (let i = 0; i < keys.length; i++) {
    const key = keys[i];
    if (counts[key] > 0) {
      compact[key] = counts[key];
    }
  }

  return compact;
}

function createSummaryReport(pin, args, plan, results, exitCode) {
  return {
    schemaVersion: SUMMARY_SCHEMA_VERSION,
    suite: 'js2il-test262-mvp',
    pin: {
      commit: pin.upstream.commit,
      packageVersion: pin.upstream.packageVersion,
    },
    policy: {
      pathExclusions: Array.isArray(pin.excludedFromMvp) ? pin.excludedFromMvp.slice() : [],
      unsupportedResultSources: ['metadata.unsupported', 'metadata.mvpBlockers'],
      negativeExpectations: {
        parse: {
          expectedObservedPhase: 'compile',
          validateErrorType: false,
        },
        runtime: {
          expectedObservedPhase: 'runtime',
          validateErrorType: true,
        },
      },
      baselineArtifact: {
        fileName: SUMMARY_FILE_NAME,
        format: 'js2il-test262-summary-v1',
      },
    },
    selection: {
      file: args.file,
      filter: args.filter,
      limit: args.limit,
      variant: args.variant,
      discoveredFiles: plan.discoveredCount,
      selectedFiles: plan.selectedFileCount,
      selectedCases: plan.cases.length,
      classifiedEntries: results.length,
    },
    summary: {
      exitCode,
      verdictCounts: createCountMap(results, result => result.classification.verdict, VERDICT_ORDER),
      kindCounts: createCountMap(results, result => result.classification.kind, RESULT_KIND_ORDER),
    },
    results,
  };
}

function writeSummaryReport(summaryPath, report) {
  fs.writeFileSync(summaryPath, JSON.stringify(report, null, 2) + '\n', 'utf8');
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
    console.log(formatResultStatus(plan.skipped[i]));
  }

  if (args.list) {
    for (let i = 0; i < plan.cases.length; i++) {
      const testCase = plan.cases[i];
      console.log(`LIST ${testCase.relativePath} [${testCase.variant}] ${expectedOutcomeLabel(testCase)}`);
    }

    console.log(`SUMMARY matched=0 unexpected=0 not-run=${plan.skipped.length} listed=${plan.cases.length}`);
    return 0;
  }

  fs.mkdirSync(outputRoot, { recursive: true });
  const results = plan.skipped.slice();

  for (let i = 0; i < plan.cases.length; i++) {
    const result = evaluateCase(rootPath, outputRoot, plan.cases[i], js2il, args);
    results.push(result);
    console.log(formatResultStatus(result));
    if (result.classification.verdict === 'unexpected' && result.repro) {
      console.log(`REPRO ${createReproCommand(rootPath, result.repro, args)}`);
    }
  }

  const exitCode = results.some(result => result.classification.verdict === 'unexpected') ? 1 : 0;
  const verdictCounts = createCountMap(results, result => result.classification.verdict, VERDICT_ORDER);
  const kindCounts = createCountMap(results, result => result.classification.kind, RESULT_KIND_ORDER);
  const summaryPath = defaultSummaryPath(outputRoot);
  writeSummaryReport(summaryPath, createSummaryReport(pin, args, plan, results, exitCode));

  const kindSummary = Object.keys(kindCounts)
    .map(key => `${key}=${kindCounts[key]}`)
    .join(' ');
  console.log(
    `SUMMARY matched=${verdictCounts.matched || 0} unexpected=${verdictCounts.unexpected || 0} not-run=${verdictCounts['not-run'] || 0} selected=${plan.cases.length} classified=${results.length}${kindSummary ? ` ${kindSummary}` : ''}`
  );
  console.log(`REPORT ${summaryPath}`);
  return exitCode;
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
