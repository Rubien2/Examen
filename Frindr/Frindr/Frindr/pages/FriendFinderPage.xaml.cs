using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FriendFinderPage : ContentPage
	{
        public FriendFinderPage()
        {
            InitializeComponent();

            ObservableCollection<string> AgeList = new ObservableCollection<string> {"Elke leeftijd", "18-21 Jaar", "22-25 Jaar", "26-30 Jaar", "31-36 Jaar", "37-45 Jaar", "46-50 Jaar", "50+" };
            AgePicker.ItemsSource = AgeList;
            AgePicker.SelectedIndex = 0;

            ObservableCollection<string> DistanceList = new ObservableCollection<string> { "Alle afstanden", "< 2 KM", "< 5 KM", "< 10 KM", "< 15 KM", "< 25 KM", "< 50 KM", "< 75 KM", "< 100 KM"};
            DistancePicker.ItemsSource = DistanceList;
            DistancePicker.SelectedIndex = 0;
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

    }
}