using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Frindr.pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogoutPage : ContentPage
    {
        public LogoutPage()
        {
            InitializeComponent();

            //Navigation.PopToRootAsync();
        }

        protected override void OnAppearing()
        {
            //base.OnAppearing();


            Navigation.PopToRootAsync(false);
        }

    }
}