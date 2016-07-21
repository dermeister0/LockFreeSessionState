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
        # https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
        # Warning: NuGet 3.4.4 does not support SemVer 2.0.
        Invoke-WebRequest 'https://dist.nuget.org/win-x86-commandline/v3.5.0-beta2/NuGet.exe' -OutFile $nugetExePath
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
