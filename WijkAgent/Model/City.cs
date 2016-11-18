using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgent.Model
{
    class City
    {
        public List<District> districts;
        public string cityName;

        public City(string _cityName, List<District> _districts)
        {
            cityName = _cityName;
            districts = _districts;
        }

        public void printDistricts()
        {
            Console.WriteLine("Alle Wijken:");
            foreach (District d in districts)
            {
                Console.WriteLine(d.districtName);
            }
        }
    }
}
