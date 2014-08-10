call %~dp0Build.cmd

nuget Pack %~dp0Heavysoft.LockFreeSessionState.Common\Heavysoft.LockFreeSessionState.Common.csproj -Prop Configuration=Release
nuget Pack %~dp0Heavysoft.LockFreeSessionState.HashTable\Heavysoft.LockFreeSessionState.HashTable.csproj -Prop Configuration=Release