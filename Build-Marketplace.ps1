$ErrorActionPreference = 'Stop'

$revitVersion = '2027'
$project = Join-Path $PSScriptRoot 'CopyFam.csproj'
$bundle = Join-Path $PSScriptRoot 'dist\CopyFam.bundle'
$contents = Join-Path $bundle "Contents\$revitVersion"
$zip = Join-Path $PSScriptRoot 'dist\CopyFam-v1.0.0-Autodesk-App-Store.zip'

dotnet build $project -c Release -p:RevitVersion=$revitVersion
if ($LASTEXITCODE -ne 0) { throw 'Marketplace build failed.' }

if (Test-Path -LiteralPath $bundle) {
    throw "Marketplace bundle already exists: $bundle. Remove it explicitly before rebuilding."
}

New-Item -ItemType Directory -Path $contents -Force | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Marketplace\PackageContents.xml') -Destination $bundle
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'Marketplace\CopyFam.addin') -Destination $contents
Copy-Item -LiteralPath (Join-Path $PSScriptRoot "bin\Release\$revitVersion\CopyFam.dll") -Destination $contents
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'docs\HELP.md') -Destination $bundle
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'PRIVACY.md') -Destination $bundle
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'LICENSE.md') -Destination $bundle

Compress-Archive -LiteralPath $bundle -DestinationPath $zip -CompressionLevel Optimal
Write-Host "Marketplace package created: $zip"
