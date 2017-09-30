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
            city = cityComboBox.Text;
            string cityName = string.Format(byCityName, city);
            HttpClient httpClinet = new HttpClient();
            try
            {
                response = await httpClinet.GetStringAsync(cityName);
            }
            catch (Exception httpex)
            {
                MessageBox.Show(httpex.Message + " Check your Internet Connection and Try Again!");
            }
            _allWeather = JsonConvert.DeserializeObject<AllWeather>(response);

            resultTextBox.Text = Convert.ToString(Math.Round( Convert.ToDouble(_allWeather.main.temp)))+ "° C";
            cityNameLabel.Content = city + ", Sri Lanka";

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
        }
    }
}

