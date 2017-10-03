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
using Microsoft.Win32;

namespace weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        AllWeather _allWeather;
        string response;
        public MainWindow()
        {
            InitializeComponent();
            _allWeather = new AllWeather();
        }
        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            saveButton.Visibility = Visibility.Hidden;
            addCityButton.Visibility = Visibility.Hidden;

            //get the response via http client
            HttpClient httpClinet = new HttpClient();
            try
            {
                response = await httpClinet.GetStringAsync(CityWeather.GetWeatherInfo("Colombo"));
            }
            catch (Exception httpex)
            {
                MessageBox.Show(httpex.Message + " Check your Internet Connection and Try Again!");
            }
            _allWeather = JsonConvert.DeserializeObject<AllWeather>(response);

            //Temperature
            resultTextBox.Text = Math.Round(Convert.ToDouble(_allWeather.main.temp)) + "° C";
            cityNameLabel.Content = _allWeather.name;

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
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //get the response via http client
            HttpClient httpClinet = new HttpClient();
            try
            {
                response = await httpClinet.GetStringAsync(CityWeather.GetWeatherInfo(searchTextBox.Text));
            }
            catch (Exception httpex)
            {
                MessageBox.Show(httpex.Message + " Check your Internet Connection and Try Again!");
            }
            _allWeather = JsonConvert.DeserializeObject<AllWeather>(response);

            //Temperature
            resultTextBox.Text = Math.Round( Convert.ToDouble(_allWeather.main.temp))+ "° C";
            cityNameLabel.Content = _allWeather.name;            

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

            /*Change background of the window according to temperature
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
            end changing background*/

            saveButton.Visibility = Visibility.Visible;
            addCityButton.Visibility = Visibility.Visible;
        }

        //Save Weather Information to a Text File
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            //Save File Dialog
            SaveFileDialog dialog = new SaveFileDialog()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (dialog.ShowDialog() == true)
            {
                //Save Selected City's infomation to a text file using SteamWriter
                using (StreamWriter str = new StreamWriter(dialog.FileName))
                {
                    str.WriteLine("\n" + "City Name         : " + _allWeather.name);
                    str.WriteLine("\n" + "Temperature       : " + _allWeather.main.temp + "° C");
                    str.WriteLine("\n" + "Temperature       : " + _allWeather.main.humidity + "%");
                    str.WriteLine("\n" + "Cloud Condition   : " + _allWeather.clouds.all + "%");
                    str.WriteLine("\n" + "Wind Speed        : " + _allWeather.wind.speed + " m/s");
                }

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
            /*using (StreamReader readtext = new StreamReader("cityname.txt"))
            {
                names.Clear(); //cityname list
                string readCity = readtext.ReadToEnd(); // reading city
                string[] citiesName = readCity.Split('\n'); // assigning to array


                foreach (string city in citiesName)
                {

                    names.Add(city); // adding to list
                }


            }*/


        }
    }
}

