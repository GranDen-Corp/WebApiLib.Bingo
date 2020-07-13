#!/usr/bin/env pwsh
param (
  [switch] $CollectCoverage = $false
)
Get-ChildItem test/*Test.csproj -Recurse | ForEach-Object { dotnet test $_.FullName /p:CollectCoverage=$CollectCoverage }
