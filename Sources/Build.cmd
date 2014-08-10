@rem VS Express 2013 for Web is used.

set FrameworkPath=C:\Windows\Microsoft.NET\Framework64\v4.0.30319\
"%FrameworkPath%msbuild.exe" %~dp0LockFreeSessionState.sln /t:Clean /p:Configuration=Release /p:VisualStudioVersion=12.0 /m
"%FrameworkPath%msbuild.exe" %~dp0LockFreeSessionState.sln /t:Build /p:Configuration=Release /p:VisualStudioVersion=12.0 /m
