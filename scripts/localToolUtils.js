"use strict";

const { execSync } = require("child_process");
const fs = require("fs");
const os = require("os");
const path = require("path");

const repoRoot = path.resolve(__dirname, "..");
const defaultPackDir = path.join(repoRoot, "out_publish");
const globalToolStorePath = path.join(os.homedir(), ".dotnet", "tools", ".store", "jroc");

function run(command, opts = {}) {
  execSync(command, { stdio: "inherit", cwd: repoRoot, ...opts });
}

function removeDir(dirPath) {
  if (fs.existsSync(dirPath)) {
    fs.rmSync(dirPath, { recursive: true, force: true });
    console.log(`Removed: ${dirPath}`);
  }
}

function resolveRepoPath(candidate) {
  return path.isAbsolute(candidate) ? candidate : path.resolve(repoRoot, candidate);
}

function getToolExecutableName() {
  return process.platform === "win32" ? "jroc.exe" : "jroc";
}

function getToolExecutablePath(toolPath) {
  return path.join(toolPath, getToolExecutableName());
}

function packToolPackage({ packDir = defaultPackDir } = {}) {
  const resolvedPackDir = resolveRepoPath(packDir);
  removeDir(resolvedPackDir);
  console.log(`Packing jroc to ${resolvedPackDir} ...`);
  run(`dotnet pack src/Cli -c Release -o "${resolvedPackDir}"`);
  return resolvedPackDir;
}

function installPackagedTool({ packDir = defaultPackDir, toolPath } = {}) {
  const resolvedPackDir = resolveRepoPath(packDir);

  if (toolPath) {
    const resolvedToolPath = resolveRepoPath(toolPath);
    removeDir(resolvedToolPath);
    console.log(`Installing jroc to local tool path ${resolvedToolPath} ...`);
    run(`dotnet tool install --tool-path "${resolvedToolPath}" --add-source "${resolvedPackDir}" jroc`);

    const executable = getToolExecutablePath(resolvedToolPath);
    if (!fs.existsSync(executable)) {
      throw new Error(`Installed tool shim not found: ${executable}`);
    }

    return {
      scope: "local",
      executable,
      packDir: resolvedPackDir,
      toolPath: resolvedToolPath,
    };
  }

  console.log("Uninstalling existing global jroc (if any)...");
  try {
    run("dotnet tool uninstall jroc -g");
  } catch {
    // Ignore failures when the tool is not installed.
  }

  console.log("Clearing tool store cache...");
  removeDir(globalToolStorePath);

  console.log("Installing jroc from local pack directory...");
  run(`dotnet tool install --global --add-source "${resolvedPackDir}" jroc`);

  return {
    scope: "global",
    executable: getToolExecutableName(),
    packDir: resolvedPackDir,
    toolPath: null,
  };
}

module.exports = {
  defaultPackDir,
  getToolExecutablePath,
  installPackagedTool,
  packToolPackage,
  removeDir,
  repoRoot,
  resolveRepoPath,
};
