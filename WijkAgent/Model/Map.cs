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
    public class Map
    {
        public double defaultLatitude = 52.701800;
        public double defaultLongtitude = 5.389761;
        public double defaultZoom = 8;
        public int idDistrict;
        public WebBrowser wb;

        //voor jouwn locatie LETOP locatie moet aan staan op laptop
        GeoCoordinateWatcher watcher;


        //Onthouden wat de laatst geselecteerd wijk was
        public List<double> currentLatitudePoints;
        public List<double> currentLongitudePoints;

        public Twitter twitter;
        public bool districtSelected = false;

        public Map()
        {
            twitter = new Twitter();
            currentLatitudePoints = new List<double>();
            currentLongitudePoints = new List<double>();
        }

        public void initialize()
        {
            this.wb = new WebBrowser();
            //goede format voor een lokaal bestand zodat je het kan gebruiken in de navigate van webbrowser
            string _curDir = Directory.GetCurrentDirectory();
            var _url = new Uri(String.Format("file:///{0}/{1}", _curDir, "../../Resource/Map.html"));

            //url openen
            this.wb.Navigate(_url);
            this.wb.ScriptErrorsSuppressed = true;

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

        #region ChangeDisctrict_Method
        public void changeDistrict(List<double> _latitudePoints, List<double> _longitudePoints)
        {
            //Watcher aanmaken zodat hij elke keer als je van wijk veranderd je coordinaten worden opgehaald
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            //als de status van de watcher is veranderd stuur ga naar de methode getcurrentlocation
            watcher.StatusChanged += GetCurrentLocation;
            //watcher starten
            watcher.Start();

            currentLatitudePoints = _latitudePoints;
            currentLongitudePoints = _longitudePoints;
            //aantal twitter resultaten
            int _twitterResults = 2000;
            //standaard zoom deze wordt later berekend op de grootte van de wijk. als er toch niet iets verkeerd gaat wordt deze zoom gebruikt
            int _zoom = 14;

            //kijken of de goede coordinaten er zijn
            //Ze moeten dezelfde lengte hebben en allebij minimaal 3 punten anders is het geen geldige polygoon
            if (currentLatitudePoints.Count != currentLongitudePoints.Count || currentLatitudePoints.Count < 3 || currentLongitudePoints.Count < 3)
            {
                MessageBox.Show("Er zijn geen geldige coordinaten voor deze wijk bekend");
            }
            else
            {
                //middelpunt van de wijk
                double _centerLat = (currentLatitudePoints.Max() + currentLatitudePoints.Min()) / 2;
                double _centerLong = (currentLongitudePoints.Max() + currentLongitudePoints.Min()) / 2;

                Marker test = new Marker(500, _centerLat, _centerLong, "blue-pushpin");
                test.addMarkerToMap(this.wb);

                Object[] _initArgs = new Object[3] { _centerLat, _centerLong, _zoom };
                //invokescript heeft voor de argumenten een object nodig waar deze in staan
                this.wb.Document.InvokeScript("initialize", _initArgs);

                //eerst de wijk leeg maken van markers 
                this.wb.Document.InvokeScript("clearMarkers");
                this.twitter.tweetsList.Clear();

                //wijk tekenenen
                drawDistrict(currentLatitudePoints, currentLongitudePoints);

                this.twitter.SearchResults(_centerLat, _centerLong, calculateRadiusKm(_latitudePoints, _longitudePoints, _centerLat, _centerLong), _twitterResults);
                //de markers plaatsen
                this.twitter.setTwitterMarkers(this.wb);

                //voor debuggen radius
                double _test = Math.Floor(calculateRadiusKm(currentLatitudePoints, currentLongitudePoints, _centerLat, _centerLong) * 1000);
                Object[] _circleArgs = new Object[3] { _centerLat, _centerLong, _test };
                this.wb.Document.InvokeScript("SetCircle", _circleArgs); 

                //Er is een wijk geselecteerd
                districtSelected = true;
            }
        }
        #endregion

        #region DrawDistrict_Method
        public void drawDistrict(List<double> _latitudePoints, List<double> _longitudePoints)
        {
            ///maak 2 lange strings van alle coordinaten, lat en long apart
            string _strLatitude = string.Join(" ", _latitudePoints.ToArray());
            string _strLongtitude = string.Join(" ", _longitudePoints.ToArray());

            //de 2 lane string meesturen zodat de wijk getekend kan worden
            Object[] _polyargs = new Object[2] { _strLatitude, _strLongtitude };
            this.wb.Document.InvokeScript("drawPolygon", _polyargs);
        }
        #endregion

        #region CalculateRadiusInKm_Method
        public double calculateRadiusKm(List<double> _latitudePoints, List<double> _longitudePoints, double _centerLat, double _centerLong)
        {
            //wordt eerst berekend in meters
            double _metresFromCenterToCorner = 0;
            //geocoordinate klasse gebruiken. Deze klasse heeft methoe om de distance te berekenen tussen 2 gps coordinaten
            var _centerCoord = new GeoCoordinate(_centerLat, _centerLong);

            //foreach (Tweet t in _twitter.tweetsList)
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
        #endregion

        #region hightlightMarker
        public void hightlightMarker(int _id)
        {
            //stuur het id mee naar een functie in javascript
            Object[] _markerArgs = new Object[1] { _id };
            this.wb.Document.InvokeScript("hightlightMarker", _markerArgs);
        }
        #endregion

        #region GetCurrentLocation
        private void GetCurrentLocation(object sender, GeoPositionStatusChangedEventArgs e)
        {
            //als de status ready is
            if (e.Status == GeoPositionStatus.Ready)
            {
                //nieuwe marker toevoegen met het id dat 1 hoger is dan de twitter list lengte 
                Marker _m = new Marker(twitter.tweetsList.Count + 1, watcher.Position.Location.Latitude, watcher.Position.Location.Longitude, "blue-pushpin");
                _m.addMarkerToMap(this.wb);
                watcher.Stop();
            }
        }
        #endregion

    }
}
