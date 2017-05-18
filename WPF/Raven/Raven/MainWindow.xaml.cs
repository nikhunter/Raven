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
using Raven.Program;

namespace Raven {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static ObservableCollection<string> TripTileCollection { get; set; } = new ObservableCollection<string>(); // Collection of Map  pins, uses custom Datapoint class as type
        public static ObservableCollection<Datapoint> PushPinCollection { get; set; } = new ObservableCollection<Datapoint>(); // Collection of Map  pins, uses custom Datapoint class as type

        public MainWindow() {
            InitializeComponent();

            var binding = new Binding();
            binding.Source = TripTileCollection;
            Dong.SetBinding(ItemsControl.ItemsSourceProperty, binding);


            TripTileCollection.Add("One");
            TripTileCollection.Add("One");
        }

        private void TileBtn_OnClick(object sender, RoutedEventArgs e) {
            TripTileCollection.Add("One");
        }
    }
}