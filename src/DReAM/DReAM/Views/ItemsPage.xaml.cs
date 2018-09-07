using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using DReAM.Models;
using DReAM.Views;
using DReAM.ViewModels;

namespace DReAM.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemsPage : ContentPage
    {
        ItemsViewModel viewModel;
        ITextToSpeech speech = DependencyService.Get<ITextToSpeech>();

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
        }

        async void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            var item = args.SelectedItem as Item;
            if (item == null)
                return;

            if (speech != null)
            {
                speech.Speak(item.Text);
            }

            if (item.Action == "New")
            {
                await Navigation.PushAsync(new View.Editor());
            }
            else if(item.Action == "Open")
            {
                await DisplayAlert("Alert", "Coming Soon", "OK");
            }
            else if (item.Action == "Interpret")
            {
                await DisplayAlert("Alert", "Coming Soon", "OK");
            }
            else if (item.Action == "Chat")
            {
                await DisplayAlert("Alert", "Coming Soon", "OK");
            }
            else if (item.Action == "Docs")
            {
                Device.OpenUri(new Uri("https://juliar.org/documentation"));
            }
            else if (item.Action == "Share")
            {
                await DisplayAlert("Alert", "Coming Soon", "OK");
                await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));
            }
            else if (item.Action == "Contact")
            {
                Device.OpenUri(new Uri("mailto:admin@juliar.org?subject=Questions%20about%20DReAM"));
            }
            else if (item.Action == "Help")
            {
                Device.OpenUri(new Uri("https://juliar.org/help"));
            }
            else if (item.Action == "Settings")
            {
                await Navigation.PushAsync(new View.Settings());
            }

           

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            
            await Navigation.PushModalAsync(new NavigationPage(new NewItemPage()));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }
    }
}