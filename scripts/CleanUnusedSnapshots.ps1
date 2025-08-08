<#
.SYNOPSIS
  Finds and optionally deletes unused *.verified.txt snapshot files.

.DESCRIPTION
  A snapshot (*.verified.txt) is considered unused if its derived test name
  (the portion after 'ExecutionTests.' or 'GeneratorTests.' in the filename)
  is not referenced in any test source (*.cs) under the supplied root (default: Js2IL.Tests).

  Reference detection performs a simple textual search for the test name token
  (word-boundary match) in test source files. It also looks for nameof(TestName).

.PARAMETER Root
  Root directory containing the test project. Defaults to 'Js2IL.Tests' relative to script.

.PARAMETER Delete
  If specified, deletes the unused snapshot files. Otherwise they are only listed.

.PARAMETER Quiet
  Suppress per-file output; only summary shown.

.EXAMPLE
  # List unused snapshots
  ./scripts/CleanUnusedSnapshots.ps1

.EXAMPLE
  # Delete unused snapshots
  ./scripts/CleanUnusedSnapshots.ps1 -Delete

.NOTES
  Designed for local maintenance; does not modify tracked (already committed) files automatically.
  Exit code 0 always (safe for CI listing). Add custom logic if you want failing builds when unused found.
#>

[CmdletBinding()] param(
    [string]$Root = "Js2IL.Tests",
    [switch]$Delete,
    [switch]$Quiet
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Write-Info($msg){ if(-not $Quiet){ Write-Host $msg } }

if(-not (Test-Path $Root)){
    Write-Error "Root path '$Root' not found."
    exit 1
}

$snapshotFiles = Get-ChildItem -Path $Root -Recurse -Filter *.verified.txt
if(-not $snapshotFiles){
    Write-Host "No snapshot files found under '$Root'."
    return
}

$testSources = Get-ChildItem -Path $Root -Recurse -Filter *.cs
$unused = @()

foreach($snap in $snapshotFiles){
  $base = $snap.BaseName
  # Derive test name: remove trailing .verified, then leading ExecutionTests./GeneratorTests. prefix
  $core = $base
  if($core.EndsWith('.verified')){ $core = $core.Substring(0, $core.Length - '.verified'.Length) }
  $testName = $core
  foreach($prefix in 'ExecutionTests.','GeneratorTests.'){
    if($core.StartsWith($prefix)) { $testName = $core.Substring($prefix.Length); break }
  }

  # Scan sources collecting directories that reference this testName
  $pattern = "\b$([Regex]::Escape($testName))\b"
  $matchDirs = [System.Collections.Generic.HashSet[string]]::new()
  foreach($src in $testSources){
    $dir = $src.DirectoryName
    if(Select-String -Path $src.FullName -Pattern $pattern -Quiet){
      $null = $matchDirs.Add($dir)
      continue
    }
    if(Select-String -Path $src.FullName -SimpleMatch "nameof($testName)" -Quiet){
      $null = $matchDirs.Add($dir)
      continue
    }
  }

  $isReferenced = $matchDirs.Count -gt 0
  $dirMatches = $matchDirs.Contains($snap.DirectoryName)
  $isEmpty = ($snap.Length -eq 0)

  if(-not $isReferenced){
    $unused += $snap
    Write-Info "UNUSED: $($snap.FullName) (no test referencing '$testName')"
    continue
  }
  if(-not $dirMatches){
    # Snapshot lives in a directory that doesn't host a referencing test source.
    $unused += $snap
    $reason = if($isEmpty){"misplaced duplicate (empty)"} else {"misplaced duplicate"}
    $dirs = ($matchDirs | Sort-Object) -join '; '
    Write-Info "UNUSED: $($snap.FullName) (derived test name: $testName, $reason; expected dir(s): $dirs)"
    continue
  }
  # Otherwise it's a valid snapshot; keep it.
}

if($unused.Count -eq 0){
    Write-Host "No unused snapshots detected. ($($snapshotFiles.Count) total)"
    return
}

Write-Host "Unused snapshot count: $($unused.Count) of $($snapshotFiles.Count) total."

if($Delete){
    foreach($f in $unused){
        try {
            Remove-Item -Force $f.FullName
            Write-Info "Deleted: $($f.FullName)"
        } catch {
            Write-Warning "Failed to delete $($f.FullName): $_"
        }
    }
    Write-Host "Deletion complete."
} else {
    Write-Host "Run with -Delete to remove these files."    
}

exit 0
