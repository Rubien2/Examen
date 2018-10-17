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

            ObservableCollection<string> AgeList = new ObservableCollection<string> { "Elke leeftijd", "18-21 Jaar", "22-25 Jaar", "26-30 Jaar", "31-36 Jaar", "37-45 Jaar", "46-50 Jaar", "50+" };
            AgePicker.ItemsSource = AgeList;
            AgePicker.SelectedIndex = 0;

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

            //TODO: observable collection filteren en sorteren.

            var records = MainPage.Users;
            var userHobby = MainPage.UserHobby;

            if (records != null && pages.GlobalVariables.selectedHobbies != null)
            {
                FriendFinderListView.ItemsSource = null;
                filteredUserCollection.Clear();

                var root = JsonConvert.DeserializeObject<pages.GlobalVariables.UserRecords>(records);
                var userHobbyRoot = JsonConvert.DeserializeObject<pages.GlobalVariables.UserHobbyRecords>(userHobby);

                //Convert list to observable collection. This is easier for the grouping and filtering in the listview
                var userCollection = new ObservableCollection<pages.GlobalVariables.User>(root.records);
                var userHobbies = new ObservableCollection<pages.GlobalVariables.UserHobby>(userHobbyRoot.records);
                var selectedHobbies = new ObservableCollection<pages.GlobalVariables.Hobbies>(pages.GlobalVariables.selectedHobbies);
                var selectedUserhobbies = new ObservableCollection<pages.GlobalVariables.UserHobby>();

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

                //Filter users on distance
                if(DistanceSlider.Value != 0)
                {
                    string currentUserAddres = "3904 SW";

                    double selectedDistance = DistanceSlider.Value;

                    filteredUserCollection = await FilterDistance(filteredUserCollection, currentUserAddres, selectedDistance);
                }

                //Filter users on age
                if (AgePicker.SelectedIndex != 0)
                {
                    string currentuserage = 1999.ToString();
                }

                FriendFinderListView.ItemsSource = filteredUserCollection;

            }
            else
            if (pages.GlobalVariables.selectedHobbies == null)
            {
                await DisplayAlert("", "U heeft nog geen hobby's geselecteerd. Ga naar uw profiel instellingen om een hobby te selecteren", "Ok");
            }

        }

        //function to filter users age. age format: yyyyMMdd
        private Task<ObservableCollection<pages.GlobalVariables.User>> FilterAge(ObservableCollection<pages.GlobalVariables.User> localFilteredUserCollection)
        {
            int birthday = 19990601;
            int age = CheckAge(birthday);

            foreach (var user in localFilteredUserCollection)
            {
                DateTime userBirthday =  DateTime.Parse(user.birthday);
                int userBirthdayString = int.Parse(userBirthday.ToString("yyyyMMdd"));
                int userAge = CheckAge(birthday);

                int ageDifference = (age - birthday) / 10000;

            }

            return null;

        }

        //age format = yyyyMMdd
        private int CheckAge(int birthday)
        {
            var today = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
            var age = (today - birthday) / 10000;

            return age;
        }

        //function to filter users distance. first argument is the observable collection that needs to be filtered. second argument is users location. Third argument is maximum distance in kilometers

        private async Task<ObservableCollection<pages.GlobalVariables.User>> FilterDistance(ObservableCollection<pages.GlobalVariables.User> localFilteredUserCollection, string currentUserAddres, double maxDistance)
        {

            var distanceFilteredUserCollection = new ObservableCollection<pages.GlobalVariables.User>();

            Geocoder geocoder = new Geocoder();
            var currentUserPosition = await geocoder.GetPositionsForAddressAsync(currentUserAddres);

            var currentUserCoordinates = currentUserPosition.ToArray();

            foreach (var user in localFilteredUserCollection)
            {
                var userPosition = await geocoder.GetPositionsForAddressAsync(user.location);
                var userCoordinates = userPosition.ToArray();

                double distance = CalculateDistance(currentUserCoordinates[0].Latitude,
                    currentUserCoordinates[0].Longitude, userCoordinates[0].Latitude, userCoordinates[0].Longitude, 'K');

                if (distance <= maxDistance)
                {
                    distanceFilteredUserCollection.Add(user);
                }
            }

            //filteredUserCollection = distanceFilteredUserCollection;
            return distanceFilteredUserCollection;

        }

        

        //voodoo code to calculate distance between 2 points from geodatasource.com

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

        //prefent you from going back to the register page
        protected override bool OnBackButtonPressed()
        {

            return true;

            //return base.OnBackButtonPressed();
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

        //filter users when selected age changed
        private void AgePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadUsers();
        }

        bool updateUsers = true;

        private async void DistanceSlider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (DistanceSlider.Value == 0) lblSelectedDistance.Text = "Afstand: elke";
            else lblSelectedDistance.Text = "Afstand: " + Math.Round(DistanceSlider.Value) + " KM";

            if (updateUsers == true)
            {
                updateUsers = false;
                await Task.Delay(1000);
                LoadUsers();
                updateUsers = true;
            }            
        }

        private void DistanceSlider_Unfocused(object sender, FocusEventArgs e)
        {
            LoadUsers();
        }
    }
}