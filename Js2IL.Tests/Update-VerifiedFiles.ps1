# Path to your Verify test results directory
$verifyBasePath = "."

# Find all received files
$receivedFiles = Get-ChildItem -Path $verifyPath -Recurse -File -Filter "*.received.*"

if (-not $receivedFiles) {
    Write-Host "No received files found in $verifyPath" -ForegroundColor Red
    exit 1
}

foreach ($file in $receivedFiles) {
    $verifiedFile = $file.FullName -replace '\.received(\.\w+)$', '.verified$1'

    # Make sure the target directory exists
    $targetDir = Split-Path $verifiedFile
    if (-not (Test-Path $targetDir)) {
        New-Item -ItemType Directory -Path $targetDir -Force | Out-Null
    }

    Copy-Item -Path $file.FullName -Destination $verifiedFile -Force
    Write-Host "Updated:" (Split-Path $verifiedFile -Leaf)
}

Write-Host "✅ All verified files updated from received files." -ForegroundColor Green
