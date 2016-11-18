using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgent.Model
{
    class District
    {
        public string districtName;
        public List<double> lat;
        public List<double> lon;
        
        public District(string _name, List<double> _lat, List<double> _lon)
        {
            districtName = _name;
            lat = _lat;
            lon = _lon;
        }
    }
}
