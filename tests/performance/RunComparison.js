"use strict";\r\n\r\n// Performance Comparison Script
// Compares Node.js vs Jint vs JS2IL performance on PrimeJavaScript.js

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
const js2ilOutput = path.join(__dirname, 'out_comparison');
if (fs.existsSync(js2ilOutput)) {
    fs.rmSync(js2ilOutput, { recursive: true, force: true });
}
fs.mkdirSync(js2ilOutput, { recursive: true });

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
// 4. JS2IL Benchmark
// ============================================================================
log("4. Compiling with JS2IL...", 'yellow');
try {
    const compileStart = Date.now();
    execSync(`js2il "${path.join(__dirname, 'PrimeJavaScript.js')}" "${path.join(js2ilOutput, 'PrimeJavaScript')}"`, {
        encoding: 'utf8',
        stdio: 'pipe'
    });
    const compileEnd = Date.now();
    const compileDuration = compileEnd - compileStart;
    log(`   [OK] Compilation completed in: ${compileDuration} ms`, 'green');

    console.log();
    log("5. Running JS2IL compiled benchmark...", 'yellow');
    const js2ilStart = Date.now();
    const js2ilResult = execSync(`dotnet "${path.join(js2ilOutput, 'PrimeJavaScript', 'PrimeJavaScript.dll')}"`, {
        encoding: 'utf8',
        stdio: 'pipe'
    });
    const js2ilEnd = Date.now();
    const js2ilMs = js2ilEnd - js2ilStart;

    // Extract passes and duration from JS2IL output
    const js2ilMatch = js2ilResult.match(/rogiervandam-[^;]+;(\d+);([\d.]+);/);
    if (js2ilMatch) {
        const js2ilPasses = parseInt(js2ilMatch[1]);
        const js2ilDuration = parseFloat(js2ilMatch[2]);
        log(`   [OK] JS2IL completed: ${js2ilMs} ms total`, 'green');
        log(`        Passes: ${js2ilPasses} in ${js2ilDuration} s`, 'gray');

        results.push({
            runtime: "JS2IL",
            totalTimeMs: js2ilMs,
            passes: js2ilPasses,
            passDuration: js2ilDuration,
            passesPerSecond: Math.round((js2ilPasses / js2ilDuration) * 100) / 100,
            compileDuration
        });
    }
} catch (error) {
    log(`   [ERROR] JS2IL failed: ${error.message}`, 'red');
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
const yantraResult = results.find(r => r.runtime === "YantraJS");
const js2ilResult = results.find(r => r.runtime === "JS2IL");

if (js2ilResult && jintResult) {
    const js2ilVsJint = Math.round((js2ilResult.passes / jintResult.passes) * 100) / 100;
    log(`  * JS2IL is ${js2ilVsJint}x faster than Jint (interpreted .NET)`, 'green');
}

if (js2ilResult && yantraResult) {
    if (js2ilResult.passes > yantraResult.passes) {
        const js2ilVsYantra = Math.round((js2ilResult.passes / yantraResult.passes) * 100) / 100;
        log(`  * JS2IL is ${js2ilVsYantra}x faster than YantraJS`, 'green');
    } else {
        const yantraVsJs2il = Math.round((yantraResult.passes / js2ilResult.passes) * 100) / 100;
        log(`  * YantraJS is ${yantraVsJs2il}x faster than JS2IL`, 'yellow');
    }
}

if (js2ilResult && nodeResult) {
    if (js2ilResult.passes > nodeResult.passes) {
        const js2ilVsNode = Math.round((js2ilResult.passes / nodeResult.passes) * 100) / 100;
        log(`  * JS2IL is ${js2ilVsNode}x faster than Node.js`, 'green');
    } else {
        const nodeVsJs2il = Math.round((nodeResult.passes / js2ilResult.passes) * 100) / 100;
        log(`  * Node.js is ${nodeVsJs2il}x faster than JS2IL`, 'yellow');
    }
}

if (nodeResult && jintResult) {
    const nodeVsJint = Math.round((nodeResult.passes / jintResult.passes) * 100) / 100;
    log(`  * Node.js is ${nodeVsJint}x faster than Jint`, 'cyan');
}

if (nodeResult && yantraResult) {
    const nodeVsYantra = Math.round((nodeResult.passes / yantraResult.passes) * 100) / 100;
    log(`  * Node.js is ${nodeVsYantra}x faster than YantraJS`, 'cyan');
}

console.log();
log("Note: More passes = better performance. Test runs for 5 seconds.", 'gray');
if (js2ilResult && js2ilResult.compileDuration) {
    log(`      JS2IL compilation time: ${js2ilResult.compileDuration} ms (excluded from benchmark)`, 'gray');
}

// ============================================================================
// Write results to JSON file for CI consumption
// ============================================================================
const jsonOutput = {
    timestamp: new Date().toISOString(),
    results: {
        node: nodeResult ? { passes: nodeResult.passes, passesPerSecond: nodeResult.passesPerSecond } : null,
        js2il: js2ilResult ? { passes: js2ilResult.passes, passesPerSecond: js2ilResult.passesPerSecond, compileDuration: js2ilResult.compileDuration } : null,
        yantraJS: yantraResult ? { passes: yantraResult.passes, passesPerSecond: yantraResult.passesPerSecond } : null,
        jint: jintResult ? { passes: jintResult.passes, passesPerSecond: jintResult.passesPerSecond } : null
    },
    comparisons: {
        vsYantraJS: js2ilResult && yantraResult ? Math.round((js2ilResult.passes / yantraResult.passes) * 100) / 100 : null,
        vsJint: js2ilResult && jintResult ? Math.round((js2ilResult.passes / jintResult.passes) * 100) / 100 : null,
        vsNode: js2ilResult && nodeResult ? Math.round((nodeResult.passes / js2ilResult.passes) * 100) / 100 : null
    }
};

const jsonPath = path.join(__dirname, 'results.json');
fs.writeFileSync(jsonPath, JSON.stringify(jsonOutput, null, 2));
log(`\nResults written to: ${jsonPath}`, 'gray');
