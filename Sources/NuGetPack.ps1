$root = $PSScriptRoot

Remove-Item "$root\*.nupkg"

& "$root\Build.ps1"

& "$root\Scripts\nuget" pack "$root\Heavysoft.LockFreeSessionState.Common\Heavysoft.LockFreeSessionState.Common.csproj" -Prop Configuration=Release
& "$root\Scripts\nuget" pack "$root\Heavysoft.LockFreeSessionState.HashTable\Heavysoft.LockFreeSessionState.HashTable.csproj" -Prop Configuration=Release
& "$root\Scripts\nuget" pack "$root\Heavysoft.LockFreeSessionState.Soss\Heavysoft.LockFreeSessionState.Soss.csproj" -Prop Configuration=Release
