﻿using System;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SqlLinkEnum
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
                    String check = $"select version from openquery(\"{target}\", 'select @@version as version');";
                    cmd = new SqlCommand(check, conn);
                    read = cmd.ExecuteReader();
                    read.Read();
                    Console.WriteLine($"[+] Target server version: \n\n{read[0]}");
                    read.Close();

                    String user = $"select myuser from openquery(\"{target}\", 'select SYSTEM_USER as myuser');";
                    cmd = new SqlCommand(user, conn);
                    read = cmd.ExecuteReader();
                    read.Read();
                    Console.WriteLine($"[+] Executing as {read[0]} on {target}");
                    read.Close();
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