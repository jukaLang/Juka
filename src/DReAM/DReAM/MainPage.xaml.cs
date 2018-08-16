using System;
using Xamarin.Forms;

namespace DReAM
{
    public partial class MainPage : ContentPage
	{
        ITextToSpeech speech = DependencyService.Get<ITextToSpeech>();

        public MainPage()
		{
			InitializeComponent();
            if (speech != null)
            {
                speech.Speak("Welcome to Juliar DReAM Programming Language");
            }
        }

        private async void button_Clicked(object sender, EventArgs e)
        {
            if (sender is Button)
            {
                Button clickedbutton = (Button)sender;
                if (speech != null)
                {
                    speech.Speak(clickedbutton.Text);
                }

                if (clickedbutton.Equals(button_new))
                {
                    await App.Current.MainPage.Navigation.PushAsync(new View.Editor() { Title = "Juliar DReAM Editor" });
                }
                else if (clickedbutton.Equals(button_open))
                {
                    await DisplayAlert("Alert", "Coming Soon", "OK");
                }
                else if (clickedbutton.Equals(button_interpret))
                {
                    await DisplayAlert("Alert", "Coming Soon", "OK");
                }
                else if (clickedbutton.Equals(button_chat))
                {
                    await DisplayAlert("Alert", "Coming Soon", "OK");
                }
                else if (clickedbutton.Equals(button_docs))
                {
                    Device.OpenUri(new Uri("https://juliar.org/documentation"));
                }
                else if (clickedbutton.Equals(button_share))
                {


                    /*var url = string.Empty;
                    var appId = string.Empty;


                    switch (Device.RuntimePlatform)
                    {
                        case Device.iOS:
                            appId = "your_id";
                            url = $"itms-apps://itunes.apple.com/WebObjects/MZStore.woa/wa/viewContentsUserReviews?id={appId}&amp;onlyLatestVersion=true&amp;pageNumber=0&amp;sortOrdering=1&amp;type=Purple+Software";
                            break;
                        case Device.Android:
                            appId = "your_id";
                            url = $"https://play.google.com/store/apps/details?id={appId}";
                            break;
                        case Device.UWP:
                        default:
                            break;
                    }
                    Device.OpenUri(new Uri(appId));*/
                }
                else if (clickedbutton.Equals(button_contact))
                {
                    Device.OpenUri(new Uri("mailto:admin@juliar.org?subject=Questions%20about%20DReAM"));
                }
                else if (clickedbutton.Equals(button_commit))
                {
                    Device.OpenUri(new Uri("https://juliar.org/help"));
                }
                else if (clickedbutton.Equals(button_settings))
                {
                    await App.Current.MainPage.Navigation.PushAsync(new View.Settings() { Title = "Juliar DReAM Settings" });
                }

            }
        }
	}
}
/*
<Button x:Name="button_new" Text="New" Grid.Row="1" Grid.Column="0" BackgroundColor="Gray" Clicked="button_Clicked" />
        <Button x:Name="button_open" Text="Open" Grid.Row="1" Grid.Column="1" BackgroundColor="Gray" Clicked="button_Clicked" />
        <Button x:Name="button_sync" Text="Sync" Grid.Row="1" Grid.Column="2" BackgroundColor="Gray" Clicked="button_Clicked" />
        
        <Button x:Name="button_interpret" Text="Interpret" Grid.Row="2" Grid.Column="0" BackgroundColor="Gray" Clicked="button_Clicked" />
        <Button x:Name="button_docs" Text="Docs" Grid.Row="2" Grid.Column="1" BackgroundColor="Gray" Clicked="button_Clicked" />
        <Button x:Name="button_rate" Text="Rate Us" Grid.Row="2" Grid.Column="2" BackgroundColor="Gray" Clicked="button_Clicked" />


        <Button x:Name="button_contact" Text="Contact" Grid.Row="3" Grid.Column="0" BackgroundColor="Gray" Clicked="button_Clicked" />
        <Button x:Name="button_commit" Text="Help out" Grid.Row="3" Grid.Column="1" BackgroundColor="Gray" Clicked="button_Clicked" />
        <Button x:Name="button_about" Text="About" Grid.Row="3" Grid.Column="2" BackgroundColor="Gray" Clicked="button_Clicked" />
        */
