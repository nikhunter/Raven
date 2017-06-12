using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maps.MapControl.WPF;

namespace Raven
{
    public class Tile
    {
        public Location StartLocation { get; set; }
        public Location CenterLocation { get; set; }
        public Location EndLocation { get; set; }
        public int ZoomLevel { get; set; }
        public MapLayer Route { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public double Distance { get; set; }
        public string Duration { get; set; }

        public Tile(Location startLocation, Location center, Location endLocation, int zoomLevel, MapLayer route, string title, string date, double distance, string duration) {
            StartLocation = startLocation;
            CenterLocation = center;
            EndLocation = endLocation;
            ZoomLevel = zoomLevel;
            Route = route;
            Title = title;
            Date = date;
            Distance = distance;
            Duration = duration;
        }
    }
}
