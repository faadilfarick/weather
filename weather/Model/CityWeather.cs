using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace weather.Model
{
    class CityWeather
    {        
        public int Id { get; set; }
        public double temperature { get; set; }
        public double humidity { get; set; }
        public string cloud_condition { get; set; }
        public double wind_speed { get; set; }

        //Getting Current Weather Details by Calling the OpenWeatheMap API passing given city name
        static public string passCityName(string City)
        {
            const string byCityName = @"http://api.openweathermap.org/data/2.5/weather?q={0},&units=metric&appid=6b10c51dd696fece6817ad9d8e9b6936";
            string city = City;
            string cityName = string.Format(byCityName, city);
            return cityName;
        }
        //Getting Seven day Weather Forecast by Calling OpenWeatherMap API passing given city name
        static public string passCityNameForcast(string City)
        {
            const string byCityNameForcast = @"http://api.openweathermap.org/data/2.5/forecast/daily?q={0},&units=metric&appid=542ffd081e67f4512b705f89d2a611b2";
            string city = City;
            string cityNameForcast = string.Format(byCityNameForcast, city);
            return cityNameForcast;
        }

    }
}
