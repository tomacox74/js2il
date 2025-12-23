#!/usr/bin/env node
/**
 * Syncs execution test snapshots with Node.js output.
 * 
 * For each execution test in Js2IL.Tests:
 * 1. Runs the JavaScript file with Node.js
 * 2. Compares the output to the current verified snapshot
 * 3. Updates the snapshot if they differ
 * 
 * Usage:
 *   node scripts/syncExecutionSnapshots.js [--dry-run] [--filter <pattern>]
 * 
 * Options:
 *   --dry-run    Show what would change without making changes
 *   --filter     Only process tests matching the pattern (e.g., "Array_Pop")
 */

const fs = require('fs');
const path = require('path');
const { execSync } = require('child_process');

const testsRoot = path.join(__dirname, '..', 'Js2IL.Tests');

// Tests to ignore (e.g., timeout tests, error-path tests, environment-specific)
const ignoredTests = [
    'TryFinally_NoCatch_Throw',
    'Promise_Resolve_FinallyThrows',
    'Promise_Scheduling_StarvationTest',
    'SetTimeout_OneSecondDelay',
    'SetImmediate_ExecutesBeforeSetTimeout',
    'Global___dirname_PrintsDirectory',
    'Environment_EnumerateProcessArgV'
];

// Test category directories that contain JavaScript/ and Snapshots/ folders
const testCategories = [
    'Array',
    'ArrowFunction',
    'BinaryOperator',
    'Classes',
    'CommonJS',
    'CompoundAssignment',
    'ControlFlow',
    'Date',
    'Function',
    'Integration',
    'JSON',
    'Literals',
    'Math',
    'Node',
    'Promise',
    'String',
    'TryCatch',
    'TypedArray',
    'UnaryOperator',
    'Variable'
];

// Parse command line arguments
const args = process.argv.slice(2);
const dryRun = args.includes('--dry-run');
const filterIdx = args.indexOf('--filter');
const filterPattern = filterIdx !== -1 ? args[filterIdx + 1] : null;

let updated = 0;
let unchanged = 0;
let errors = 0;
let skipped = 0;

function normalizeLineEndings(str) {
    // Remove BOM and normalize line endings
    return str.replace(/^\ufeff/, '').replace(/\r\n/g, '\n').replace(/\r/g, '\n');
}

function normalizeForComparison(str) {
    // Normalize: remove BOM, normalize line endings, trim trailing whitespace
    let normalized = normalizeLineEndings(str).trimEnd();
    
    // Handle empty string marker used by Verify framework
    if (normalized === 'emptyString') {
        normalized = '';
    }
    
    return normalized;
}

function runNodeScript(jsFilePath, timeoutMs = 5000) {
    try {
        const output = execSync(`node "${jsFilePath}"`, {
            encoding: 'utf-8',
            timeout: timeoutMs,
            stdio: ['pipe', 'pipe', 'pipe']
        });
        return { success: true, output: normalizeLineEndings(output) };
    } catch (err) {
        if (err.killed) {
            return { success: false, error: `Timeout after ${timeoutMs}ms` };
        }
        // If the script ran but threw an error, capture stderr/stdout
        const stdout = err.stdout ? normalizeLineEndings(err.stdout.toString()) : '';
        const stderr = err.stderr ? normalizeLineEndings(err.stderr.toString()) : '';
        return { 
            success: false, 
            error: err.message,
            output: stdout,
            stderr: stderr
        };
    }
}

function processCategory(category) {
    const jsDir = path.join(testsRoot, category, 'JavaScript');
    const snapshotsDir = path.join(testsRoot, category, 'Snapshots');
    
    if (!fs.existsSync(jsDir)) {
        return; // No JavaScript folder in this category
    }
    
    if (!fs.existsSync(snapshotsDir)) {
        console.log(`  ⚠️  No Snapshots folder for ${category}`);
        return;
    }
    
    const jsFiles = fs.readdirSync(jsDir).filter(f => f.endsWith('.js'));
    
    for (const jsFile of jsFiles) {
        const testName = path.basename(jsFile, '.js');
        
        // Skip ignored tests
        if (ignoredTests.includes(testName)) {
            console.log(`  ⏭️  ${testName}: ignored`);
            skipped++;
            continue;
        }
        
        // Apply filter if specified
        if (filterPattern && !testName.includes(filterPattern)) {
            continue;
        }
        
        const jsFilePath = path.join(jsDir, jsFile);
        const snapshotName = `ExecutionTests.${testName}.verified.txt`;
        const snapshotPath = path.join(snapshotsDir, snapshotName);
        
        // Check if snapshot exists
        if (!fs.existsSync(snapshotPath)) {
            console.log(`  ⏭️  ${testName}: No execution snapshot (skipped)`);
            skipped++;
            continue;
        }
        
        // Run the script with Node
        const result = runNodeScript(jsFilePath);
        
        if (!result.success) {
            // Some tests may intentionally throw errors - check if we got output anyway
            if (!result.output && !result.stderr) {
                console.log(`  ❌ ${testName}: ${result.error}`);
                errors++;
                continue;
            }
            // Use whatever output we got (some tests expect error output)
        }
        
        const nodeOutput = (result.output || '').trimEnd();
        
        // Read current snapshot and normalize for comparison
        const currentSnapshotRaw = fs.readFileSync(snapshotPath, 'utf-8');
        const currentSnapshot = normalizeForComparison(currentSnapshotRaw);
        
        // Compare normalized versions
        if (nodeOutput === currentSnapshot) {
            unchanged++;
            if (args.includes('--verbose')) {
                console.log(`  ✓ ${testName}: unchanged`);
            }
        } else {
            if (dryRun) {
                console.log(`  📝 ${testName}: would update`);
                console.log(`     Current (${currentSnapshot.length} chars):`);
                console.log(`     ${currentSnapshot.split('\n').slice(0, 3).join(' | ')}...`);
                console.log(`     Node (${nodeOutput.length} chars):`);
                console.log(`     ${nodeOutput.split('\n').slice(0, 3).join(' | ')}...`);
            } else {
                // Write with trailing newline for consistency with Verify framework
                const outputToWrite = nodeOutput.length === 0 ? 'emptyString\n' : nodeOutput + '\n';
                fs.writeFileSync(snapshotPath, outputToWrite, 'utf-8');
                console.log(`  ✅ ${testName}: updated`);
            }
            updated++;
        }
    }
}

console.log('Syncing execution test snapshots with Node.js output...');
console.log(`Mode: ${dryRun ? 'DRY RUN (no changes)' : 'UPDATE'}`);
if (filterPattern) {
    console.log(`Filter: ${filterPattern}`);
}
console.log('');

for (const category of testCategories) {
    const jsDir = path.join(testsRoot, category, 'JavaScript');
    if (fs.existsSync(jsDir)) {
        console.log(`📁 ${category}/`);
        processCategory(category);
    }
}

console.log('');
console.log('Summary:');
console.log(`  Unchanged: ${unchanged}`);
console.log(`  Updated:   ${updated}${dryRun ? ' (would update)' : ''}`);
console.log(`  Skipped:   ${skipped}`);
console.log(`  Errors:    ${errors}`);

if (updated > 0 && !dryRun) {
    console.log('');
    console.log('✅ Snapshots updated successfully!');
}

process.exit(errors > 0 ? 1 : 0);
