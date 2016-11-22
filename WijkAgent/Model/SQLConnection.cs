using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WijkAgent.Model
{
    class SQLConnection
    {
        public MySqlConnection conn;
        public MySqlDataReader rdr;
        public MySqlCommand cmd;
        string myConnectionString;

        public SQLConnection()
        {
            myConnectionString = "server=127.0.0.1;port=3307;uid=root;" +
                        "pwd=usbw;database=wijkagent;";
            rdr = null;

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;


                Console.WriteLine("Connectie is gemaakt");
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void select()
        {

        }
    }

}
