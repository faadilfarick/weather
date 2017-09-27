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
        const string byCityName = @"http://api.openweathermap.org/data/2.5/weather?q={0},Lk&appid=6b10c51dd696fece6817ad9d8e9b6936";
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
            string ex = string.Format(byCityName, city);
            HttpClient httpClinet = new HttpClient();
            try
            {
                response = await httpClinet.GetStringAsync(ex);
            }
            catch (Exception httpex)
            {
                MessageBox.Show(httpex.Message + " Check your Internet Connection and Try Again!");
            }
            _allWeather = JsonConvert.DeserializeObject<AllWeather>(response);

            resultTextBox.Text = Convert.ToString(Math.Round( Convert.ToDouble(_allWeather.main.temp), 2)-273.15);



            //resultTextBox.Text = response;
        }
    }
}

