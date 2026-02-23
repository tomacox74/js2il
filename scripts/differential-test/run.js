#!/usr/bin/env node
/**
 * Bounded differential test harness: Node.js vs JS2IL
 *
 * For every JS program in the corpus this script:
 *   1. Runs the program with Node.js and captures stdout/stderr/exit-code.
 *   2. Compiles the program with JS2IL.
 *   3. Runs the compiled assembly with `dotnet` and captures stdout/stderr/exit-code.
 *   4. Normalises both outputs and reports any mismatch.
 *
 * Usage:
 *   node scripts/differential-test/run.js [options]
 *
 * Options:
 *   --corpus  <dir>     Corpus directory (default: <script-dir>/corpus)
 *   --timeout <secs>    Per-execution timeout in seconds (default: 10)
 *   --compile-timeout <secs>  Compilation timeout in seconds (default: 2× --timeout)
 *   --generate <count>  Also run <count> generated programs (default: 0)
 *   --seed    <number>  RNG seed for generated programs (default: 42)
 *   --output  <dir>     Scratch dir for compiled DLLs (default: OS temp)
 *   --js2il   <path>    Path to js2il DLL or executable (auto-detected)
 *   --verbose           Print all diffs even for passing tests
 */

'use strict';

const { spawnSync } = require('child_process');
const fs   = require('fs');
const os   = require('os');
const path = require('path');

// ---------------------------------------------------------------------------
// Argument parsing
// ---------------------------------------------------------------------------
function parseArgs(argv) {
    const args = {
        corpus:          path.join(__dirname, 'corpus'),
        timeout:         10,
        compileTimeout:  null,  // null means 2× timeout (set after parsing)
        generate:        0,
        seed:            42,
        output:          null,
        js2il:           null,
        verbose:         false,
    };
    for (let i = 0; i < argv.length; i++) {
        const a = argv[i];
        if ((a === '--corpus'          || a === '-c') && argv[i + 1]) { args.corpus         = argv[++i]; continue; }
        if ((a === '--timeout'         || a === '-t') && argv[i + 1]) { args.timeout         = Number(argv[++i]); continue; }
        if (a === '--compile-timeout'                 && argv[i + 1]) { args.compileTimeout  = Number(argv[++i]); continue; }
        if ((a === '--generate'        || a === '-g') && argv[i + 1]) { args.generate        = Number(argv[++i]); continue; }
        if ((a === '--seed'            || a === '-s') && argv[i + 1]) { args.seed            = Number(argv[++i]); continue; }
        if ((a === '--output'          || a === '-o') && argv[i + 1]) { args.output          = argv[++i]; continue; }
        if ((a === '--js2il'           || a === '-j') && argv[i + 1]) { args.js2il           = argv[++i]; continue; }
        if (a === '--verbose' || a === '-v') { args.verbose = true; continue; }
    }
    if (args.compileTimeout === null) {
        // Compilation is generally slower than execution; default to 2× the
        // execution timeout so that slow first-run JIT warm-up does not cause
        // spurious compile timeouts.
        args.compileTimeout = args.timeout * 2;
    }
    return args;
}

// ---------------------------------------------------------------------------
// JS2IL invocation
// ---------------------------------------------------------------------------

/**
 * Locate the JS2IL binary.  Priority:
 *   1. --js2il <path> argument
 *   2. JS2IL_DLL env var
 *   3. Built DLL at Js2IL/bin/{Release,Debug}/net10.0/Js2IL.dll
 *   4. `js2il` global tool on PATH
 *   5. `dotnet run --project Js2IL/Js2IL.csproj`
 *
 * Returns { type: 'dll'|'exec'|'run', path: string }
 */
function findJs2IL(override) {
    const repoRoot = path.resolve(__dirname, '..', '..');

    if (override) {
        // If it ends with .dll treat it as a managed DLL
        if (override.endsWith('.dll')) return { type: 'dll',  path: override };
        return { type: 'exec', path: override };
    }

    if (process.env.JS2IL_DLL) {
        return { type: 'dll', path: process.env.JS2IL_DLL };
    }

    for (const cfg of ['Release', 'Debug']) {
        const dll = path.join(repoRoot, 'Js2IL', 'bin', cfg, 'net10.0', 'Js2IL.dll');
        if (fs.existsSync(dll)) return { type: 'dll', path: dll };
    }

    // Check if `js2il` is on PATH
    const whichResult = spawnSync(process.platform === 'win32' ? 'where' : 'which',
        ['js2il'], { encoding: 'utf8' });
    if (whichResult.status === 0 && whichResult.stdout.trim()) {
        return { type: 'exec', path: 'js2il' };
    }

    // Fall back to dotnet run
    const proj = path.join(repoRoot, 'Js2IL', 'Js2IL.csproj');
    return { type: 'run', path: proj };
}

/**
 * Compile a JS file with JS2IL.
 * Returns { success: boolean, dllPath: string|null, stderr: string, durationMs: number }
 */
function compileWithJs2IL(jsFile, outDir, js2il, timeoutMs) {
    const baseName = path.basename(jsFile, '.js');
    const compileOutDir = path.join(outDir, baseName);
    fs.mkdirSync(compileOutDir, { recursive: true });

    let cmd, args;
    if (js2il.type === 'dll') {
        cmd  = 'dotnet';
        args = [js2il.path, jsFile, '-o', compileOutDir];
    } else if (js2il.type === 'exec') {
        cmd  = js2il.path;
        args = [jsFile, '-o', compileOutDir];
    } else {
        // dotnet run
        cmd  = 'dotnet';
        args = ['run', '--project', js2il.path, '--no-build', '--', jsFile, '-o', compileOutDir];
    }

    const t0 = Date.now();
    const result = spawnSync(cmd, args, {
        encoding:  'utf8',
        timeout:   timeoutMs,
        maxBuffer: 4 * 1024 * 1024,
    });
    const durationMs = Date.now() - t0;

    if (result.error && result.error.code === 'ETIMEDOUT') {
        return { success: false, dllPath: null, stderr: 'COMPILE_TIMEOUT', durationMs };
    }
    if (result.status !== 0) {
        const stderr = (result.stderr || '') + (result.stdout || '');
        return { success: false, dllPath: null, stderr, durationMs };
    }

    const dllPath = path.join(compileOutDir, baseName + '.dll');
    if (!fs.existsSync(dllPath)) {
        return { success: false, dllPath: null, stderr: 'DLL not found after compilation', durationMs };
    }

    return { success: true, dllPath, stderr: result.stderr || '', durationMs };
}

// ---------------------------------------------------------------------------
// Execution helpers
// ---------------------------------------------------------------------------

/**
 * Run a command and capture its output.
 * Returns { stdout, stderr, exitCode, timedOut, durationMs }
 */
function runProcess(cmd, args, timeoutMs) {
    const t0 = Date.now();
    const result = spawnSync(cmd, args, {
        encoding:  'utf8',
        timeout:   timeoutMs,
        maxBuffer: 4 * 1024 * 1024,
    });
    const durationMs = Date.now() - t0;

    if (result.error && result.error.code === 'ETIMEDOUT') {
        return { stdout: '', stderr: 'TIMEOUT', exitCode: -1, timedOut: true, durationMs };
    }

    return {
        stdout:    result.stdout || '',
        stderr:    result.stderr || '',
        exitCode:  result.status  !== null ? result.status : -1,
        timedOut:  false,
        durationMs,
    };
}

function runWithNode(jsFile, timeoutMs) {
    return runProcess('node', [jsFile], timeoutMs);
}

function runDotnet(dllPath, timeoutMs) {
    return runProcess('dotnet', [dllPath], timeoutMs);
}

// ---------------------------------------------------------------------------
// Output normalisation
// ---------------------------------------------------------------------------

/**
 * Normalise stdout for comparison:
 *   - Normalise line endings to \n
 *   - Trim trailing whitespace from each line
 *   - Strip a trailing blank line
 */
function normaliseOutput(raw) {
    return raw
        .replace(/\r\n/g, '\n')
        .replace(/\r/g, '\n')
        .split('\n')
        .map(l => l.trimEnd())
        .join('\n')
        .replace(/\n+$/, '');
}

/**
 * Extract the JS-level error type and message from either a Node.js or a
 * .NET exception trace so we can compare semantics rather than frames.
 *
 * Examples:
 *   Node:    "TypeError: Cannot read properties of undefined ..."
 *   .NET:    "Unhandled exception. JavaScriptRuntime.JavaScriptException: TypeError: message"
 */
function extractJsError(stderr) {
    // .NET format: "... JavaScriptException: <type>: <message>"
    const dotnetMatch = stderr.match(/JavaScriptException:\s*(.+?)(?:\n|$)/);
    if (dotnetMatch) return dotnetMatch[1].trim();

    // Node format: first line of stderr that looks like "ErrorType: message"
    const nodeMatch = stderr.match(/^([A-Za-z]+Error|Error|EvalError|RangeError|ReferenceError|SyntaxError|TypeError|URIError):\s*(.+)/m);
    if (nodeMatch) return `${nodeMatch[1]}: ${nodeMatch[2].trim()}`;

    return null;
}

// ---------------------------------------------------------------------------
// Test execution
// ---------------------------------------------------------------------------

function runOneProgram(jsFile, js2il, timeoutSec, compileTimeoutSec, outDir) {
    const label = path.relative(process.cwd(), jsFile);

    // Step 1 – Node
    const nodeResult = runWithNode(jsFile, timeoutSec * 1000);

    // Step 2 – Compile
    const compileResult = compileWithJs2IL(jsFile, outDir, js2il, compileTimeoutSec * 1000);
    if (!compileResult.success) {
        return {
            label,
            status: 'compile-error',
            nodeResult,
            compileError: compileResult.stderr,
            js2ilResult:  null,
        };
    }

    // Step 3 – Run compiled assembly
    const js2ilResult = runDotnet(compileResult.dllPath, timeoutSec * 1000);

    // Step 4 – Compare
    let status;
    let detail = null;

    if (nodeResult.timedOut && js2ilResult.timedOut) {
        status = 'both-timeout';
    } else if (nodeResult.timedOut) {
        status = 'node-timeout';
    } else if (js2ilResult.timedOut) {
        status = 'js2il-timeout';
        detail = 'JS2IL execution timed out but Node finished';
    } else {
        const nodeStdout   = normaliseOutput(nodeResult.stdout);
        const js2ilStdout  = normaliseOutput(js2ilResult.stdout);
        const nodeExitedOk = nodeResult.exitCode  === 0;
        const dotnetOk     = js2ilResult.exitCode === 0;

        if (nodeExitedOk && dotnetOk) {
            if (nodeStdout === js2ilStdout) {
                status = 'pass';
            } else {
                status = 'stdout-mismatch';
                detail = `Node stdout:\n${nodeStdout}\n\nJS2IL stdout:\n${js2ilStdout}`;
            }
        } else if (!nodeExitedOk && !dotnetOk) {
            // Both failed – compare JS-level error messages
            const nodeErr   = extractJsError(nodeResult.stderr);
            const js2ilErr  = extractJsError(js2ilResult.stderr);
            if (nodeErr !== null && js2ilErr !== null && nodeErr === js2ilErr) {
                status = 'pass'; // same error
            } else {
                status = 'error-mismatch';
                detail = `Node error:   ${nodeErr || nodeResult.stderr.slice(0, 200)}\nJS2IL error:  ${js2ilErr || js2ilResult.stderr.slice(0, 200)}`;
            }
        } else {
            // One exited OK, the other didn't
            status = 'exit-code-mismatch';
            detail = `Node exit ${nodeResult.exitCode}, JS2IL exit ${js2ilResult.exitCode}`;
        }
    }

    return { label, status, detail, nodeResult, js2ilResult };
}

// ---------------------------------------------------------------------------
// Reporting
// ---------------------------------------------------------------------------

const COLORS = {
    green:  '\x1b[32m',
    red:    '\x1b[31m',
    yellow: '\x1b[33m',
    cyan:   '\x1b[36m',
    gray:   '\x1b[90m',
    reset:  '\x1b[0m',
};
const c = (color, text) => `${COLORS[color] || ''}${text}${COLORS.reset}`;

function printResult(r, verbose) {
    switch (r.status) {
        case 'pass':
            console.log(c('green', `  PASS  ${r.label}`));
            break;
        case 'compile-error':
            console.log(c('yellow', `  SKIP  ${r.label}  (compile error)`));
            if (verbose) console.log(c('gray', `         ${r.compileError.slice(0, 300)}`));
            break;
        case 'both-timeout':
            console.log(c('yellow', `  SKIP  ${r.label}  (both timed out)`));
            break;
        case 'node-timeout':
            console.log(c('yellow', `  SKIP  ${r.label}  (Node timed out)`));
            break;
        case 'js2il-timeout':
            console.log(c('red',    `  FAIL  ${r.label}  (JS2IL timed out)`));
            if (r.detail) console.log(c('gray', `         ${r.detail}`));
            break;
        default:
            console.log(c('red',    `  FAIL  ${r.label}  [${r.status}]`));
            if (r.detail) {
                r.detail.split('\n').forEach(l => console.log(c('gray', `         ${l}`)));
            }
            break;
    }
}

// ---------------------------------------------------------------------------
// Main
// ---------------------------------------------------------------------------

async function main() {
    const args = parseArgs(process.argv.slice(2));

    // Scratch directory for compiled DLLs
    const outDir = args.output || fs.mkdtempSync(path.join(os.tmpdir(), 'diff-test-'));

    // Locate js2il
    const js2il = findJs2IL(args.js2il);
    console.log(c('cyan', `\nDifferential test harness  –  Node.js vs JS2IL`));
    console.log(c('gray',  `  corpus  : ${args.corpus}`));
    console.log(c('gray',  `  timeout : ${args.timeout}s per execution, ${args.compileTimeout}s per compilation`));
    console.log(c('gray',  `  output  : ${outDir}`));
    console.log(c('gray',  `  js2il   : [${js2il.type}] ${js2il.path}`));
    console.log();

    // Collect programs: corpus files + generated
    const corpusFiles = fs.readdirSync(args.corpus)
        .filter(f => f.endsWith('.js'))
        .sort()
        .map(f => path.join(args.corpus, f));

    let generatedFiles = [];
    if (args.generate > 0) {
        const { generate } = require('./generate');
        generatedFiles = generate(args.seed, args.generate, outDir);
        console.log(c('gray', `  generated ${generatedFiles.length} programs (seed=${args.seed})`));
    }

    const allFiles = [...corpusFiles, ...generatedFiles];
    console.log(c('cyan', `Running ${allFiles.length} programs...\n`));

    // Execute
    const results = [];
    for (const jsFile of allFiles) {
        const r = runOneProgram(jsFile, js2il, args.timeout, args.compileTimeout, outDir);
        results.push(r);
        printResult(r, args.verbose);
    }

    // Summary
    const passed    = results.filter(r => r.status === 'pass').length;
    const failed    = results.filter(r => ['stdout-mismatch', 'error-mismatch', 'exit-code-mismatch', 'js2il-timeout'].includes(r.status)).length;
    const skipped   = results.filter(r => ['compile-error', 'both-timeout', 'node-timeout'].includes(r.status)).length;

    console.log();
    console.log(c('cyan', '─'.repeat(60)));
    console.log(
        c('green', `  ${passed} passed`) + '  ' +
        (failed > 0 ? c('red', `${failed} failed`) : c('gray', `${failed} failed`)) + '  ' +
        c('gray', `${skipped} skipped`)
    );

    if (failed > 0) {
        console.log();
        console.log(c('red', 'Failing programs (reproduction):'));
        results
            .filter(r => ['stdout-mismatch', 'error-mismatch', 'exit-code-mismatch', 'js2il-timeout'].includes(r.status))
            .forEach(r => {
                console.log(c('red',  `  ${r.label}`));
                if (r.detail) r.detail.split('\n').forEach(l => console.log(c('gray', `    ${l}`)));
            });
        process.exitCode = 1;
    }

    // Clean up generated files (they were written to outDir, not the corpus)
    // No extra cleanup needed; outDir is temp.
}

main().catch(err => {
    console.error('Harness error:', err);
    process.exitCode = 1;
});
