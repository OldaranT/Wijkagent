using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        private int trendingLenght = 14;

        // twitter API user
        private string consumerKey = "fNPtDmFBih08YN8q79VQkGWwO";
        private string consumerSecret = "O1sud0fJ4z5V7oOUrwTNcUegRbFo75JmjucDw8hYGkxSicOXui";
        private string UserAccessToken = "235252497-UuElQ941bwmHQAFkvDrIlMA336CPU9btbJWmczcJ";
        private string userAccessSecret = "UWML47HHrI6rcSEaZxdKVV3EXyL7g8sB0Le4YbFtsFD6N";

        #region Constructor
        public Twitter()
        {
            // inloggen bij Twitter
            Auth.SetUserCredentials(consumerKey, consumerSecret, UserAccessToken, userAccessSecret);
        }
        #endregion

        #region SearchTweets_Method
        public void SearchResults(double latitude, double longitude, double radius, int maxResults)
        {
            // laad scherm laten zien
            if (startTwitterSearch != null)
                startTwitterSearch();

            // pak de datum van gisteren
            DateTime today = DateTime.Now.AddDays(-1);
            int todayDay = today.Day;
            int todayMonth = today.Month;
            int todayYear = today.Year;

            // zoeken op tweets met het onderstaande filter
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
                    var limitTime = DateTime.Now.AddHours(-24);

                    // voeg tweets toe aan lijst
                    AddTweets(new Tweet(counter, tweetLatitude, tweetLongitude, user, message, date, limitTime));

                    counter++;
                }
            }
        }
        #endregion

        #region Hier worden de tweets toegevoegd aan een list
        public void AddTweets(Tweet _tweet)
        {
            // controleren of de tweet wel van de afgelopen 24 uur is
            if (_tweet.date > _tweet.limitTime)
            {
                tweetsList.Add(_tweet);
            }
        }
        #endregion

        #region PlaceTwitterWaypointOnMap_method
        public void setTwitterMarkers(WebBrowser _wb)
        {
            // voor elke tweet wordt een marker toegevoegd aan de map
            foreach (Tweet t in this.tweetsList)
            {
                Marker _m = new Marker(t.id, t.latitude, t.longitude, "blue-marker");
                _m.addMarkerToMap(_wb);
            }
        }
        #endregion

        #region TrendingTopics
        public string TrendingTopics()
        {
            string output = "";
            List<string> trendingTweetWord = new List<string>();

            var _tekst = "";

            // maak een lange string van alle berichten
            foreach (var tweet in tweetsList)
            {
                _tekst += tweet.message + " ";
            }

            // maak lijst van woorden nadat je alle woorden apart pakt
            // mits de woorden groter zijn dan 3
            var words =
            Regex.Split(_tekst.ToLower(), @"\W+")
            .Where(s => s.Length > 3)
            .GroupBy(s => s)
            .OrderByDescending(g => g.Count());

            // als er geen woorden zijn gevonden, return: Er zijn geen trending topics.
            if (words.Count() < 1)
            {
                output = "Er zijn geen trending topics.";
            }
            else
            {
                // als het woord langer is dan de opgegeven lengte, plaats er een spatie tussen
                // zodat het woord op de volgende regel komt
                foreach (var word in words)
                {
                    if (word.Key.Length > trendingLenght)
                    {
                        string splittedTweetWord = "";
                        var wordSplit = Split(word.Key, trendingLenght);
                        foreach (string split in wordSplit)
                        {
                            splittedTweetWord += split + " ";
                        }
                        trendingTweetWord.Add(splittedTweetWord);
                    }
                    else
                    {
                        trendingTweetWord.Add(word.Key);
                    }
                }

                // als het aantal trending woorden kleiner is dan 3, dan print hij de hele lijst
                // als het aantal trending woorden 3 of meer is, print hij de eerste drie
                int _wordCount = trendingTweetWord.Count();
                if (_wordCount < 3)
                {
                    output = "Trending topics:\n";
                    for (int i = 0; i < _wordCount; i++)
                    {
                        output += (i + 1) + ": " + trendingTweetWord[i] + "\n";
                    }
                }
                else
                {
                    output = "Trending topics:\n" + "1: " + trendingTweetWord[0] + "\n2: " + trendingTweetWord[1] + "\n3: " + trendingTweetWord[2];
                }

            }
            return output;
        }
        #endregion
        
        #region TredingTags
        public List<string> TrendingTags()
        {
            List<string> outputTrendingTags = new List<string>();

            // haal alle berichten met een hashtag er uit, hij maakt hier een lijst van
            var tagsMessage =
                from tweet in tweetsList
                where tweet.message.Contains("#")
                select tweet.message;

            // als het aantal 0 is, dan print hij: Er zijn geen tags getweet!
            if (tagsMessage.Count() < 1)
            {

            }
            else
            {
                // initialiseren van messageTagsString
                string messageTagsString = "";

                // maak een lange string van alle woorden
                foreach (string tagMessageWord in tagsMessage)
                {
                    messageTagsString += tagMessageWord + " ";
                }
                
                //Stop alle hashtags in een array
                var tags = Regex.Split(messageTagsString, @"\s+")
                    .Where(a => a.StartsWith("#"))
                    .GroupBy(s => s)
                    .OrderByDescending(g => g.Count());


                //Controleer of het woord langer is dan een specifiek aantal karakters
                //Voor de hashtags
                //Zo ja, split het woord en voeg het woord toe
                //Zo nee, voeg het wooord alleen toe, zonder aanpassing
                int tagCount = 0;
                foreach (var tag in tags)
                {
                    if (tagCount < 4)
                    {
                        outputTrendingTags.Add(tag.Key);
                        tagCount++;
                    }
                }
            }
            return outputTrendingTags;
        }
        #endregion

        #region Split
        public List<string> Split(string s, int partLength)
        {
            // split de meegegeven string in twee of meerdere delen, met de lengte van de meegegeven int
            var ListOut = new List<string>();

            for (var i = 0; i < s.Length; i += partLength)
            {
                ListOut.Add(s.Substring(i, Math.Min(partLength, s.Length - i)));
            }
            return ListOut;
        }
        #endregion

        public List<Tweet> getTweetsWithSelectedTag(string _usedTag)
        {
            List<Tweet> tweetsWithTag = new List<Tweet>();

            var tweets =
                from tweet in tweetsList
                where tweet.message.Contains(_usedTag)
                select tweet;

            foreach(var tweet in tweets)
            {
                tweetsWithTag.Add(tweet);
            }

            return tweetsWithTag;
        }
    }
}
