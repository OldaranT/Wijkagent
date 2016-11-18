using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgent.Model
{
    class Province
    {
        public List<City> citys;
        public string provinceName;

        public Province(string _provinceName, List<City> _citys)
        {
            provinceName = _provinceName;
            citys = _citys;
        }

        public void printAllData()
        {
            Console.WriteLine("Alle steden:");
            foreach (City c in citys)
            {
                Console.WriteLine(c.cityName);
                c.printDistricts();
            }
        }
    }
}
