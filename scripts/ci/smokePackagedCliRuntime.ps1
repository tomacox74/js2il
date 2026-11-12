param(
    [Parameter(Mandatory = $true)]
    [ValidateSet(10, 11)]
    [int]$RuntimeMajor,

    [Parameter(Mandatory = $true)]
    [string]$ToolVersion,

    [Parameter(Mandatory = $true)]
    [string]$ArtifactRoot,

    [Parameter(Mandatory = $true)]
    [string]$EvidenceRoot
)

$ErrorActionPreference = 'Stop'

function Invoke-Captured {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [string[]]$Arguments,

        [Parameter(Mandatory = $true)]
        [string]$LogPath,

        [switch]$AllowFailure
    )

    $output = @(& $FilePath @Arguments 2>&1)
    $exitCode = $LASTEXITCODE
    if ($AllowFailure) {
        $global:LASTEXITCODE = 0
    }
    $output | Set-Content -Path $LogPath -Encoding utf8
    foreach ($line in $output) {
        Write-Host "$line"
    }

    if (-not $AllowFailure -and $exitCode -ne 0) {
        throw "'$FilePath $($Arguments -join ' ')' failed with exit code $exitCode. See $LogPath."
    }

    return [pscustomobject]@{
        ExitCode = $exitCode
        Output = $output
    }
}

function New-LocalFeedConfig {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Path,

        [Parameter(Mandatory = $true)]
        [string[]]$Sources
    )

    $sourceXml = for ($index = 0; $index -lt $Sources.Count; $index++) {
        $escapedSource = [System.Security.SecurityElement]::Escape($Sources[$index])
        "<add key=`"source$index`" value=`"$escapedSource`" />"
    }
    @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    $($sourceXml -join "`n    ")
  </packageSources>
</configuration>
"@ | Set-Content -Path $Path -Encoding utf8
}

function Assert-IsolatedRuntime {
    param(
        [Parameter(Mandatory = $true)]
        [string]$DotnetPath,

        [Parameter(Mandatory = $true)]
        [int]$ExpectedMajor,

        [Parameter(Mandatory = $true)]
        [string]$EvidencePath
    )

    $info = Invoke-Captured $DotnetPath @('--info') (Join-Path $EvidencePath 'dotnet-info.txt')
    $sdkOutput = Invoke-Captured $DotnetPath @('--list-sdks') (Join-Path $EvidencePath 'dotnet-sdks.txt')
    $runtimeOutput = Invoke-Captured $DotnetPath @('--list-runtimes') (Join-Path $EvidencePath 'dotnet-runtimes.txt')
    $version = Invoke-Captured $DotnetPath @('--version') (Join-Path $EvidencePath 'dotnet-version.txt')

    $sdkMajors = @(
        $sdkOutput.Output |
            ForEach-Object {
                if ("$_" -match '^\s*(\d+)\.') {
                    [int]$Matches[1]
                }
            }
    )
    $runtimeMajors = @(
        $runtimeOutput.Output |
            ForEach-Object {
                if ("$_" -match '^\s*Microsoft\.\S+\s+(\d+)\.') {
                    [int]$Matches[1]
                }
            }
    )

    if ($sdkMajors.Count -eq 0 -or @($sdkMajors | Where-Object { $_ -ne $ExpectedMajor }).Count -gt 0) {
        throw "The isolated SDK installation must contain only .NET $ExpectedMajor SDKs."
    }

    if ($runtimeMajors.Count -eq 0 -or @($runtimeMajors | Where-Object { $_ -ne $ExpectedMajor }).Count -gt 0) {
        throw "The isolated runtime installation must contain only .NET $ExpectedMajor runtimes."
    }

    if ($version.Output[0].ToString() -notlike "$ExpectedMajor.*") {
        throw "Expected the .NET $ExpectedMajor SDK, but 'dotnet --version' returned '$($version.Output[0])'."
    }

    $coreRuntime = @(
        $runtimeOutput.Output |
            Where-Object { "$_" -match "^\s*Microsoft\.NETCore\.App\s+$ExpectedMajor\." }
    )
    if ($coreRuntime.Count -eq 0) {
        throw "The isolated installation does not contain Microsoft.NETCore.App $ExpectedMajor."
    }
}

function Assert-HostTraceRuntime {
    param(
        [Parameter(Mandatory = $true)]
        [string]$TracePath,

        [Parameter(Mandatory = $true)]
        [int]$ExpectedMajor
    )

    if (-not (Test-Path $TracePath)) {
        throw "The .NET host did not produce the expected trace: $TracePath"
    }

    $trace = Get-Content -Path $TracePath -Raw
    $runtimePathPattern = "shared[\\/]+Microsoft\.NETCore\.App[\\/]+$ExpectedMajor\."
    if ($trace -notmatch $runtimePathPattern) {
        throw "Host trace '$TracePath' does not show resolution to Microsoft.NETCore.App $ExpectedMajor."
    }
}

if (-not (Test-Path $env:RUNNER_TEMP)) {
    throw 'RUNNER_TEMP is not set by the workflow runner.'
}

$artifactRoot = [System.IO.Path]::GetFullPath($ArtifactRoot)
$feedPath = Join-Path $artifactRoot 'feed'
$fixtureSource = Join-Path $artifactRoot 'simple.js'
$sourceRevision = (Get-Content (Join-Path $artifactRoot 'source-sha.txt') -Raw).Trim()
$evidenceRoot = [System.IO.Path]::GetFullPath($EvidenceRoot)
$dotnetRoot = Join-Path $env:RUNNER_TEMP "isolated-dotnet-$RuntimeMajor"
$cliHome = Join-Path $env:RUNNER_TEMP "cli-home-$RuntimeMajor"
$consumerRoot = Join-Path $env:RUNNER_TEMP "consumer-$RuntimeMajor"

foreach ($requiredPath in @($feedPath, $fixtureSource)) {
    if (-not (Test-Path $requiredPath)) {
        throw "Required packaged smoke input is missing: $requiredPath"
    }
}
if ((Test-Path $dotnetRoot) -or (Test-Path $consumerRoot) -or (Test-Path $evidenceRoot)) {
    throw 'The isolated SDK, consumer, and evidence directories must start empty.'
}

New-Item -ItemType Directory -Path $dotnetRoot, $cliHome, $consumerRoot, $evidenceRoot -Force | Out-Null
$artifactFixture = Join-Path $consumerRoot 'tests/simple.js'
New-Item -ItemType Directory -Path (Split-Path $artifactFixture) -Force | Out-Null
Copy-Item $fixtureSource $artifactFixture
Copy-Item (Join-Path $artifactRoot 'source-sha.txt') $evidenceRoot
Set-Content -Path (Join-Path $evidenceRoot 'tool-version.txt') -Value $ToolVersion -Encoding utf8

$installScript = Join-Path $env:RUNNER_TEMP "dotnet-install-$RuntimeMajor"
if ($IsWindows) {
    $installScript += '.ps1'
    Invoke-WebRequest 'https://dot.net/v1/dotnet-install.ps1' -OutFile $installScript
    & $installScript -Channel "$RuntimeMajor.0" -InstallDir $dotnetRoot -NoPath
} else {
    $installScript += '.sh'
    Invoke-WebRequest 'https://dot.net/v1/dotnet-install.sh' -OutFile $installScript
    & bash $installScript --channel "$RuntimeMajor.0" --install-dir $dotnetRoot --no-path
}
if ($LASTEXITCODE -and $LASTEXITCODE -ne 0) {
    throw "Installing the isolated .NET $RuntimeMajor SDK failed with exit code $LASTEXITCODE."
}

$env:DOTNET_ROOT = $dotnetRoot
if ($IsWindows) {
    $env:DOTNET_ROOT_X64 = $dotnetRoot
}
$env:DOTNET_MULTILEVEL_LOOKUP = '0'
$env:DOTNET_CLI_HOME = $cliHome
$env:NUGET_PACKAGES = Join-Path $cliHome 'nuget/packages'
Get-ChildItem Env: |
    Where-Object { $_.Name -like 'DOTNET_ROLL_FORWARD*' } |
    ForEach-Object { Remove-Item "Env:$($_.Name)" }
$env:PATH = @($dotnetRoot, $env:PATH) -join [System.IO.Path]::PathSeparator

$dotnetPath = Join-Path $dotnetRoot $(if ($IsWindows) { 'dotnet.exe' } else { 'dotnet' })
if (-not (Test-Path $dotnetPath)) {
    throw "The isolated dotnet muxer was not installed at $dotnetPath."
}
Set-Location $consumerRoot

$environment = @(
    "Operating system: $([System.Environment]::OSVersion.Platform)"
    "Architecture: $([System.Runtime.InteropServices.RuntimeInformation]::OSArchitecture)"
    "Expected SDK/runtime major: $RuntimeMajor"
    "Tool package version: $ToolVersion"
    "Source revision: $sourceRevision"
    "DOTNET_ROOT: $dotnetRoot"
    "DOTNET_MULTILEVEL_LOOKUP: $env:DOTNET_MULTILEVEL_LOOKUP"
    "DOTNET_ROLL_FORWARD overrides: none"
)
$environment | Set-Content -Path (Join-Path $evidenceRoot 'environment.txt') -Encoding utf8
Assert-IsolatedRuntime $dotnetPath $RuntimeMajor $evidenceRoot

$localConfig = Join-Path $consumerRoot 'NuGet.Config'
New-LocalFeedConfig $localConfig @($feedPath)
$toolPath = Join-Path $consumerRoot 'tools'
New-Item -ItemType Directory -Path $toolPath | Out-Null
Invoke-Captured $dotnetPath @(
    'tool', 'install', '--tool-path', $toolPath, 'jroc',
    '--version', $ToolVersion, '--configfile', $localConfig, '--verbosity', 'quiet'
) (Join-Path $evidenceRoot 'tool-install.log') | Out-Null

$toolExecutable = Join-Path $toolPath $(if ($IsWindows) { 'jroc.exe' } else { 'jroc' })
if (-not (Test-Path $toolExecutable)) {
    throw "The locally installed JROC tool was not found at $toolExecutable."
}
$env:PATH = @($toolPath, $env:PATH) -join [System.IO.Path]::PathSeparator

$cliTrace = Join-Path $evidenceRoot 'cli-host.trace'
$env:DOTNET_HOST_TRACE = '1'
$env:DOTNET_HOST_TRACE_VERBOSITY = '4'
$env:DOTNET_HOST_TRACEFILE = $cliTrace
Invoke-Captured $toolExecutable @('tests/simple.js', '-o', 'out') (Join-Path $evidenceRoot 'compile.log') | Out-Null
Remove-Item Env:DOTNET_HOST_TRACE, Env:DOTNET_HOST_TRACE_VERBOSITY, Env:DOTNET_HOST_TRACEFILE
Assert-HostTraceRuntime $cliTrace $RuntimeMajor

$runtimeConfig = Join-Path $consumerRoot 'out/simple.runtimeconfig.json'
if (-not (Test-Path $runtimeConfig)) {
    throw "The compiler did not produce $runtimeConfig."
}
Copy-Item $runtimeConfig $evidenceRoot

$appTrace = Join-Path $evidenceRoot 'app-host.trace'
$env:DOTNET_HOST_TRACE = '1'
$env:DOTNET_HOST_TRACE_VERBOSITY = '4'
$env:DOTNET_HOST_TRACEFILE = $appTrace
$appResult = Invoke-Captured $dotnetPath @('out/simple.dll') (Join-Path $evidenceRoot 'run.log')
Remove-Item Env:DOTNET_HOST_TRACE, Env:DOTNET_HOST_TRACE_VERBOSITY, Env:DOTNET_HOST_TRACEFILE
Assert-HostTraceRuntime $appTrace $RuntimeMajor

$appOutput = @($appResult.Output | ForEach-Object { "$_" })
foreach ($expectedLine in @('x is 3', 'addNumbers(5, 3) returned 8')) {
    if ($appOutput -notcontains $expectedLine) {
        throw "Expected generated program output to include '$expectedLine'."
    }
}

if ($RuntimeMajor -eq 11) {
    $oldConfig = Join-Path $consumerRoot 'NuGet.NuGetOrg.Config'
    New-LocalFeedConfig $oldConfig @('https://api.nuget.org/v3/index.json')
    $oldToolPath = Join-Path $consumerRoot 'released-0.12.35'
    New-Item -ItemType Directory -Path $oldToolPath | Out-Null
    Invoke-Captured $dotnetPath @(
        'tool', 'install', '--tool-path', $oldToolPath, 'jroc',
        '--version', '0.12.35', '--configfile', $oldConfig, '--verbosity', 'quiet'
    ) (Join-Path $evidenceRoot 'negative-control-install.log') | Out-Null

    $oldTool = Join-Path $oldToolPath $(if ($IsWindows) { 'jroc.exe' } else { 'jroc' })
    $negativeResult = Invoke-Captured $oldTool @('tests/simple.js', '-o', 'negative-control') `
        (Join-Path $evidenceRoot 'negative-control.log') -AllowFailure
    if ($negativeResult.ExitCode -eq 0) {
        throw 'Released JROC 0.12.35 unexpectedly ran on a .NET 11-only installation.'
    }
    $negativeOutput = $negativeResult.Output -join "`n"
    if ($negativeOutput -notmatch 'Microsoft\.NETCore\.App' -or $negativeOutput -notmatch '10\.0\.0') {
        throw 'Released JROC 0.12.35 failed for a reason other than its missing .NET 10 runtime.'
    }
}
