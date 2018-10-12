using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;

namespace Frindr
{
    class Connection
    {
        readonly static string Location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "main.sqlite");
        public SqliteConnection SQLConnection { get; set; } = new SqliteConnection("Data Source=" + Location);

        public bool IsOnline()
        {
            RestfulClass rest = new RestfulClass();
            string check = rest.GetData("/records/hobby/1");

            try
            {
                HobbyRecords json = JsonConvert.DeserializeObject<HobbyRecords>(check);
                return true;
            }
            catch (ArgumentNullException)
            {
                return false;
            }
        }
    }

    class JsonValues
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string pwd { get; set; }
        public string location { get; set; }
        public string birthday { get; set; }
        public string imagePath { get; set; }
        public int userVisible { get; set; }
        public int locationVisible { get; set; }
    }

    class UserRecords
    {
        public List<JsonValues> records { get; set; }
    }

    class HobbyJson
    {
        public int? id { get; set; }
        public string hobby { get; set; }
        public int hobbyCategoryId { get; set; }
    }

    class HobbyRecords
    {
        public List<HobbyJson> records { get; set; }
    }

    class Hash
    {
        public string HashString(string passwordString)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] toBeHashed = sha.ComputeHash(Encoding.UTF8.GetBytes(passwordString));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < toBeHashed.Length; i++)
                {
                    builder.Append(toBeHashed[i].ToString());
                }
                return builder.ToString();
            }
        }
    }
}