using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace WijkAgent.Model
{
    public delegate void TwitterSearch();

    public class Twitter
    {
        public List<Tweet> tweetsList = new List<Tweet>();
        public event TwitterSearch startTwitterSearch;
        public event TwitterSearch doneTwitterSearch;

        //Twitter API user
        private string consumerKey = "fNPtDmFBih08YN8q79VQkGWwO";
        private string consumerSecret = "O1sud0fJ4z5V7oOUrwTNcUegRbFo75JmjucDw8hYGkxSicOXui";
        private string UserAccessToken = "235252497-UuElQ941bwmHQAFkvDrIlMA336CPU9btbJWmczcJ";
        private string userAccessSecret = "UWML47HHrI6rcSEaZxdKVV3EXyL7g8sB0Le4YbFtsFD6N";

        #region Constructor
        public Twitter()
        {
            //Inloggen bij Twitter
            Auth.SetUserCredentials(consumerKey, consumerSecret, UserAccessToken, userAccessSecret);
        }
        #endregion

        #region SearchTweets_Method
        public void SearchResults(double latitude, double longitude, double radius, int maxResults)
        {
            if (startTwitterSearch != null)
                startTwitterSearch();

            //Pak de datum van gisteren
            DateTime today = DateTime.Now.AddDays(-1);
            int todayDay = today.Day;
            int todayMonth = today.Month;
            int todayYear = today.Year;

            //Zoeken op tweets met het onderstaande filter
            var searchParameter = new SearchTweetsParameters("")
            {
                GeoCode = new GeoCode(latitude, longitude, radius, DistanceMeasure.Kilometers),
                MaximumNumberOfResults = maxResults,
                FilterTweetsNotContainingGeoInformation = true,
                Since = new DateTime(todayYear, todayMonth, todayDay),
            };

            var tweets = Search.SearchTweets(searchParameter);

            int counter = 1;
            foreach (var matchingtweets in tweets)
            {
                if (matchingtweets.Coordinates != null)
                {
                    var user = matchingtweets.CreatedBy.Name.ToJson();
                    var date = matchingtweets.CreatedAt;
                    var message = matchingtweets.ToString();
                    double tweetLatitude = matchingtweets.Coordinates.Latitude;
                    double tweetLongitude = matchingtweets.Coordinates.Longitude;
                    var createTime = matchingtweets.CreatedAt;
                    var limitTime = DateTime.Now.AddHours(-24);

                    //Voeg tweets toe aan lijst
                    AddTweets(new Tweet(counter, tweetLatitude, tweetLongitude, user, message, date, createTime, limitTime));

                    counter++;
                }
            }
            if (doneTwitterSearch != null)
                doneTwitterSearch();
        }
        #endregion

        #region Hier worden de tweets toegevoegd aan een list
        public void AddTweets(Tweet _tweet)
        {
            //controleren of de tweet wel van de afgelopen 24 uur is
            if (_tweet.createTime > _tweet.limitTime)
            {
                tweetsList.Add(_tweet);
            }

        }
        #endregion

        #region PlaceTwitterWaypointOnMap_method
        public void setTwitterMarkers(WebBrowser _wb)
        {
            //Voor elke tweet wordt een marker toegevoegd aan de map
            foreach (Tweet t in this.tweetsList)
            {
                Marker _m = new Marker(t.id, t.latitude, t.longitude, 'T');
                _m.addMarkerToMap(_wb);
            }
        }
        #endregion


    }
}
