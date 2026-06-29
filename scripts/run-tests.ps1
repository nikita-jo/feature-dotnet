#!/usr/bin/env pwsh
# scripts/run-tests.ps1
# Restore + build + run all tests with coverage; emit cobertura and opencover xmls at the repo root.
[CmdletBinding()]
param(
    [string]$Solution = "EmployeeManagementPortal.sln",
    [string]$TestProject = "tests/EmployeeManagementPortal.Tests/EmployeeManagementPortal.Tests.csproj",
    [string]$CoverageDir = "coverage"
)

$ErrorActionPreference = "Stop"

Write-Host "==> Restoring $Solution"
dotnet restore $Solution

Write-Host "==> Building $Solution (Debug)"
dotnet build $Solution --no-restore --configuration Debug

Write-Host "==> Running tests with coverage"
if (Test-Path $CoverageDir) { Remove-Item -Recurse -Force $CoverageDir }
dotnet test $TestProject `
    --no-build `
    --configuration Debug `
    --results-directory $CoverageDir `
    --collect:"XPlat Code Coverage" `
    -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura,opencover `
    --logger "trx;LogFileName=test-results.trx"

$cobertura = Get-ChildItem -Path $CoverageDir -Filter "coverage.cobertura.xml" -Recurse | Select-Object -First 1
$opencover = Get-ChildItem -Path $CoverageDir -Filter "coverage.opencover.xml" -Recurse | Select-Object -First 1

if ($cobertura) { Copy-Item $cobertura.FullName coverage.cobertura.xml -Force }
if ($opencover) { Copy-Item $opencover.FullName coverage.opencover.xml -Force }

Get-ChildItem -Path . -Filter "coverage*.xml" | Format-Table Name, Length