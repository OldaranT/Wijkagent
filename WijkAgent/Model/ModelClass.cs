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

        public void TweetsToDb(SQLConnection conn)
        {
            //databaseConnectie.conn.Open();
            foreach (Tweet tweet in map.twitter.tweetsList) {
                user = tweet.user;
                lat = tweet.latitude;
                lon = tweet.longitude;
                message = tweet.message;
                datetime = tweet.date;


                string stm = "INSERT INTO twitter(iddistrict, user, latitude, longitude, message, datetime) VALUES ('1', @user, @lat, @lon, @message, @datetime)";
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = conn.conn;
                cmd.CommandText = stm;
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@lat", lat);
                cmd.Parameters.AddWithValue("@lon", lon);
                cmd.Parameters.AddWithValue("@message", message);
                cmd.Parameters.AddWithValue("@datetime", datetime);
                //cmd.Prepare();
                cmd.BeginExecuteNonQuery();
                //cmd.BeginExecuteNonQuery();
                Console.WriteLine("executed");
                Console.WriteLine(stm);
            }
        }
    }
}
