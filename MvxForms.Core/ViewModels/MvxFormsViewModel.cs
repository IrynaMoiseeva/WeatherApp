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
        const string text = "Today rain is expected";
        public MvxFormsViewModel()
        {
        }

        public override Task Initialize()
        {
            //TODO: Add starting logic here

            return base.Initialize();
        }

        public IMvxCommand GetWeatherCommand => new MvxCommand(GetWeather);

        private string city;
        public string City
        {
            get { return city; }
            set { SetProperty(ref city, value); }
        }
        private string highTemp;
        public string HighTemp
        {
            get { return highTemp; }
            set { SetProperty(ref highTemp, value); }
        }

        private string lowTemp;
        public string LowTemp
        {
            get { return lowTemp; }
            set { SetProperty(ref lowTemp, value); }
        }

        private string description;
        public string Description
        {
            get { return description; }
            set { SetProperty(ref description, value); }
        }

        private string messege;
        public string Messege
        {
            get { return messege; }
            set { SetProperty(ref messege, value); }
        }

        private int Celcius(double calvin)
        {
            return (int)calvin - 273;

        }

        private async void GetWeather()
        {

            try
            {
                var api_key = "03e8168ffbb8cafb6b8b6679c528ec97";

                var citytext = City;
                Messege = "";

                string URL = string.Format("https://api.openweathermap.org/data/2.5/weather?q={0}&appid={1}", citytext, api_key);

                HttpClient httpClient = new HttpClient();
                var response = await httpClient.GetAsync(URL);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var weatherList = JsonConvert.DeserializeObject<RootObject>(content);

                    HighTemp = Celcius(weatherList.main.temp_max).ToString() + "°C";
                    LowTemp = Celcius(weatherList.main.temp_min).ToString() + "°C";
                    Description = weatherList.weather[0].description;
                    //Xamarin.Forms.Application.Current.Properties.Remove("Date");
                    if (weatherList.rain!=null) // we have data about rain 
                    { 
                        if (!Xamarin.Forms.Application.Current.Properties.ContainsKey("Date")) // add new key and value
                        {
                            Xamarin.Forms.Application.Current.Properties["Date"] = DateTime.Now.Date;
                            Messege = text; // show note about reain 
                        }
                        else   // value is already exist 
                        {
                            var dateString = Xamarin.Forms.Application.Current.Properties["Date"];
                            var dateValue = DateTime.Parse(dateString.ToString());

                            if (dateValue != DateTime.Now.Date)  // data has been expired 
                            {
                                Xamarin.Forms.Application.Current.Properties["Date"] = DateTime.Now.Date; // refresh data 
                                Messege = text; // show note about reain 
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

    }

}

