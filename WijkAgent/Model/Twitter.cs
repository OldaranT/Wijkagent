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

        //Twitter API user
        private string consumerKey = "fNPtDmFBih08YN8q79VQkGWwO";
        private string consumerSecret = "O1sud0fJ4z5V7oOUrwTNcUegRbFo75JmjucDw8hYGkxSicOXui";
        private string UserAccessToken = "235252497-UuElQ941bwmHQAFkvDrIlMA336CPU9btbJWmczcJ";
        private string userAccessSecret = "UWML47HHrI6rcSEaZxdKVV3EXyL7g8sB0Le4YbFtsFD6N";

        private int maxTrendingLength = 14;

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
            //Laad scherm laten zien
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
                    var limitTime = DateTime.Now.AddHours(-24);

                    //Voeg tweets toe aan lijst
                    AddTweets(new Tweet(counter, tweetLatitude, tweetLongitude, user, message, date, limitTime));

                    counter++;
                }
            }
        }
        #endregion

        #region Hier worden de tweets toegevoegd aan een list
        public void AddTweets(Tweet _tweet)
        {
            //controleren of de tweet wel van de afgelopen 24 uur is
            if (_tweet.date > _tweet.limitTime)
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
                Marker _m = new Marker(t.id, t.latitude, t.longitude, "blue-marker");
                _m.addMarkerToMap(_wb);
            }
        }
        #endregion

        #region trending method
        public string TrendingTopics()
        {
            string output = "";

            //twitterTrendingList
            List<string> trendingTweetWord = new List<string>();

            //Initaliseren van _tekst
            var _tekst = "";

            //Maak een lange string van alle twitterberichten          
            foreach (var tweets in tweetsList)
            {
                _tekst += tweets.message + " ";
            }

            //Haal een string op, filter alle woorden eruit
            //Groepeer de woorden die groter zijn dan 3 tekens
            //Sorteer van groot naar klein (van meest voorkomende naar minst voorkomende)
            var words =
            Regex.Split(_tekst.ToLower(), @"\W+")
            .Where(s => s.Length > 3)
            .GroupBy(s => s)
            .OrderByDescending(g => g.Count());

            //Controleer of het woord langer is dan een specifiek aantal karakters
            //Voor de woorden
            //Zo ja, split het woord en voeg het woord toe
            //Zo nee, voeg het wooord alleen toe, zonder aanpassing
            if (words.Count() < 1)
            {
                output = "Er zijn geen trending topics.";
            }
            else
            {
                foreach (var word in words)
                {
                    if (word.Key.Length > maxTrendingLength)
                    {
                        string splittedTweetWord = "";
                        var wordSplit = word.Key.SplitInParts(maxTrendingLength);
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

                //Print de trending woorden op het scherm in een label
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

        public string TrendingTags()
        {
            string output = "";

            List<string> trendingTags = new List<string>();

            //Pak alle twitterberichten die een hashtag bevatten
            var tagsMessage =
                from tweet in tweetsList
                where tweet.message.Contains("#")
                select tweet.message;


            if (tagsMessage.Count() < 1)
            {
                output = "Er zijn geen tags getweet!";
            }
            else
            {
                //Initialiseren van messageTagsString
                string messageTagsString = "";

                //Maak een lange string van alle woorden
                foreach (string tagMessageWord in tagsMessage)
                {
                    messageTagsString += tagMessageWord + " ";
                }

                //Stop alle hashtags in een array
                var tags = Regex.Split(messageTagsString.ToLower(), @"\s+")
                    .Where(a => a.StartsWith("#"))
                    .GroupBy(s => s)
                    .OrderByDescending(g => g.Count());

                //Controleer of het woord langer is dan een specifiek aantal karakters
                //Voor de hashtags
                //Zo ja, split het woord en voeg het woord toe
                //Zo nee, voeg het wooord alleen toe, zonder aanpassing
                foreach (var tag in tags)
                {
                    if (tag.Key.Length > maxTrendingLength)
                    {
                        string splittedTag = "";
                        var tagSplit = tag.Key.SplitInParts(maxTrendingLength);
                        foreach (string split in tagSplit)
                        {
                            splittedTag += split + " ";
                        }
                        trendingTags.Add(splittedTag);
                    }
                    else
                    {
                        trendingTags.Add(tag.Key);
                    }
                }

                //Print de trending hashtags op het scherm in een label
                int _tagCount = trendingTags.Count();
                if (_tagCount < 3)
                {
                    output = "Trending tags:\n";
                    for (int i = 0; i < _tagCount; i++)
                    {
                        output += (i + 1) + ": " + trendingTags[i] + "\n";
                    }
                }
                else
                {
                    output = "Trending tags:\n" + "1: " + trendingTags[0] + "\n2: " + trendingTags[1] + "\n3: " + trendingTags[2];
                }
            }
            return output;

        }
        #endregion

    }
}
