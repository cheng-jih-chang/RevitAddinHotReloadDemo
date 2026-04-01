$ErrorActionPreference = "Stop"

Write-Host "=== BUILD RevitLogic ==="
dotnet build .\RevitLogic\RevitLogic.csproj -v minimal

$srcDll = ".\RevitLogic\bin\Debug\net48\RevitLogic.dll"
$srcPdb = ".\RevitLogic\bin\Debug\net48\RevitLogic.pdb"
$dstDll = ".\dist\RevitLogic.dll"
$dstPdb = ".\dist\RevitLogic.pdb"

Write-Host "=== BEFORE COPY ==="
(Get-Item $srcDll).LastWriteTime
(Get-Item $dstDll).LastWriteTime

Write-Host "=== COPY to dist ==="
Copy-Item $srcDll $dstDll -Force
if (Test-Path $srcPdb) { Copy-Item $srcPdb $dstPdb -Force }

Write-Host "=== AFTER COPY ==="
(Get-Item $dstDll).LastWriteTime
Get-FileHash $dstDll

Write-Host "RevitLogic updated"