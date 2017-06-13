using System;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace Raven.Controls {
    /// <summary>
    /// Interaction logic for TileMap.xaml
    /// </summary>
    public partial class TileMap {
        public TileMap() {
            var pins = MainWindow.TripTileCollection;
            var index = pins.Count - 1;
            InitializeComponent();

            try {
                var startLocation = new Pushpin {Background = Brushes.Green, Location = pins[pins.Count - 1].StartLocation};
                var endLocation = new Pushpin {Background = Brushes.Red, Location = pins[pins.Count - 1].EndLocation};

                TripTileMap.Children.Add(pins[pins.Count - 1].Route);
                TripTileMap.Loaded += (s, e) => {
                    TripTileMap.SetView(pins[index].Bounds);
                    TripTileMap.Children.Add(startLocation);
                    TripTileMap.Children.Add(endLocation);
                };
            }
            catch (Exception) {
                // ignored
            }
        }
    }
}