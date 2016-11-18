using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgent.Model
{
    class Marker
    {
        public int id;
        public double latitude;
        public double longtitude;

        public Marker(int id, double latitude, double longtitude)
        {
            this.id = id;
            this.latitude = latitude;
            this.longtitude = longtitude;
        }
    }
}
