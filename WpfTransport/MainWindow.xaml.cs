using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ApiLibrary;
using Newtonsoft.Json;

namespace WpfTransport
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string longitude, latitude, rayon, color;
        int count = 0;
        Api stopsApi, lineApi;
        List<BusLine> lineInfos;
        ListViewItem li;
        SolidColorBrush mySolidColorBrush = new SolidColorBrush();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            longitude = lg.Text;
            latitude = lt.Text;
            rayon = ray.Text;
            stopsApi = new Api($"http://data.metromobilite.fr/api/linesNear/json?x={longitude}&y={latitude}&dist={rayon}&details=true");
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

            rlt.Items.Clear();

            // Affichage des arrets et de leurs lignes
            foreach (BusStop busStop in busStopsWithoutDoubles)
            {
                rlt.Items.Add($"Arret : {busStop.name}");
                count = 1;
                foreach (string line in busStop.lines)
                {
                    if (count == 7)
                    {

                    }
                    lineApi = new Api("http://data.metromobilite.fr/api/routers/default/index/routes?codes=" + line);
                    lineInfos = JsonConvert.DeserializeObject<List<BusLine>>(lineApi.getResponse());

                    color = $"#{lineInfos.First().color}";
                    li = new ListViewItem();
                    li.Content = $"        {line} : {lineInfos.First().longName}";
                    if (color != "#")
                    {
                        mySolidColorBrush = (SolidColorBrush)(new BrushConverter().ConvertFrom(color));
                        li.Foreground = mySolidColorBrush;
                    }
                    rlt.Items.Add(li);
                    count++;
                }
            }
        }
    }
}
