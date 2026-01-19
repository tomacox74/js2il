#!/usr/bin/env node
/*
Runs a generator test, decompiles the resulting assembly to a C# project, and opens it in VS Code.

Usage:
  node scripts/decompileGeneratorTest.js <Category> <TestName>

Examples:
  node scripts/decompileGeneratorTest.js Async Async_HelloWorld
  node scripts/decompileGeneratorTest.js Function Function_ReturnsStaticValueAndLogs
  node scripts/decompileGeneratorTest.js Classes Classes_ClassWithMethod_HelloWorld

Requirements:
  - ilspycmd must be in PATH
  - code (VS Code CLI) must be in PATH
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

  // Step 3: Create output directory for decompiled project
  const tempDir = os.tmpdir();
  const safeOutputName = String(testName).replace(/[\\/]/g, '_');
  const decompileOutputDir = path.join(tempDir, 'Js2IL.Decompiled', safeOutputName);
  
  // Clean up previous decompilation if exists
  if (fs.existsSync(decompileOutputDir)) {
    fs.rmSync(decompileOutputDir, { recursive: true, force: true });
  }
  fs.mkdirSync(decompileOutputDir, { recursive: true });

  console.log(`Decompiling to: ${decompileOutputDir}`);

  // Step 4: Decompile using ilspycmd to a C# project
  const ilspyResult = childProcess.spawnSync(
    'ilspycmd',
    [
      '-p',  // Generate project files
      '-o', decompileOutputDir,
      assemblyPath,
    ],
    {
      stdio: 'inherit',
      shell: true,
      cwd: process.cwd(),
    }
  );

  if (ilspyResult.status !== 0) {
    console.error('ilspycmd decompilation failed.');
    process.exit(1);
  }

  // Step 5: Find the .csproj file
  const files = fs.readdirSync(decompileOutputDir);
  const csprojFile = files.find(f => f.endsWith('.csproj'));
  
  if (!csprojFile) {
    console.error('No .csproj file found in decompiled output.');
    console.error('Files in output directory:', files);
    process.exit(1);
  }

  const projectPath = path.join(decompileOutputDir, csprojFile);
  console.log(`Opening project: ${projectPath}`);

  // Step 6: Open in VS Code
  const codeResult = childProcess.spawnSync(
    'code',
    [decompileOutputDir],
    {
      stdio: 'inherit',
      shell: true,
      cwd: process.cwd(),
    }
  );

  if (codeResult.status !== 0) {
    console.error('Failed to open VS Code.');
    process.exit(1);
  }

  console.log('Done!');
}

main();
