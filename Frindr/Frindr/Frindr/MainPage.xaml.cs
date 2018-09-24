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

            var registerPage = new LoginPage();

            Navigation.PushModalAsync(registerPage);

            //App.Current.MainPage = new NavigationPage(this);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Register register = new Register();
            Navigation.PushModalAsync(register);
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            Login login = new Login();
            Navigation.PushModalAsync(login);
        }
    }
}
