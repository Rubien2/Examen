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
}
