using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maps.MapControl.WPF;

namespace ApiLibrary
{
    public class MyPushpin : Microsoft.Maps.MapControl.Pushpin
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
