param
(
    [Parameter(Mandatory = $true)]
    [string] $apiKey
)

$root = $PSScriptRoot

Get-Item *.nupkg | % { & "$root\Scripts\nuget" push $_.FullName -Source nuget.org -ApiKey $apiKey }
