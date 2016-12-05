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
            return _stm +" ORDER BY datetime ";

        }
        public string AddLimitToQeury(string _stm , int _resultMax)
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
            string tempCatgoryWhereQuery = "category.name = @categoryInput";
            _stm = _stm + tempCatgoryWhereQuery;
            return _stm;
        }

        public string WhereDateQuery(string _stm, DateTime _fromDateInput, DateTime _tillDateInput)
        {

            string tempDateWhereQuery = "twitter.datetime BETWEEN @fromDateInput AND @tillDateInput";
            _stm = _stm + tempDateWhereQuery;
            return _stm;
        }
        #endregion

    }

}
