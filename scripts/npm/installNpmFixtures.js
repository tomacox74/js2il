/*
  Installs a small set of public npm packages into a gitignored folder for
  local/integration testing of Node/CommonJS resolution.

  Usage:
    node scripts/npm/installNpmFixtures.js
    node scripts/npm/installNpmFixtures.js --clean

  Notes:
  - Uses `npm install` into `test_output/npm-fixtures`.
  - Runs with `--ignore-scripts` for safety/reproducibility.
*/

const fs = require('fs');
const path = require('path');
const childProcess = require('child_process');

function readJson(filePath) {
  return JSON.parse(fs.readFileSync(filePath, 'utf8'));
}

function writeJson(filePath, value) {
  fs.writeFileSync(filePath, JSON.stringify(value, null, 2) + '\n', 'utf8');
}

function rmrf(targetPath) {
  fs.rmSync(targetPath, { recursive: true, force: true });
}

function ensureDir(dirPath) {
  fs.mkdirSync(dirPath, { recursive: true });
}

function run(command, args, options) {
  const result = childProcess.spawnSync(command, args, {
    stdio: 'inherit',
    shell: process.platform === 'win32',
    ...options,
  });

  if (result.status !== 0) {
    process.exit(result.status ?? 1);
  }
}

function main() {
  const repoRoot = path.resolve(__dirname, '..', '..');
  const fixturesConfigPath = path.join(__dirname, 'fixtures.json');
  const config = readJson(fixturesConfigPath);

  const clean = process.argv.includes('--clean');

  const outputDir = path.resolve(repoRoot, config.outputDir);
  const nodeModulesDir = path.join(outputDir, 'node_modules');
  const packageLockPath = path.join(outputDir, 'package-lock.json');
  const packageJsonPath = path.join(outputDir, 'package.json');

  ensureDir(outputDir);

  if (clean) {
    rmrf(nodeModulesDir);
    rmrf(packageLockPath);
  }

  const dependencies = {};
  for (const p of config.packages) {
    dependencies[p.name] = p.version;
  }

  const pkg = {
    name: 'js2il-npm-fixtures',
    private: true,
    description: 'Local test fixtures for JS2IL npm/CommonJS module resolution',
    version: '0.0.0',
    license: 'UNLICENSED',
    dependencies,
  };

  writeJson(packageJsonPath, pkg);

  console.log(`Installing npm fixtures into: ${path.relative(repoRoot, outputDir)}`);
  console.log('Packages:');
  for (const p of config.packages) {
    console.log(`  - ${p.name}@${p.version}`);
  }

  run('npm', ['install', '--no-audit', '--no-fund', '--ignore-scripts', '--omit=dev'], {
    cwd: outputDir,
  });

  console.log('Done.');
}

main();
