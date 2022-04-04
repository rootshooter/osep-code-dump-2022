using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ps1XorBufEncrypter
{
    class Program
    {
        static void Main(string[] args)
        {
            // insert 32- or 64-bit chsharp shellcode
            byte[] buf = new byte[1] { 0x18 };

            // perfrom xor with key 0x3c
            Console.WriteLine("[+] Encrypting shellcode");
            byte[] crypt = new byte[buf.Length];
            for (int i = 0; i < buf.Length; i++)
            {
                crypt[i] = (byte)((uint)buf[i] ^ 0x3c);
            }

            // build the shellcode and format it
            StringBuilder hex = new StringBuilder(crypt.Length * 2);
            foreach (byte b in crypt)
            {
                hex.AppendFormat("0x{0:x2},", b);
            }

            // print the shellcode in ps1 format
            Console.WriteLine("[+] Here is your new shellcode:\n");
            string NewHex = hex.ToString();
            char[] MyChar = { ',', ' ' };
            string final = NewHex.TrimEnd(MyChar);
            Console.WriteLine($"[Byte[]] $buf = {final}");
        }
    }
}
