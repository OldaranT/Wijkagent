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

            //Zoeken op tweets
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
                    var pastTime = matchingtweets.CreatedAt;
                    var nowTime = DateTime.Now.AddHours(-24);

                    //Add tweets to list
                    AddTweets(new Tweet(counter, tweetLatitude, tweetLongitude, user, message, date, pastTime, nowTime));

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
            //controleren of de tweet wel van vandaag is
            if (_tweet.pastTime > _tweet.nowTime)
            {
                tweetsList.Add(_tweet);
            }

        }
        #endregion

        #region Hier worden alle tweets geprint
        public void printTweetList()
        {

            foreach (Tweet tweets in tweetsList)
            {
                if (tweets.pastTime > tweets.nowTime)
                {
                    Console.Write(tweets.id + "\t" + tweets.user + "\n" + tweets.message + "\n" + tweets.date + "\n" + tweets.latitude + " - " + tweets.longitude + "\n\n");
                }
            }
        }
        #endregion

        #region PlaceTwitterWaypointOnMap_method
        public void setTwitterMarkers(WebBrowser _wb)
        {
            foreach (Tweet t in this.tweetsList)
            {
                Marker _m = new Marker(t.id, t.latitude, t.longitude, 'T');
                _m.addMarkerToMap(_wb);
            }
        }
        #endregion

        //back-up

        //    #region GetTrendingTopic
        //    public string getTrendingTopic(string _tekst)
        //    {
        //        var words =
        //        Regex.Split(_tekst, @"\W+")
        //.Where(s => s.Length > 3)
        //.GroupBy(s => s)
        //.OrderByDescending(g => g.Count());

        //        foreach (var word in words)
        //        {
        //            trendingTekst.Add(word.Key);
        //        }

        //        return trendingTekst[0];
        //    }
        //    #endregion

    }
}
