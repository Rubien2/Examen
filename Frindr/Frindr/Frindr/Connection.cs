using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Frindr
{
    class Connection
    {
        readonly static string Location = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "main.sqlite");
        public SqliteConnection SQLConnection { get; set; } = new SqliteConnection("Data Source=" + Location);
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

    class Records
    {
        public List<JsonValues> records { get; set; }
    }
}
