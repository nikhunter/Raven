using System.Windows;
using System.Windows.Media;
using Microsoft.Maps.MapControl.WPF;

namespace Raven.Controls {
    /// <summary>
    /// Interaction logic for TileMap.xaml
    /// </summary>
    public partial class TileMap {

        public TileMap() {
            InitializeComponent();
        }

        private void TileMap_OnLoaded(object sender, RoutedEventArgs e) {
            var startLocation = new Pushpin { Background = Brushes.Green, Location = ((Tile)DataContext).StartLocation };
            var endLocation = new Pushpin { Background = Brushes.Red, Location = ((Tile)DataContext).EndLocation };

            Children.Add(((Tile)DataContext).Route);
            Children.Add(startLocation);
            Children.Add(endLocation);
            SetView(((Tile)DataContext).Bounds);
        }
    }
}