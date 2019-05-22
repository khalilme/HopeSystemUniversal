using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeUWP.Models
{
    public class NavigationItem
    {
        public string Label { get; set; }
        public Type PageType { get; set; }
        public string Number { get; set; }
    }
}
