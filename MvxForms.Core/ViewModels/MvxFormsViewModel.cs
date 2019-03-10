using System;
using System.Net.Http;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.ViewModels;
using Newtonsoft.Json;

namespace MvxForms.Core.ViewModels
{
    public class MvxFormsViewModel : MvxViewModel
    {
        private const string ApiKey = "03e8168ffbb8cafb6b8b6679c528ec97";

        private const string Text = "Today rain is expected";

        private const string CelsiusSign = "°C";

        private const string DateKey = "Date";

        public IMvxAsyncCommand GetWeatherCommand => new MvxAsyncCommand(GetWeatherAsync);

        private string city;
        private string highTemp;
        private string lowTemp;
        private string description;
        private string message;

        public string City
        {
            get => city;
            set { SetProperty(ref city, value); }
        }

        public string HighTemp
        {
            get => highTemp;
            set { SetProperty(ref highTemp, value); }
        }

        public string LowTemp
        {
            get => lowTemp;
            set { SetProperty(ref lowTemp, value); }
        }

        public string Description
        {
            get => description;
            set { SetProperty(ref description, value); }
        }

        public string Message
        {
            get => message;
            set { SetProperty(ref message, value); }
        }

        private async Task GetWeatherAsync()
        {
            try
            {
                var cityText = City;
                var Url = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", cityText, ApiKey);

                var httpClient = new HttpClient();
                var response = await httpClient.GetAsync(Url);
                Message = string.Empty;

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var weatherList = JsonConvert.DeserializeObject<RootObject>(content);

                    HighTemp = $"{Celcius(weatherList.main.temp_max)} {CelsiusSign}";
                    LowTemp = $"{Celcius(weatherList.main.temp_min)} {CelsiusSign}";
                    Description = weatherList.weather[0].description;

                    if (weatherList.rain != null) // we have data about rain 
                    { 
                        if (!Xamarin.Forms.Application.Current.Properties.ContainsKey(DateKey)) // add new key and value
                        {
                            Xamarin.Forms.Application.Current.Properties[DateKey] = DateTime.Now.Date.ToUniversalTime();
                            Message = Text; // show note about rain 
                        }
                        else   // value is already exist 
                        {
                            var dateString = Xamarin.Forms.Application.Current.Properties[DateKey].ToString();
                            var dateValue = DateTime.Parse(dateString);

                            if (dateValue != DateTime.Now.Date.ToUniversalTime())  // data has been expired 
                            {
                                Xamarin.Forms.Application.Current.Properties[DateKey] = DateTime.Now.Date; // refresh data 
                                Message = Text; // show note about rain 
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private int Celcius(double calvin)
        {
            return (int)calvin - 273;
        }
    }
}