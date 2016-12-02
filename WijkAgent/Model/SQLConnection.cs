using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WijkAgent.Model
{
    public class SQLConnection
    {
        public MySqlConnection conn;
        public MySqlDataReader rdr;
        public MySqlDataAdapter insrt;
        
        public MySqlCommand cmd;
        public string myConnectionString;

        public SQLConnection()
        {
            myConnectionString = "server=michelvaartjes.nl;uid=micheic28_agent;" +
                        "pwd=kek420;database=micheic28_wijkagent;";
            rdr = null;
            insrt = null;

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


        #region SaveDefaultDistrictUser
        public void SaveDefaultDistrictUser(string _username, int _iddistrict)
        {
            //Open database connectie
            conn.Open();

            string insertstm = "UPDATE account SET iddistrict = @iddistrict WHERE username = @username";
            MySqlCommand updatecmd = new MySqlCommand();
            updatecmd.Connection = conn;
            updatecmd.CommandText = insertstm;
            updatecmd.Parameters.AddWithValue("@iddistrict", _iddistrict);
            updatecmd.Parameters.AddWithValue("@username", _username);

            try
            {
                updatecmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //Sluit database connectie
            conn.Close();
        }
        #endregion

    }

}
