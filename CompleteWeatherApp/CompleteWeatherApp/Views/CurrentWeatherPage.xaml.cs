using CompleteWeatherApp.Helper;
using CompleteWeatherApp.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompleteWeatherApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        private string City { get; set; } = "Vladivostok";
        private Location Location { get; set; }

        public CurrentWeatherPage()
        {
            InitializeComponent();
            func_by_loc();
        }

        private async void func_by_loc()
        {
            try
            {
                await GetCoordinates();
                await GetCity(Location);
                await GetWeatherInfo();
                Title = City.Replace(",", ", ");
            }
            catch(Exception ex)
            {

            }
        }

        private async void func_by_city()
        {
            string NewCity = await DisplayPromptAsync("City", "Enter city", "Ok", "Cancel");
            if (NewCity != null)
            {
                try
                {
                    City = NewCity;
                    if (await GetWeatherInfo() == true)
                    {
                        Title = City;
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }

        private async Task<bool> GetCoordinates()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                Location = await Geolocation.GetLocationAsync(request);

                if (Location != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                return false;
            }
        }

        private async Task<bool> GetCity(Location location)
        {
            var places = await Geocoding.GetPlacemarksAsync(Location);
            var currentPlace = places?.FirstOrDefault();

            if (currentPlace != null)
            {
                City = $"{currentPlace.Locality},{currentPlace.CountryName}";
                return true;
            }
            return false;
        }

        private async Task<bool> GetWeatherInfo()
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={City}&appid=bd9dd69e6042fa1dfa85c616e505445c&units=metric";

            var result = await ApiCaller.Get(url);

            if(result.Successful)
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                    descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.weather[0].icon}";
                    cityTxt.Text = weatherInfo.name.ToUpper();
                    temperatureTxt.Text = weatherInfo.main.temp.ToString("0");
                    humidityTxt.Text = $"{weatherInfo.main.humidity}%";
                    pressureTxt.Text = $"{weatherInfo.main.pressure} hpa";
                    windTxt.Text = $"{weatherInfo.wind.speed} m/s";
                    cloudinessTxt.Text = $"{weatherInfo.clouds.all}%";

                    var dt = new DateTime().ToUniversalTime().AddSeconds(weatherInfo.dt);
                    dateTxt.Text = dt.ToString("dddd, MMM dd").ToUpper();
                    GetForecast();
                    return true;
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                    return false;
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No weather information found", "OK");
                return false;
            }
        }

        private async void GetForecast()
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={City}&appid=bd9dd69e6042fa1dfa85c616e505445c&units=metric";
            var result = await ApiCaller.Get(url);

            if (result.Successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    dayOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0");

                    dayTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoImg.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0");

                    dayThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeImg.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0");

                    dayFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourImg.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0");

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }

        private void ToolbarItem_byloc(object sender, EventArgs e)
        {
            func_by_loc();
        }

        private void ToolbarItem_bycity(object sender, EventArgs e)
        {
            func_by_city();
        }
    }
}