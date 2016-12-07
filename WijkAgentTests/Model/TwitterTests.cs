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

        //Twitter
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

        //Twitter
        [TestMethod()]
        public void SearchResultsTest_ShouldFindNoResult_WhenMaxResultsIsZero()
        {
            //Arrange
            double latitude = 51.979745;
            double longitude = 5.901053;
            int radius = 100;
            int maxResults = 0;

            Twitter twitter = new Twitter();

            //Act
            twitter.SearchResults(latitude, longitude, radius, maxResults);

            //Assert
            Assert.AreEqual(0, twitter.tweetsList.Count);
        }

        //Twitter
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

        //Twitter
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


        ////////////////////////////////////////////////////////////////////////////

        //Tweet
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

        //Tweet
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

    }
}