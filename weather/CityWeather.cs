using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace weather
{
    class CityWeather
    {
        public int Id { get; set; }
        public double temperature { get; set; }
        public double humidity { get; set; }
        public string cloud_condition { get; set; }
        public double wind_speed { get; set; }
    }
}
