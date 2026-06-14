// Performance Comparison Script
// Compares Node.js vs Jint vs Okojo vs YantraJS vs JROC performance on PrimeJavaScript.js

const { execSync } = require('child_process');
const fs = require('fs');
const path = require('path');

// ANSI color codes
const colors = {
    cyan: '\x1b[36m',
    yellow: '\x1b[33m',
    green: '\x1b[32m',
    red: '\x1b[31m',
    gray: '\x1b[90m',
    white: '\x1b[37m',
    reset: '\x1b[0m'
};

function log(message, color = 'white') {
    console.log(`${colors[color]}${message}${colors.reset}`);
}

function header(title) {
    log("╔═══════════════════════════════════════════════════════════════╗", 'cyan');
    log(`║${title.padStart(Math.floor((63 + title.length) / 2)).padEnd(63)}║`, 'cyan');
    log("╚═══════════════════════════════════════════════════════════════╝", 'cyan');
    console.log();
}

// Results storage
const results = [];

// Clean output directories
const jrocOutput = path.join(__dirname, 'out_comparison');
if (fs.existsSync(jrocOutput)) {
    fs.rmSync(jrocOutput, { recursive: true, force: true });
}
fs.mkdirSync(jrocOutput, { recursive: true });

header("JavaScript Runtime Performance Comparison: Prime Sieve");

// ============================================================================
// 1. Node.js Benchmark
// ============================================================================
log("1. Running Node.js benchmark...", 'yellow');
try {
    const nodeStart = Date.now();
    const nodeResult = execSync(`node "${path.join(__dirname, 'PrimeJavaScript.js')}"`, {
        encoding: 'utf8',
        stdio: 'pipe'
    });
    const nodeEnd = Date.now();
    const nodeMs = nodeEnd - nodeStart;

    // Extract passes and duration from Node output
    const nodeMatch = nodeResult.match(/rogiervandam-[^;]+;(\d+);([\d.]+);/);
    if (nodeMatch) {
        const nodePasses = parseInt(nodeMatch[1]);
        const nodeDuration = parseFloat(nodeMatch[2]);
        log(`   [OK] Node.js completed: ${nodeMs} ms total`, 'green');
        log(`        Passes: ${nodePasses} in ${nodeDuration} s`, 'gray');

        results.push({
            runtime: "Node.js",
            totalTimeMs: nodeMs,
            passes: nodePasses,
            passDuration: nodeDuration,
            passesPerSecond: Math.round((nodePasses / nodeDuration) * 100) / 100
        });
    } else {
        log("   [WARN] Could not parse Node.js output", 'yellow');
    }
} catch (error) {
    log(`   [ERROR] Node.js not available or failed: ${error.message}`, 'red');
}

console.log();

// ============================================================================
// 2. Jint Benchmark
// ============================================================================
log("2. Running Jint benchmark...", 'yellow');
try {
    const jintDir = path.join(__dirname, 'JintComparison');
    const jintResult = execSync('dotnet run -c Release', {
        cwd: jintDir,
        encoding: 'utf8',
        stdio: 'pipe'
    });

    // Extract timing from Jint output
    const jintTimeMatch = jintResult.match(/=== Jint Total Execution Time: (\d+)ms \(([\d.]+)s\) ===/);
    let jintMs = 0;
    if (jintTimeMatch) {
        jintMs = parseInt(jintTimeMatch[1]);
    }

    // Extract passes and duration from Jint output
    const jintMatch = jintResult.match(/rogiervandam-jint;(\d+);([\d.]+);/);
    if (jintMatch) {
        const jintPasses = parseInt(jintMatch[1]);
        const jintDuration = parseFloat(jintMatch[2]);
        log(`   [OK] Jint completed: ${jintMs} ms total`, 'green');
        log(`        Passes: ${jintPasses} in ${jintDuration} s`, 'gray');

        results.push({
            runtime: "Jint",
            totalTimeMs: jintMs,
            passes: jintPasses,
            passDuration: jintDuration,
            passesPerSecond: Math.round((jintPasses / jintDuration) * 100) / 100
        });
    }
} catch (error) {
    log(`   [ERROR] Jint failed: ${error.message}`, 'red');
}

console.log();

// ============================================================================
// 3. YantraJS Benchmark
// ============================================================================
log("3. Running YantraJS benchmark...", 'yellow');
try {
    const yantraDir = path.join(__dirname, 'YantraJSComparison');
    const yantraResult = execSync('dotnet run -c Release', {
        cwd: yantraDir,
        encoding: 'utf8',
        stdio: 'pipe'
    });

    // Extract timing from YantraJS output
    const yantraTimeMatch = yantraResult.match(/=== YantraJS Total Execution Time: (\d+)ms \(([\d.]+)s\) ===/);
    let yantraMs = 0;
    if (yantraTimeMatch) {
        yantraMs = parseInt(yantraTimeMatch[1]);
    }

    // Extract passes and duration from YantraJS output
    const yantraMatch = yantraResult.match(/rogiervandam-yantrajs;(\d+);([\d.]+);/);
    if (yantraMatch) {
        const yantraPasses = parseInt(yantraMatch[1]);
        const yantraDuration = parseFloat(yantraMatch[2]);
        log(`   [OK] YantraJS completed: ${yantraMs} ms total`, 'green');
        log(`        Passes: ${yantraPasses} in ${yantraDuration} s`, 'gray');

        results.push({
            runtime: "YantraJS",
            totalTimeMs: yantraMs,
            passes: yantraPasses,
            passDuration: yantraDuration,
            passesPerSecond: Math.round((yantraPasses / yantraDuration) * 100) / 100
        });
    }
} catch (error) {
    log(`   [ERROR] YantraJS failed: ${error.message}`, 'red');
}

console.log();

// ============================================================================
// 4. Okojo Benchmark
// ============================================================================
log("4. Running Okojo benchmark...", 'yellow');
try {
    const okojoDir = path.join(__dirname, 'OkojoComparison');
    const okojoResult = execSync('dotnet run -c Release', {
        cwd: okojoDir,
        encoding: 'utf8',
        stdio: 'pipe'
    });

    const okojoTimeMatch = okojoResult.match(/=== Okojo Total Execution Time: (\d+)ms \(([\d.]+)s\) ===/);
    let okojoMs = 0;
    if (okojoTimeMatch) {
        okojoMs = parseInt(okojoTimeMatch[1]);
    }

    const okojoMatch = okojoResult.match(/rogiervandam-okojo;(\d+);([\d.]+);/);
    if (okojoMatch) {
        const okojoPasses = parseInt(okojoMatch[1]);
        const okojoDuration = parseFloat(okojoMatch[2]);
        log(`   [OK] Okojo completed: ${okojoMs} ms total`, 'green');
        log(`        Passes: ${okojoPasses} in ${okojoDuration} s`, 'gray');

        results.push({
            runtime: "Okojo",
            totalTimeMs: okojoMs,
            passes: okojoPasses,
            passDuration: okojoDuration,
            passesPerSecond: Math.round((okojoPasses / okojoDuration) * 100) / 100
        });
    }
} catch (error) {
    log(`   [ERROR] Okojo failed: ${error.message}`, 'red');
}

console.log();

// ============================================================================
// 5. JROC Benchmark
// ============================================================================
log("5. Compiling with JROC...", 'yellow');
try {
    const compileStart = Date.now();
    execSync(`jroc "${path.join(__dirname, 'PrimeJavaScript.js')}" "${path.join(jrocOutput, 'PrimeJavaScript')}"`, {
        encoding: 'utf8',
        stdio: 'pipe'
    });
    const compileEnd = Date.now();
    const compileDuration = compileEnd - compileStart;
    log(`   [OK] Compilation completed in: ${compileDuration} ms`, 'green');

    console.log();
    log("6. Running JROC compiled benchmark...", 'yellow');
    const jrocStart = Date.now();
    const jrocResult = execSync(`dotnet "${path.join(jrocOutput, 'PrimeJavaScript', 'PrimeJavaScript.dll')}"`, {
        encoding: 'utf8',
        stdio: 'pipe'
    });
    const jrocEnd = Date.now();
    const jrocMs = jrocEnd - jrocStart;

    // Extract passes and duration from JROC output
    const jrocMatch = jrocResult.match(/rogiervandam-[^;]+;(\d+);([\d.]+);/);
    if (jrocMatch) {
        const jrocPasses = parseInt(jrocMatch[1]);
        const jrocDuration = parseFloat(jrocMatch[2]);
        log(`   [OK] JROC completed: ${jrocMs} ms total`, 'green');
        log(`        Passes: ${jrocPasses} in ${jrocDuration} s`, 'gray');

        results.push({
            runtime: "JROC",
            totalTimeMs: jrocMs,
            passes: jrocPasses,
            passDuration: jrocDuration,
            passesPerSecond: Math.round((jrocPasses / jrocDuration) * 100) / 100,
            compileDuration
        });
    }
} catch (error) {
    log(`   [ERROR] JROC failed: ${error.message}`, 'red');
}

console.log();
console.log();

// ============================================================================
// Summary Table
// ============================================================================
header("RESULTS SUMMARY");
console.log();

// Sort by passes (descending) to show fastest first
results.sort((a, b) => b.passes - a.passes);

// Display table
log("Runtime      Total Time    Passes    Pass/Sec    Performance", 'white');
log("─────────────────────────────────────────────────────────────", 'gray');

const baseline = results[0];
results.forEach(result => {
    const runtime = result.runtime.padEnd(12);
    const totalTime = `${result.totalTimeMs} ms`.padEnd(13);
    const passes = result.passes.toString().padEnd(9);
    const passPerSec = result.passesPerSecond.toString().padEnd(11);

    let color = 'white';
    let perf;
    if (result.runtime === baseline.runtime) {
        perf = "[FASTEST]";
        color = 'green';
    } else {
        const speedRatio = Math.round((baseline.passes / result.passes) * 100) / 100;
        perf = `${speedRatio}x slower`;
        color = speedRatio < 2 ? 'yellow' : 'red';
    }

    log(`${runtime} ${totalTime} ${passes} ${passPerSec} ${perf}`, color);
});

console.log();
header("KEY FINDINGS");

// Calculate comparisons
const nodeResult = results.find(r => r.runtime === "Node.js");
const jintResult = results.find(r => r.runtime === "Jint");
const okojoResult = results.find(r => r.runtime === "Okojo");
const yantraResult = results.find(r => r.runtime === "YantraJS");
const jrocResult = results.find(r => r.runtime === "JROC");

if (jrocResult && jintResult) {
    const jrocVsJint = Math.round((jrocResult.passes / jintResult.passes) * 100) / 100;
    log(`  * JROC is ${jrocVsJint}x faster than Jint (interpreted .NET)`, 'green');
}

if (jrocResult && okojoResult) {
    if (jrocResult.passes > okojoResult.passes) {
        const jrocVsOkojo = Math.round((jrocResult.passes / okojoResult.passes) * 100) / 100;
        log(`  * JROC is ${jrocVsOkojo}x faster than Okojo`, 'green');
    } else {
        const okojoVsJroc = Math.round((okojoResult.passes / jrocResult.passes) * 100) / 100;
        log(`  * Okojo is ${okojoVsJroc}x faster than JROC`, 'yellow');
    }
}

if (jrocResult && yantraResult) {
    if (jrocResult.passes > yantraResult.passes) {
        const jrocVsYantra = Math.round((jrocResult.passes / yantraResult.passes) * 100) / 100;
        log(`  * JROC is ${jrocVsYantra}x faster than YantraJS`, 'green');
    } else {
        const yantraVsJroc = Math.round((yantraResult.passes / jrocResult.passes) * 100) / 100;
        log(`  * YantraJS is ${yantraVsJroc}x faster than JROC`, 'yellow');
    }
}

if (jrocResult && nodeResult) {
    if (jrocResult.passes > nodeResult.passes) {
        const jrocVsNode = Math.round((jrocResult.passes / nodeResult.passes) * 100) / 100;
        log(`  * JROC is ${jrocVsNode}x faster than Node.js`, 'green');
    } else {
        const nodeVsJroc = Math.round((nodeResult.passes / jrocResult.passes) * 100) / 100;
        log(`  * Node.js is ${nodeVsJroc}x faster than JROC`, 'yellow');
    }
}

if (nodeResult && jintResult) {
    const nodeVsJint = Math.round((nodeResult.passes / jintResult.passes) * 100) / 100;
    log(`  * Node.js is ${nodeVsJint}x faster than Jint`, 'cyan');
}

if (nodeResult && okojoResult) {
    const nodeVsOkojo = Math.round((nodeResult.passes / okojoResult.passes) * 100) / 100;
    log(`  * Node.js is ${nodeVsOkojo}x faster than Okojo`, 'cyan');
}

if (nodeResult && yantraResult) {
    const nodeVsYantra = Math.round((nodeResult.passes / yantraResult.passes) * 100) / 100;
    log(`  * Node.js is ${nodeVsYantra}x faster than YantraJS`, 'cyan');
}

console.log();
log("Note: More passes = better performance. Test runs for 5 seconds.", 'gray');
if (jrocResult && jrocResult.compileDuration) {
    log(`      JROC compilation time: ${jrocResult.compileDuration} ms (excluded from benchmark)`, 'gray');
}

// ============================================================================
// Write results to JSON file for CI consumption
// ============================================================================
const jsonOutput = {
    timestamp: new Date().toISOString(),
    results: {
        node: nodeResult ? { passes: nodeResult.passes, passesPerSecond: nodeResult.passesPerSecond } : null,
        jroc: jrocResult ? { passes: jrocResult.passes, passesPerSecond: jrocResult.passesPerSecond, compileDuration: jrocResult.compileDuration } : null,
        okojo: okojoResult ? { passes: okojoResult.passes, passesPerSecond: okojoResult.passesPerSecond } : null,
        yantraJS: yantraResult ? { passes: yantraResult.passes, passesPerSecond: yantraResult.passesPerSecond } : null,
        jint: jintResult ? { passes: jintResult.passes, passesPerSecond: jintResult.passesPerSecond } : null
    },
    comparisons: {
        vsOkojo: jrocResult && okojoResult ? Math.round((jrocResult.passes / okojoResult.passes) * 100) / 100 : null,
        vsYantraJS: jrocResult && yantraResult ? Math.round((jrocResult.passes / yantraResult.passes) * 100) / 100 : null,
        vsJint: jrocResult && jintResult ? Math.round((jrocResult.passes / jintResult.passes) * 100) / 100 : null,
        vsNode: jrocResult && nodeResult ? Math.round((nodeResult.passes / jrocResult.passes) * 100) / 100 : null
    }
};

const jsonPath = path.join(__dirname, 'results.json');
fs.writeFileSync(jsonPath, JSON.stringify(jsonOutput, null, 2));
log(`\nResults written to: ${jsonPath}`, 'gray');
