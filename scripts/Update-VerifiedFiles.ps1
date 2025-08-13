param(
    [string]$VerifyRoot = (Get-Location)
)

# Ensure root exists
if (-not (Test-Path $VerifyRoot)) {
    Write-Host "Root path not found: $VerifyRoot" -ForegroundColor Red
    exit 1
}

# Find all *.received.* files under root
$receivedFiles = Get-ChildItem -Path $VerifyRoot -Recurse -File -Filter "*.received.*"

if (-not $receivedFiles) {
    Write-Host "No received files found in $VerifyRoot" -ForegroundColor Yellow
    exit 0
}

foreach ($file in $receivedFiles) {
    $verifiedFile = $file.FullName -replace '\\.received(\.\w+)$', '.verified$1'
    $targetDir = Split-Path $verifiedFile
    if (-not (Test-Path $targetDir)) {
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    }
    Copy-Item -Path $file.FullName -Destination $verifiedFile -Force
    Write-Host "Updated: $(Split-Path $verifiedFile -Leaf)" -ForegroundColor Cyan
}

Write-Host "✅ All verified files updated from received files." -ForegroundColor Green
