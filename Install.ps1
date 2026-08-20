$ErrorActionPreference = 'Stop'
$supportedVersions = @('2024', '2025', '2026', '2027')
$installed = @()

foreach ($version in $supportedVersions) {
    $dll = Join-Path $PSScriptRoot "bin\Release\$version\CopyFam.dll"
    if (-not (Test-Path -LiteralPath $dll -PathType Leaf)) { continue }

    $addinsDirectory = Join-Path $env:APPDATA "Autodesk\Revit\Addins\$version"
    $manifestPath = Join-Path $addinsDirectory 'CopyFam.addin'
    New-Item -ItemType Directory -Path $addinsDirectory -Force | Out-Null
    $escapedDll = [Security.SecurityElement]::Escape($dll)
    $manifest = @"
<?xml version="1.0" encoding="utf-8" standalone="no"?>
<RevitAddIns>
  <AddIn Type="Application">
    <Name>CopyFam</Name>
    <Assembly>$escapedDll</Assembly>
    <AddInId>86C08E54-C47B-4D81-8B20-538BF8239088</AddInId>
    <FullClassName>CopyFam.App</FullClassName>
    <VendorId>LOCAL</VendorId>
    <VendorDescription>Cong Thanh Lam — thanhtklam990@gmail.com</VendorDescription>
  </AddIn>
</RevitAddIns>
"@
    Set-Content -LiteralPath $manifestPath -Value $manifest -Encoding UTF8
    $installed += $version
    Write-Host "Đã cài cho Revit $version."
}

if ($installed.Count -eq 0) { throw 'Chưa có bản build. Hãy chạy .\Build-All.ps1 trước.' }
Write-Host 'Hãy đóng và mở lại Revit.'
