#requires -version 3

Import-Module "$PSScriptRoot\Scripts\BuildHelpers.psm1"

$solution = "$PSScriptRoot\LockFreeSessionStateAll.sln"

Initialize-BuildVariables
Invoke-NugetRestore $solution

$env:HVChangeset = (git 'rev-parse' 'HEAD').SubString(0, 7)

Invoke-SolutionBuild $solution 'Release' 'Clean'
Invoke-SolutionBuild $solution 'Release' 'Build'
