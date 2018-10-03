using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;

namespace Frindr.pages
{
    public class GlobalVariables
    {
        public static string records;

        public static string GetRecords()
        {
            records = FetchRecords();
            return records;
        }

        public static string FetchRecords()
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

        public class Hobbies
        {
            public int id { get; set; }
            public string hobby { get; set; }
            public int hobbyCategoryId { get; set; }
        }

        public class Records
        {
            public List<Hobbies> records { get; set; }
        }

        public class Category : ObservableCollection<Hobbies>
        {
            public int id { get; set; }
            public string name { get; set; }
        }

    }

}
