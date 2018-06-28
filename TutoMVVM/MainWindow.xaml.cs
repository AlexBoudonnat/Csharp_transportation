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

namespace TutoMVVM
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string longitude, latitude, rayon;
        BusStopViewModel busStopVM = new BusStopViewModel();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            longitude = lg.Text;
            latitude = lt.Text;
            rayon = ray.Text;

            busStopVM.BusStops = busStopVM.GetBusStopsFromApi(longitude, latitude, rayon);
            busStopVM.BusStops = busStopVM.InfosLineBusStops();
            results.ItemsSource = busStopVM.BusStops;

            string toto = busStopVM.BusStops[0].InfosBusLines[0].shortName;
        }
    }
}
