using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLibrary
{
    public class BusStop
    {
        public string name { get; set; }
        public double lon { get; set; }
        public double lat { get; set; }
        public IEnumerable<string> lines { get; set; }
        public List<BusLine> InfosBusLines { get; set; }

        public List<BusLine> GetInfos()
        {
            List<BusLine> busLines = new List<BusLine>();
            foreach (string line in lines)
            {
                Api lineApi = new Api("http://data.metromobilite.fr/api/routers/default/index/routes?codes=" + line);
                List<BusLine> busInfos = JsonConvert.DeserializeObject<List<BusLine>>(lineApi.getResponse());
                busLines.Add(busInfos.First());
            }

            return busLines;
        }
    }
}
