﻿using System;
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
using System.Threading;

namespace Frindr.pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectHobbyPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        private ObservableCollection<GlobalVariables.Category> grouped { get; set; }

        private ObservableCollection<GlobalVariables.Hobbies> hobbiesCollection = GlobalVariables.hobbiesCollection;
        private ObservableCollection<GlobalVariables.Hobbies> selectedHobbies   = GlobalVariables.selectedHobbies;

        public SelectHobbyPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            
            
            FillListView();
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
                selectedHobbies = new ObservableCollection<GlobalVariables.Hobbies>(root.records);

                grouped = new ObservableCollection<GlobalVariables.Category>();

                RestfulClass rest = new RestfulClass();
                string json = rest.GetData($"/records/userHobby?filter=userId,eq,{GlobalVariables.loginUser.id}");
                GlobalVariables.UserHobbyRecords hobbyRecords = JsonConvert.DeserializeObject<GlobalVariables.UserHobbyRecords>(json);

                //Define Hobby categories
                var VideoGamesGroup         = new GlobalVariables.Category() { id = 1, name = "Video games" };
                var SportGroup              = new GlobalVariables.Category() { id = 2, name = "Sporten" };
                var HobbyAndFreeTimeGroup   = new GlobalVariables.Category() { id = 3, name = "Hobby en vrije tijd" };
                var DoItYourselfGroup       = new GlobalVariables.Category() { id = 4, name = "Doe-het-zelf" };
                var TechnologieGroup        = new GlobalVariables.Category() { id = 5, name = "Technologie" };
                var OtherGroup              = new GlobalVariables.Category() { id = 0, name = "Overig"};

                //add hobbies to groups
                foreach(var hobby in selectedHobbies)
                {
                    foreach (var userhobby in hobbyRecords.records)
                    {
                        if (hobby.id == userhobby.hobbyId)
                        {
                            hobby.selected = true;

                            break;
                        }
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

        private void SafeHobbySelectionButton_Clicked(object sender, EventArgs e)
        {
            var selected = hobbiesCollection
            .Where(p => p.selected)
            .ToList();
            GlobalVariables.selectedHobbies = new ObservableCollection<GlobalVariables.Hobbies>(selected);

            RestfulClass rest = new RestfulClass();
            //get current user id

            string json = rest.GetData($"/records/user?filter=id,eq,{GlobalVariables.loginUser.id}");
            UserRecords deJson = JsonConvert.DeserializeObject<UserRecords>(json);
            Thread.Sleep(200);

            string json1 = rest.GetData($"/records/userHobby?filter=userId,eq,{GlobalVariables.loginUser.id}");
            GlobalVariables.UserHobbyRecords records = JsonConvert.DeserializeObject<GlobalVariables.UserHobbyRecords>(json1);

            //ignore hobbies previously selected
            foreach (var hobby in selected)
            {
                

                GlobalVariables.hobbyUser.userId = deJson.records[0].id ?? default(int);
                GlobalVariables.hobbyUser.hobbyId = hobby.id;

                bool hobbyExists = false;

                foreach (var userhobby in records.records)
                {
                    if (hobby.id == userhobby.hobbyId)
                    {
                        hobbyExists = true;
                        break;
                    }
                }
                if (!hobbyExists)
                {
                    string json2 = JsonConvert.SerializeObject(GlobalVariables.hobbyUser);
                    rest.CreateData("/records/userHobby/", json2);
                }
            }
            foreach (var unselected in hobbiesCollection)
            {
                string json3 = rest.GetData($"/records/userHobby?filter=userId,eq,{GlobalVariables.loginUser.id}");
                GlobalVariables.UserHobbyRecords hobbyRecords = JsonConvert.DeserializeObject<GlobalVariables.UserHobbyRecords>(json3);

                foreach (var hobby in hobbyRecords.records)
                {
                    if (!unselected.selected && hobby.hobbyId == unselected.id)
                    {

                        rest.DeleteData($"/records/userHobby/{hobby.id}");
                        break;
                    }
                }
                
            }
            Navigation.PopModalAsync();
        }
    }
}
