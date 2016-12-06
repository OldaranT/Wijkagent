using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;

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

        public void select()
        {

        }
        #region haal alle categorieën op methode
        public Dictionary<int,string> GetAllCategory()
        {
            Dictionary<int,string> category = new Dictionary<int,string>();
            try
            {
                this.conn.Open();
                string stm = "SELECT * FROM category";
                MySqlCommand command = new MySqlCommand(stm, this.conn);
                this.rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    category.Add(Int32.Parse(rdr.GetString(0)), rdr.GetString(1));
                }
                this.conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error bericht: " + e.Message);
            }


            return category;
        }
        #endregion
        #region haal alle vandaag getwitterde twitterberichten in een wijk op
        public Dictionary<int, string> GetAllTwitterMessageFromDistrictToday(int _idDistrict)
        {
            Dictionary<int, string> twitterMessages = new Dictionary<int, string>();

            //goede format die ook in de database staat
            string startDate = DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss");
            string endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            try
            {
                this.conn.Open();
                string stm = "SELECT idtwitter, user, message FROM twitter WHERE iddistrict = @idDistrict AND datetime between @startDate AND @endDate AND save = false";
                MySqlCommand command = new MySqlCommand(stm, this.conn);
                command.Parameters.AddWithValue("@idDistrict", _idDistrict);
                command.Parameters.AddWithValue("@startDate", startDate);
                command.Parameters.AddWithValue("@endDate", endDate);
                this.rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    twitterMessages.Add(Int32.Parse(rdr.GetString(0)), rdr.GetString(1) + "\n" + rdr.GetString(2));
                }

                this.conn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error bericht: " + e.Message);
            }

            return twitterMessages;
        }
        #endregion
        internal void updateTwitterMessageCategory(int _twitterId, string _category)
        {
            try
            {
                this.conn.Open();
                string stm = "UPDATE twitter SET idcategory = (SELECT idcategory FROM category WHERE name = @category), save = 1 WHERE idtwitter = @idTwitter";
                MySqlCommand command = new MySqlCommand(stm, this.conn);
                command.Parameters.AddWithValue("@idTwitter", _twitterId);
                command.Parameters.AddWithValue("@category", _category);
                command.ExecuteNonQuery();

                this.conn.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }

        }
    }

}
