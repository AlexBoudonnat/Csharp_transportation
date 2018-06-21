using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
using ApiLibrary;

namespace TransportCommun
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create a new Api object to get bus stops et lines infos
            Api stopsApi = new Api("http://data.metromobilite.fr/api/linesNear/json?x=5.731369500000028&y=45.18446720000001&dist=800&details=true");
            Api lineApi;

            // read file into a string and deserialize JSON to a type
            List<BusStop> busStops = JsonConvert.DeserializeObject<List<BusStop>>(stopsApi.getResponse());

            // Prepare a list of lines to get infos
            List<Line> lineInfos;

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

            // Affichage des arrets et de leurs lignes
            foreach (BusStop busStop in busStopsWithoutDoubles)
            {
                WriteLine($"Arret : {busStop.name}");
                Write("Lignes : ");
                foreach (string line in busStop.lines)
                {
                    lineApi = new Api("http://data.metromobilite.fr/api/routers/default/index/routes?codes=" + line);
                    lineInfos = JsonConvert.DeserializeObject<List<Line>>(lineApi.getResponse());

                    //WriteLine($"{line} : {lineInfos.longName} - couleur : {lineInfos.color}"); //
                    WriteLine($"{line} : {lineInfos.First().longName} - couleur : {lineInfos.First().color}");
                }
                WriteLine("\n");
            }
            ReadKey();
        }
    }
}
