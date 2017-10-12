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
using MahApps.Metro.Controls.Dialogs;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Microsoft.Win32;
using weather.ViewModel;
using weather.Model;
using System.Windows.Threading;

namespace weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        AllWeather _allWeather;
        ForcastWeather _forcastWeather;
        CurrentCity cc = new CurrentCity();
        string response,forcastresponse;
        public MainWindow()
        {
            InitializeComponent();
            _allWeather = new AllWeather();
            _forcastWeather = new ForcastWeather();
        }
        private async void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            upperGrid.Background = new ImageBrush(new BitmapImage(new Uri(@"http://thedesignblitz.com/wp-content/uploads/2014/03/blur_1x-1.jpg")));
            favUpperGrid.Background = new ImageBrush(new BitmapImage(new Uri(@"http://thedesignblitz.com/wp-content/uploads/2014/03/blur_1x-1.jpg")));
            try
            {
                //GetWeatherDetails of the current City By calling Get Weather Details Methods
                await GetWeatherDetails(CurrentCity.GetCurrentCity());
            }
            catch { }                       
            
            //Timer Setup for reshreshing the weather details of the city displayed each hour
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += Timer_Tick;
            timer.Interval = TimeSpan.FromHours(1);
            timer.Start();            

            //Call GetFavourite Cities Method
            getFavouriteCities();
            //Set Favourites button icon according to favourited or not
            SetFavouriteButtonIcon();
            refreshButton.Background = new ImageBrush(new BitmapImage(new Uri(@"http://downloadicons.net/sites/default/files/refresh-button-icon-56615.png")));
        }
        async void Timer_Tick(object sender, EventArgs e)
        {
            await GetWeatherDetails(_allWeather.name);
            //MessageBox.Show("timer ran");
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
                catch{}
            }
            SetFavouriteButtonIcon();
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

        private void setCurrentCityButton_Click(object sender, RoutedEventArgs e)
        {
            //Update Current City to a text file using SteamWriter
            using (StreamWriter str = new StreamWriter("currentCity.txt"))
            {
                    str.Write(_allWeather.name);
            }
            
        }

        //Add/Remove City to Favourites
        private void setFavouriteButton_Click(object sender, RoutedEventArgs e)
        {
            //Remove a city from favourites
            if(favouriteComboBox.Items.Contains(_allWeather.name))
            {
                favouriteComboBox.Items.Remove(_allWeather.name);
                cityList.Remove(_allWeather.name);
                File.WriteAllText("citynames.txt", String.Empty);
                foreach (var line in cityList)
                {
                    using (StreamWriter str = new StreamWriter("citynames.txt", true))
                    {
                        str.WriteLine(line, true);
                    }
                }
                setFavouriteButton.Background = new ImageBrush(new BitmapImage(new Uri(@"https://cdn2.iconfinder.com/data/icons/business-and-internet/512/Star-512.png")));
                this.ShowMessageAsync("Removed", "The City was removed from favourites");
            }
            else
            {
                //Adding a city to favourites
                using (StreamWriter str = new StreamWriter("citynames.txt", true))
                {
                    str.WriteLine(_allWeather.name, true);
                }
                favouriteComboBox.Items.Add(_allWeather.name);
                setFavouriteButton.Background = new ImageBrush(new BitmapImage(new Uri(@"https://cdn0.iconfinder.com/data/icons/large-black-icons/512/Favourites_favorites_folder.png")));
                this.ShowMessageAsync("Added to Favourites", Convert.ToString(cityNameLabel.Content));
            }       
        }
        void SetFavouriteButtonIcon()
        {
            if (cityList.Contains(_allWeather.name))
            {
                //this.ShowMessageAsync("Already Favourited","");
                setFavouriteButton.Background = new ImageBrush(new BitmapImage(new Uri(@"https://cdn0.iconfinder.com/data/icons/large-black-icons/512/Favourites_favorites_folder.png")));
            }
            else
            {
                setFavouriteButton.Background = new ImageBrush(new BitmapImage(new Uri(@"https://cdn2.iconfinder.com/data/icons/business-and-internet/512/Star-512.png")));
            }
        }
	    //List for saving favourite cities
        List<string> cityList = new List<string>();

        private void getFavouriteCities()
        {
            try
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory + @"\citynames.txt";
                if (File.Exists(path))
                {
                    string[] lineOfContents = File.ReadAllLines(path);
                    foreach (var line in lineOfContents)
                    {
                        favouriteComboBox.Items.Add(line.ToString());
                        cityList.Add(line);
                    }
                }
            }
            catch { }
        }

        private async void goFavouriteButton_Click(object sender, RoutedEventArgs e)
        {
            if (favouriteComboBox.SelectedIndex == -1)
            {
                await this.ShowMessageAsync("Not Selected", "Favouriye City not Selected or not Available");
            }
            else
            {
                tabcontrol.SelectedIndex = 0;
                await GetWeatherDetails(Convert.ToString(favouriteComboBox.SelectedValue));
            }
        }

        private async void refreshButton_Click(object sender, RoutedEventArgs e)
        {
            await GetWeatherDetails(_allWeather.name);
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
                forcastresponse = await httpClinet.GetStringAsync(CityWeather.passCityNameForcast(City));
            }
            catch
            {
                await this.ShowMessageAsync("Not Connected", message: "Unable to Connect to Internet! Please check your connection");
            }

            //Converting the JSON response to classes in the AllWeather Class
            _allWeather = JsonConvert.DeserializeObject<AllWeather>(response);
            _forcastWeather = JsonConvert.DeserializeObject<ForcastWeather>(forcastresponse);

            //Temperature
            temperatureLabel.Content = Math.Round(Convert.ToDouble(_allWeather.main.temp)) + "° C";
            //cityNameLabel.Content = _allWeather.name + ", " + _allWeather.sys.country; //City name and country
            cityNameLabel.Content = _allWeather.name + ", " + _allWeather.sys.country; //City name and country

            //Humidity
            humidityLabel.Content = "Humidity   " + Convert.ToString(Math.Round(Convert.ToDouble(_allWeather.main.humidity))) + " %";

            //Cloud Condition
            cloudLabel.Content = _allWeather.weather[0].description;
            cloudPercentageLabel.Content = Convert.ToString(Math.Round(Convert.ToDouble(_allWeather.clouds.all))) + " % clouds";

            //Wind Speed
            windLabel.Content = "Wind  " + (Math.Round(Convert.ToDouble(_allWeather.wind.speed), 2)) + " m/s";

            //Set Relevant image to the image box considering the cloud condition of the given city
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri("http://openweathermap.org/img/w/" + _allWeather.weather[0].icon + ".png");
            bitmap.EndInit();
            imageBox.Source = bitmap;

            //Day1 Forecast
            Forecast1DayLabel.Content = Convert.ToString(GetDate(_forcastWeather.list[0].dt).DayOfWeek);
            Forecast1TempLabel.Content = Math.Round(Convert.ToDouble(_forcastWeather.list[0].temp.day)) + "° C";
            Forecast1DescLabel.Content = _forcastWeather.list[0].weather[0].description;
            Forecast1Rectangle.Fill = new ImageBrush(new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + _forcastWeather.list[0].weather[0].icon + ".png")));

            //Day2 Forecast
            Forecast2DayLabel.Content = Convert.ToString(GetDate(_forcastWeather.list[1].dt).DayOfWeek);
            Forecast2TempLabel.Content = Math.Round(Convert.ToDouble(_forcastWeather.list[1].temp.day)) + "° C";
            Forecast2DescLabel.Content = _forcastWeather.list[1].weather[0].description;
            Forecast2Rectangle.Fill = new ImageBrush(new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + _forcastWeather.list[1].weather[0].icon + ".png")));

            //Day3 Forecast
            Forecast3DayLabel.Content = Convert.ToString(GetDate(_forcastWeather.list[2].dt).DayOfWeek);
            Forecast3TempLabel.Content = Math.Round(Convert.ToDouble(_forcastWeather.list[2].temp.day)) + "° C";
            Forecast3DescLabel.Content = _forcastWeather.list[2].weather[0].description;
            Forecast3Rectangle.Fill = new ImageBrush(new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + _forcastWeather.list[2].weather[0].icon + ".png")));

            //Day4 Forecast
            Forecast4DayLabel.Content = Convert.ToString(GetDate(_forcastWeather.list[3].dt).DayOfWeek);
            Forecast4TempLabel.Content = Math.Round(Convert.ToDouble(_forcastWeather.list[3].temp.day)) + "° C";
            Forecast4DescLabel.Content = _forcastWeather.list[3].weather[0].description;
            Forecast4Rectangle.Fill = new ImageBrush(new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + _forcastWeather.list[3].weather[0].icon + ".png")));

            //Day5 Forecast
            Forecast5DayLabel.Content = Convert.ToString(GetDate(_forcastWeather.list[4].dt).DayOfWeek);
            Forecast5TempLabel.Content = Math.Round(Convert.ToDouble(_forcastWeather.list[4].temp.day)) + "° C";
            Forecast5DescLabel.Content = _forcastWeather.list[4].weather[0].description;
            Forecast5Rectangle.Fill = new ImageBrush(new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + _forcastWeather.list[4].weather[0].icon + ".png")));

            //Day6 Forecast
            Forecast6DayLabel.Content = Convert.ToString(GetDate(_forcastWeather.list[5].dt).DayOfWeek);
            Forecast6TempLabel.Content = Math.Round(Convert.ToDouble(_forcastWeather.list[5].temp.day)) + "° C";
            Forecast6DescLabel.Content = _forcastWeather.list[5].weather[0].description;
            Forecast6Rectangle.Fill = new ImageBrush(new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + _forcastWeather.list[5].weather[0].icon + ".png")));

            //Day7 Forecast
            Forecast7DayLabel.Content = Convert.ToString(GetDate(_forcastWeather.list[6].dt).DayOfWeek);
            Forecast7TempLabel.Content = Math.Round(Convert.ToDouble(_forcastWeather.list[6].temp.day)) + "° C";
            Forecast7DescLabel.Content = _forcastWeather.list[6].weather[0].description;
            Forecast7Rectangle.Fill = new ImageBrush(new BitmapImage(new Uri(@"http://openweathermap.org/img/w/" + _forcastWeather.list[6].weather[0].icon + ".png")));

            //homeTab.Header = new ImageBrush(new BitmapImage(new Uri(@"https://cdn3.iconfinder.com/data/icons/streamline-icon-set-free-pack/48/Streamline-18-512.pngs")));
            //FavTab.Header = new ImageSource(@"https://d30y9cdsu7xlg0.cloudfront.net/png/59948-200.png");
        }

        private async void refreshButton_Click_1(object sender, RoutedEventArgs e)
        {
            await (GetWeatherDetails(_allWeather.name));
        }
        DateTime GetDate(double miliseconds)
        {
            DateTime day = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc).ToLocalTime();
            day = day.AddSeconds(miliseconds).ToLocalTime();
            return day;
        }
    }
}

