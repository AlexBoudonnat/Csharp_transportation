using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApiLibrary;
using Newtonsoft.Json;

namespace TutoMVVM
{
    public class BusStopViewModel
    {
        public List<BusStop> BusStops { get; set; }

        public List<BusStop> GetBusStopsFromApi(string lon, string lat, string rad)
        {
            Api stopsApi = new Api($"http://data.metromobilite.fr/api/linesNear/json?x={lon}&y={lat}&dist={rad}&details=true");
            List<BusStop> busStops = JsonConvert.DeserializeObject<List<BusStop>>(stopsApi.getResponse());

            // Nouvelle liste d'objet BusStop sans les doublons
            List<BusStop> busStopsWithoutDoubles = busStops.GroupBy(busStop => busStop.name).Select(x => x.First()).ToList();

            // Injection des lignes manquantes pour chaques arret avec suppression des doublons de ligne
            foreach (BusStop busStopWD in busStopsWithoutDoubles)
            {
                foreach (BusStop busStop in busStops)
                {
                    if (busStopWD.name.Equals(busStop.name))
                    {
                        IEnumerable<string> newLines = busStopWD.lines.Union(busStop.lines);
                        busStopWD.GetType().GetProperty("lines").SetValue(busStopWD, newLines);
                    }
                }
            }
            return busStopsWithoutDoubles;
        }

        public List<BusStop> InfosLineBusStops()
        {
            foreach (BusStop busStop in BusStops)
            {
                busStop.InfosBusLines = busStop.GetInfos();
            }

            return BusStops;
        }
    }
}
