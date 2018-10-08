using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections;

namespace Frindr.pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectHobbyPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public SelectHobbyPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            FillListView();
        }

        private ObservableCollection<GlobalVariables.Category> grouped { get; set; }

        void FillListView()
        {
            var records = MainPage.hobbies;


            if (records != null)
            {
                MyListView.ItemsSource = null;
          
                var root = JsonConvert.DeserializeObject<GlobalVariables.HobbyRecords>(records);
                //Convert list to observable collection. This is easier for the grouping in the listview
                ObservableCollection<GlobalVariables.Hobbies> hobbiesCollection = new ObservableCollection<GlobalVariables.Hobbies>(root.records);

                grouped = new ObservableCollection<GlobalVariables.Category>();

                //Define Hobby categories
                var VideoGamesGroup         = new GlobalVariables.Category() { id = 1, name = "Video games" };
                var SportGroup              = new GlobalVariables.Category() { id = 2, name = "Sporten" };
                var HobbyAndFreeTimeGroup   = new GlobalVariables.Category() { id = 3, name = "Hobby en vrije tijd" };
                var DoItYourselfGroup       = new GlobalVariables.Category() { id = 4, name = "Doe-het-zelf" };
                var TechnologieGroup        = new GlobalVariables.Category() { id = 5, name = "Technologie" };
                var OtherGroup              = new GlobalVariables.Category() { id = 0, name = "Overig"};

                //add hobbies to groups
                foreach(var hobby in hobbiesCollection)
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
    }
}
