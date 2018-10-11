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
using Xamarin.Forms.Maps;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FriendFinderPage : ContentPage
	{

        public static pages.GlobalVariables.User SelectedUser { get; set; }
        ObservableCollection<pages.GlobalVariables.User> filteredUserCollection = new ObservableCollection<pages.GlobalVariables.User>();

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

        private async void LoadUsers()
        {

            //TODO: afstand berekenen, observable collection filteren en sorteren.

            var records     = MainPage.users;
            var userHobby   = MainPage.userHobby;

            if (records != null && pages.GlobalVariables.selectedHobbies != null)
            {
                FriendFinderListView.ItemsSource = null;

                var root = JsonConvert.DeserializeObject<pages.GlobalVariables.UserRecords>(records);
                var userHobbyRoot = JsonConvert.DeserializeObject<pages.GlobalVariables.UserHobbyRecords>(userHobby);

                //Convert list to observable collection. This is easier for the grouping and filtering in the listview
                var userCollection          = new ObservableCollection<pages.GlobalVariables.User>(root.records);
                var userHobbies             = new ObservableCollection<pages.GlobalVariables.UserHobby>(userHobbyRoot.records);
                var selectedHobbies         = new ObservableCollection<pages.GlobalVariables.Hobbies>(pages.GlobalVariables.selectedHobbies);
                var selectedUserhobbies     = new ObservableCollection<pages.GlobalVariables.UserHobby>();

                //fill selectedUserHobbies with userHobbies items where selectedHobbies contains userHobbies.hobbyId 
                foreach (var uHobby in userHobbies)
                {
                    if (selectedHobbies.Any(p => p.id == uHobby.hobbyId))
                    {
                        selectedUserhobbies.Add(uHobby);
                    }
                }

                //loop through each user and check if user.id is present in selectedUserHobbies.userId column. add to filtered user collection if it is.
                foreach (var user in userCollection)
                {
                    if (selectedUserhobbies.Any(p => p.userId == user.id))
                    {
                        filteredUserCollection.Add(user);
                    }
                }

                //TODO: calculate distance. and depending on distance in KM filter filterUserCollection further

                string currentUserAddres = "3904 sw";


                if (DistancePicker.SelectedIndex != 0)
                {
                    string selectedDistanceWithoutLetters = System.Text.RegularExpressions.Regex.Replace(DistancePicker.SelectedItem.ToString(), "[^0-9.]", "");

                    double selectedDistance = double.Parse(selectedDistanceWithoutLetters);

                    FilterDistance(filteredUserCollection, currentUserAddres, selectedDistance);

                }
                else
                {
                    FriendFinderListView.ItemsSource = filteredUserCollection;
                }


                

            }
            else
            if (pages.GlobalVariables.selectedHobbies == null)
            {
                await DisplayAlert("","U heeft nog geen hobby's geselecteerd. Ga naar uw profiel instellingen om een hobby te selecteren","Ok");
            }
            
        }

        private void FriendFinderListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).IsEnabled = false;

            SelectedUser = (pages.GlobalVariables.User)FriendFinderListView.SelectedItem;

            OtherProfilePage otherProfilePage = new OtherProfilePage();
            Navigation.PushModalAsync(otherProfilePage);

            ((ListView)sender).SelectedItem = null;

            ((ListView)sender).IsEnabled = true;
        }

        //filter users when distance changed
        private async void DistancePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DistancePicker.SelectedIndex != 0)
            {
                string selectedDistanceWithoutLetters = System.Text.RegularExpressions.Regex.Replace(DistancePicker.SelectedItem.ToString(), "[^0-9.]", "");

                double selectedDistance = double.Parse(selectedDistanceWithoutLetters);

                string currentUserAddres = "3904 SW";

                FilterDistance(filteredUserCollection, currentUserAddres, selectedDistance);
            }
            
        }

        //voodoo code to calculate distance between 2 points

        private double CalculateDistance(double lat1, double lon1, double lat2, double lon2, char unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == 'K')
            {
                dist = dist * 1.609344;
            }
            else if (unit == 'N')
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }


        //function to filter users distance. first argument is the observable collection that needs to be filtered. second argument is users location. Third argument is maximum distance in kilometers

        private async void FilterDistance(ObservableCollection<pages.GlobalVariables.User> localFilteredUserCollection, string currentUserAddres, double maxDistance)
        {
            try
            {

            
            Geocoder geocoder = new Geocoder();
            var currentUserPosition = await geocoder.GetPositionsForAddressAsync(currentUserAddres);

            var currentUserCoordinates = currentUserPosition.ToArray();

            foreach (var user in localFilteredUserCollection)
            {
                var userPosition = await geocoder.GetPositionsForAddressAsync(user.location);
                var userCoordinates = userPosition.ToArray();

                double distance = CalculateDistance(currentUserCoordinates[0].Latitude, currentUserCoordinates[0].Longitude, userCoordinates[0].Latitude, userCoordinates[0].Longitude, 'K');

                if (distance >= maxDistance)
                {
                    localFilteredUserCollection.Remove(user);
                }
            }

            }
            catch (Exception e)
            {
                await DisplayAlert("", e.ToString(), "");
                Console.WriteLine(e.ToString());
            }

            try
            {
                FriendFinderListView.ItemsSource = null;
                FriendFinderListView.ItemsSource = localFilteredUserCollection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

        }

    }
}