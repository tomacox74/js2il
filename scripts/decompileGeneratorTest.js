#!/usr/bin/env node
"use strict";

/*
Runs a generator test and opens the resulting assembly in ILSpy.

Usage:
  node scripts/decompileGeneratorTest.js <Category> <TestName>

Examples:
  node scripts/decompileGeneratorTest.js Async Async_HelloWorld
  node scripts/decompileGeneratorTest.js Function Function_ReturnsStaticValueAndLogs
  node scripts/decompileGeneratorTest.js Classes Classes_ClassWithMethod_HelloWorld
  node scripts/decompileGeneratorTest.js Node.FS FS_ReadWrite_Utf8

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

function normalizeCategory(category) {
  const parts = String(category)
    .trim()
    .replace(/[\\/]+/g, '.')
    .split('.')
    .filter(Boolean);

  if (parts.length === 0) {
    throw new Error('Category must contain at least one name segment.');
  }

  return {
    namespace: parts.join('.'),
    path: parts.join('/'),
  };
}

function getCandidateCategoryRoots(category) {
  const tempDir = os.tmpdir();
  const categoryRoots = new Set([category.namespace, category.path]);
  return [...categoryRoots].flatMap((categoryRoot) => [
    path.join(tempDir, 'Jroc.Tests', `${categoryRoot}.GeneratorTests`),
    path.join(tempDir, 'Jroc.Tests', `${categoryRoot}.ExecutionTests`),
  ]);
}

function tryFindLatestGeneratedAssemblyInRoot(categoryRoot, assemblyFileName) {
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

function tryFindLatestGeneratedAssembly(category, assemblyFileBaseName) {
  const assemblyFileName = `${assemblyFileBaseName}.dll`;
  for (const categoryRoot of getCandidateCategoryRoots(category)) {
    const generated = tryFindLatestGeneratedAssemblyInRoot(categoryRoot, assemblyFileName);
    if (generated) {
      return generated;
    }
  }
  return null;
}

function findProjectRoot(startDir) {
  // The compiled DLL may live anywhere (e.g. test_output/...).
  // Walk upward from __dirname until we find the repo markers.
  let dir = startDir;
  while (true) {
    const sln = path.join(dir, 'jroc.sln');
    const testsDir = path.join(dir, 'tests', 'Jroc.Tests');
    if (fs.existsSync(sln) && fs.existsSync(testsDir)) {
      return dir;
    }
    const parent = path.dirname(dir);
    if (!parent || parent === dir) break;
    dir = parent;
  }
  throw new Error(`Could not locate jroc repo root starting from: ${startDir}`);
}

const projectRoot = findProjectRoot(__dirname);

function getGeneratorTestSnapshotPath(categoryPath, testName) {
  return path.join(
    projectRoot,
    'tests',
    'Jroc.Tests',
    categoryPath,
    'Snapshots',
    `GeneratorTests.${testName}.verified.txt`
  );
}

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

  const category = normalizeCategory(args[0]);
  const testName = args[1];
  const snapshotPath = getGeneratorTestSnapshotPath(category.path, testName);
  if (!fs.existsSync(snapshotPath)) {
    console.error(`Generator test snapshot not found: ${snapshotPath}`);
    console.error('Check the category and test name before running the helper.');
    process.exit(1);
  }

  const fullTestName = `Jroc.Tests.${category.namespace}.GeneratorTests.${testName}`;
  const testProject = path.join(projectRoot, 'tests', 'Jroc.Tests', 'Jroc.Tests.csproj');

  console.log(`Running test: ${fullTestName}`);
  const testEnvironment = {
    ...process.env,
    JROC_WRITE_TEST_ARTIFACTS: '1',
  };

  // Step 1: Run the generator test with explicit artifact materialization enabled.
  const testResult = childProcess.spawnSync(
    'dotnet',
    ['test', testProject, '--filter', `FullyQualifiedName=${fullTestName}`, '--no-build'],
    {
      stdio: 'inherit',
      shell: true,
      cwd: projectRoot,
      env: testEnvironment,
    }
  );

  const assemblyFileBaseName = getAssemblyFileBaseName(testName);
  let assemblyPath =
    testResult.status === 0
      ? tryFindLatestGeneratedAssembly(category, assemblyFileBaseName)
      : null;

  // `dotnet test --no-build` can exit successfully without running tests when
  // the test project has not been built yet. Only skip the build retry when
  // the requested test actually produced an assembly.
  if (!assemblyPath) {
    console.log('Test did not produce an assembly, retrying with build...');
    const retryResult = childProcess.spawnSync(
      'dotnet',
      ['test', testProject, '--filter', `FullyQualifiedName=${fullTestName}`],
      {
        stdio: 'inherit',
        shell: true,
        cwd: projectRoot,
        env: testEnvironment,
      }
    );

    if (retryResult.status !== 0) {
      console.error(`Test ${fullTestName} failed or not found.`);
      process.exit(1);
    }

    assemblyPath = tryFindLatestGeneratedAssembly(category, assemblyFileBaseName);
  }

  // Step 2: Find the generated assembly
  // JROC_WRITE_TEST_ARTIFACTS=1 writes to one of:
  //   %TEMP%/Jroc.Tests/{Category}.GeneratorTests/{runId}/{assemblyName}.dll
  //   %TEMP%/Jroc.Tests/{Category}.ExecutionTests/{runId}/{assemblyName}.dll
  // where assemblyName is the basename of the JS entry file.
  if (!assemblyPath) {
    console.error(`Assembly not found for category '${category.path}' and test '${testName}'.`);
    console.error('Looked under:');
    const categoryRoots = getCandidateCategoryRoots(category);
    for (let i = 0; i < categoryRoots.length; i += 1) {
      console.error(`  - ${categoryRoots[i]}`);
    }
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
