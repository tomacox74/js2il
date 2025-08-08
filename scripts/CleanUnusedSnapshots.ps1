<#!
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
!>

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
    # Derive test name (strip known class prefixes ExecutionTests./GeneratorTests.)
    $testName = $base
    if($base -match '^(ExecutionTests|GeneratorTests)\.(.+)$'){
        $testName = $matches[2]
    }

    # Build regex pattern for word boundary or nameof usage
    $pattern = "\b$([Regex]::Escape($testName))\b"  # plain word boundary
    $found = $false
    foreach($src in $testSources){
        if(Select-String -Path $src.FullName -Pattern $pattern -Quiet){
            $found = $true
            break
        }
        # Secondary check for nameof(TestName)
        if(Select-String -Path $src.FullName -SimpleMatch "nameof($testName)" -Quiet){
            $found = $true
            break
        }
    }

    if(-not $found){
        $unused += $snap
        Write-Info "UNUSED: $($snap.FullName) (derived test name: $testName)"
    }
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
