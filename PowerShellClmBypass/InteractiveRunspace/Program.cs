using System;
using System.IO;
using System.Text;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace InteractiveRunspace
{
    class Program
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
        static void Main(string[] args)
        {
            string cmd;
            string cwd = Directory.GetCurrentDirectory();
            const int BufferSize = 4000;
            Console.SetIn(new StreamReader(Console.OpenStandardInput(), Encoding.UTF8, false, BufferSize));

            Runspace rs = RunspaceFactory.CreateRunspace();
            PowerShell ps = PowerShell.Create();
            rs.Open();
            ps.Runspace = rs;

            while (true)
            {
                Console.Write($"PS {cwd}>");
                cmd = Console.ReadLine();

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
