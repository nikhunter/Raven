using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Device.Location;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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

        public MainWindow() {
            // Initiate LoginWindow element
            var loginWindow = new LoginWindow();
            Hide();
            loginWindow.Show();
            InitializeComponent();

            loginWindow.Closed += delegate {
                if (loginWindow.LoginSuccess) {
                    LoadTripTiles();
                    Show();
                }
                else {
                    Close();
                }
            };

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject),
                new FrameworkPropertyMetadata(Int32.MaxValue)); // Sets ToolTip duration to the max value of a long
        }

        private static void LoadTripTiles() {
            MySqlConnection connection = new MySqlConnection(ConnectionString);
            DataTable dt = new DataTable();
            connection.Open();

            try {
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM trips";

                using (MySqlDataReader dr = command.ExecuteReader()) {
                    dt.Load(dr);
                    foreach (DataRow row in dt.Rows) {
                        try {
                            // TODO Convert row data to RootObject for Tile creation
                            var logs = row["log_file"].ToString();
                            var title = row["driver_reg"].ToString();
                            var date = row["time_started"].ToString();
                            DateTime dateStart = DateTime.Parse(row["time_started"].ToString());
                            DateTime dateEnd = DateTime.Parse(row["time_ended"].ToString());

                            // TODO Fetch coordinates of first and last 'result'
                            List<RootObject> results = JsonConvert.DeserializeObject<List<RootObject>>(logs);

                            // TESTING PURPOSES ONLY
                            Location startLocation = new Location(double.Parse(results[0].Latitude, CultureInfo.InvariantCulture), double.Parse(results[0].Longitude, CultureInfo.InvariantCulture));
                            Location centerLocation = new Location(double.Parse(results[1].Latitude, CultureInfo.InvariantCulture), double.Parse(results[1].Longitude, CultureInfo.InvariantCulture));
                            Location endLocation = new Location(double.Parse(results[2].Latitude, CultureInfo.InvariantCulture), double.Parse(results[2].Longitude, CultureInfo.InvariantCulture));
                            // TESTING PURPOSES ONLY

                            // Set ZoomLevel to fit all pins

                            // Create MapLayer of driven route

                            // Format date
                            date = date.Replace('-', '/');
                            date = date.Replace(" ", " - ");

                            // Calculate distance // TODO Get the total distance between all pins
                            var distance = CalculateDistance(startLocation, endLocation) / 1000; // TEMPORARILY - Bird flight distance only
                            // foreach (row in results)
                            // Find distance, add it to Total
                            // var distance = total;

                            // Calculate duration
                            // MAYBE Change to get difference between first and last index of 'results' collection
                            string duration;
                            TimeSpan difference = dateEnd - dateStart;
                            if (difference.TotalDays >= 1) {
                                duration = difference.Days + "d " + difference.Hours + "h " + difference.Minutes + "m";
                            }
                            else if (difference.TotalHours >= 1) {
                                duration = difference.Hours + "h " + difference.Minutes + "m";
                            }
                            else {
                                duration = difference.Minutes + "m";
                            }

                            // Create TripTile
                            TripTileCollection.Add(new Tile(startLocation, centerLocation, endLocation, 11, null, title, date, distance, duration));
                        }
                        catch (Exception) {
                            //ignore
                        }
                    }
                }
            }
            catch (MySqlException exception) {
                MessageBox.Show(exception.ToString());
            }
        }

        public class RootObject {
            [JsonProperty(PropertyName = "DeltaTime")]
            public string DeltaTime { get; set; }

            [JsonProperty(PropertyName = "Rpm")]
            public string Rpm { get; set; }

            [JsonProperty(PropertyName = "Speed")]
            public string Speed { get; set; }

            [JsonProperty(PropertyName = "Lat")]
            public string Latitude { get; set; }

            [JsonProperty(PropertyName = "Lng")]
            public string Longitude { get; set; }
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

                return firstCoordinate.GetDistanceTo(secondCoordinate);
            }
            catch (Exception) {
                return 0;
            }
        }

        public static string DoFormat(double myNumber) {
            var s = $"{myNumber:0.00}";

            return s.EndsWith("00") ? myNumber.ToString() : s;
        }

        public Location MidPoint(GeoCoordinate posA, GeoCoordinate posB) {
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

        public double DegreeToRadian(double angle) {
            return Math.PI * angle / 180.0;
        }

        public double RadianToDegree(double angle) {
            return angle * (180.0 / Math.PI);
        }

        private void SearchBtn_OnClick(object sender, RoutedEventArgs e) {
            if (Regex.IsMatch(SearchDetailsBox.Text, "[A-z]{2}[0-9]{5}")) {
                SqlGetByReg(SearchDetailsBox.Text);
            }
        }
    }
}