using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Zmanim;
using Zmanim.TimeZone;
using Zmanim.TzDatebase;
using Zmanim.Utilities;

namespace ZmanimFuncs
{
    class Program
    {
        static void Main(string[] args)
        {
            GetZmanim("48237");
            Console.WriteLine(GetZmanim("48237").SofZmanShmaGRA);
            Console.ReadKey(true);
        }

        static ThisWeeksZmanim GetZmanim(string zipCode)
        {
            //Result results[] = new Result();
            var wc = new WebClient();
            string json=wc.DownloadString(
                        "https://maps.googleapis.com/maps/api/geocode/json?address=" + zipCode + "&key=AIzaSyCOCQc4SSF0I0fMNyB-G-TgVOQbeIre0dI");
            var results = JsonConvert.DeserializeObject<JResults>(json);
                //JsonConvert.DeserializeObject<Result[]>(
                //    wc.DownloadString(
                //        "https://maps.googleapis.com/maps/api/geocode/json?address=" + zipCode + "&key=AIzaSyCOCQc4SSF0I0fMNyB-G-TgVOQbeIre0dI"));

            var twz = new ThisWeeksZmanim
            {
                FriPlag = GetTime(results.results[0], Next(DayOfWeek.Friday), "Plag"),
                FriShkia=GetTime(results.results[0],Next(DayOfWeek.Friday),"Shkia"),
                SofZmanShmaMGA = GetTime(results.results[0], Next(DayOfWeek.Saturday), "SofZmanShmaMGA"),
                SofZmanShmaGRA = GetTime(results.results[0], Next(DayOfWeek.Saturday), "SofZmanShmaGRA"),
                Shkia = GetTime(results.results[0], Next(DayOfWeek.Saturday), "SofZmanShmaGRA")

            };
            
            return twz;
        }

        static DateTime GetTime(Result loc, DateTime date, string zman)
        {
            string locationName = loc.address_components[1].short_name + ", " + loc.address_components[3].short_name; //"Lakewood, NJ";
            double latitude = loc.geometry.location.lat; //40.09596; //Lakewood, NJ
            double longitude = loc.geometry.location.lng;//-74.22213; //Lakewood, NJ
            double elevation = 0; //optional elevation
            ITimeZone timeZone = new OlsonTimeZone("America/New_York");
            GeoLocation location = new GeoLocation(locationName, latitude, longitude, elevation, timeZone);
            ComplexZmanimCalendar zc = new ComplexZmanimCalendar(date, location);
            if (zman == "Plag")
            {
                var plagHamincha = zc.GetPlagHamincha();
                if (plagHamincha != null) return (DateTime) plagHamincha;
            }
            else if (zman == "Shkia")
            {
                var dateTime = zc.GetSunset();
                if (dateTime != null) return (DateTime) dateTime;
            }
            else if (zman == "SofZmanShmaMGA")
            {
                var sofZmanShmaMga = zc.GetSofZmanShmaMGA();
                if (sofZmanShmaMga != null) return (DateTime) sofZmanShmaMga;
            }
            else if (zman == "SofZmanShmaGRA")
            {
                var sofZmanShmaGra = zc.GetSofZmanShmaGRA();
                if (sofZmanShmaGra != null) return (DateTime) sofZmanShmaGra;
            }
            return DateTime.Now;
        }
        static DateTime Next(DayOfWeek dayOfWeek)
        {
            int start = (int)DateTime.Now.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return DateTime.Now.AddDays(target - start);
        }
    }
}
