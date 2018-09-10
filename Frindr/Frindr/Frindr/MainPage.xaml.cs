using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using SQLite;
using System.IO;

namespace Frindr
{
    public partial class MainPage : ContentPage
    {
        string userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "User.txt");

        public MainPage()
        {
            InitializeComponent();
            App.Current.MainPage = new NavigationPage(this);
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
