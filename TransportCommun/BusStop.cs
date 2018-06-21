using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportCommun
{
    class BusStop
    {
        public string name { get; set; }
        public double lon { get; set; }
        public double lat { get; set; }
        public IEnumerable<string> lines { get; set; }
    }
}
