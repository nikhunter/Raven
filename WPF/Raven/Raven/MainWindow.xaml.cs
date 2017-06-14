﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Device.Location;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
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
        public static string Username = "";

        public MainWindow() {
            // Initiate LoginWindow element
            var loginWindow = new LoginWindow();
            Hide();
            loginWindow.Show();
            InitializeComponent();

            loginWindow.Closed += delegate {
                if (loginWindow.LoginSuccess) {
                    Title = $"Raven - Logged in as {Username}";
                    LoadTripTiles();
                    Show();
                }
                else {
                    Close();
                }
            };

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject),
                new FrameworkPropertyMetadata(int.MaxValue)); // Sets ToolTip duration to the max value of a long
        }

        private void LoadTripTiles() {
            var connection = new MySqlConnection(ConnectionString);
            var dt = new DataTable();
            connection.Open();

            try {
                var command = connection.CreateCommand();
                command.CommandText = "SELECT * FROM trips";

                // TODO Make this into a separate method
                using (var dr = command.ExecuteReader()) {
                    dt.Load(dr);
                    foreach (DataRow row in dt.Rows) {
                        try {
                            // TODO Convert row data to RootObject for Tile creation
                            var logs = row["log_file"].ToString();
                            var title = row["driver_reg"].ToString();
                            var date = row["time_started"].ToString();
                            var dateStart = DateTime.Parse(row["time_started"].ToString());
                            var dateEnd = DateTime.Parse(row["time_ended"].ToString());

                            // TODO Fetch coordinates of first and last 'result'
                            var results = JsonConvert.DeserializeObject<List<RootObject>>(logs);

                            // Set Bounds to fit all pins
                            var mostNorth = double.Parse(results[0].Latitude, CultureInfo.InvariantCulture);
                            var mostSouth = double.Parse(results[0].Latitude, CultureInfo.InvariantCulture);
                            var mostEast = double.Parse(results[0].Longitude, CultureInfo.InvariantCulture);
                            var mostWest = double.Parse(results[0].Longitude, CultureInfo.InvariantCulture);

                            for (var i = 1; i < results.Count; i++) {
                                var lat = double.Parse(results[i].Latitude, CultureInfo.InvariantCulture);
                                var lng = double.Parse(results[i].Longitude, CultureInfo.InvariantCulture);

                                if (lat > mostNorth) {
                                    mostNorth = lat;
                                }
                                else if (lat < mostSouth) {
                                    mostSouth = lat;
                                }
                                if (lng > mostEast) {
                                    mostEast = lng;
                                }
                                else if (lat < mostWest) {
                                    mostWest = lng;
                                }
                            }
                            var bounds = new LocationRect(new Location(mostNorth + 0.015, mostWest + 0.0075), new Location(mostSouth - 0.002, mostEast - 0.0075));

                            // Set start and end locations
                            var startLocation = new Location(double.Parse(results[0].Latitude, CultureInfo.InvariantCulture), double.Parse(results[0].Longitude, CultureInfo.InvariantCulture));
                            var endLocation = new Location(double.Parse(results[results.Count - 1].Latitude, CultureInfo.InvariantCulture), double.Parse(results[results.Count - 1].Longitude, CultureInfo.InvariantCulture));

                            // Create MapLayer of driven route
                            var polyLineLayer = new MapLayer(); // Layer used only for MapPolyLines for easier cleaning
                            for (var i = 1; i < results.Count; i++) {
                                var polyLine = new MapPolyline();
                                var colourBrush = new SolidColorBrush {Color = Color.FromRgb(232, 123, 45)};
                                polyLine.Stroke = colourBrush;
                                polyLine.StrokeThickness = 2;
                                polyLine.Opacity = 1.0;

                                polyLine.Locations = new LocationCollection {
                                    new Location(double.Parse(results[i - 1].Latitude, CultureInfo.InvariantCulture), double.Parse(results[i - 1].Longitude, CultureInfo.InvariantCulture)),
                                    new Location(double.Parse(results[i].Latitude, CultureInfo.InvariantCulture), double.Parse(results[i].Longitude, CultureInfo.InvariantCulture))
                                };
                                polyLineLayer.Children.Add(polyLine); // Adds a new line to the layer
                            }

                            // Format date
                            date = date.Replace('-', '/');
                            date = date.Replace(" ", " - ");

                            // Calculate distance
                            var distance = 0.0;
                            for (var i = 1; i < results.Count; i++) {
                                var oldIndex = new Location(double.Parse(results[i - 1].Latitude, CultureInfo.InvariantCulture), double.Parse(results[i - 1].Longitude, CultureInfo.InvariantCulture));
                                var newIndex = new Location(double.Parse(results[i].Latitude, CultureInfo.InvariantCulture), double.Parse(results[i].Longitude, CultureInfo.InvariantCulture));

                                distance = distance + CalculateDistance(oldIndex, newIndex);
                            }
                            distance = distance / 1000; // Convert from meters to kilometers

                            // Calculate duration
                            // MAYBE Change to get difference between first and last index of 'results' collection
                            string duration;
                            var difference = dateEnd - dateStart;
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
                            TripTileCollection.Add(new Tile(startLocation, endLocation, bounds, polyLineLayer, title, date, distance, duration));
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
            var connection = new MySqlConnection(ConnectionString);
            var dt = new DataTable();
            connection.Open();

            try {
                var command = connection.CreateCommand();
                command.CommandText = $"SELECT * FROM trips WHERE driver_reg='{reg}'";

                using (var dr = command.ExecuteReader()) {
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

        // Returns the Location in the middle of two GeoCoordinates
        public Location MidPoint(Location posA, Location posB) {
            var midPoint = new GeoCoordinate();

            var dLon = DegreeToRadian(posB.Longitude - posA.Longitude);
            var bx = Math.Cos(DegreeToRadian(posB.Latitude)) * Math.Cos(dLon);
            var by = Math.Cos(DegreeToRadian(posB.Latitude)) * Math.Sin(dLon);

            midPoint.Latitude = RadianToDegree(Math.Atan2(
                Math.Sin(DegreeToRadian(posA.Latitude)) + Math.Sin(DegreeToRadian(posB.Latitude)),
                Math.Sqrt(
                    (Math.Cos(DegreeToRadian(posA.Latitude)) + bx) *
                    (Math.Cos(DegreeToRadian(posA.Latitude)) + bx) + by * by)));

            midPoint.Longitude = posA.Longitude +
                                 RadianToDegree(Math.Atan2(by, Math.Cos(DegreeToRadian(posA.Latitude)) + bx));

            // MAYBE Add an offset to Latitude
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
            else if (SearchDetailsBox.Text == string.Empty) {
                TripTileCollection.Clear();
                LoadTripTiles();
            }
        }

        private void TripItemsControl_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TileGrid.Visibility = Visibility.Collapsed;
            RavenMainWindow.WindowState = WindowState.Maximized;
        }

        private void RavenMainWindow_OnMouseRightButtonUp(object sender, MouseButtonEventArgs e) {

            TileGrid.Visibility = Visibility.Visible;
            RavenMainWindow.WindowState = WindowState.Normal;
        }
    }
}