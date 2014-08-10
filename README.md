Lock-free session state module for ASP.NET
==========================================

Usage
-----

1. Install the package.
```
Install-Package Heavysoft.LockFreeSessionState.HashTable
```
2. Add lines to web.config:
```xml
  <system.webServer>
    <modules>
      <remove name="Session" />
      <add name="Session" type="Heavysoft.Web.SessionState.HashTableSessionStateModule,Heavysoft.LockFreeSessionState.HashTable,PublicKeyToken=ea16f0ccebd288da" />      
    </modules>
  </system.webServer>
```
