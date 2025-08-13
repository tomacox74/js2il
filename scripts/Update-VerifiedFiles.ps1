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
    # Typical pattern: Foo.received.txt -> Foo.verified.txt
    # Use a simpler, reliable replacement: replace the first '.received.' occurrence.
    if ($file.FullName -match '\.received\.') {
        $verifiedFile = $file.FullName -replace '\.received\.', '.verified.'
    }
    else {
        # Fallback: if name ends with .received.<ext>
        $verifiedFile = $file.FullName -replace '\.received(\.[A-Za-z0-9]+)$', '.verified$1'
    }

    if ($verifiedFile -eq $file.FullName) {
        Write-Host "Skipping (no transform): $($file.Name)" -ForegroundColor DarkYellow
        continue
    }

    $targetDir = Split-Path $verifiedFile
    if (-not (Test-Path $targetDir)) {
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    }
    Copy-Item -Path $file.FullName -Destination $verifiedFile -Force
    Write-Host "Updated: $(Split-Path $verifiedFile -Leaf)" -ForegroundColor Cyan
}

Write-Host "✅ All verified files updated from received files." -ForegroundColor Green
