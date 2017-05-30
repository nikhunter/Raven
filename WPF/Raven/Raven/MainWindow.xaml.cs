using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Device.Location;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using MySql.Data.MySqlClient;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using Raven.Windows;

namespace Raven {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow {
        public static ObservableCollection<Tile> TripTileCollection { get; set; } =
            new ObservableCollection<Tile>(); // Collection of Map  pins, uses custom Datapoint class as type

        private const string ConnectionString = "Server=raven-gps.com;Database=raven;Uid=root;Pwd=Raven123";

        private readonly Location _start = new Location(55.758019, 12.392440);
        private readonly Location _end = new Location(55.759491, 12.457088);
        private Location _center;

        public MainWindow() {
            // Initiate LoginWindow element
            var loginWindow = new LoginWindow();
            Hide();
            loginWindow.Show();
            InitializeComponent();

            loginWindow.Closed += delegate {
                if (loginWindow.LoginSuccess) {
                    Show();
                }
                else {
                    Close();
                }
            };

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject),
                new FrameworkPropertyMetadata(Int32.MaxValue)); // Sets ToolTip duration to the max value of a long
        }

        // TODO SELECT * FROM trips, and create TripTiles based on results
        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e) {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            DataTable dt = new DataTable();
            connection.Open();

            try {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM trips";

                using (MySqlDataReader dr = command.ExecuteReader()) {
                    dt.Load(dr);
                    foreach (DataRow row in dt.Rows) {
                        // TODO Convert row data to RootObject for Tile creation
                        //var rowValue = row["log_file"].ToString();

                        //string json = @"{""key1"":""value1"",""key2"":""value2""}";

                        //Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(rowValue);

                        //foreach (var value in values) {
                        //    MessageBox.Show(value.ToString());
                        //}
                    }
                }
            }
            catch (MySqlException exception) {
                MessageBox.Show(exception.ToString());
            }
        }

        public static T Deserialize<T>(byte[] data) where T : class
        {
            using (var stream = new MemoryStream(data))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
                return JsonSerializer.Create().Deserialize(reader, typeof(T)) as T;
        }

        public class RootObject
        {
            [JsonProperty(PropertyName = "TimeDelta")]
            public string TimeDelta { get; set; }
            [JsonProperty(PropertyName = "104")]
            public string EngineLoad { get; set; }
            [JsonProperty(PropertyName = "105")]
            public string EngineCoolantTemp { get; set; }
            [JsonProperty(PropertyName = "10C")]
            public string RPM { get; set; }
            [JsonProperty(PropertyName = "10D")]
            public string Speed { get; set; }
            [JsonProperty(PropertyName = "10E")]
            public string TimingAdvance { get; set; }
            [JsonProperty(PropertyName = "10F")]
            public string IntakeAirTemp { get; set; }
            [JsonProperty(PropertyName = "111")]
            public string ThrottlePosition { get; set; }
            [JsonProperty(PropertyName = "12F")]
            public string FuelLevelInput { get; set; }
        }

        private void MainWindow_OnClosed(object sender, EventArgs e) {
        }

        public void SqlGetByReg(string reg) {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            DataTable dt = new DataTable();
            connection.Open();

            try {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM trips WHERE driver_reg='{reg}'";

                using (MySqlDataReader dr = command.ExecuteReader()) {
                    dt.Load(dr);
                    foreach (var line in dt.Rows[0].ItemArray) {
                        MessageBox.Show(line.ToString());
                    }
                }
            }
            catch (MySqlException exception) {
                MessageBox.Show(exception.ToString());
            }
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
            double bx = Math.Cos(DegreeToRadian(posB.Latitude)) * Math.Cos(dLon);
            double by = Math.Cos(DegreeToRadian(posB.Latitude)) * Math.Sin(dLon);

            midPoint.Latitude = RadianToDegree(Math.Atan2(
                Math.Sin(DegreeToRadian(posA.Latitude)) + Math.Sin(DegreeToRadian(posB.Latitude)),
                Math.Sqrt(
                    (Math.Cos(DegreeToRadian(posA.Latitude)) + bx) *
                    (Math.Cos(DegreeToRadian(posA.Latitude)) + bx) + by * by)));
            // (Math.Cos(DegreesToRadians(posA.Latitude))) + Bx) + By * By)); // Your Code

            midPoint.Longitude = posA.Longitude +
                                 RadianToDegree(Math.Atan2(by, Math.Cos(DegreeToRadian(posA.Latitude)) + bx));

            return new Location(midPoint.Latitude, midPoint.Longitude);
        }

        private double DegreeToRadian(double angle) {
            return Math.PI * angle / 180.0;
        }

        private double RadianToDegree(double angle) {
            return angle * (180.0 / Math.PI);
        }

        private void ChangeLocations() {
            _start.Latitude = _start.Latitude + 0.01;
            _start.Longitude = _start.Longitude + 0.01;
            _end.Latitude = _start.Latitude + 0.01;
            _end.Longitude = _start.Longitude + 0.01;
        }

        private void TileBtn_OnClick(object sender, RoutedEventArgs e) {
            _center = MidPoint(new GeoCoordinate(_start.Latitude, _start.Longitude),
                new GeoCoordinate(_end.Latitude, _end.Longitude));
            TripTileCollection.Add(new Tile(_start, _end, _center, 11, null, "BK79499", "22/05/2017", "12:54", 5.0, 15));
            ChangeLocations();
        }

        private void SearchBtn_OnClick(object sender, RoutedEventArgs e) {
            SqlGetByReg("BE70846");
        }
    }
}