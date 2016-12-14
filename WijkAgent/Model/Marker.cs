using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WijkAgent.Model
{
    class Marker
    {
        public int id;
        public double latitude;
        public double longtitude;
        public string icon;

        #region Constructor
        public Marker(int _id, double _latitude, double _longtitude, string _icon )
        {
            this.id = _id;
            this.latitude = _latitude;
            this.longtitude = _longtitude;
            this.icon = _icon;
        }
        #endregion

        #region AddMarkerToMap
        public void addMarkerToMap(WebBrowser _wb)
        {
            Object[] _markerArgs = new Object[4];
            _markerArgs[0] = this.latitude;
            _markerArgs[1] = this.longtitude;
            _markerArgs[2] = this.icon;
            _markerArgs[3] = this.id;
            // invokescript heeft voor de argumenten een object nodig waar deze in staan
            _wb.Document.InvokeScript("AddMarker", _markerArgs);
        }
        #endregion
    }
}
