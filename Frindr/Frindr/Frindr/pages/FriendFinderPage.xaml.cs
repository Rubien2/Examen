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

            var records     = MainPage.Users;
            var userHobby   = MainPage.UserHobby;

            if (records != null && pages.GlobalVariables.selectedHobbies != null)
            {
                FriendFinderListView.ItemsSource = null;

                var root = JsonConvert.DeserializeObject<pages.GlobalVariables.UserRecords>(records);
                var userHobbyRoot = JsonConvert.DeserializeObject<pages.GlobalVariables.UserHobbyRecords>(userHobby);


                //Convert list to observable collection. This is easier for the grouping and filtering in the listview
                var userCollection = new ObservableCollection<pages.GlobalVariables.User>(root.records);
                var userHobbies = new ObservableCollection<pages.GlobalVariables.UserHobby>(userHobbyRoot.records);
                var filteredUserCollection = new ObservableCollection<pages.GlobalVariables.User>();
                //var filteredUserHobbies     = new ObservableCollection<pages.GlobalVariables.UserHobby>();
                List<int> filteredUserHobbiesUserIdList = null;
                /*
                try { 
                foreach (var h in pages.GlobalVariables.selectedHobbies)
                {
                    if(h != null)
                    {
                            //filteredUserHobbies = new ObservableCollection<pages.GlobalVariables.UserHobby>(userHobbies.Where(p => p.hobbyId == h.id));
                        if(userHobbies.Any(p => p.hobbyId == h.id))
                            {
                                foreach (var uh in userHobbies.Where(p => p.hobbyId == h.id))
                                {
                                    if (uh.userId != null)
                                    {
                                        filteredUserHobbiesUserIdList.Add(uh.userId);
                                    }
                                }
                            }
                    }
                }
                }
                catch (NullReferenceException e)
                {
                    await DisplayAlert("derp", e.ToString(), "ok");
                }
                */
                FriendFinderListView.ItemsSource = userCollection;

                //TODO: dynamisch locatie ophalen van ingelogde gebruiker

                string currentUserAddres = "3904SW";
                //var currentUserPosition = await GetCoordinatesOfAddresAsync(currentUserAddres);

                /*
                
                    string userLocation = i.location;

                    var userPosition = await GetCoordinatesOfAddresAsync(i.location);

                    var distance = Distance(currentUserPosition[0], currentUserPosition[1], userPosition[0], userPosition[1], 'K');
                } 
                */
                //userCollection.Select<>;

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

        private async Task<double[]> GetCoordinatesOfAddresAsync(string addres)
        {

            Geocoder gc = new Geocoder();

            IEnumerable<Position> result = await gc.GetPositionsForAddressAsync(addres);
            
            double[] userCoordinates = null;

            foreach (Position pos in result)
            {

                System.Diagnostics.Debug.WriteLine("Lat: {0}, Lng: {1}", pos.Latitude, pos.Longitude);

                userCoordinates[0] = pos.Latitude;
                userCoordinates[1] = pos.Longitude;
                
            }

            return userCoordinates;

        }

        //Voodoo code to measure distance from geodatasource.com

        private double Distance(double lat1, double lon1, double lat2, double lon2, char unit)
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

    }
}