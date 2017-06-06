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
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using System.Web.Script.Serialization;
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
                    Show();
                }
                else {
                    Close();
                }
            };

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject),
                new FrameworkPropertyMetadata(Int32.MaxValue)); // Sets ToolTip duration to the max value of a long
        }

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
                        var logs = row["log_file"].ToString();
                        var title = row["driver_reg"].ToString();
                        var date = row["time_started"].ToString();
                        DateTime dateStart = DateTime.Parse(row["time_started"].ToString());
                        DateTime dateEnd = DateTime.Parse(row["time_ended"].ToString());

                        // TODO Fetch coordinates of first and last 'result'
                        List<RootObject> results = JsonConvert.DeserializeObject<List<RootObject>>(logs);

                        // Format date
                        date = date.Replace('-', '/');
                        date = date.Replace(" ", " - ");

                        // Calculate distance // TODO Get the total distance between all pins
                        // foreach (row in results)
                        // Find distance, add it to Total
                        // var distance = total;

                        // Calculate duration // TODO Separate time from time_started/time_ended
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

                        // TODO Add TripTiles based on 'results'
                        TripTileCollection.Add(new Tile(null, null, null, 14, null, title, date, null, 20, duration));
                    }
                }
            }
            catch (MySqlException exception) {
                MessageBox.Show(exception.ToString());
            }
        }

        public class RootObject {
            [JsonProperty(PropertyName = "TimeDelta")]
            public string TimeDelta { get; set; }

            [JsonProperty(PropertyName = "104")]
            public string EngineLoad { get; set; }

            [JsonProperty(PropertyName = "105")]
            public string EngineCoolantTemp { get; set; }

            [JsonProperty(PropertyName = "10C")]
            public string Rpm { get; set; }

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

            /*
            [JsonProperty(PropertyName = "lat")]
            public string Latitude { get; set; }

            [JsonProperty(PropertyName = "lng")]
            public string Longitude { get; set; }
            */
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