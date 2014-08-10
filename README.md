Lock-free session state module for ASP.NET
==========================================

Usage
-----

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
