using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using Tweetinvi.Parameters;

namespace TwitterAPI.Model
{
    class Twitter
    {
        public List<Tweet> tweetsList = new List<Tweet>();

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

        #region Hier worden de tweets gezocht
        public void SearchResults(double latitude, double longitude, double radius, int maxResults)
        {
            //Pak de datum van vandaag
            DateTime _today = DateTime.Now;
            int _todayDay = _today.Day;
            int _todayMonth = _today.Month;
            int _todayYear = _today.Year;

            //Zoeken op tweets
            var searchParameter = new SearchTweetsParameters("")
            {
                GeoCode = new GeoCode(latitude, longitude, radius, DistanceMeasure.Kilometers),
                MaximumNumberOfResults = maxResults,
                FilterTweetsNotContainingGeoInformation = true,
                Since = new DateTime(_todayYear, _todayMonth, _todayDay)
            };

            var tweets = Search.SearchTweets(searchParameter);

            int _counter = 1;
            foreach (var matchingtweets in tweets)
            {
                if (matchingtweets.Coordinates != null)
                {
                    var _user = matchingtweets.CreatedBy.Name.ToJson();
                    var _date = matchingtweets.CreatedAt.ToLongDateString() + " - " + matchingtweets.CreatedAt.ToLongTimeString();
                    var _message = matchingtweets.ToString();
                    var _latitude = matchingtweets.Coordinates.Latitude;
                    var _longitude = matchingtweets.Coordinates.Longitude;

                    //Add tweets to list
                    AddTweets(new Tweet(_counter, _latitude, _longitude, _user, _message, _date));

                    _counter++;
                }
            }
        }
        #endregion

        #region Hier worden de tweets toegevoegd aan een list
        public void AddTweets(Tweet _tweet)
        {
            tweetsList.Add(_tweet);
        }
        #endregion

        #region Hier worden alle tweets geprint
        public void printTweetList()
        {
            foreach (Tweet tweets in tweetsList)
            {
                Console.Write(tweets.id + "\t" + tweets.user + "\n" + tweets.message + "\n" + tweets.date + "\n" + tweets.latitude + " - " + tweets.longitude + "\n\n");
            }
        }
        #endregion

    }
}
