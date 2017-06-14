﻿using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Maps.MapControl.WPF;

namespace Raven.Controls {
    /// <summary>
    /// Interaction logic for TileMap.xaml
    /// </summary>
    public partial class TileMap : Map {
        public Location StartLocation { get; set; }
        public Location EndLocation { get; set; }
        public LocationRect Bounds { get; set; }
        public MapLayer Route { get; set; }

        public TileMap() {
            InitializeComponent();
        }

        private void TileMap_OnLoaded(object sender, RoutedEventArgs e) {
            //Children.Add(Route);
            StartPin.Location = StartLocation;
            EndPin.Location = EndLocation;
            //SetView(Bounds);
        }
    }
}