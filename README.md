# Umbraco 2fa plugin with Google authenticator
2FA with Google Authenticator and YubiKey (thank's Sebastian)
This version contains google authenticator mechanisem only, and not the yubikey.

Configuration-
1. Change the connection string in the web.config  <add name="umbracoDbDSN"> to your already exists instance of umnraco database.
2. Make sure to install with nuget google authenticator dll.
3. After the umbraco is loading, make sure that the table TwoFactor was create on your database. 
This table will stored your users and manage whether it need 2fa or not.
4. All the relevant code is in the folder of App_plugins/2FactorAuthentication and on the root folder: 2FactorAuthentication
5. For login, you need the user name and passward that already exists in your database(don't use the local mdf file). After regular login you 
can activate the 2fa by setup it on the dashboard under the tab of "two steps verification".
