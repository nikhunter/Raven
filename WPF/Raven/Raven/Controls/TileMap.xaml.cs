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
using Microsoft.Maps.MapControl.WPF;

namespace Raven.Controls {
    /// <summary>
    /// Interaction logic for TileMap.xaml
    /// </summary>
    public partial class TileMap : UserControl {
        public TileMap() {
            ObservableCollection<Tile> pins = MainWindow.TripTileCollection;
            InitializeComponent();

            try {
                TripTileMap.Center = pins[pins.Count - 1].CenterLocation;
                TripTileMap.ZoomLevel = pins[pins.Count - 1].ZoomLevel;

                Pushpin startLocation = new Pushpin {Location = pins[pins.Count - 1].StartLocation};
                Pushpin endLocation = new Pushpin {Location = pins[pins.Count - 1].EndLocation};

                TripTileMap.Children.Add(startLocation);
                TripTileMap.Children.Add(endLocation);
            }
            catch (Exception) {
                // ignored
            }
        }
    }
}