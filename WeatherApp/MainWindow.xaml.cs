using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void SelectLocation(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            location.Location = myMap.ViewportPointToLocation(e.GetPosition(this));
        }

        public void mapQuery(object sender, EventArgs e)
        {
            string queryLocation = "&lat=" + location.Location.ToString().Split(',')[0] + "&lon=" + location.Location.ToString().Split(',')[1];
            Query(queryLocation);
        }

        public void textQuery(object sender, EventArgs e)
        {
            HttpClient client = new HttpClient();

            string uriString = "http://api.openweathermap.org/geo/1.0/direct?limit=1&appid=0ccaf311e9c2b9d0dc75f35ccb924a0b&q=" + textInput.Text.Replace(' ', '+');

            Uri uri = new Uri(uriString);

            var response = client.GetAsync(uri).Result;

            if(!response.IsSuccessStatusCode)
            {
                //error
            }
            else
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                GeoApiResponse geoApiResponse = JsonConvert.DeserializeObject<GeoApiResponse[]>(responseContent)[0];

                Query("&lat=" + geoApiResponse.lat + "&lon=" + geoApiResponse.lon);
            }
        }

        private void Query(string text)
        {
            HttpClient client = new HttpClient();

            string uriString = "https://api.openweathermap.org/data/2.5/forecast?appid=0ccaf311e9c2b9d0dc75f35ccb924a0b&cnt=3&units=metric" + text;

            Uri uri = new Uri(uriString);

            var response = client.GetAsync(uri).Result;

            if (!response.IsSuccessStatusCode)
            {
                //error
            }
            else
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
                OWApiResponse forecast = JsonConvert.DeserializeObject<OWApiResponse>(responseContent);

                weather.Text = "";

                for(int i = 0; i < 3;  i++)
                {
                    weather.Text += $"{forecast.list[i].dt_txt}\n" +
                        $"  temperature: {forecast.list[i].main.temp}°C" +
                        $"  sensed temperature {forecast.list[i].main.feels_like}°C\n" +
                        $"  pressure: {forecast.list[i].main.pressure}hPa\n" +
                        $"  weather: {forecast.list[i].weather[0].description}\n" +
                        $"  humidity: {forecast.list[i].main.humidity}%" +
                        $"  propability of rain: {forecast.list[i].pop}%\n" +
                        $"  cloudiness: {forecast.list[i].clouds.all}%\n" +
                        $"\n";
                }
            }
        }
    }
}