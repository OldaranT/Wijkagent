using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Device.Location;
using System.Threading;

namespace WijkAgent.Model
{
    public class Map
    {
        public double defaultLatitude = 52.701800;
        public double defaultLongtitude = 5.389761;
        public double defaultZoom = 8;
        public int idDistrict;
        public WebBrowser wb;
        public SQLConnection sql = new SQLConnection();
        //gebr.naam van degene die is ingelogd
        public string username;
        //collega marker id's
        public List<int> colleagueIdList = new List<int>();
        // voor eigen locatie mits locatie aan staat op laptop
        GeoCoordinateWatcher watcher;
        //de thread voor colega
        Thread mapThread;

        // onthouden wat de laatst geselecteerd wijk was
        public List<double> currentLatitudePoints;
        public List<double> currentLongitudePoints;

        public Twitter twitter;
        public bool districtSelected = false;

        #region Constructor
        public Map()
        {
            twitter = new Twitter();
            currentLatitudePoints = new List<double>();
            currentLongitudePoints = new List<double>();

            this.wb = new WebBrowser();
            // goede format voor een lokaal bestand zodat je het kan gebruiken in de navigate van webbrowser
            string _curDir = Directory.GetCurrentDirectory();
            var _url = new Uri(String.Format("file:///{0}/{1}", _curDir, "../../Resource/Map.html"));

            // url openen
            this.wb.Navigate(_url);
            this.wb.ScriptErrorsSuppressed = true;
            Console.WriteLine("tst1 " + this.wb.InvokeRequired);

            // kijken of het geladen is, zo nee blijf doorladen
            while (this.wb.ReadyState != WebBrowserReadyState.Complete)
            {
                Application.DoEvents();
            }

            // nu kan je dingen op de map doen


            // watcher aanmaken zodat elke keer als je van wijk veranderd je coordinaten worden opgehaald
            watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.MovementThreshold = 5;

            // als de status van de watcher is veranderd  ga naar de methode: getcurrentlocation
            watcher.StatusChanged += GetCurrentLocation;

            // watcher starten
            watcher.Start();


        }
        #endregion

        #region Initialize
        public void initialize()
        {
            // map aanroepen
            Object[] _initArgs = new Object[3] { defaultLatitude, defaultLongtitude, defaultZoom };

            // invokescript heeft voor de argumenten een object nodig waar deze in staan
            this.wb.Document.InvokeScript("initialize", _initArgs);

        }
        #endregion

        #region ChangeDistrict
        public void changeDistrict(List<double> _latitudePoints, List<double> _longitudePoints)
        {

            currentLatitudePoints = _latitudePoints;
            currentLongitudePoints = _longitudePoints;

            // aantal twitter resultaten
            int _twitterResults = 2000;

            // standaard zoom, deze wordt later berekend op de grootte van de wijk
            // als er toch niet iets verkeerd gaat wordt deze zoom gebruikt
            int _zoom = 14;

            // kijken of de goede coordinaten er zijn
            // ze moeten dezelfde lengte hebben en allebij minimaal 3 punten, 
            //anders is het geen geldige polygoon
            if (currentLatitudePoints.Count != currentLongitudePoints.Count || currentLatitudePoints.Count < 3 || currentLongitudePoints.Count < 3)
            {
                MessageBox.Show("Er zijn geen geldige coordinaten voor deze wijk bekend");
            }
            else
            {
                // middelpunt van de wijk
                double _centerLat = (currentLatitudePoints.Max() + currentLatitudePoints.Min()) / 2;
                double _centerLong = (currentLongitudePoints.Max() + currentLongitudePoints.Min()) / 2;

                Object[] _initArgs = new Object[3] { _centerLat, _centerLong, _zoom };
                // invokescript heeft voor de argumenten een object nodig waar deze in staan
                this.wb.Document.InvokeScript("initialize", _initArgs);

                // eerst de wijk leeg maken van markers 
                this.wb.Document.InvokeScript("clearMarkers");
                this.twitter.tweetsList.Clear();

                // wijk tekenenen
                drawDistrict(currentLatitudePoints, currentLongitudePoints);

                this.twitter.SearchResults(_centerLat, _centerLong, calculateRadiusKm(_latitudePoints, _longitudePoints, _centerLat, _centerLong), _twitterResults);
                // de markers plaatsen
                this.twitter.setTwitterMarkers(this.wb);

                // voor debuggen radius
                double _test = Math.Floor(calculateRadiusKm(currentLatitudePoints, currentLongitudePoints, _centerLat, _centerLong) * 1000);
                Object[] _circleArgs = new Object[3] { _centerLat, _centerLong, _test };
                this.wb.Document.InvokeScript("SetCircle", _circleArgs);

                // er is een wijk geselecteerd
                ShowColleagues();
                districtSelected = true;
            }
        }
        #endregion

        #region GetChangedPosition
        private void GeoPositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            Object[] args = new Object[3] { twitter.tweetsList.Count + 1, this.watcher.Position.Location.Latitude, this.watcher.Position.Location.Longitude };
            Console.WriteLine(watcher.Position.Location.Latitude + "  " + watcher.Position.Location.Longitude + "  " + this.username);
            wb.Document.InvokeScript("changeMarkerLocation", args);
            try
            {
                sql.ChangeAccountLocation(this.username, this.watcher.Position.Location.Latitude, this.watcher.Position.Location.Longitude);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        #endregion

        #region DrawDistrict
        public void drawDistrict(List<double> _latitudePoints, List<double> _longitudePoints)
        {
            /// maak 2 lange strings van alle coordinaten, lat en long apart
            string _strLatitude = string.Join(" ", _latitudePoints.ToArray());
            string _strLongtitude = string.Join(" ", _longitudePoints.ToArray());

            // de 2 lange strings meesturen zodat de wijk getekend kan worden
            Object[] _polyargs = new Object[2] { _strLatitude, _strLongtitude };
            this.wb.Document.InvokeScript("drawPolygon", _polyargs);
        }
        #endregion

        #region CalculateRadiusKm
        public double calculateRadiusKm(List<double> _latitudePoints, List<double> _longitudePoints, double _centerLat, double _centerLong)
        {
            // wordt eerst berekend in meters
            double _metresFromCenterToCorner = 0;
            // geocoordinate klasse gebruiken. 
            // deze klasse heeft een methode om de distance te berekenen tussen 2 gps coordinaten
            var _centerCoord = new GeoCoordinate(_centerLat, _centerLong);

            for (int i = 0; i < _longitudePoints.Count(); i++)
            {
                var _puntCoord = new GeoCoordinate(_latitudePoints[i], _longitudePoints[i]);
                // methode om de afstand te berekenen 
                // dit doe ik voor elk punt om te kijken welke het verst van het midden punt af ligt
                var _distance = _centerCoord.GetDistanceTo(_puntCoord);

                if (_distance > _metresFromCenterToCorner)
                {
                    _metresFromCenterToCorner = _distance;
                }
            }

            // is in meters, moet naar km
            double _radiusKm = (_metresFromCenterToCorner / 1000);
            return _radiusKm;
        }
        #endregion

        #region HightlightMarker
        public void hightlightMarker(int _id)
        {
            // stuur het id mee naar een functie in javascript
            Object[] _markerArgs = new Object[1] { _id };
            this.wb.Document.InvokeScript("hightlightMarker", _markerArgs);
        }
        #endregion

        #region ShowColleagues
        public void ShowColleagues()
        {
            //reset alle collega's
            if (colleagueIdList.Count > 0)
            {
                for (int i = 0; i < colleagueIdList.Count; i++)
                {
                    Console.WriteLine("id: " + colleagueIdList[i]);
                    this.wb.Invoke(new Action(() => { this.wb.Document.InvokeScript("removeMarker", new Object[1] { colleagueIdList[i] }); }));
                }
                colleagueIdList.Clear();
            }

            //elke marker heeft een id nodig de tweet list heeft een id en je eigen locatie heeft de tweetlist + 1. Begin dus 1 verder dan dat
            int markerId = twitter.tweetsList.Count + 2;
            Dictionary<int, string> _adjecentDistricts = sql.GetAllAdjacentDistricts(6);

            foreach (KeyValuePair<int, string> district in _adjecentDistricts)
            {
                //voor elke aanliggende district kijken wie het is en zijn locatie. district.key is de id van een district
                Dictionary<string, List<double>> _colleagueDic = sql.GetColleagueLocation(district.Key);
                //nu markers maken van elke collega
                foreach (var colleague in _colleagueDic)
                {
                    Marker colleagueMarker = new Marker(markerId, colleague.Value[0], colleague.Value[1], "pink-pushpin", colleague.Key);
                    colleagueMarker.addMarkerToMap(this.wb);
                    colleagueIdList.Add(markerId);
                    markerId = markerId + 1;
                }

            }

            if (colleagueIdList.Count > 0)
            {
                //start een thread die 5 sec duurt als er collega's op de kaart zijn
                this.mapThread = new Thread(new ThreadStart(ColleagueThread));
                mapThread.Start();
            }
        }
        #endregion

        #region GetCurrentLocation
        private void GetCurrentLocation(object sender, GeoPositionStatusChangedEventArgs e)
        {
            Console.WriteLine("eerst"); 
            //als de status is veranderd wil ik elke keer dat de positie veranderd weer de gegevens ophalen
            watcher.PositionChanged += GeoPositionChanged;

            // als de status ready is
            if (e.Status == GeoPositionStatus.Ready)
            {
                Console.WriteLine("id: " + twitter.tweetsList.Count + 1);
                // nieuwe marker toevoegen met het id dat 1 hoger is dan de twitter list lengte 
                Marker _m = new Marker(twitter.tweetsList.Count + 1, watcher.Position.Location.Latitude, watcher.Position.Location.Longitude, "blue-pushpin", "Eigen locatie");
                _m.addMarkerToMap(this.wb);
            }
        }
        #endregion

        public void ColleagueThread()
        {
            //wacht 5 seconden en haal opnieuw de  collega's locatie op
            Thread.Sleep(5000);
            ShowColleagues();
            mapThread.Abort();
        }
    }
}
