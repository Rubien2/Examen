using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Frindr.pages;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class HobbyRegisterPage : ContentPage
	{

        private ObservableCollection<GlobalVariables.Category> grouped { get; set; }

        private ObservableCollection<GlobalVariables.Hobbies> hobbiesCollection = GlobalVariables.hobbiesCollection;
        private ObservableCollection<GlobalVariables.Hobbies> selectedHobbies = GlobalVariables.selectedHobbies;


        public HobbyRegisterPage ()
		{
			InitializeComponent ();

            FillListView();
		}

        private void CreateAccountButton_Clicked(object sender, EventArgs e)
        {

            var selected = hobbiesCollection
             .Where(p => p.selected)
             .ToList();
            GlobalVariables.selectedHobbies = new ObservableCollection<GlobalVariables.Hobbies>(selected);

            RestfulClass rest = new RestfulClass();
            string json = JsonConvert.SerializeObject(GlobalVariables.loginUser);
            rest.CreateData("/records/user/", json);

            MenuPage menuPage = new MenuPage();
            Navigation.PushModalAsync(menuPage);
        }

        void FillListView()
        {
            var records = MainPage.Hobbies;


            if (records != null)
            {
                MyListView.ItemsSource = null;

                var root = JsonConvert.DeserializeObject<GlobalVariables.HobbyRecords>(records);
                //Convert list to observable collection. This is easier for the grouping in the listview
                hobbiesCollection = new ObservableCollection<GlobalVariables.Hobbies>(root.records);

                grouped = new ObservableCollection<GlobalVariables.Category>();

                //Define Hobby categories
                var VideoGamesGroup =       new GlobalVariables.Category() { id = 1, name = "Video games" };
                var SportGroup =            new GlobalVariables.Category() { id = 2, name = "Sporten" };
                var HobbyAndFreeTimeGroup = new GlobalVariables.Category() { id = 3, name = "Hobby en vrije tijd" };
                var DoItYourselfGroup =     new GlobalVariables.Category() { id = 4, name = "Doe-het-zelf" };
                var TechnologieGroup =      new GlobalVariables.Category() { id = 5, name = "Technologie" };
                var OtherGroup =            new GlobalVariables.Category() { id = 0, name = "Overig" };

                //add hobbies to groups
                foreach (var hobby in hobbiesCollection)
                {
                    if (selectedHobbies != null && selectedHobbies.Any(p => p.id == hobby.id))
                    {
                        hobby.selected = true;
                    }

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

    }
}