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
        public char type;
        public string color;
        public string label;

        public Marker(int _id, double _latitude, double _longtitude, char _type )
        {
            this.id = _id;
            this.latitude = _latitude;
            this.longtitude = _longtitude;
            //alles even voor de zekerheid naar hoofdletter voor de check
            this.type = Char.ToUpper(_type);

            setMarkerColor();
        }

        public void setMarkerColor()
        {
            //kijken welke kleur de marker moet zijn
            switch (type)
            {
                case 'T':
                    this.color = "blue";
                    this.label = "Twitter"; 
                    break;
                case 'I':
                    this.color = "white";
                    this.label = "Instagram";
                    break;
                default:
                    this.color = "red";
                    this.label = "Unknown";
                    break;
            }
        }

        public void addMarker(WebBrowser wb)
        {
            Object[] _markerArgs = new Object[5];
            _markerArgs[0] = this.latitude;
            _markerArgs[1] = this.longtitude;
            _markerArgs[2] = this.color;
            _markerArgs[4] = this.label;
            //invokescript heeft voor de argumenten een object nodig waar deze in staan
            wb.Document.InvokeScript("AddMarker", _markerArgs);
        }
    }
}
