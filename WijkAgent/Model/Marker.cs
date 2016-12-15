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
        public string title;

        #region Constructor
        public Marker(int _id, double _latitude, double _longtitude, string _icon, string _title )
        {
            this.id = _id;
            this.latitude = _latitude;
            this.longtitude = _longtitude;
            this.icon = _icon;
            this.title = _title;
        }
        #endregion

        #region AddMarkerToMap
        public void addMarkerToMap(WebBrowser _wb)
        {
            Object[] _markerArgs = new Object[5];
            _markerArgs[0] = this.latitude;
            _markerArgs[1] = this.longtitude;
            _markerArgs[2] = this.icon;
            _markerArgs[3] = this.id;
            _markerArgs[4] = this.title;
            // invokescript heeft voor de argumenten een object nodig waar deze in staan
            _wb.Document.InvokeScript("AddMarker", _markerArgs);
        }
        #endregion
    }
}
