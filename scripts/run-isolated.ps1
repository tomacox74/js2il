param(
  [Parameter(Mandatory = $true)]
  [string] $FilePath,

  [Parameter(Mandatory = $false)]
  [int] $TimeoutSec = 30,

  [Parameter(Mandatory = $true)]
  [string] $Stdout,

  [Parameter(Mandatory = $true)]
  [string] $Stderr,

  [Parameter(ValueFromRemainingArguments = $true)]
  [string[]] $Args
)

$ErrorActionPreference = 'Stop'

function Ensure-ParentDir([string] $Path) {
  $parent = Split-Path -Parent $Path
  if ([string]::IsNullOrWhiteSpace($parent)) { return }
  New-Item -ItemType Directory -Force $parent | Out-Null
}

Ensure-ParentDir $Stdout
Ensure-ParentDir $Stderr

if (Test-Path $Stdout) { Remove-Item $Stdout -Force -ErrorAction SilentlyContinue }
if (Test-Path $Stderr) { Remove-Item $Stderr -Force -ErrorAction SilentlyContinue }

$p = Start-Process -FilePath $FilePath -ArgumentList $Args -WorkingDirectory $PWD -PassThru -RedirectStandardOutput $Stdout -RedirectStandardError $Stderr

$exited = Wait-Process -Id $p.Id -Timeout $TimeoutSec -ErrorAction SilentlyContinue
if (-not $exited) {
  Stop-Process -Id $p.Id -Force -ErrorAction SilentlyContinue
  throw "Timeout after ${TimeoutSec}s: $FilePath $($Args -join ' ')"
}

exit $p.ExitCode
