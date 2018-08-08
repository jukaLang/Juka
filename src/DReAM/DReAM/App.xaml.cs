using DReAM.View;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace DReAM
{
	public partial class App : Application
	{
		public App ()
		{
			InitializeComponent();

            //MainPage = new NavigationPage(new WeatherPage());
            MainPage = new SpeakerView();
        }

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
