using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterAPI.Model
{
    class Tweet
    {
        public int id { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string user { get; set; }
        public string message { get; set; }
        public string date { get; set; }
        public DateTime pastTime { get; set; }
        public DateTime nowTime { get; set; }


        public Tweet(int _id, double _latitude, double _longitude, string _user, string _message, string _date, DateTime _pastTime, DateTime _nowTime)
        {
            id = _id;
            latitude = _latitude;
            longitude = _longitude;
            user = _user;
            message = _message;
            date = _date;
            pastTime = _pastTime;
            nowTime = _nowTime;

        }
    }
}
