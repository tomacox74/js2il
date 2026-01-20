#!/usr/bin/env node
/*
Runs a generator test and opens the resulting assembly in ILSpy.

Usage:
  node scripts/decompileGeneratorTest.js <Category> <TestName>

Examples:
  node scripts/decompileGeneratorTest.js Async Async_HelloWorld
  node scripts/decompileGeneratorTest.js Function Function_ReturnsStaticValueAndLogs
  node scripts/decompileGeneratorTest.js Classes Classes_ClassWithMethod_HelloWorld

Requirements:
  - ilspy must be in PATH
*/

const childProcess = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');
const os = require('node:os');

function getAssemblyFileBaseName(testName) {
  // GeneratorTestsBase uses Path.GetFileNameWithoutExtension(testFilePath),
  // so nested test names like "CommonJS_Require_X/a" produce "a.dll".
  const normalized = String(testName).replace(/\\/g, '/');
  const segments = normalized.split('/').filter(Boolean);
  return segments.length > 0 ? segments[segments.length - 1] : String(testName);
}

function tryFindLatestGeneratedAssembly(category, assemblyFileBaseName) {
  const tempDir = os.tmpdir();
  const categoryRoot = path.join(tempDir, 'Js2IL.Tests', `${category}.GeneratorTests`);
  const assemblyFileName = `${assemblyFileBaseName}.dll`;

  if (!fs.existsSync(categoryRoot)) {
    return null;
  }

  const runDirs = fs
    .readdirSync(categoryRoot, { withFileTypes: true })
    .filter((d) => d.isDirectory())
    .map((d) => path.join(categoryRoot, d.name));

  let bestPath = null;
  let bestMtime = -1;

  for (const runDir of runDirs) {
    const candidate = path.join(runDir, assemblyFileName);
    if (!fs.existsSync(candidate)) continue;
    const stat = fs.statSync(candidate);
    if (stat.mtimeMs > bestMtime) {
      bestMtime = stat.mtimeMs;
      bestPath = candidate;
    }
  }

  return bestPath;
}

function findProjectRoot(startDir) {
  // The compiled DLL may live anywhere (e.g. test_output/...).
  // Walk upward from __dirname until we find the repo markers.
  let dir = startDir;
  while (true) {
    const sln = path.join(dir, 'js2il.sln');
    const testsDir = path.join(dir, 'Js2IL.Tests');
    if (fs.existsSync(sln) && fs.existsSync(testsDir)) {
      return dir;
    }
    const parent = path.dirname(dir);
    if (!parent || parent === dir) break;
    dir = parent;
  }
  throw new Error(`Could not locate js2il repo root starting from: ${startDir}`);
}

const projectRoot = findProjectRoot(__dirname);

function openInIlSpy(assemblyPath) {
  // Launch GUI and let this script exit without waiting.
  const child = childProcess.spawn('ilspy', [assemblyPath], {
    detached: true,
    stdio: 'ignore',
    shell: true,
    cwd: process.cwd(),
  });

  child.unref();
}

function main() {
  const args = process.argv.slice(2);

  if (args.length < 2) {
    console.error('Usage: node scripts/decompileGeneratorTest.js <Category> <TestName>');
    console.error('');
    console.error('Examples:');
    console.error('  node scripts/decompileGeneratorTest.js Async Async_HelloWorld');
    console.error('  node scripts/decompileGeneratorTest.js Function Function_ReturnsStaticValueAndLogs');
    process.exit(1);
  }

  const category = args[0];
  const testName = args[1];
  const fullTestName = `Js2IL.Tests.${category}.GeneratorTests.${testName}`;
  const testProject = path.join(projectRoot, 'Js2IL.Tests', 'Js2IL.Tests.csproj');

  console.log(`Running test: ${fullTestName}`);

  // Step 1: Run the generator test
  const testResult = childProcess.spawnSync(
    'dotnet',
    ['test', testProject, '--filter', `FullyQualifiedName=${fullTestName}`, '--no-build'],
    {
      stdio: 'inherit',
      shell: true,
      cwd: projectRoot,
    }
  );

  if (testResult.status !== 0) {
    // Try with build
    console.log('Test failed or not found, retrying with build...');
    const retryResult = childProcess.spawnSync(
      'dotnet',
      ['test', testProject, '--filter', `FullyQualifiedName=${fullTestName}`],
      {
        stdio: 'inherit',
        shell: true,
        cwd: projectRoot,
      }
    );

    if (retryResult.status !== 0) {
      console.error(`Test ${fullTestName} failed or not found.`);
      process.exit(1);
    }
  }

  // Step 2: Find the generated assembly
  // GeneratorTestsBase writes to a per-run GUID directory:
  //   %TEMP%/Js2IL.Tests/{Category}.GeneratorTests/{runId}/{assemblyName}.dll
  // where assemblyName is the basename of the JS entry file.
  const assemblyFileBaseName = getAssemblyFileBaseName(testName);
  const assemblyPath =
    tryFindLatestGeneratedAssembly(category, assemblyFileBaseName) ??
    null;

  if (!assemblyPath) {
    const tempDir = os.tmpdir();
    const categoryRoot = path.join(tempDir, 'Js2IL.Tests', `${category}.GeneratorTests`);
    console.error(`Assembly not found for category '${category}' and test '${testName}'.`);
    console.error(`Looked under: ${categoryRoot}`);
    console.error('Make sure the test ran successfully and generated the assembly.');
    process.exit(1);
  }

  console.log(`Found assembly: ${assemblyPath}`);

  // Step 3: Open the assembly in ILSpy
  try {
    console.log('Opening in ILSpy...');
    openInIlSpy(assemblyPath);
  } catch (err) {
    console.error('Failed to launch ILSpy (is `ilspy` on PATH?).');
    console.error(err);
    process.exit(1);
  }

  console.log('Done!');
}

main();
