using System;
using Xamarin.Forms;

namespace Frindr
{
    public partial class MainPage : ContentPage
    {
        public bool Registered { get; set; }

        public MainPage()
        {
            InitializeComponent();

            var newPage = new MenuPage();

            Navigation.PushModalAsync(newPage);

            /*var page = (Page)Activator.CreateInstance(typeof(FriendFinderPage));

            var Detail = new NavigationPage(page);
            
            Navigation.PushModalAsync(Detail);
            */
            //App.Current.MainPage = new NavigationPage(this);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            RegisterPage register = new RegisterPage();
            Navigation.PushModalAsync(register);
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            LoginPage login = new LoginPage();
            Navigation.PushModalAsync(login);
        }
    }
}
