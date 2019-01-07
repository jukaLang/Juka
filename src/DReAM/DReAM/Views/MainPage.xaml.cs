using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Dream.Resx;

namespace Dream.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        ITextToSpeech speech = DependencyService.Get<ITextToSpeech>();

        public MainPage()
        {
            InitializeComponent();

            string welcome = AppResources.welcome;
            if (speech != null && welcome != null)
            {
                speech.Speak(welcome);
            }
            else
            {
                Debug.WriteLine("welcome is null");
            }
        }
    }
}