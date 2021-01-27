using CompleteWeatherApp.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CompleteWeatherApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new CurrentWeatherPage()) { Title = "test", BarBackgroundColor = Color.FromHex("#B0C4DE") };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
