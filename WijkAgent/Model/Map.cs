using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Device.Location;

namespace WijkAgent.Model
{
    class Map
    {
        public double defaultLatitude = 52.2814566;
        public double defaultLongtitude = 5.3465267;
        public double defaultZoom = 7;
        public WebBrowser wb;
        public Twitter twitter;


        public Map()
        {
            twitter = new Twitter();
        }

        public void initialize()
        {
            this.wb = new WebBrowser();
            //goede format voor een lokaal bestand zodat je het kan gebruiken in de navigate van webbrowser
            string _curDir = Directory.GetCurrentDirectory();
            var _url = new Uri(String.Format("file:///{0}/{1}", _curDir, "../../Resource/Map.html"));

            //url openen
            this.wb.Navigate(_url);

            //Kijken of het geladen is zo nee blijf doorladen
            while (this.wb.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }
            //nu kan je dingen op de map doen
            //map aanroepen
            Object[] _initArgs = new Object[3] { defaultLatitude, defaultLongtitude, defaultZoom };
            //invokescript heeft voor de argumenten een object nodig waar deze in staan
            this.wb.Document.InvokeScript("initialize", _initArgs);
            
        }

        public void changeDistrict(District _district)
        {
            //standaard zoom deze wordt later berekend op de grootte van de wijk. als er toch niet iets verkeerd gaat wordt deze zoom gebruikt
            int _zoom = 14;

            //de punten van de wijk
            List<double> _latitudePoints = _district.lat;
            List<double> _longitudePoints = _district.lon;

            //kijken of de goede coordinaten er zijn
            //Ze moeten dezelfde lengte hebben en allebij minimaal 3 punten anders is het geen geldige polygoon
            if (_latitudePoints.Count != _longitudePoints.Count || _latitudePoints.Count < 3 || _longitudePoints.Count < 3)
            {
                MessageBox.Show("Er zijn geen geldige coordinaten voor deze wijk bekend");
            }

            //middelpunt van de wijk
            double _centerLat = _latitudePoints.Sum() / _latitudePoints.Count();
            double _centerLong = _longitudePoints.Sum() / _latitudePoints.Count();

            Object[] _initArgs = new Object[3] { _centerLat, _centerLong, _zoom };
            //invokescript heeft voor de argumenten een object nodig waar deze in staan
            this.wb.Document.InvokeScript("initialize", _initArgs);

            //eerst de wijk leeg maken van markers 
            this.wb.Document.InvokeScript("clearMarkers");
            this.twitter.tweetsList.Clear();

            //wijk tekenenen
            drawDistrict(_latitudePoints, _longitudePoints);

            this.twitter.SearchResults(_centerLat, _centerLong, calculateRadiusKm(_latitudePoints, _longitudePoints, _centerLat, _centerLong), 2000);
            //debug console
            this.twitter.printTweetList();
            //de markers plaatsen
            this.twitter.setTwitterMarkers(this.wb);

            //voor debuggen radius
            double _test = Math.Floor(calculateRadiusKm(_latitudePoints, _longitudePoints, _centerLat, _centerLong) * 1000);
            Console.WriteLine("test: " + _test);
            Object[] _circleArgs = new Object[3] { _centerLat, _centerLong, _test };
            this.wb.Document.InvokeScript("SetCircle", _circleArgs);

        }

        public void drawDistrict(List<double> _latitudePoints, List<double> _longitudePoints)
        {
            ///maak 2 lange strings van alle coordinaten, lat en long apart
            string _strLatitude = string.Join(" ", _latitudePoints.ToArray());
            string _strLongtitude = string.Join(" ", _longitudePoints.ToArray());

            //de 2 lane string meesturen zodat de wijk getekend kan worden
            Object[] _polyargs = new Object[2] { _strLatitude, _strLongtitude };
            this.wb.Document.InvokeScript("drawPolygon", _polyargs);
        }

        public double calculateRadiusKm(List<double> _latitudePoints, List<double> _longitudePoints, double _centerLat, double _centerLong)
        {
            //wordt eerst berekend in meters
            double _metresFromCenterToCorner = 0;
            //geocoordinate klasse gebruiken. Deze klasse heeft methoe om de distance te berekenen tussen 2 gps coordinaten
            var _centerCoord = new GeoCoordinate(_centerLat, _centerLong);
            for (int i = 0; i < _longitudePoints.Count(); i++)
            {
                var _puntCoord = new GeoCoordinate(_latitudePoints[i], _longitudePoints[i]);
                //methode om de afstand te berekenen dit doe ik voor elk punt om te kijken welke het verst van het midden punt af ligt
                var _distance = _centerCoord.GetDistanceTo(_puntCoord);

                if(_distance > _metresFromCenterToCorner)
                {
                    _metresFromCenterToCorner = _distance;
                }
            }

            //is in meters moet naar km
            double _radiusKm = (_metresFromCenterToCorner / 1000);

            return _radiusKm;
        }
    }
}
