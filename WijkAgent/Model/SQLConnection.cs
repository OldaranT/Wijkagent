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

        #region Constructor
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
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion

        #region SaveDefaultDistrictUser
        public void SaveDefaultDistrictUser(string _username, int _iddistrict)
        {
            // open database connectie
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
            // sluit database connectie
            conn.Close();
        }
        #endregion

        #region DeleteNotSavedTweetsForDistrict
        public void DeleteUnSavedTweetsForDistrict(int _iddistrict)
        {
            //Open database connectie
            conn.Open();

            //statement voor verwijderen van twitter berichten van desbetrefende wijk. 
            string deletestm = "DELETE FROM twitter WHERE iddistrict = @iddistrict AND datetime < @datetime AND save = false";
            MySqlCommand deletecmd = new MySqlCommand();
            deletecmd.Connection = conn;
            deletecmd.CommandText = deletestm;
            deletecmd.Parameters.AddWithValue("@iddistrict", _iddistrict);

            //Drie dagen terug tellen van de huidige dag.
            deletecmd.Parameters.AddWithValue("@datetime", DateTime.Now.AddDays(-3));

            //Onthouden hoeveel rijen zijn verwijderd.
            int effectedRows = deletecmd.ExecuteNonQuery();

            //Sluit database connectie
            conn.Close();
            string message = "Succes! ";

            if (effectedRows > 0)
            {
                message += "Er zijn " + effectedRows + " tweets verwijderd.";
            }
            else
            {
                message += "Er is niets om schoon te maken";
            }

            MessageBox.Show(message);
        }
        #endregion

        #region QueryVoorGeschiedenis.
        public string AddSelectTwitterToQuery(string _stm)
        {
            return _stm + "SELECT twitter.* FROM twitter ";
        }

        public string JoinDistrictQuery(string _stm)
        {
            string tempDistrictJoinQuery = "JOIN district ON twitter.iddistrict = district.iddistrict ";
            _stm = _stm + tempDistrictJoinQuery;
            return _stm;
        }

        public string JoinCatgoryQuery(string _stm)
        {
            string tempCatgoryJoinQuery = "JOIN category ON twitter.idcategory = category.idcategory ";
            _stm = _stm + tempCatgoryJoinQuery;
            return _stm;
        }

        public string AddWhereToQuery(string _stm)
        {
            return _stm + "WHERE ";
        }

        public string AddAndToQuery(string _stm)
        {
            return _stm + "AND ";
        }
        public string AddOrderByTimeToQuery(string _stm)
        {
            return _stm + " ORDER BY datetime ";
        }
        public string AddLimitToQeury(string _stm, int _resultMax)
        {
            return _stm + "LIMIT " + _resultMax.ToString();
        }

        public string WhereDistrictQuery(string _stm)
        {
            string tempDistrictWhereQuery = "district.name = @districtInput ";
            _stm = _stm + tempDistrictWhereQuery;
            return _stm;
        }

        public string WhereUserQuery(string _stm)
        {
            string tempUserWhereQuery = "twitter.user = @userInput ";
            _stm = _stm + tempUserWhereQuery;
            return _stm;
        }

        public string WhereCategoryQuery(string _stm)
        {
            string tempCatgoryWhereQuery = "category.name = @categoryInput ";
            _stm = _stm + tempCatgoryWhereQuery;
            return _stm;
        }

        public string WhereKeyWordQuery(string _stm)
        {
            string tempKeyWordWhereQuery = "twitter.message LIKE @keyWordInput ";
            _stm = _stm + tempKeyWordWhereQuery;
            return _stm;
        }

        public string WhereDateQuery(string _stm, DateTime _fromDateInput, DateTime _tillDateInput)
        {

            string tempDateWhereQuery = "twitter.datetime BETWEEN @fromDateInput AND @tillDateInput";
            _stm = _stm + tempDateWhereQuery;
            return _stm;
        }
        #endregion

        #region Haal alle categorieën op methode
        public Dictionary<int, string> GetAllCategory()
        {
            Dictionary<int, string> category = new Dictionary<int, string>();
            this.conn.Open();
            try
            {
                string stm = "SELECT * FROM category ORDER BY name";
                MySqlCommand command = new MySqlCommand(stm, this.conn);
                this.rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    category.Add(Int32.Parse(rdr.GetString(0)), rdr.GetString(1));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error bericht: " + e.Message);
            }
            this.conn.Close();
            return category;
        }
        #endregion

        #region Haal alle vandaag getwitterde twitterberichten in een wijk op
        public Dictionary<int, string> GetAllTwitterMessageFromDistrictToday(int _idDistrict)
        {
            Dictionary<int, string> twitterMessages = new Dictionary<int, string>();

            // goede format die ook in de database staat
            // 24 uur geleden vanaf nu
            string startDate = DateTime.Now.Subtract(new TimeSpan(24, 0, 0)).ToString("yyyy-MM-dd HH:mm:ss");
            // hoe laat het nu is
            string endDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            this.conn.Open();
            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Error bericht: " + e.Message);
            }

            this.conn.Close();
            return twitterMessages;
        }
        #endregion

        #region Update Twitterberichten die nog geen categorie hebben en deze een categorie meegeven
        public void updateTwitterMessageCategory(int _twitterId, string _category)
        {
            this.conn.Open();
            try
            {
                string stm = "UPDATE twitter SET idcategory = (SELECT idcategory FROM category WHERE name = @category), save = 1 WHERE idtwitter = @idTwitter";
                MySqlCommand command = new MySqlCommand(stm, this.conn);
                command.Parameters.AddWithValue("@idTwitter", _twitterId);
                command.Parameters.AddWithValue("@category", _category);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            this.conn.Close();
        }
        #endregion

        #region Get_Latest_Selected_Iddistrict_From_User
        public int GetLatestSelectedDisctrictFromUser(string _username)
        {
            // wanneer er geen iddistrict wordt gevonden zal deze functie -1 returnen! 
            int idDisctrict = -1;
            this.conn.Open();

            string stm = "SELECT iddistrict FROM account WHERE username = @username AND iddistrict IS NOT NULL";
            MySqlCommand command = new MySqlCommand(stm, this.conn);
            command.Parameters.AddWithValue("@username", _username);
            this.rdr = command.ExecuteReader();

            if (rdr.Read())
            {
                idDisctrict = Convert.ToInt32(rdr.GetString(0));
            }
            else
            {
                idDisctrict = -1;
            }

            this.conn.Close();
            return idDisctrict;
        }
        #endregion

        #region Get_Selected_DistrictName
        public string GetSelectedDistrictName(int _idDistrict)
        {
            // wanneer er geen iddestrict wordt gevonden zal deze functie -1 returnen! 
            string districtName = "";
            this.conn.Open();
            try
            {
                string stm = "SELECT name FROM district WHERE iddistrict = @iddistrict";
                MySqlCommand command = new MySqlCommand(stm, this.conn);
                command.Parameters.AddWithValue("@iddistrict", _idDistrict);
                this.rdr = command.ExecuteReader();

                while (rdr.Read())
                {
                    districtName = rdr.GetString(0);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error bericht(GetSelectedDistrictName): " + e.Message + Environment.NewLine + "Dit was gevonden: " + districtName);
            }
            this.conn.Close();
            return districtName;
        }
        #endregion

        #region Get_all_coordinates_from_district
        public Dictionary<string, List<double>> GetAllCoordinatesFromDistrict(int _idDistrict)
        {
            Dictionary<string, List<double>> cordinates = new Dictionary<string, List<double>>();
            List<double> latitudeList = new List<double>();
            List<double> longtitudeList = new List<double>();

            conn.Open();
            string stm = "SELECT * FROM coordinate WHERE iddistrict = @iddistrict ORDER BY idcoordinate DESC";
            MySqlCommand cmd = new MySqlCommand(stm, conn);
            cmd.Parameters.AddWithValue("@iddistrict", _idDistrict);
            rdr = cmd.ExecuteReader();

            try
            {
                // hier wordt de database lijst uitgelezen
                while (rdr.Read())
                {
                    latitudeList.Add(Convert.ToDouble(rdr.GetString(2)));
                    longtitudeList.Add(Convert.ToDouble(rdr.GetString(3)));
                }
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            } 
            // database connectie sluiten
            conn.Close();

            cordinates.Add("latitudes", latitudeList);
            cordinates.Add("longitudes", longtitudeList);

            // coordinaten returnen
            return cordinates;
        }
        #endregion

        #region Haal alle aanliggende wijken op
        public Dictionary<int, string> GetAllAdjacentDistricts(int _idDistrict)
        {
            Dictionary<int, string> adjecentDistricts = new Dictionary<int, string>();

            conn.Open();
            try
            {
                string stmt = "SELECT district.iddistrict, district.name FROM district JOIN neighbordistrict ON neighbordistrict.idneighbordistrict = district.iddistrict WHERE neighbordistrict.iddistrict = @idDistrict";
                MySqlCommand command = new MySqlCommand(stmt, this.conn);
                command.Parameters.AddWithValue("@idDistrict", _idDistrict);
                this.rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    adjecentDistricts.Add(Int32.Parse(rdr.GetString(0)), rdr.GetString(1));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error bericht: " + e.Message);
            }
            conn.Close();

            return adjecentDistricts;
        }
        #endregion

        #region reset de eigen locatie coordinaten
        public void ChangeAccountLocation(string _username, double? _latitude, double? _longitude)
        {
            this.conn.Open();
            try
            {
                string stm = "UPDATE account SET latitude = @latitude, longitude = @longitude WHERE username = @username";
                MySqlCommand command = new MySqlCommand(stm, this.conn);
                command.Parameters.AddWithValue("@username", _username);
                command.Parameters.AddWithValue("@latitude", _latitude);
                command.Parameters.AddWithValue("@longitude", _longitude);
                this.rdr = command.ExecuteReader();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error bericht: " + e.Message);
            }
            this.conn.Close();
        }
        #endregion

        #region GetColleaguesLocation
        public Dictionary<string, List<double>> GetColleagueLocation(int _idDisctrict, string _ownUsername)
        {
            Dictionary<string, List<double>> colleagueLocation = new Dictionary<string, List<double>>();
            this.conn.Open();
            try
            {
                //will iedereen hebben die een lat en een long hebben. als ze deze niet hebbe zijn ze offline
                string stmt = "SELECT username, latitude, longitude FROM account WHERE iddistrict = @idDistrict AND latitude IS NOT NULL AND longitude IS NOT NULL AND username != @ownUsername";
                MySqlCommand command = new MySqlCommand(stmt, this.conn);
                command.Parameters.AddWithValue("@idDistrict", _idDisctrict);
                command.Parameters.AddWithValue("@ownUsername", _ownUsername);
                this.rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    List<double> _coordinates = new List<double>();
                    //zijn strings moeten doubles zijn
                    _coordinates.Add(double.Parse(rdr.GetString(1)));
                    _coordinates.Add(double.Parse(rdr.GetString(2)));

                    colleagueLocation.Add(rdr.GetString(0), _coordinates);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error bericht: " + e.Message);
            }
            this.conn.Close();

            return colleagueLocation;
        }
        #endregion

        #region haal verberg tijd op van wijk
        public int GetRefreshButtonHide(int _idDistrict)
        {
            int seconds = 60;
            conn.Open();
            try
            {
                string stm = "SELECT refreshTime FROM district WHERE iddistrict = @iddistrict";
                MySqlCommand command = new MySqlCommand(stm, this.conn);
                command.Parameters.AddWithValue("@iddistrict", _idDistrict);
                this.rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    seconds = rdr.GetInt32(0);
                }
            }
            catch(Exception e)
            {

            }
            conn.Close();
            return seconds;
        }
        #endregion
    }

}
