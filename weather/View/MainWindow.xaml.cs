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
using weather.ViewModel;
using weather.Model;

namespace weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        AllWeather _allWeather;
        CurrentCity cc = new CurrentCity();
        string response;
        public MainWindow()
        {
            InitializeComponent();
            _allWeather = new AllWeather();
        }
        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            upperGrid.Background = new ImageBrush(new BitmapImage(new Uri(@"http://thedesignblitz.com/wp-content/uploads/2014/03/blur_1x-1.jpg")));
            setFavouriteButton.Background = new ImageBrush(new BitmapImage(new Uri(@"https://cdn0.iconfinder.com/data/icons/large-black-icons/512/Favourites_favorites_folder.pngs")));

            //GetWeatherDetails of the current City By calling Get Weather Details Methods
            try
            {
                await GetWeatherDetails(CurrentCity.GetCurrentCity());
            }
            catch{}

            //Favourite Button Load
            await DisplayTileWeatherDetails(Convert.ToString(tile1.Content));
            
        }
        //Searching for a City
        async void searchButton_Click(object sender, RoutedEventArgs e)
        {
            if (searchTextBox.Text == "")
            {
                MessageBox.Show("Please Enter City Name");
            }
            else
            {
                //get the response via http client
                try
                {
                    await GetWeatherDetails(searchTextBox.Text);
                }
                catch
                {
                    
                }
                setFavouriteButton.Visibility = Visibility.Visible;
                setCurrentCityButton.Visibility = Visibility.Visible;
            }
                        
        }


        //Save Weather Information to a Text File
        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            //Save File Dialog
            SaveFileDialog dialog = new SaveFileDialog()
            {
                //Filtering saveing file type
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (dialog.ShowDialog() == true)
            {
                //Save Selected City's information to a text file using SteamWriter
                using (StreamWriter str = new StreamWriter(dialog.FileName))
                {
                    str.WriteLine("City Name         : " + _allWeather.name);
                    str.WriteLine("Temperature       : " + _allWeather.main.temp + "° C");
                    str.WriteLine("Humidity          : " + _allWeather.main.humidity + "%");
                    str.WriteLine("Cloud Condition   : " + _allWeather.clouds.all + "%");
                    str.WriteLine("Wind Speed        : " + _allWeather.wind.speed + " m/s");
                }

            }
        }
        
        //Remove placeholder of searchTextBox when clicking on it
        private void searchTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            searchTextBox.Text = "";
        }

        //Getting Weather Details and Applying them to relavent output areas
        async Task GetWeatherDetails(string city)
        {
            MainWindow mainwindow = new MainWindow();
            string City = city;

            //Using HttpClient Class to send and recieve HTTP responses from identified classes
            HttpClient httpClinet = new HttpClient();
            try
            {
                //Calling GetWeather Info Method
                response = await httpClinet.GetStringAsync(CityWeather.passCityName(City));
            }
            catch (Exception httpex)
            {
                MessageBox.Show(httpex.Message + " Check your Internet Connection and Try Again!");
            }

            //Converting the JSON response to classes in the AllWeather Class
            _allWeather = JsonConvert.DeserializeObject<AllWeather>(response);

            //Temperature
            temperatureLabel.Content = Math.Round(Convert.ToDouble(_allWeather.main.temp)) + "° C";
            //cityNameLabel.Content = _allWeather.name + ", " + _allWeather.sys.country;
            cityNameLabel.Content = _allWeather.name;

            //Humidity
            humidityLabel.Content = "Humidity   " + Convert.ToString(Math.Round(Convert.ToDouble(_allWeather.main.humidity))) + " %";

            //Cloud Condition
            cloudLabel.Content = _allWeather.weather[0].description;
            cloudPercentageLabel.Content = Convert.ToString(Math.Round(Convert.ToDouble(_allWeather.clouds.all))) + " % clouds";

            //Wind Speed
            windLabel.Content = "Wind   " + Convert.ToString(Math.Round(Convert.ToDouble(_allWeather.wind.speed), 2)) + " m/s";

            //Set Relevant image to the image box considering the cloud condition of the given city
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("http://openweathermap.org/img/w/" + _allWeather.weather[0].icon + ".png");
            bitmap.EndInit();
            imageBox.Source = bitmap;
        }

        private void setCurrentCityButton_Click(object sender, RoutedEventArgs e)
        {
            //Update Current City to a text file using SteamWriter
            using (StreamWriter str = new StreamWriter("currentCity.txt"))
            {
                    str.WriteLine(_allWeather.name);
            }
            
        }

        //Add City to Favourites
        private void setFavouriteButton_Click(object sender, RoutedEventArgs e)
        {
            setFavouriteButton.Background = new ImageBrush(new BitmapImage(new Uri(@"https://cdn0.iconfinder.com/data/icons/large-black-icons/512/Favourites_favorites_folder.png")));

            try
            {
                using (StreamReader str = new StreamReader("citynames.txt"))
                {

                }
            }
            catch{}

            //List for saving favourite cities
            List <string> cityList = new List<string>();
            cityList.Add(cityNameLabel.Content.ToString());

            string city = string.Join("\n", cityList.ToArray());

            
            using (StreamReader readtext = new StreamReader("citynames.txt"))
            {
                cityList.Clear(); //cityname list
                string readCity = readtext.ReadToEnd(); // reading city
                string[] citiesName = readCity.Split('\n'); // assigning to array
                
                foreach (string cityname in citiesName)
                {
                    cityList.Add(city); // adding to list
                }
                
            }
            using (StreamWriter str = new StreamWriter("citynames.txt", true))
            {
                str.WriteLine(city);

            }

        }

        private async void Tile_Click(object sender, RoutedEventArgs e)
        {
            tabcontrol.SelectedIndex = 0;
            await GetWeatherDetails(Convert.ToString(tile1.Content));
        }
        //Favourite Tile stuff
        async Task DisplayTileWeatherDetails(string tile)
        {
            string tileresponse = tile;
            //Using HttpClient Class to send and recieve HTTP responses from identified classes
            HttpClient httpClinet = new HttpClient();
            try
            {
                //Calling GetWeather Info Method
                tileresponse = await httpClinet.GetStringAsync(CityWeather.passCityName(tileresponse));
            }
            catch { }

            //Converting the JSON response to classes in the AllWeather Class
            var _allWeather = JsonConvert.DeserializeObject<AllWeather>(tileresponse);

            //Set Relevant image to the tile considering the cloud condition of the given city
            tile1.Background = new ImageBrush(new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + _allWeather.weather[0].icon + ".png")));
        }
    }
}

