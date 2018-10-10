using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Frindr
{

    public class MenuPageMenuItem
    {
        public MenuPageMenuItem()
        {
            TargetType = typeof(MenuPageDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}