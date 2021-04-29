using System;
using System.Data.SqlClient;

namespace GameServer
{
    class Database
    {
        static SqlConnection conn;
        static SqlDataAdapter adapter;
        static SqlCommand cmd;
        static SqlDataReader reader;

        const string connString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename='C:\\Users\\USER\\Desktop\\unity\\Maze Rollers\\GameServer\\GameServer\\GameDB.mdf';Integrated Security=True";
        
        public static bool GetData(out int id, string username, out string password, out int colour1, out int colour2)
        {
            //create connection objects
            string query = $"SELECT * FROM Players WHERE username='{username}'";
            conn = new SqlConnection(connString);
            cmd = new SqlCommand(query, conn);

            //create out variables
            id = -1;
            password = "";
            colour1 = -1;
            colour2 = -1;

            try
            {
                conn.Open();
                reader = cmd.ExecuteReader();

                //read data from first entry
                if (reader.Read())
                {
                    //return data
                    id = reader.GetInt32(0);
                    username = reader.GetString(1);
                    password = reader.GetString(2);
                    colour1 = reader.GetInt32(3);
                    colour2 = reader.GetInt32(4);
                }

                //close objects
                reader.Close();
                cmd.Dispose();
                conn.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception at database: {ex}");
                return false;
            }
        }

        //create entry for user
        public static bool RegisterUser(string username, string password)
        {
            //create query and connection objects
            string query = $"INSERT INTO Players VALUES ('{username}', '{password}', {0}, {4})";
            conn = new SqlConnection(connString);
            adapter = new SqlDataAdapter();
            cmd = new SqlCommand(query, conn);

            try
            {
                conn.Open();

                //execute insert query
                adapter.InsertCommand = cmd;
                adapter.InsertCommand.ExecuteNonQuery();

                //close objects
                cmd.Dispose();
                conn.Close();

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception at database: {ex}");
                return false;
            }
        }

        public static bool SetColours(int id, int colour1, int colour2)
        {
            //create connection objects
            string query = $"UPDATE Players SET colour1={colour1}, colour2={colour2} WHERE id={id}";
            conn = new SqlConnection(connString);
            adapter = new SqlDataAdapter();
            cmd = new SqlCommand(query, conn);

            try
            {
                conn.Open();

                //execute command
                adapter.UpdateCommand = cmd;
                adapter.UpdateCommand.ExecuteNonQuery();

                //close
                cmd.Dispose();
                conn.Close();

                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Exception at database: {ex}");
                return false;
            }
        }
    }
}
