using ApiLibrary;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
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

namespace MetroMap
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Pushpin pin, stopPin;
        string longitude, latitude, rayon, color, url, linesInfo;
        int count = 0;
        Api stopsApi, lineApi;
        List<BusLine> lineInfos;
        ListViewItem li;
        SolidColorBrush mySolidColorBrush = new SolidColorBrush();

        public MainWindow()
        {
            InitializeComponent();

            //Set focus on map
            metroMap.Focus();

        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (metroMap.Mode.ToString() == "Microsoft.Maps.MapControl.WPF.RoadMode")
            {
                //Set the map mode to Aerial with labels
                metroMap.Mode = new AerialMode(true);
            }


            else
            {
                //Set the map mode to RoadMode
                metroMap.Mode = new RoadMode();
            }
        }

        private void MapWithPushpins_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // Disables the default mouse double-click action.
            e.Handled = true;

            // Determin the location to place the pushpin at on the map.

            //Get the mouse click coordinates
            Point mousePosition = e.GetPosition(this);
            //Convert the mouse coordinates to a locatoin on the map
            Location pinLocation = metroMap.ViewportPointToLocation(mousePosition);

            // Remove previous pins
            metroMap.Children.Clear();          

            // The pushpin to add to the map.
            pin = new Pushpin();
            pin.Location = pinLocation;

            // Adds the pushpin to the map.
            //metroMap.Children.Add(pin);

            longitude = pinLocation.Longitude.ToString().Replace(",",".");
            latitude = pinLocation.Latitude.ToString().Replace(",", ".");
            rayon = "800";
            url = $"http://data.metromobilite.fr/api/linesNear/json?x={longitude}&y={latitude}&dist=500&details=true";
            stopsApi = new Api(url);
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

            // Affichage des pins des arrets et de leurs lignes
            foreach (BusStop busStop in busStopsWithoutDoubles)
            {
                count = 1;
                linesInfo = "";
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
                    linesInfo += "\n- " + lineInfos.First().longName;
                    count++;
                }
                MapLayer stopPin = new MapLayer();
                Image image = new Image();
                image.Height = 150;
                //Define the URI location of the image
                BitmapImage myBitmapImage = new BitmapImage();
                myBitmapImage.BeginInit();
                myBitmapImage.UriSource = new Uri("https://cdn3.iconfinder.com/data/icons/maps-and-pins-4/512/map_pin_destination_location_adress_bus_stop-256.png");

                myBitmapImage.DecodePixelHeight = 35;
                myBitmapImage.EndInit();
                image.Source = myBitmapImage;
                image.Opacity = 1;
                image.Stretch = System.Windows.Media.Stretch.None;

                Location location = new Location(busStop.lat, busStop.lon);
                PositionOrigin position = PositionOrigin.Center;
                stopPin.AddChild(image, location, position);

                ToolTipService.SetToolTip(stopPin, busStop.name + " :" + linesInfo);
                metroMap.Children.Add(stopPin);
            }
        }
    }
}
