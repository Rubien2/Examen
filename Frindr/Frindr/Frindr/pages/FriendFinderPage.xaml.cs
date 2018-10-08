using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Reflection;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FriendFinderPage : ContentPage
	{
        public static pages.GlobalVariables.User SelectedUser { get; set; }

        public FriendFinderPage()
        {
            InitializeComponent();

            ObservableCollection<string> AgeList = new ObservableCollection<string> {"Elke leeftijd", "18-21 Jaar", "22-25 Jaar", "26-30 Jaar", "31-36 Jaar", "37-45 Jaar", "46-50 Jaar", "50+" };
            AgePicker.ItemsSource = AgeList;
            AgePicker.SelectedIndex = 0;

            ObservableCollection<string> DistanceList = new ObservableCollection<string> { "Alle afstanden", "< 2 KM", "< 5 KM", "< 10 KM", "< 15 KM", "< 25 KM", "< 50 KM", "< 75 KM", "< 100 KM"};
            DistancePicker.ItemsSource = DistanceList;
            DistancePicker.SelectedIndex = 0;

            LoadUsers();
        }

        private void ShowFilterButton_Clicked(object sender, EventArgs e)
        {
            if (ExtraFilterStackLayout.IsVisible)
            {
                ExtraFilterStackLayout.IsVisible = false;
                ShowFilterButton.Text = "Show";
            }
            else
            {
                ExtraFilterStackLayout.IsVisible = true;
                ShowFilterButton.Text = "Hide";
            }
        }

        private void LoadUsers()
        {

            //TODO: afstand berekenen, observable collection filteren en sorteren.

            var records = MainPage.users;

            if (records != null)
            {
                FriendFinderListView.ItemsSource = null;

                var root = JsonConvert.DeserializeObject<pages.GlobalVariables.UserRecords>(records);
                //Convert list to observable collection. This is easier for the grouping in the listview
                ObservableCollection<pages.GlobalVariables.User> userCollection = new ObservableCollection<pages.GlobalVariables.User>(root.records);
                FriendFinderListView.ItemsSource = userCollection;
            }
        }

        private void FriendFinderListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).IsEnabled = false;

            SelectedUser = (pages.GlobalVariables.User)FriendFinderListView.SelectedItem;

            OtherProfilePage otherProfilePage = new OtherProfilePage();
            Navigation.PushModalAsync(otherProfilePage);

            ((ListView)sender).SelectedItem = null;

        }
    }
}