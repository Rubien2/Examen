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

            LoadUsers();
            SetRangeSliderTextFormat();

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

            var records = GlobalVariables.GetUsers();
            var userHobby = GlobalVariables.GetUserHobbies();

            if (records != null && GlobalVariables.selectedHobbies != null)
            {
                //FriendFinderListView.ItemsSource = null;
                filteredUserCollection.Clear();

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


                //Filter users on distance
                if(DistanceRangeSlider.UpperValue != DistanceRangeSlider.MaximumValue)
                {
                    //huidig adres gebruiker
                    //gooit null reference
                    string currentUserAddres = GlobalVariables.loginUser.location;

                    double selectedDistance = DistanceRangeSlider.UpperValue;

                    filteredUserCollection = await FilterDistance(filteredUserCollection, currentUserAddres, selectedDistance);
                }


                filteredUserCollection = await FilterAge(filteredUserCollection);
                
                FriendFinderListView.ItemsSource = filteredUserCollection;

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

            }
        }

        //function to filter users age. age format: yyyyMMdd
        private async Task<ObservableCollection<pages.GlobalVariables.User>> FilterAge(ObservableCollection<pages.GlobalVariables.User> localFilteredUserCollection)
        {

            var ageFilteredUserCollection = new ObservableCollection<pages.GlobalVariables.User>();

            foreach (var user in localFilteredUserCollection)

            {
                DateTime userBirthday =  DateTime.Parse(user.birthday);
                int userBirthdayString = int.Parse(userBirthday.ToString("yyyyMMdd"));
                int userAge = CheckAge(userBirthdayString);


                if(userAge >= AgeRangeSlider.LowerValue && userAge <= AgeRangeSlider.UpperValue || userAge >= AgeRangeSlider.LowerValue && AgeRangeSlider.UpperValue == AgeRangeSlider.MaximumValue)
                {
                    ageFilteredUserCollection.Add(user);
                }

            SelectedUser = (GlobalVariables.User)FriendFinderListView.SelectedItem;

            }


            return ageFilteredUserCollection;

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
                    user.distance = distance.ToString();
                    distanceFilteredUserCollection.Add(user);
                }
            }

            //filteredUserCollection = distanceFilteredUserCollection;
            return distanceFilteredUserCollection;

        }

        private void SetRangeSliderTextFormat()
        {
            
            DistanceRangeSlider.TextFormat = "Onbeperkt";

            AgeRangeSlider.TextFormat = "0 Jaar";

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



        //function to filter users distance. first argument is the observable collection that needs to be filtered. second argument is users location. Third argument is maximum distance in kilometers

        private async void FilterDistance(ObservableCollection<GlobalVariables.User> localFilteredUserCollection, string currentUserAddres, double maxDistance)
        {

            var distanceFilteredUserCollection = new ObservableCollection<GlobalVariables.User>();

            Geocoder geocoder = new Geocoder();
            var currentUserPosition = await geocoder.GetPositionsForAddressAsync(currentUserAddres);
        
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
        private void DistanceRangeSlider_DragCompleted(object sender, ValueChangedEventArgs e)
        {
            LoadUsers();
        }

        private void AgeRangeSlider_DragCompleted(object sender, FocusEventArgs e)
        {
            LoadUsers();
        }

        private void DistanceRangeSlider_UpperValueChanged(object sender, EventArgs e)
        {
            if (DistanceRangeSlider.UpperValue == DistanceRangeSlider.MaximumValue)
            {
                DistanceRangeSlider.TextFormat = "Onbeperkt";
            }
            else
            {
                DistanceRangeSlider.TextFormat = "0 KM";
            }
        }
    }
}