using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgent.Model
{
    public class ModelClass
    {
        public SQLConnection databaseConnectie;
        public Map map;
        private string user;
        private double lat;
        private double lon;
        private string message;
        private DateTime datetime;

        public ModelClass()
        {
            databaseConnectie = new SQLConnection();
            map = new Map();
        }

        public void TweetsToDb()
        {
            foreach (Tweet tweet in map.twitter.tweetsList)
            {
                bool inDatabase = true;
                user = tweet.user;
                lat = tweet.latitude;
                lon = tweet.longitude;
                message = tweet.message;
                datetime = tweet.date;

                databaseConnectie.conn.Open();
                string stm = "SELECT * FROM twitter WHERE user = @user AND datetime = @datetime";
                MySqlCommand cmd = new MySqlCommand(stm, databaseConnectie.conn);
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@datetime", datetime);
                databaseConnectie.rdr = cmd.ExecuteReader();

                //Controleert of het twitter bericht al in de database staat
                if (!databaseConnectie.rdr.Read())
                {
                    inDatabase = false;
                    Console.WriteLine("Niet in database, voor nu.....hahahaah.....");
                }

                databaseConnectie.conn.Close();

                if (!inDatabase)
                {
                    databaseConnectie.conn.Open();
                    string insertstm = "INSERT INTO twitter(iddistrict, user, latitude, longitude, message, datetime) VALUES (@iddistrict, @user, @lat, @lon, @message, @datetime)";
                    MySqlCommand insertcmd = new MySqlCommand();
                    insertcmd.Connection = databaseConnectie.conn;
                    insertcmd.CommandText = insertstm;
                    insertcmd.Parameters.AddWithValue("@iddistrict", map.idDistrict);
                    insertcmd.Parameters.AddWithValue("@user", user);
                    insertcmd.Parameters.AddWithValue("@lat", lat);
                    insertcmd.Parameters.AddWithValue("@lon", lon);
                    insertcmd.Parameters.AddWithValue("@message", message);
                    insertcmd.Parameters.AddWithValue("@datetime", datetime);
                    try
                    {
                        insertcmd.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    databaseConnectie.conn.Close();
                }
            }
        }
    }
}
