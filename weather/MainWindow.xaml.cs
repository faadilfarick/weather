using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;

namespace weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        AllWeather _allWeather;
        const string byCityName = @"http://api.openweathermap.org/data/2.5/weather?q={0},&units=metric&appid=6b10c51dd696fece6817ad9d8e9b6936";
        string response;
        public MainWindow()
        {
            InitializeComponent();
            _allWeather = new AllWeather();
        }
        static string city;       

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
                    
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            city = searchTextBox.Text;
            string cityName = string.Format(byCityName, city);
            HttpClient httpClinet = new HttpClient();
            //get the response via http client
            try
            {
                response = await httpClinet.GetStringAsync(cityName);
            }
            catch (Exception httpex)
            {
                MessageBox.Show(httpex.Message + " Check your Internet Connection and Try Again!");
            }
            _allWeather = JsonConvert.DeserializeObject<AllWeather>(response);
            //Temperature
            resultTextBox.Text = Convert.ToString(Math.Round( Convert.ToDouble(_allWeather.main.temp)))+ "° C";
            cityNameLabel.Content = _allWeather.name + ", Sri Lanka";

            //Change background of the window according to temperature
            if(_allWeather.main.temp <= 20)
            {
                mainGrid.Background = new ImageBrush(new BitmapImage(new Uri(@"http://finchcoasters.org.uk/wp-content/uploads/2013/09/winter-470x260.jpg")));
            }
            else if(_allWeather.main.temp <= 25)
            {
                mainGrid.Background = new ImageBrush(new BitmapImage(new Uri(@"https://tribkcpq.files.wordpress.com/2013/06/clouds-mostly-sunny.jpg")));
            }
            else
            {
                mainGrid.Background = new ImageBrush(new BitmapImage(new Uri(@"https://i0.wp.com/blog.allstate.com/wp-content/uploads/2015/06/Heat-Desert-Thinkstock-cropped.png?resize=684%2C340&ssl=1")));
            }
            //end changing background

            //Humidity
            humidityLabel.Content = "Humidity";
            humidityTextBox.Text = Convert.ToString(Math.Round(Convert.ToDouble(_allWeather.main.humidity))) + " %";

            //Cloud Condition
            cloudLabel.Content = "Cloud Codition";
            cloudTextBox.Text = Convert.ToString(Math.Round(Convert.ToDouble(_allWeather.clouds.all))) + " %";

            //Wind Speed
            windLabel.Content = "Wind Speed";
            windTextBox.Text = Convert.ToString(Math.Round(Convert.ToDouble(_allWeather.wind.speed), 2)) + " m/s";

            //imageBox
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("http://openweathermap.org/img/w/" + _allWeather.weather[0].icon + ".png");
            bitmap.EndInit();
            imageBox.Source = bitmap;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            using (StreamWriter str = new StreamWriter("city_weather.txt"))
            {
                str.WriteLine("\n" + "City Name "+ _allWeather.name);
                Favourites fav = new Favourites();
                fav.Show();
            }
        }

        List<string> namelist = new List<string>();

        //Add City to Favourites
        private void addCityButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (StreamReader str = new StreamReader("cityname.txt"))
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);                
            }

            namelist.Add(cityNameLabel.Content.ToString());

            string city = string.Join("\n", namelist.ToArray());

            using (StreamWriter str = new StreamWriter("cityname.txt"))
            {
                str.WriteLine(city);
                
            }

        }
    }
}

