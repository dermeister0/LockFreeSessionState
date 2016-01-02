function Initialize-BuildVariables()
{
    $scriptPath = ($env:VS150COMNTOOLS, $env:VS140COMNTOOLS, $env:VS120COMNTOOLS, $env:VS110COMNTOOLS -ne $null)[0] + 'vsvars32.bat'
    &"$PSScriptRoot\Invoke-Environment.ps1" "`"$scriptPath`""
}

function Invoke-NugetRestore([string] $solutionPath)
{
    $nugetExePath = "$PSScriptRoot\nuget.exe"
    
    if (!(Test-Path $nugetExePath))
    {
        Invoke-WebRequest 'http://nuget.org/nuget.exe' -OutFile $nugetExePath
    }
    
    &$nugetExePath 'restore' $solutionPath
}

function Invoke-SolutionBuild([string] $solutionPath, [string] $configuration, [string] $target = 'Build')
{
    msbuild.exe $solutionPath '/m' "/t:$target" "/p:Configuration=$configuration" '/verbosity:normal'
    if ($LASTEXITCODE)
    {
        throw "Build failed."
    }
}
