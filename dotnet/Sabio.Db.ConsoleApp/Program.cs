using System;
using System.Data.SqlClient;

namespace Sabio.Db.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string connString = "Data Source=localhost;Initial Catalog=Sabio01;User ID=SabioUser;Password=Sabiopass1!";

            TestConnection(connString);

            Console.ReadLine();//This waits for you to hit the enter key before closing window
        }

        private static void TestConnection(string connString)
        {
            bool isConnected = IsServerConnected(connString);
            Console.WriteLine("DB isConnected = {0}", isConnected);
        }

        private static bool IsServerConnected(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
    }
}