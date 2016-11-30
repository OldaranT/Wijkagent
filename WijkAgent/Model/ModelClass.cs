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
                user = tweet.user;
                lat = tweet.latitude;
                lon = tweet.longitude;
                message = tweet.message;
                datetime = tweet.date;

                databaseConnectie.conn.Open();
                string stm = "SELECT * FROM twitter WHERE message = @message";
                MySqlCommand cmd = new MySqlCommand(stm, databaseConnectie.conn);
                cmd.Parameters.AddWithValue("@message", message);
                databaseConnectie.rdr = cmd.ExecuteReader();

                databaseConnectie.conn.Close();
                //Console.WriteLine(cmd);

                //string stm = "INSERT INTO twitter(iddistrict, user, latitude, longitude, message, datetime) VALUES (@iddistrict, @user, @lat, @lon, @message, @datetime)";
                //MySqlCommand cmd = new MySqlCommand();
                //cmd.Connection = databaseConnectie.conn;
                //cmd.CommandText = stm;
                //cmd.Parameters.AddWithValue("@iddistrict", 2);
                //cmd.Parameters.AddWithValue("@user", user);
                //cmd.Parameters.AddWithValue("@lat", lat);
                //cmd.Parameters.AddWithValue("@lon", lon);
                //cmd.Parameters.AddWithValue("@message", message);
                //cmd.Parameters.AddWithValue("@datetime", datetime);
                //try
                //{
                //    cmd.ExecuteNonQuery();
                //    Console.WriteLine("executed");
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
            }
        }
    }
}
