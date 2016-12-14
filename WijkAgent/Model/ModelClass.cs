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
        public string username;
        public int newTweets;
        private string user;
        private double lat;
        private double lon;
        public int idDistrict;
        private string message;
        private DateTime datetime;

        public ModelClass(string _username)
        {
            databaseConnectie = new SQLConnection();
            map = new Map();
            username = _username;
            newTweets = 0;
        }

        #region InsertNewTweetsIntoDatabase
        public void TweetsToDb()
        {
            newTweets = 0;
            foreach (Tweet tweet in map.twitter.tweetsList)
            {
                bool inDatabase = true;
                idDistrict = map.idDistrict;
                user = tweet.user;
                lat = tweet.latitude;
                lon = tweet.longitude;
                message = tweet.message;
                datetime = tweet.date;

                databaseConnectie.conn.Open();
                string stm = "SELECT * FROM twitter WHERE user = @user AND datetime = @datetime AND iddistrict = @iddistrict";
                MySqlCommand cmd = new MySqlCommand(stm, databaseConnectie.conn);
                cmd.Parameters.AddWithValue("@user", user);
                cmd.Parameters.AddWithValue("@datetime", datetime);
                cmd.Parameters.AddWithValue("@iddistrict", idDistrict);
                databaseConnectie.rdr = cmd.ExecuteReader();

                //Controleert of het twitter bericht al in de database staat
                if (!databaseConnectie.rdr.Read())
                {
                    inDatabase = false;
                    Console.WriteLine("Twitter bericht staat niet in database. Bericht is opgeslagen.");
                    newTweets++;
                }

                databaseConnectie.conn.Close();

                if (!inDatabase)
                {
                    //Twitter bericht in database opslaan
                    databaseConnectie.conn.Open();
                    string insertstm = "INSERT INTO twitter(iddistrict, user, latitude, longitude, message, datetime) VALUES (@iddistrict, @user, @lat, @lon, @message, @datetime)";
                    MySqlCommand insertcmd = new MySqlCommand();
                    insertcmd.Connection = databaseConnectie.conn;
                    insertcmd.CommandText = insertstm;
                    insertcmd.Parameters.AddWithValue("@iddistrict", idDistrict);
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
                        Console.WriteLine("TweetsToDb: " + ex.Message);
                    }
                    databaseConnectie.conn.Close();
                }
            }
        }
        #endregion

        #region ChangeDistrictMethod
        public void ChangeDistrict(int _idDistrict)
        {
            map.idDistrict = _idDistrict;

            //Alle coordinaten(latitude en longitude) vekrijgen
            Dictionary<string, List<double>> allCoordinates = databaseConnectie.GetAllCoordinatesFromDistrict(_idDistrict);

            map.changeDistrict(allCoordinates["latitudes"], allCoordinates["longitudes"]);
        }

        public void ChangeDistrict()
        {
            ChangeDistrict(map.idDistrict);
        }
        #endregion
    }

}
