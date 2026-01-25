param(
    [Parameter(Mandatory = $false)]
    [string]$Root = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# This script is kept as a thin wrapper for backwards compatibility.
# The implementation now lives in: scripts/ECMA262/splitEcma262SectionsIntoSubsections.js

$scriptPath = Join-Path $PSScriptRoot "splitEcma262SectionsIntoSubsections.js"

if (-not (Test-Path -LiteralPath $scriptPath)) {
    throw "Expected JS implementation at: $scriptPath"
}

$node = (Get-Command node -ErrorAction SilentlyContinue)
if ($null -eq $node) {
    throw "node is required to run $scriptPath"
}

$args = @($scriptPath)
if (-not [string]::IsNullOrWhiteSpace($Root)) {
    $args += @("--root", $Root)
}

& node @args
