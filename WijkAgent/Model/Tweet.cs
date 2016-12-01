using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WijkAgent.Model
{
    public class Tweet
    {
        public int id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string user { get; set; }
        public string message { get; set; }
        public DateTime date { get; set; }
        public DateTime createTime { get; set; }
        public DateTime limitTime { get; set; }

        public List<string> links {get; set;}

        public Tweet(int _id, double _latitude, double _longitude, string _user, string _message, DateTime _date, DateTime _createTime, DateTime _limitTime)
        {
            id = _id;
            latitude = _latitude;
            longitude = _longitude;
            message = Regex.Replace(_message, @"http[^\s]+", "");
            date = _date;
            createTime = _createTime;
            limitTime = _limitTime;

            //quotes verwijderen
            user = _user.Replace("\"", "");
            links = new List<string>();

            //het woord http uit de resultaten filteren
            foreach (string word in _message.Split(' '))
            {
                if (word.StartsWith("http"))
                {
                    links.Add(word);
                }
            }
        }
    }
}
