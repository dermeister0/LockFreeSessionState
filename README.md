Lock-free session state module for ASP.NET
==========================================

This module is a work-around for ASP.NET session lock. It allows to execute slow requests in parallel. Each request can use a session state in read/write mode.

It may be not safe, but can help you enhance site performance if other solutions are not available.

BTW, the best solution is to use read-only session state on all pages except the authentication.

Discussion: http://stackoverflow.com/a/25231036/991267

Usage
-----

### HashTable module

Uses Hashtable-based implementation from MSDN.

* Install the package.

    ```
    Install-Package Heavysoft.LockFreeSessionState.HashTable
    ```
* These lines will be added to web.config:

    ```xml
      <system.webServer>
        <modules>
          <remove name="Session" />
          <add name="Session" type="Heavysoft.Web.SessionState.HashTableSessionStateModule,Heavysoft.LockFreeSessionState.HashTable,PublicKeyToken=ea16f0ccebd288da" />      
        </modules>
      </system.webServer>
    ```

* Lock-free session module will be loaded instead of the standard ASP.NET SessionStateModule.

https://www.nuget.org/packages/Heavysoft.LockFreeSessionState.HashTable/

### SOSS module

Uses ScaleOut StateServer software.

```
Install-Package Heavysoft.LockFreeSessionState.Soss
```

https://www.nuget.org/packages/Heavysoft.LockFreeSessionState.Soss/

Custom storage
--------------

* Install the common package.

    ```
    Install-Package Heavysoft.LockFreeSessionState.Common
    ```
* Inherit the **LockFreeSessionStateModule** class. Implement abstract methods.

https://www.nuget.org/packages/Heavysoft.LockFreeSessionState.Common/
