using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using DReAM.Views;
using Plugin.Multilingual;
using DReAM.Resx;
using System.Globalization;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace DReAM
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            AppResources.Culture = CrossMultilingual.Current.DeviceCultureInfo;

            MainPage = new MainPage();
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
