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

  console.log(`Running test: ${fullTestName}`);

  // Step 1: Run the generator test
  const testResult = childProcess.spawnSync(
    'dotnet',
    ['test', 'Js2IL.Tests', '--filter', `FullyQualifiedName=${fullTestName}`, '--no-build'],
    {
      stdio: 'inherit',
      shell: true,
      cwd: process.cwd(),
    }
  );

  if (testResult.status !== 0) {
    // Try with build
    console.log('Test failed or not found, retrying with build...');
    const retryResult = childProcess.spawnSync(
      'dotnet',
      ['test', 'Js2IL.Tests', '--filter', `FullyQualifiedName=${fullTestName}`],
      {
        stdio: 'inherit',
        shell: true,
        cwd: process.cwd(),
      }
    );

    if (retryResult.status !== 0) {
      console.error(`Test ${fullTestName} failed or not found.`);
      process.exit(1);
    }
  }

  // Step 2: Find the generated assembly
  // Generator tests output to: %TEMP%/Js2IL.Tests/{Category}.GeneratorTests/{TestName}.dll
  const tempDir = os.tmpdir();
  const testOutputDir = path.join(tempDir, 'Js2IL.Tests', `${category}.GeneratorTests`);
  const assemblyPath = path.join(testOutputDir, `${testName}.dll`);

  if (!fs.existsSync(assemblyPath)) {
    console.error(`Assembly not found at: ${assemblyPath}`);
    console.error('Make sure the test ran successfully and generated the assembly.');
    process.exit(1);
  }

  console.log(`Found assembly: ${assemblyPath}`);

  // Step 3: Create output directory for decompiled project
  const decompileOutputDir = path.join(tempDir, 'Js2IL.Decompiled', testName);
  
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
