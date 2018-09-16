using System;
using System.IO;
using SQLite;

namespace Frindr
{
    class Connection
    {
        public SQLiteConnection SQLConnection { get; set; } = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "user.db3"));
    }
}
