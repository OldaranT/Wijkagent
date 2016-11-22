﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitterAPI.Model;
using System.Device.Location;

namespace WijkAgent.Model
{
    class Map
    {
        public double defaultLatitude = 52.2814566;
        public double defaultLongtitude = 5.3465267;
        public double defaultZoom = 7;
        public WebBrowser wb;


        public Map()
        {

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
            double _centerLat = _latitudePoints.Min() + ((_latitudePoints.Max() - _latitudePoints.Min()) / 2);
            double _centerLong = _longitudePoints.Min() + ((_longitudePoints.Max() - _longitudePoints.Min()) / 2);

            Object[] _initArgs = new Object[3] { _centerLat, _centerLong, setZoom() };
            //invokescript heeft voor de argumenten een object nodig waar deze in staan
            this.wb.Document.InvokeScript("initialize", _initArgs);

            //wijk tekenenen
            drawDistrict(_latitudePoints, _longitudePoints);

            //twitter berichten ophalen
            getTwitterMessages(_centerLong, _centerLat, _latitudePoints[0], _longitudePoints[0]);

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

        public void getTwitterMessages(double _centerLong, double _centerLat, double _pointLat, double _pointLong)
        {
            //berekening om de aantal km voor de radius te berekenen. Vragen hier die geocoordinate klasse aan
            var _centerCoord = new GeoCoordinate(_centerLat, _centerLong);
            var _puntCoord = new GeoCoordinate(_pointLat, _pointLong);

            //is in meters moet naar km
            double _aantalKm = (_centerCoord.GetDistanceTo(_puntCoord) / 1000);

            //krijg de tweets van de coordinaten
            Twitter _twitter = new Twitter();
            _twitter.SearchResults(_centerLat, _centerLong, _aantalKm, 100);
            _twitter.printTweetList();

            foreach (Tweet t in _twitter.tweetsList)
            {
                Marker _m = new Marker(t.id, t.latitude, t.longitude, 'T');
                _m.addMarker(this.wb);
            }
        }

        public int setZoom()
        {
            //moet nog gemaakt worden
            return 16;
        }
    }
}