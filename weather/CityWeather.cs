using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace weather
{
    class CityWeather
    {
        AllWeather _allWeather;

        public int Id { get; set; }
        public double temperature { get; set; }
        public double humidity { get; set; }
        public string cloud_condition { get; set; }
        public double wind_speed { get; set; }

        //Calling the API by passing different city names
        static public string GetWeatherInfo(string City)
        {
            const string byCityName = @"http://api.openweathermap.org/data/2.5/weather?q={0},&units=metric&appid=6b10c51dd696fece6817ad9d8e9b6936";
            string city = City;
            string cityName = string.Format(byCityName, city);
            return cityName;
        }

        //get the response via http client
        static public async void getResponse(string Response)
        {
            AllWeather _allWeather = new AllWeather();

            string response = Response;
            HttpClient httpClinet = new HttpClient();
            try
            {
                response = await httpClinet.GetStringAsync(CityWeather.GetWeatherInfo(response));
            }
            catch (Exception httpex)
            {
                MessageBox.Show(httpex.Message + " Check your Internet Connection and Try Again!");
            }
            _allWeather = JsonConvert.DeserializeObject<AllWeather>(response);
        }
    }
}
