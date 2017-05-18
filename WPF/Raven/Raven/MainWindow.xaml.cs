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


        public MainWindow()
        {
            InitializeComponent();

            TripTileCollection.Add("One");
            TripTileCollection.Add("Two");
            TripTileCollection.Add("Three");
            TripTileCollection.Add("Four");
            TripTileCollection.Add("Five");

            TripTileCollection.Add("Six");
            TripTileCollection.Add("Seven");
            TripTileCollection.Add("Eight");
            TripTileCollection.Add("Nine");
            TripTileCollection.Add("Ten");
        }
    }
}