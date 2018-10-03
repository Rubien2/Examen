﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Text.RegularExpressions;

namespace Frindr
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PersonalRegisterPage : ContentPage
	{
		public PersonalRegisterPage ()
		{
			InitializeComponent ();
		}

        private void NextButton_Clicked(object sender, EventArgs e)
        {
            if (CheckName(NameEntry.Text) && CheckLocation(LocationEntry.Text) && CheckAge())
            {
                HobbyRegisterPage hobbyRegisterPage = new HobbyRegisterPage();
                Navigation.PushModalAsync(hobbyRegisterPage);
            }
        }

        private bool CheckLocation(string location)
        {
            string regex = @"^[1-9][0-9]{3}\s?[a-zA-Z]{2}$";

            if (location == null)
            {
                DisplayAlert("","Voer een Postcode in", "ok");
                return false;
            }
        
            bool isLocationValid = Regex.IsMatch(location, regex);
            if (isLocationValid)
            {
                return true;
            }
            else
            {
                DisplayAlert("","Postcode is ongeldig","ok");
                return false;
            }
        }

        private bool CheckAge()
        {
            var birthday = int.Parse(BirthdayPicker.Date.ToString("yyyyMMdd"));
            var today = int.Parse(DateTime.Today.ToString("yyyyMMdd"));
            var age = (today - birthday) /10000;

            if (age >= 18) return true;
            else
            {
                DisplayAlert("","Je moet minimaal 18 jaar zijn","ok");
                return false;
            }
        }

        private bool CheckName(string name)
        {
            if (name != null)
            {
                return true;
            }
            else
            {
                DisplayAlert("", "Voer een naam in", "ok");
                return false;
            }
        }
    }
}