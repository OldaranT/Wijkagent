using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgent.Model
{
    class ModelClass
    {
        public List<Province> provincesList;
        public List<City> cityList1;
        public List<City> cityList2;
        public List<City> cityList3;
        public List<District> districtList1;
        public List<District> districtList2;
        public List<District> districtList3;
        public SQLConnection databaseConnectie;

        public ModelClass()
        {
            databaseConnectie = new SQLConnection();
            District district1 = new District("Berkum", new List<double> { 52.500385, 52.503833, 52.509658, 52.507072 }, new List<double> { 6.055248, 6.047266, 6.055119, 6.066792 });
            District district2 = new District("Diezerpoort", new List<double> { 52.507914, 52.507773, 52.526227, 52.528619 }, new List<double> { 6.080343, 6.117903, 6.104799, 6.074473 });
            District district3 = new District("Spoolde", new List<double> { 52.500385, 52.503833, 52.509658, 52.507072 }, new List<double> { 6.055248, 6.047266, 6.055119, 6.066792 });
            District district4 = new District("Bloemenbuurt", new List<double> { 52.500385, 52.503833, 52.509658, 52.507072 }, new List<double> { 6.055248, 6.047266, 6.055119, 6.066792 });
            District district5 = new District("Regenboogbuurt", new List<double> { 52.500385, 52.503833, 52.509658, 52.507072 }, new List<double> { 6.055248, 6.047266, 6.055119, 6.066792 });
            District district6 = new District("Faunabuurt", new List<double> { 52.500385, 52.503833, 52.509658, 52.507072 }, new List<double> { 6.055248, 6.047266, 6.055119, 6.066792 });
            District district7 = new District("Haarlemmerbuurt", new List<double> { 52.500385, 52.503833, 52.509658, 52.507072 }, new List<double> { 6.055248, 6.047266, 6.055119, 6.066792 });
            District district8 = new District("Jordaan", new List<double> { 52.500385, 52.503833, 52.509658, 52.507072 }, new List<double> { 6.055248, 6.047266, 6.055119, 6.066792 });
            District district9 = new District("Houthavens", new List<double> { 52.500385, 52.503833, 52.509658, 52.507072 }, new List<double> { 6.055248, 6.047266, 6.055119, 6.066792 });
            districtList1 = new List<District> { district1, district2, district3 };
            districtList2 = new List<District> { district4, district5, district6 };
            districtList3 = new List<District> { district7, district8, district9 };
            City city1 = new City("Zwolle", districtList1);
            City city2 = new City("Almere", districtList2);
            City city3 = new City("Amsterdam", districtList3);
            cityList1 = new List<City> { city1 };
            cityList2 = new List<City> { city2 };
            cityList3 = new List<City> { city3 };
            Province province1 = new Province("Overijssel", cityList1);
            Province province2 = new Province("Flevoland", cityList2);
            Province province3 = new Province("Noord-Holland", cityList3);
            provincesList = new List<Province>();
            provincesList.Add(province1);
            provincesList.Add(province2);
            provincesList.Add(province3);

        }

        public void printAllData()
        {
            foreach(Province p in provincesList)
            {
                Console.WriteLine("Alle Provinicies: ");
                Console.WriteLine(p.provinceName);
                p.printAllData();
            }


        }
    }
}
