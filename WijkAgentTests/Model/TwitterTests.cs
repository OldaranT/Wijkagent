using Microsoft.VisualStudio.TestTools.UnitTesting;
using WijkAgent.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WijkAgent.Model.Tests
{
    [TestClass()]
    public class TwitterTests
    {

        #region Twitter

        #region SearchResultsTest_ShouldFindNoResult_WhenRadiusIsZero
        [TestMethod()]
        public void SearchResultsTest_ShouldFindNoResult_WhenRadiusIsZero()
        {
            //Arrange
            double latitude = 51.979745;
            double longitude = 5.901053;
            int radius = 0;
            int maxResults = 1000;

            Twitter twitter = new Twitter();


            //Act
            twitter.SearchResults(latitude, longitude, radius, maxResults);

            //Assert
            Assert.AreEqual(0, twitter.tweetsList.Count);
        }
        #endregion

        #region AddTweets_ShouldAddTweetToList_WhenTimeIsLessThan24HoursAgo
        [TestMethod()]
        public void AddTweets_ShouldAddTweetToList_WhenTimeIsLessThan24HoursAgo()
        {
            //Arrange
            int id = 1;
            double latitude = 51.979745;
            double longitude = 5.901053;
            string user = "Ruben";
            string message = "Dit is een unit test";
            DateTime date = DateTime.Now.AddHours(-23);
            DateTime limitTime = DateTime.Now.AddHours(-24);
            Tweet tweet = new Tweet(id, latitude, longitude, user, message, date, limitTime);
            Twitter twitter = new Twitter();

            //Act
            twitter.AddTweets(tweet);

            //Assert
            Assert.AreEqual(1, twitter.tweetsList.Count);
        }
        #endregion

        #region AddTweets_ShouldNotAddTweetToList_WhenTimeIsMoreThan24HoursAgo
        [TestMethod()]
        public void AddTweets_ShouldNotAddTweetToList_WhenTimeIsMoreThan24HoursAgo()
        {
            //Arrange
            int id = 1;
            double latitude = 51.979745;
            double longitude = 5.901053;
            string user = "Ruben";
            string message = "Dit is een unit test";
            DateTime date = DateTime.Now.AddHours(-26);
            DateTime limitTime = DateTime.Now.AddHours(-24);
            Tweet tweet = new Tweet(id, latitude, longitude, user, message, date, limitTime);
            Twitter twitter = new Twitter();

            //Act
            twitter.AddTweets(tweet);

            //Assert
            Assert.AreEqual(0, twitter.tweetsList.Count);
        }
        #endregion

        #endregion

        #region Tweet

        #region Tweet_UserShouldNotContainQuote_WhenTweetIsDeclared
        [TestMethod()]
        public void Tweet_UserShouldNotContainQuote_WhenTweetIsDeclared()
        {
            //Arrange
            int id = 1;
            double latitude = 51.979745;
            double longitude = 5.901053;
            string user = "\"Ruben";
            string message = "Dit is een unit test";
            DateTime date = DateTime.Now.AddHours(-20);
            DateTime limitTime = DateTime.Now.AddHours(-24);
            Tweet tweet = new Tweet(id, latitude, longitude, user, message, date, limitTime);
            Twitter twitter = new Twitter();

            //Act
            twitter.AddTweets(tweet);

            //Assert
            Assert.AreEqual(false, twitter.tweetsList[0].user.Contains("\""));
        }
        #endregion

        #region Tweet_ShouldAddItemToLinkList_WhenMessageContainsHttp
        [TestMethod()]
        public void Tweet_ShouldAddItemToLinkList_WhenMessageContainsHttp()
        {
            //Arrange
            int id = 1;
            double latitude = 51.979745;
            double longitude = 5.901053;
            string user = "Ruben";
            string message = "Dit is een unit test met een link. http://test.com";
            DateTime date = DateTime.Now.AddHours(-20);
            DateTime limitTime = DateTime.Now.AddHours(-24);
            Tweet tweet = new Tweet(id, latitude, longitude, user, message, date, limitTime);
            Twitter twitter = new Twitter();

            //Act
            twitter.AddTweets(tweet);

            //Assert
            Assert.AreEqual(1, tweet.links.Count);
        }
        #endregion
        #endregion

        #region Map
        [TestMethod()]
        public void calculateRadiusKm_ShouldReturnRadius_WhenCoordinatesAreGiven()
        {
            //Arrange
            Map map = new Map();
            List<double> latitudePoints = new List<double>();
            List<double> longitudePoints = new List<double>();
            latitudePoints.Add(52.500385);
            latitudePoints.Add(52.503833);
            latitudePoints.Add(52.509658);
            latitudePoints.Add(52.507072);
            longitudePoints.Add(6.055248);
            longitudePoints.Add(6.047266);
            longitudePoints.Add(6.055119);
            longitudePoints.Add(6.066792);
            double centerLat = (latitudePoints.Max() + latitudePoints.Min()) / 2;
            double centerLong = (longitudePoints.Max() + longitudePoints.Min()) / 2;

            double expectedOutcome = 0.69961280774376;

            //Act
            double radius = map.calculateRadiusKm(latitudePoints, longitudePoints, centerLat, centerLong);
            string stringRadius = string.Format("{0:0.00}", radius.ToString());
            radius = Convert.ToDouble(stringRadius);

            //Assert
            Assert.AreEqual(expectedOutcome, radius);
        }
        #endregion


    }
}