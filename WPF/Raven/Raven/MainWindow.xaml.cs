using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Device.Location;
using Microsoft.Maps.MapControl.WPF;
using Raven.Controls;

namespace Raven {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static ObservableCollection<Tile> TripTileCollection { get; set; } =
            new ObservableCollection<Tile>(); // Collection of Map  pins, uses custom Datapoint class as type

        private Location start = new Location(55.758019, 12.392440);
        private Location end = new Location(55.759491, 12.457088);
        private Location center;

        public MainWindow() {
            InitializeComponent();

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject),
                new FrameworkPropertyMetadata(Int32.MaxValue)); // Sets ToolTip duration to the max value of a long

            //var binding = new Binding();
            //binding.Source = TripTileCollection;
            //Dong.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        // Returns the distance (in metric meters) between two locations
        public static double CalculateDistance(Location previousPin, Location currentPin) {
            try {
                var firstCoordinate = new GeoCoordinate(previousPin.Longitude, previousPin.Latitude);
                var secondCoordinate = new GeoCoordinate(currentPin.Longitude, currentPin.Latitude);

                // Returns distance in meters
                return firstCoordinate.GetDistanceTo(secondCoordinate);
            }
            catch (Exception) {
                return 0;
            }
        }

        private Location MidPoint(GeoCoordinate posA, GeoCoordinate posB) {
            GeoCoordinate midPoint = new GeoCoordinate();

            double dLon = DegreeToRadian(posB.Longitude - posA.Longitude);
            double Bx = Math.Cos(DegreeToRadian(posB.Latitude)) * Math.Cos(dLon);
            double By = Math.Cos(DegreeToRadian(posB.Latitude)) * Math.Sin(dLon);

            midPoint.Latitude = RadianToDegree(Math.Atan2(
                Math.Sin(DegreeToRadian(posA.Latitude)) + Math.Sin(DegreeToRadian(posB.Latitude)),
                Math.Sqrt(
                    (Math.Cos(DegreeToRadian(posA.Latitude)) + Bx) *
                    (Math.Cos(DegreeToRadian(posA.Latitude)) + Bx) + By * By)));
            // (Math.Cos(DegreesToRadians(posA.Latitude))) + Bx) + By * By)); // Your Code

            midPoint.Longitude = posA.Longitude +
                                 RadianToDegree(Math.Atan2(By, Math.Cos(DegreeToRadian(posA.Latitude)) + Bx));

            return new Location(midPoint.Latitude, midPoint.Longitude);
        }

        private double DegreeToRadian(double angle) {
            return Math.PI * angle / 180.0;
        }

        private double RadianToDegree(double angle) {
            return angle * (180.0 / Math.PI);
        }

        private void ChangeLocations() {
            start.Latitude = start.Latitude + 0.01;
            start.Longitude = start.Longitude + 0.01;
            end.Latitude = start.Latitude + 0.01;
            end.Longitude = start.Longitude + 0.01;
        }

        private void TileBtn_OnClick(object sender, RoutedEventArgs e) {
            center = MidPoint(new GeoCoordinate(start.Latitude, start.Longitude), new GeoCoordinate(end.Latitude, end.Longitude));
            TripTileCollection.Add(new Tile(start, end, center, 11, null, "BK79499", "22/05/2017", "12:54", 5.0, 15));
            ChangeLocations();
        }
    }
}