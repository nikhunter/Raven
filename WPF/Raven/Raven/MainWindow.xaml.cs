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
using Raven.Controls;

namespace Raven {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static ObservableCollection<Tile> TripTileCollection { get; set; } = new ObservableCollection<Tile>(); // Collection of Map  pins, uses custom Datapoint class as type

        public MainWindow() {
            InitializeComponent();

            TripTileCollection.Add(new Tile(new Location(55.758019, 12.392440), new Location(55.759491, 12.457088), new Location(55.759254, 12.422076), 11, null, "BE70846", "19/05/2017", "13:30", 17.2, 20));
            TripTileCollection.Add(new Tile(new Location(55.758119, 12.392140), new Location(55.759191, 12.457188), new Location(55.759154, 12.422176), 11, null, "BK79499", "22/05/2017", "12:54", 5.0, 15));

            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(DependencyObject),
                new FrameworkPropertyMetadata(Int32.MaxValue)); // Sets ToolTip duration to the max value of a long

            //var binding = new Binding();
            //binding.Source = TripTileCollection;
            //Dong.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        private void TileBtn_OnClick(object sender, RoutedEventArgs e) {
            TripTileCollection.Add(new Tile(new Location(55.758019, 12.392440), new Location(55.759491, 12.457088), new Location(55.759254, 12.422076), 11, null, "BE70846", "19/05/2017", "13:30", 17.2, 20));
            TripTileCollection.Add(new Tile(new Location(55.758119, 12.392140), new Location(55.759191, 12.457188), new Location(55.759154, 12.422176), 11, null, "BK79499", "22/05/2017", "12:54", 5.0, 15));
        }
    }
}