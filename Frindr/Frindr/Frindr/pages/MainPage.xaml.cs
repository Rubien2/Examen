using System;
using Xamarin.Forms;

namespace Frindr
{
    public partial class MainPage : ContentPage
    {
        public bool Registered { get; set; }
        public static string users { get; set; }
        public static string hobbies { get; set; }
        public static string userHobby { get; set; }

        public MainPage()
        {
            InitializeComponent();

            users = pages.GlobalVariables.GetUsers();
            hobbies = pages.GlobalVariables.GetHobbies();
            userHobby = pages.GlobalVariables.GetUserHobbies();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Navigation.PushModalAsync(new FirstRegisterPage());
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
