$ErrorActionPreference = 'Stop'
$supportedVersions = @('2024', '2025', '2026', '2027')
$built = @()

foreach ($version in $supportedVersions) {
    $revitPath = "C:\Program Files\Autodesk\Revit $version"
    $apiPath = Join-Path $revitPath 'RevitAPI.dll'
    if (-not (Test-Path -LiteralPath $apiPath -PathType Leaf)) {
        Write-Host "Bỏ qua Revit $version (không cài trên máy)."
        continue
    }

    Write-Host "Đang build cho Revit $version..."
    dotnet build (Join-Path $PSScriptRoot 'CopyFam.csproj') -c Release -p:RevitVersion=$version -p:RevitInstallPath="$revitPath"
    if ($LASTEXITCODE -ne 0) { throw "Build Revit $version thất bại." }
    $built += $version
}

if ($built.Count -eq 0) { throw 'Không tìm thấy Revit 2024–2027 trên máy.' }
Write-Host "Build xong: Revit $($built -join ', ')."
