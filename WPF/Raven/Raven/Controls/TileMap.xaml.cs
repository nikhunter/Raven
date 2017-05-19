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

namespace Raven.Controls
{
    /// <summary>
    /// Interaction logic for TileMap.xaml
    /// </summary>
    public partial class TileMap : UserControl
    {
        public static ObservableCollection<Tile> TripTileCollection { get; set; } = new ObservableCollection<Tile>(); // Collection of Map  pins, uses custom Datapoint class as type

        public TileMap()
        {
            InitializeComponent();
            TripTileCollection.Add(new Tile(new Location(55.758019, 12.392440), new Location(55.759491, 12.457088), new Location(55.759254, 12.422076), 11, null, "BE70846", "19/05/2017", "13:30", 17.2, 20));
            TripTileCollection.Add(new Tile(new Location(55.758019, 12.392440), new Location(55.759491, 12.457088), new Location(55.759254, 12.422076), 11, null, "BE70846", "19/05/2017", "13:30", 17.2, 20));
            TripTileCollection.Add(new Tile(new Location(55.758019, 12.392440), new Location(55.759491, 12.457088), new Location(55.759254, 12.422076), 11, null, "BE70846", "19/05/2017", "13:30", 17.2, 20));
        }
    }
}
