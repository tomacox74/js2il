'use strict';

const { spawn } = require('node:child_process');
const fs = require('node:fs');
const path = require('node:path');

const limits = Object.freeze({
  outputBytes: 1024 * 1024,
  workspaceBytes: 64 * 1024 * 1024,
  workspaceFiles: 256,
  observableBytes: 1024 * 1024,
  bundleBytes: 128 * 1024 * 1024
});

function workspaceFiles(directory, budget = limits) {
  const files = [];
  let bytes = 0;
  function visit(current) {
    for (const name of fs.readdirSync(current).sort()) {
      const file = path.join(current, name);
      let stat;
      try {
        stat = fs.lstatSync(file);
      } catch (error) {
        if (error.code === 'ENOENT') continue; // A running fixture may remove temporary files.
        throw error;
      }
      if (stat.isSymbolicLink() || !stat.isFile() && !stat.isDirectory()) {
        throw new Error('Workspace contains a symlink or special file.');
      }
      files.push({ path: path.relative(directory, file).split(path.sep).join('/'), bytes: stat.size, directory: stat.isDirectory() });
      bytes += stat.isFile() ? stat.size : 0;
      if (files.length > budget.workspaceFiles || bytes > budget.workspaceBytes) {
        throw new Error('Workspace file/byte budget exceeded.');
      }
      if (stat.isDirectory()) visit(file);
    }
  }
  visit(directory);
  return files.filter(file => !file.directory);
}

function fixtureEnvironment(directory, executables) {
  const home = path.join(directory, '.home');
  const temporary = path.join(directory, '.tmp');
  fs.mkdirSync(home, { recursive: true });
  fs.mkdirSync(temporary, { recursive: true });
  return {
    PATH: [...new Set([...executables.map(file => path.dirname(file)), '/usr/bin', '/bin'])].join(path.delimiter),
    HOME: home, DOTNET_CLI_HOME: home, TMPDIR: temporary, TMP: temporary, TEMP: temporary,
    LANG: 'C.UTF-8', LC_ALL: 'C.UTF-8', TZ: 'UTC',
    DOTNET_NOLOGO: '1', DOTNET_CLI_TELEMETRY_OPTOUT: '1', DOTNET_SKIP_FIRST_TIME_EXPERIENCE: '1',
    DOTNET_EnableDiagnostics: '0'
  };
}

function runBounded(command, args, { cwd, env, timeoutMs, budget = limits, monitor = true }) {
  if (process.platform !== 'linux') throw new Error('The current isolated execution profile requires Linux process groups.');
  if (!Number.isSafeInteger(timeoutMs) || timeoutMs < 1) throw new Error('Invalid process timeout.');
  return new Promise(resolve => {
    const startedAt = new Date().toISOString();
    const start = performance.now();
    const stdout = [];
    const stderr = [];
    let captured = 0;
    let state = 'completed';
    let error = null;
    let settled = false;
    const child = spawn(command, args, { cwd, env, detached: true, stdio: ['ignore', 'pipe', 'pipe'] });
    function terminate() {
      if (!child.pid) return;
      try {
        process.kill(-child.pid, 'SIGKILL');
      } catch (failure) {
        if (failure.code !== 'ESRCH') {
          state = 'cleanup-error';
          error = failure.code ?? failure.message;
        }
      }
    }
    function stop(reason, detail) {
      if (state === 'completed') {
        state = reason;
        error = detail;
      }
      terminate();
    }
    function capture(destination, chunk) {
      const remaining = budget.outputBytes - captured;
      if (remaining > 0) destination.push(chunk.subarray(0, remaining));
      captured += chunk.length;
      if (captured > budget.outputBytes) stop('output-limit', 'Combined stdout/stderr byte budget exceeded.');
    }
    const onParentExit = () => terminate();
    const onInterrupt = signal => {
      terminate();
      process.stderr.write(`Interrupted ${signal}; execution bundle is incomplete.\n`);
      process.exit(signal === 'SIGINT' ? 130 : 143);
    };
    const onSigint = () => onInterrupt('SIGINT');
    const onSigterm = () => onInterrupt('SIGTERM');
    process.once('exit', onParentExit);
    process.once('SIGINT', onSigint);
    process.once('SIGTERM', onSigterm);
    child.stdout.on('data', chunk => capture(stdout, chunk));
    child.stderr.on('data', chunk => capture(stderr, chunk));
    child.stdout.on('error', failure => stop('spawn-error', failure.code ?? failure.message));
    child.stderr.on('error', failure => stop('spawn-error', failure.code ?? failure.message));
    child.on('error', failure => stop('spawn-error', failure.code ?? failure.message));
    child.on('exit', terminate);
    const timeout = setTimeout(() => stop('timeout', `Exceeded ${timeoutMs}ms completion budget.`), timeoutMs);
    const watchdog = monitor ? setInterval(() => {
      try {
        workspaceFiles(cwd, budget);
      } catch (failure) {
        stop('resource-limit', failure.message);
      }
    }, 50) : null;
    child.on('close', (exitCode, signal) => {
      if (settled) return;
      settled = true;
      clearTimeout(timeout);
      if (watchdog) clearInterval(watchdog);
      process.removeListener('exit', onParentExit);
      process.removeListener('SIGINT', onSigint);
      process.removeListener('SIGTERM', onSigterm);
      if (monitor && state === 'completed') {
        try {
          workspaceFiles(cwd, budget);
        } catch (failure) {
          state = 'resource-limit';
          error = failure.message;
        }
      }
      resolve({
        state, exitCode, signal, error, startedAt, finishedAt: new Date().toISOString(),
        durationMs: Math.round(performance.now() - start),
        command, args, cwd, environment: env, stdout: Buffer.concat(stdout), stderr: Buffer.concat(stderr)
      });
    });
  });
}

module.exports = { limits, workspaceFiles, fixtureEnvironment, runBounded };
