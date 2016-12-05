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

        public void select()
        {

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

        public string WhereDistrictQuery(string _stm, string _districtInput)
        {
            string tempDistrictWhereQuery = "district.name = '" + _districtInput + "' ";
            _stm = _stm + tempDistrictWhereQuery;
            return _stm;
        }

        public string WhereUserQuery(string _stm, string _userInput)
        {
            string tempUserWhereQuery = "twitter.user = '" + _userInput + "' ";
            _stm = _stm + tempUserWhereQuery;
            return _stm;
        }

        public string WhereCategoryQuery(string _stm, string _categoryInput)
        {
            string tempCatgoryWhereQuery = "category.name = '" + _categoryInput.ToLower() + "' ";
            _stm = _stm + tempCatgoryWhereQuery;
            return _stm;
        }

        public string WhereDateQuery(string _stm, DateTime _fromDateInput, DateTime _tillDateInput)
        {

            string tempDateWhereQuery = "twitter.datetime BETWEEN '" + _fromDateInput.ToString("yyyy-MM-dd ") + " 00:00:01.000000' AND '" + _tillDateInput.ToString("yyyy-MM-dd") + " 23:59:59.000000'";
            _stm = _stm + tempDateWhereQuery;
            return _stm;
        }
    }

}
