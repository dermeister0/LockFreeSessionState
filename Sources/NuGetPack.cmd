powershell -NoProfile -ExecutionPolicy Bypass -File %~dp0Build.ps1

%~dp0\Scripts\nuget Pack %~dp0Heavysoft.LockFreeSessionState.Common\Heavysoft.LockFreeSessionState.Common.csproj -Prop Configuration=Release
%~dp0\Scripts\nuget Pack %~dp0Heavysoft.LockFreeSessionState.HashTable\Heavysoft.LockFreeSessionState.HashTable.csproj -Prop Configuration=Release
%~dp0\Scripts\nuget Pack %~dp0Heavysoft.LockFreeSessionState.Soss\Heavysoft.LockFreeSessionState.Soss.csproj -Prop Configuration=Release
