using System;
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
        public int? ID { get; set; }
        public string User { get; set; }
        public string Pwd { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string Birthday { get; set; }
        public string ImagePath { get; set; }
        public bool UserVisibility { get; set; }
        public bool LocationVisibility { get; set; }
    }
}
