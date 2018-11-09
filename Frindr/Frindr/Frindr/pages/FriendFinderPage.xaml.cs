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

        public static GlobalVariables.WrapUser SelectedUser { get; set; }
        ObservableCollection<GlobalVariables.User> filteredUserCollection = new ObservableCollection<GlobalVariables.User>();

        ObservableCollection<GlobalVariables.WrapUser> wrappedFilteredUserCollection = new ObservableCollection<GlobalVariables.WrapUser>();

        ImageSource defaultImage;
        static string userHobby = GlobalVariables.GetUserHobbies();
        static GlobalVariables.UserHobbyRecords userHobbyRoot = JsonConvert.DeserializeObject<GlobalVariables.UserHobbyRecords>(userHobby);
        ObservableCollection<GlobalVariables.UserHobby> userHobbies = new ObservableCollection<GlobalVariables.UserHobby>(userHobbyRoot.records);

        static string getHobbies = GlobalVariables.GetHobbies();
        static GlobalVariables.HobbyRecords hobbyRoot = JsonConvert.DeserializeObject<GlobalVariables.HobbyRecords>(getHobbies);
        ObservableCollection<GlobalVariables.Hobbies> allHobbies = new ObservableCollection<GlobalVariables.Hobbies>(hobbyRoot.records);

        ObservableCollection<GlobalVariables.Hobbies> selectedHobbies = new ObservableCollection<GlobalVariables.Hobbies>();

        //this is not a test

        public FriendFinderPage()
        {
            InitializeComponent();

            //get default image
            selectedHobbies = GetSelectedHobbies();

            RestfulClass restfulClass = new RestfulClass();
            defaultImage = restfulClass.GetImage("Default.png");

            LoadUsers();
            SetRangeSliderTextFormat();
        }

        private void ShowFilterButton_Clicked(object sender, EventArgs e)
        {
            if (ExtraFilterStackLayout.IsVisible)
            {
                ExtraFilterStackLayout.IsVisible = false;

            }
            else
            {
                ExtraFilterStackLayout.IsVisible = true;

            }
        }

        private ObservableCollection<GlobalVariables.Hobbies> GetSelectedHobbies()
        {
            var sHobbies = new ObservableCollection<GlobalVariables.Hobbies>();

            var selectedUserHobbies = userHobbies.Where(p => p.userId == GlobalVariables.loginUser.id);
            foreach (var hobby in allHobbies)
            {
                if (selectedUserHobbies.Any(p => p.hobbyId == hobby.id))
                    sHobbies.Add(hobby);
                
            }

            return sHobbies;
        }

        private async void LoadUsers()
        {
            //TODO: observable collection filteren en sorteren.

            DistanceRangeSlider.IsEnabled = false;
            AgeRangeSlider.IsEnabled = false;

            var records = GlobalVariables.GetUsers();
            
            var json = JsonConvert.DeserializeObject<GlobalVariables.HobbyRecords>(getHobbies);


            if (records != null && json.records != null)
            {

                wrappedFilteredUserCollection.Clear();

                ObservableCollection<GlobalVariables.WrapUser> selectedWrappedFilteredUserCollection = new ObservableCollection<GlobalVariables.WrapUser>();
                FriendFinderListView.ItemsSource = selectedWrappedFilteredUserCollection;

                var root = JsonConvert.DeserializeObject<GlobalVariables.UserRecords>(records);
                
                //Convert list to observable collection. This is easier for the grouping and filtering in the listview
                var wrappedUserCollection = new ObservableCollection<GlobalVariables.WrapUser>();
                var userCollection = new ObservableCollection<GlobalVariables.User>(root.records);
                var selectedUserhobbies = new ObservableCollection<GlobalVariables.UserHobby>();
               
                //wrap user collection
                //check of userCollection is filled with different users
                
                foreach (var user in userCollection)
                {
                    GlobalVariables.WrapUser wrapUser = new GlobalVariables.WrapUser();
                    wrapUser.User = user;
                    wrappedUserCollection.Add(wrapUser);
                }

                //fill selectedUserHobbies with userHobbies items where selectedHobbies contains userHobbies.hobbyId
                foreach (var uHobby in userHobbies)
                {
                    if (selectedHobbies.Any(p => p.id == uHobby.hobbyId))
                    {
                        selectedUserhobbies.Add(uHobby);
                    }
                }

                //loop through each user and check if user.id is present in selectedUserHobbies.userId column. add to filtered user collection if it is.
                RestfulClass restfulClass = new RestfulClass();

                foreach (var wrappedUser in wrappedUserCollection)
                {
                    try
                    {
                        if (selectedUserhobbies.Any(p => p.userId == wrappedUser.User.id) && wrappedUser.User.userVisible == 0)
                        {                            //var test = restfulClass.GetImage(GlobalVariables.loginUser.imagePath);

                            if(wrappedUser.User.imagePath == "iets" || wrappedUser.User.imagePath == "Default.png")
                            {
                                wrappedUser.imageSource = defaultImage;
                            }
                            else
                            {
                                wrappedUser.imageSource = restfulClass.GetImage(wrappedUser.User.imagePath);
                            }

                            int hobbysAdded = 0;
                            foreach (var hobby in selectedUserhobbies.Where(p => p.userId == wrappedUser.User.id))
                            {
                                foreach(var selectedHobby in selectedHobbies.Where(p => p.id == hobby.hobbyId))
                                {
                                    if (hobbysAdded >= 2) break;

                                    wrappedUser.hobbyList += selectedHobby.hobby + ", ";
                                    hobbysAdded++;

                                    
                                }
                            }

                            wrappedUser.hobbyList = wrappedUser.hobbyList.Remove(wrappedUser.hobbyList.Length - 2);

                            wrappedFilteredUserCollection.Add(wrappedUser);

                        }

                    } catch (Exception e)
                    {
                        Console.WriteLine("exception: " + e);
                    }
                }

                //gets, sets and filters based on distance
                string currentUserAddres = GlobalVariables.loginUser.location;

                double selectedDistance = DistanceRangeSlider.UpperValue;

                wrappedFilteredUserCollection = await FilterDistance(wrappedFilteredUserCollection, currentUserAddres, selectedDistance);


                foreach (var user in userCollection)
                {
                    if (user.locationVisible == 1)
                    {
                        user.location = "";
                    }
                }

                wrappedFilteredUserCollection = await FilterAge(wrappedFilteredUserCollection);

                selectedWrappedFilteredUserCollection = wrappedFilteredUserCollection;

                FriendFinderListView.ItemsSource = selectedWrappedFilteredUserCollection;

            }

            DistanceRangeSlider.IsEnabled = true;
            AgeRangeSlider.IsEnabled = true;

        }

        //function to filter users age. age format: yyyyMMdd
        private async Task<ObservableCollection<pages.GlobalVariables.WrapUser>> FilterAge(ObservableCollection<pages.GlobalVariables.WrapUser> localFilteredUserCollection)
        {

            var ageFilteredUserCollection = new ObservableCollection<pages.GlobalVariables.WrapUser>();

            foreach (var user in localFilteredUserCollection)

            {

                int userAge;

                if (user.age == 0)
                {
                    DateTime userBirthday = DateTime.Parse(user.User.birthday);
                    int userBirthdayString = int.Parse(userBirthday.ToString("yyyyMMdd"));
                    userAge = CheckAge(userBirthdayString);

                    user.age = userAge;
                }
                else
                {
                    userAge = user.age;
                }
                
                if (userAge >= AgeRangeSlider.LowerValue && userAge <= AgeRangeSlider.UpperValue || userAge >= AgeRangeSlider.LowerValue && AgeRangeSlider.UpperValue == AgeRangeSlider.MaximumValue)
                {
                    ageFilteredUserCollection.Add(user);
                }

                SelectedUser = (GlobalVariables.WrapUser)FriendFinderListView.SelectedItem;

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

        private async Task<ObservableCollection<pages.GlobalVariables.WrapUser>> FilterDistance(ObservableCollection<pages.GlobalVariables.WrapUser> localFilteredUserCollection, string currentUserAddres, double maxDistance)
        {

            var distanceFilteredUserCollection = new ObservableCollection<pages.GlobalVariables.WrapUser>();

            Geocoder geocoder = new Geocoder();
            var currentUserPosition = await geocoder.GetPositionsForAddressAsync(currentUserAddres);

            var currentUserCoordinates = currentUserPosition.ToArray();

            foreach (var user in localFilteredUserCollection)
            {
                if (user.distance == 0)
                {
                    try
                    {
                    string userLocation = user.User.location;

                    var userPosition = await geocoder.GetPositionsForAddressAsync(userLocation);
                    var userCoordinates = userPosition.ToArray();

                    double distance = CalculateDistance(currentUserCoordinates[0].Latitude,
                        currentUserCoordinates[0].Longitude, userCoordinates[0].Latitude, userCoordinates[0].Longitude, 'K');

                    user.distance = (int)Math.Round(distance);

                    int d = user.distance;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception: " + e);
                    }

                }
                
                

                if (user.distance <= maxDistance)
                {
                    //user.distance = distance.ToString();
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

        //prefent you from going back to the register page
        protected override bool OnBackButtonPressed()
        {

            return true;
            //return base.OnBackButtonPressed();
        }

        private void FriendFinderListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).IsEnabled = false;

            SelectedUser = (pages.GlobalVariables.WrapUser)FriendFinderListView.SelectedItem;

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