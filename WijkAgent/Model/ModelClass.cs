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

        public ModelClass()
        {
            databaseConnectie = new SQLConnection();

            map = new Map();
        }
    }
}
