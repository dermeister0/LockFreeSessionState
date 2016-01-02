set Version=1.1.0.0

%~dp0\Scripts\nuget push "%~dp0Heavysoft.LockFreeSessionState.Common.%Version%.nupkg"
%~dp0\Scripts\nuget push "%~dp0Heavysoft.LockFreeSessionState.HashTable.%Version%.nupkg"
%~dp0\Scripts\nuget push "%~dp0Heavysoft.LockFreeSessionState.Soss.%Version%.nupkg"
