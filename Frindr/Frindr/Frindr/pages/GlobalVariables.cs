﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using System.Net.Mail;

namespace Frindr.pages
{
    public class GlobalVariables
    {
        public static string records;
        public static string users;
        public static string userHobbies;

        public static ImageSource currentUserImage { get; set; }

        public static ObservableCollection<Hobbies> hobbiesCollection { get; set; }

        //----global account variables

        public static ObservableCollection<Hobbies> selectedHobbies { get; set; }

        //----------------------||||CLASSES||||-------------------------

        //hobby table

        private GlobalVariables()
        {
            

        }

        public static string GetHobbies()
        {
            try
            {
                RestfulClass restfulClass = new RestfulClass();
                var returnValue = restfulClass.GetData("/records/hobby");
                return returnValue;
            }
            catch (System.Net.WebException e)
            {
                return null;
            }

        }

        public static SmtpClient Client (MailMessage mail)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.strato.com", 587);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new System.Net.NetworkCredential("no-reply@frindr.nl", "frindrwachtwoord");
            smtpClient.Send(mail);
            return smtpClient;
        }

        public class Hobbies
        {
            public int id { get; set; }
            public string hobby { get; set; }
            public int hobbyCategoryId { get; set; }
            public bool selected { get; set; }
        }

        public class HobbyRecords
        {
            public List<Hobbies> records { get; set; }
        }

        public class Category : ObservableCollection<Hobbies>
        {
            public int id { get; set; }
            public string name { get; set; }
            public bool selected { get; internal set; }
        }

        //User table

        public static string GetUsers()
        {
            try
            {
                RestfulClass restfulClass = new RestfulClass();
                var returnValue = restfulClass.GetData($"/records/user?filter=id,neq,{loginUser.id}");
                return returnValue;

            }
            catch (System.Net.WebException e)
            {
                return e.ToString();
            }
        }

        public class UserRecords
        {
            public List<User> records { get; set; }
        }

        public class WrapUserRecords
        {
            public List<WrapUser> records { get; set; }
        }

        public class WrapUser
        {
            public User User { get; set; }
            public ImageSource imageSource { get; set; }
            public int distance { get; set; }
            public string hobbyList { get; set; }
            public int age { get; set; }
        }

        public class User
        {
            public int? id { get; set; }
            public string name { get; set; }
            public string email { get; set; }
            public string description { get; set; }
            public string pwd { get; set; }
            public string location { get; set; }
            public string birthday { get; set; }
            public string imagePath { get; set; }
            public int userVisible { get; set; }
            public int locationVisible { get; set; }
        }

        public static User loginUser = new User();

        //UserHobby table

        public static string GetUserHobbies()
        {
            try
            {
                RestfulClass restfulClass = new RestfulClass();
                var returnValue = restfulClass.GetData("/records/userHobby");
                return returnValue;
            }
            catch (System.Net.WebException e)
            {
                return e.ToString();
            }
        }

        public class UserHobbyRecords
        {
            public List<UserHobby> records { get; set; }
        }

        public class UserHobby
        {
            public int id { get; set; }
            public int userId { get; set; }
            public int hobbyId { get; set; }
        }

        public static UserHobby hobbyUser = new UserHobby();
    }

}
