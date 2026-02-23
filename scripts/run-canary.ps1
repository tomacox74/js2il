<#
.SYNOPSIS
  Compile a canary JS script with JS2IL, execute it under a strict timeout, and
  validate that expected output markers are present in stdout.

.DESCRIPTION
  Canary smoke tests: each run goes through compile → execute → validate.
  Any failure (compile error, execution timeout, missing marker) is treated as
  a fatal error and the script exits with a non-zero code.

.PARAMETER CanaryScript
  Path to the JavaScript source file to compile and run.

.PARAMETER ExpectedMarker
  A substring that must appear in stdout for the run to be considered passing.
  Defaults to "CANARY:" (any canary marker).

.PARAMETER Js2ILPath
  Path to the js2il executable DLL (run via `dotnet <dll>`).
  If not provided, falls back to dotnet run --no-build on Js2IL.csproj.

.PARAMETER TimeoutSec
  Wall-clock timeout in seconds for the *execution* phase (default: 30).

.PARAMETER CompileTimeoutSec
  Wall-clock timeout in seconds for the *compilation* phase (default: 60).

.PARAMETER OutputDir
  Directory for compiled artefacts.  Defaults to a temp subdirectory.

.PARAMETER KeepArtifacts
  When set, compiled artefacts are preserved on failure for inspection.

.EXAMPLE
  pwsh scripts/run-canary.ps1 -CanaryScript tests/canary/corpus/hello-world.js
  pwsh scripts/run-canary.ps1 -CanaryScript tests/canary/corpus/closures.js -ExpectedMarker "CANARY:closures:ok"
#>
[CmdletBinding()]
param(
  [Parameter(Mandatory = $true)]
  [string] $CanaryScript,

  [Parameter(Mandatory = $false)]
  [string] $ExpectedMarker = 'CANARY:',

  [Parameter(Mandatory = $false)]
  [string] $Js2ILPath = '',

  [Parameter(Mandatory = $false)]
  [int] $TimeoutSec = 30,

  [Parameter(Mandatory = $false)]
  [int] $CompileTimeoutSec = 60,

  [Parameter(Mandatory = $false)]
  [string] $OutputDir = '',

  [switch] $KeepArtifacts
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------

function Write-Step([string] $msg) {
  Write-Host "  >> $msg" -ForegroundColor Cyan
}

function Write-Ok([string] $msg) {
  Write-Host "  OK $msg" -ForegroundColor Green
}

function Write-Fail([string] $msg) {
  Write-Host "FAIL $msg" -ForegroundColor Red
}

function Remove-DirSafe([string] $dir) {
  if (Test-Path $dir) {
    Remove-Item $dir -Recurse -Force -ErrorAction SilentlyContinue
  }
}

function Run-Process {
  param(
    [string]   $Exe,
    [string[]] $Arguments,
    [string]   $StdoutFile,
    [string]   $StderrFile,
    [int]      $TimeoutSec
  )

  $parent = Split-Path -Parent $StdoutFile
  if ($parent -and !(Test-Path $parent)) { New-Item -ItemType Directory -Force $parent | Out-Null }
  $parent2 = Split-Path -Parent $StderrFile
  if ($parent2 -and !(Test-Path $parent2)) { New-Item -ItemType Directory -Force $parent2 | Out-Null }

  if (Test-Path $StdoutFile) { Remove-Item $StdoutFile -Force }
  if (Test-Path $StderrFile) { Remove-Item $StderrFile -Force }

  $psi = [System.Diagnostics.ProcessStartInfo]::new()
  $psi.FileName               = $Exe
  $psi.Arguments              = $Arguments -join ' '
  $psi.RedirectStandardOutput = $true
  $psi.RedirectStandardError  = $true
  $psi.UseShellExecute        = $false
  $psi.WorkingDirectory       = $PWD.Path

  $proc = [System.Diagnostics.Process]::Start($psi)

  # Async-read to avoid deadlock when both stdout and stderr fill their buffers.
  $stdoutTask = $proc.StandardOutput.ReadToEndAsync()
  $stderrTask = $proc.StandardError.ReadToEndAsync()

  $exited = $proc.WaitForExit($TimeoutSec * 1000)
  if (!$exited) {
    try { $proc.Kill() } catch {}
    throw "TIMEOUT after ${TimeoutSec}s: $Exe $($Arguments -join ' ')"
  }

  $stdout = $stdoutTask.GetAwaiter().GetResult()
  $stderr = $stderrTask.GetAwaiter().GetResult()

  [System.IO.File]::WriteAllText($StdoutFile, $stdout)
  [System.IO.File]::WriteAllText($StderrFile, $stderr)

  return [PSCustomObject]@{
    ExitCode = $proc.ExitCode
    Stdout   = $stdout
    Stderr   = $stderr
  }
}

# ---------------------------------------------------------------------------
# Resolve paths
# ---------------------------------------------------------------------------

$repoRoot = Split-Path -Parent $PSScriptRoot

$scriptPath = [System.IO.Path]::GetFullPath((Join-Path $repoRoot $CanaryScript))
if (!(Test-Path $scriptPath)) {
  # Try as an absolute path directly
  $scriptPath = [System.IO.Path]::GetFullPath($CanaryScript)
}
if (!(Test-Path $scriptPath)) {
  Write-Fail "Canary script not found: $CanaryScript"
  exit 1
}

$canaryName = [System.IO.Path]::GetFileNameWithoutExtension($scriptPath)

if (!$OutputDir) {
  $OutputDir = Join-Path ([System.IO.Path]::GetTempPath()) "js2il-canary-$canaryName-$([System.Guid]::NewGuid().ToString('N').Substring(0,8))"
}
New-Item -ItemType Directory -Force $OutputDir | Out-Null

$compiledDll = Join-Path $OutputDir ($canaryName + '.dll')
$compileStdout = Join-Path $OutputDir 'compile.stdout.txt'
$compileStderr = Join-Path $OutputDir 'compile.stderr.txt'
$runStdout     = Join-Path $OutputDir 'run.stdout.txt'
$runStderr     = Join-Path $OutputDir 'run.stderr.txt'

# ---------------------------------------------------------------------------
# Locate js2il
# ---------------------------------------------------------------------------

$js2ilExe  = ''
$js2ilArgs = @()

if ($Js2ILPath -and (Test-Path $Js2ILPath)) {
  # Caller supplied an explicit path (DLL → run via dotnet, or native exe)
  if ($Js2ILPath.EndsWith('.dll', [System.StringComparison]::OrdinalIgnoreCase)) {
    $js2ilExe  = 'dotnet'
    $js2ilArgs = @("`"$Js2ILPath`"")
  } else {
    $js2ilExe  = $Js2ILPath
    $js2ilArgs = @()
  }
} else {
  # Auto-discover: look for a built Js2IL.dll in the repo bin directories.
  $candidates = @(
    (Join-Path $repoRoot 'Js2IL' 'bin' 'Release' 'net10.0' 'Js2IL.dll'),
    (Join-Path $repoRoot 'Js2IL' 'bin' 'Debug'   'net10.0' 'Js2IL.dll')
  )
  $found = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
  if ($found) {
    $js2ilExe  = 'dotnet'
    $js2ilArgs = @("`"$found`"")
  } else {
    # Last resort: dotnet run (slow but always works after a restore)
    $csproj = Join-Path $repoRoot 'Js2IL' 'Js2IL.csproj'
    if (!(Test-Path $csproj)) {
      Write-Fail "Cannot locate Js2IL.dll or Js2IL.csproj; provide -Js2ILPath or build the project first."
      exit 1
    }
    $js2ilExe  = 'dotnet'
    $js2ilArgs = @('run', '--no-build', "--project", "`"$csproj`"", '--')
  }
}

# ---------------------------------------------------------------------------
# Phase 1 – Compile
# ---------------------------------------------------------------------------

Write-Host ""
Write-Host "=== Canary: $canaryName ===" -ForegroundColor Yellow
Write-Step "Compiling $scriptPath"

$compileArgs = $js2ilArgs + @("`"$scriptPath`"", '-o', "`"$OutputDir`"")
try {
  $compileResult = Run-Process -Exe $js2ilExe -Arguments $compileArgs `
    -StdoutFile $compileStdout -StderrFile $compileStderr `
    -TimeoutSec $CompileTimeoutSec
} catch {
  Write-Fail "Compile phase: $_"
  Write-Host "  Artefacts preserved in: $OutputDir"
  exit 1
}

if ($compileResult.ExitCode -ne 0) {
  Write-Fail "Compile failed (exit $($compileResult.ExitCode))"
  Write-Host "--- compile stdout ---"
  Write-Host $compileResult.Stdout
  Write-Host "--- compile stderr ---"
  Write-Host $compileResult.Stderr
  Write-Host "  Artefacts preserved in: $OutputDir"
  exit 1
}

if (!(Test-Path $compiledDll)) {
  Write-Fail "Compiled DLL not found: $compiledDll"
  Write-Host "--- compile stdout ---"
  Write-Host $compileResult.Stdout
  exit 1
}

Write-Ok "Compilation succeeded → $compiledDll"

# ---------------------------------------------------------------------------
# Phase 2 – Execute
# ---------------------------------------------------------------------------

Write-Step "Executing $compiledDll (timeout ${TimeoutSec}s)"

try {
  $runResult = Run-Process -Exe 'dotnet' -Arguments @("`"$compiledDll`"") `
    -StdoutFile $runStdout -StderrFile $runStderr `
    -TimeoutSec $TimeoutSec
} catch {
  Write-Fail "Execute phase: $_"
  Write-Host "  Artefacts preserved in: $OutputDir"
  exit 1
}

if ($runResult.ExitCode -ne 0) {
  Write-Fail "Execution failed (exit $($runResult.ExitCode))"
  Write-Host "--- run stdout ---"
  Write-Host $runResult.Stdout
  Write-Host "--- run stderr ---"
  Write-Host $runResult.Stderr
  Write-Host "  Artefacts preserved in: $OutputDir"
  exit 1
}

Write-Ok "Execution succeeded"
Write-Host "--- stdout ---"
Write-Host $runResult.Stdout

# ---------------------------------------------------------------------------
# Phase 3 – Validate output markers
# ---------------------------------------------------------------------------

Write-Step "Validating expected marker: '$ExpectedMarker'"

if (!$runResult.Stdout.Contains($ExpectedMarker)) {
  Write-Fail "Expected marker '$ExpectedMarker' not found in stdout."
  Write-Host "--- full stdout ---"
  Write-Host $runResult.Stdout
  Write-Host "  Artefacts preserved in: $OutputDir"
  exit 1
}

Write-Ok "Marker found"

# ---------------------------------------------------------------------------
# Cleanup (unless KeepArtifacts requested or failure)
# ---------------------------------------------------------------------------

if (!$KeepArtifacts) {
  Remove-DirSafe $OutputDir
}

Write-Host ""
Write-Host "=== PASS: $canaryName ===" -ForegroundColor Green
exit 0
