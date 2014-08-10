call %~dp0Build.cmd

nuget Pack %~dp0Heavysoft.LockFreeSessionState.csproj -Prop Configuration=Release