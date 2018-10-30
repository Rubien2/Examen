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
using Frindr.pages;

namespace Frindr
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendFinderPage : ContentPage
    {

        public static GlobalVariables.User SelectedUser { get; set; }
        ObservableCollection<GlobalVariables.User> filteredUserCollection = new ObservableCollection<GlobalVariables.User>();

        public FriendFinderPage()
        {
            InitializeComponent();

            ObservableCollection<string> AgeList = new ObservableCollection<string> { "Elke leeftijd", "18-21 Jaar", "22-25 Jaar", "26-30 Jaar", "31-36 Jaar", "37-45 Jaar", "46-50 Jaar", "50+" };
            AgePicker.ItemsSource = AgeList;
            AgePicker.SelectedIndex = 0;

            ObservableCollection<string> DistanceList = new ObservableCollection<string> { "Alle afstanden", "< 2 KM", "< 5 KM", "< 10 KM", "< 15 KM", "< 25 KM", "< 50 KM", "< 75 KM", "< 100 KM" };
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

            var records = GlobalVariables.GetUsers();
            var userHobby = GlobalVariables.GetUserHobbies();

            if (records != null && GlobalVariables.selectedHobbies != null)
            {
                FriendFinderListView.ItemsSource = null;

                var root = JsonConvert.DeserializeObject<GlobalVariables.UserRecords>(records);
                var userHobbyRoot = JsonConvert.DeserializeObject<GlobalVariables.UserHobbyRecords>(userHobby);

                //Convert list to observable collection. This is easier for the grouping and filtering in the listview
                var userCollection = new ObservableCollection<GlobalVariables.User>(root.records);
                var userHobbies = new ObservableCollection<GlobalVariables.UserHobby>(userHobbyRoot.records);
                var selectedHobbies = new ObservableCollection<GlobalVariables.Hobbies>(GlobalVariables.selectedHobbies);
                var selectedUserhobbies = new ObservableCollection<GlobalVariables.UserHobby>();

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
                    if (selectedUserhobbies.Any(p => p.userId == user.id) && user.userVisible == 0)
                    {
                        filteredUserCollection.Add(user);
                    }
                }

                //huidig adres gebruiker
                //gooit null reference
                string currentUserAddres = GlobalVariables.loginUser.location;


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

                foreach (var user in userCollection)
                {
                    if(user.locationVisible == 1)
                    {
                        user.location = "";
                    }
                }
            }
            else
            if (GlobalVariables.selectedHobbies == null)
            {
                await DisplayAlert("", "U heeft nog geen hobby's geselecteerd. Ga naar uw profiel instellingen om een hobby te selecteren", "Ok");
            }

        }

        private void FriendFinderListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).IsEnabled = false;

            SelectedUser = (GlobalVariables.User)FriendFinderListView.SelectedItem;

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
            else
            {
                FriendFinderListView.ItemsSource = filteredUserCollection;
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

        private async void FilterDistance(ObservableCollection<GlobalVariables.User> localFilteredUserCollection, string currentUserAddres, double maxDistance)
        {

            var distanceFilteredUserCollection = new ObservableCollection<GlobalVariables.User>();

            Geocoder geocoder = new Geocoder();
            var currentUserPosition = await geocoder.GetPositionsForAddressAsync(currentUserAddres);

            var currentUserCoordinates = currentUserPosition.ToArray();

            foreach (var user in localFilteredUserCollection)
            {
                var userPosition = await geocoder.GetPositionsForAddressAsync(user.location);
                var userCoordinates = userPosition.ToArray();

                double distance = CalculateDistance(currentUserCoordinates[0].Latitude, currentUserCoordinates[0].Longitude, userCoordinates[0].Latitude, userCoordinates[0].Longitude, 'K');

                if (distance <= maxDistance)
                {
                    distanceFilteredUserCollection.Add(user);
                }
            }

            FriendFinderListView.ItemsSource = distanceFilteredUserCollection;

        }

        //prefent you from going back to the register page
        protected override bool OnBackButtonPressed()
        {

            return true;

            //return base.OnBackButtonPressed();
        }

    }
}