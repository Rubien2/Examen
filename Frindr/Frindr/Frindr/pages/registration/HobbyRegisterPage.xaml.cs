using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HobbyRegisterPage : ContentPage
	{

        public static string hobbies { get; set; }

        public HobbyRegisterPage ()
		{
			InitializeComponent ();

            GetHobbies();

            FillListView();
		}

        private void CreateAccountButton_Clicked(object sender, EventArgs e)
        {
            MenuPage menuPage = new MenuPage();
            Navigation.PushModalAsync(menuPage);
        }

        private ObservableCollection<pages.GlobalVariables.Category> grouped { get; set; }

        void FillListView()
        {
            var records = hobbies;


            if (records != null)
            {
                MyListView.ItemsSource = null;

                var root = JsonConvert.DeserializeObject<pages.GlobalVariables.HobbyRecords>(records);
                //Convert list to observable collection. This is easier for the grouping in the listview
                ObservableCollection<pages.GlobalVariables.Hobbies> hobbiesCollection = new ObservableCollection<pages.GlobalVariables.Hobbies>(root.records);

                grouped = new ObservableCollection<pages.GlobalVariables.Category>();

                //Define Hobby categories
                var VideoGamesGroup = new pages.GlobalVariables.Category() { id = 1, name = "Video games" };
                var SportGroup = new pages.GlobalVariables.Category() { id = 2, name = "Sporten" };
                var HobbyAndFreeTimeGroup = new pages.GlobalVariables.Category() { id = 3, name = "Hobby en vrije tijd" };
                var DoItYourselfGroup = new pages.GlobalVariables.Category() { id = 4, name = "Doe-het-zelf" };
                var TechnologieGroup = new pages.GlobalVariables.Category() { id = 5, name = "Technologie" };
                var OtherGroup = new pages.GlobalVariables.Category() { id = 0, name = "Overig" };

                //add hobbies to groups
                foreach (var hobby in hobbiesCollection)
                {
                    int caseSwitch = hobby.hobbyCategoryId;
                    switch (caseSwitch)
                    {
                        case 1:
                            VideoGamesGroup.Add(hobby);
                            break;
                        case 2:
                            SportGroup.Add(hobby);
                            break;
                        case 3:
                            HobbyAndFreeTimeGroup.Add(hobby);
                            break;
                        case 4:
                            DoItYourselfGroup.Add(hobby);
                            break;
                        case 5:
                            TechnologieGroup.Add(hobby);
                            break;
                        default:
                            OtherGroup.Add(hobby);
                            break;
                    }

                }

                //set grouped as item source
                grouped.Add(VideoGamesGroup); grouped.Add(SportGroup); grouped.Add(HobbyAndFreeTimeGroup); grouped.Add(DoItYourselfGroup); grouped.Add(TechnologieGroup);
                MyListView.ItemsSource = grouped;
            }

        }

        async void Handle_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null)
                return;

            //Deselect Item
            ((ListView)sender).SelectedItem = null;
        }

        protected override bool OnBackButtonPressed()
        {

            return base.OnBackButtonPressed();
        }

        private void GetHobbies()
        {
            hobbies = MainPage.hobbies;
        }

    }
}