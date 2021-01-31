using System;
using System.Data.SqlClient;

namespace GameServer
{
    class Database
    {
        static SqlConnection conn;

        const string connString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename='C:\\Users\\USER\\Desktop\\unity\\Cyber Final Project\\GameServer\\GameServer\\GameDB.mdf';Integrated Security=True";

        public static string GetPassword(string username)
        {
            //define connection and query
            conn = new SqlConnection(connString);
            string output = "", query = $"select p.password from Players p where username = '{username}'";

            try
            {
                conn.Open();
                //create command and reader objects
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                //read data
                if (reader.Read())
                {
                    //should have only one value
                    output = reader.GetString(0);
                }

                //close objects
                reader.Close();
                cmd.Dispose();
                conn.Close();

                return output;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception at database: {ex}");
                return "-";
            }
        }

        //returns success at insert op
        public static bool InsertData(string username, string password)
        {
            //define connection and query
            conn = new SqlConnection(connString);
            string  query = $"insert into Players values ('{username}', '{password}')";

            try
            {
                conn.Open();
                //create command and reader objects
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataAdapter adapter = new SqlDataAdapter();

                //execute insert query
                adapter.InsertCommand = cmd;
                adapter.InsertCommand.ExecuteNonQuery();

                //close objects
                cmd.Dispose();
                conn.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
