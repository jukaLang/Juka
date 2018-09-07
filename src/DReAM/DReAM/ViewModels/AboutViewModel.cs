using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace DReAM.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About Juliar DReAM";

            OpenWebCommand = new Command(() => Device.OpenUri(new Uri("https://juliar.org")));
        }

        public ICommand OpenWebCommand { get; }
    }
}