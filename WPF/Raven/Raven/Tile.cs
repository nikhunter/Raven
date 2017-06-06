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
        public Location EndLocation { get; set; }
        public Location CenterLocation { get; set; }
        public int ZoomLevel { get; set; }
        public MapLayer Route { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public string TimeStarted { get; set; }
        public double Distance { get; set; }
        public string Duration { get; set; }

        public Tile(Location startLocation, Location endLocation, Location center, int zoomLevel, MapLayer route, string title, string date, string timeStarted, double distance, string duration) {
            StartLocation = startLocation;
            EndLocation = endLocation;
            CenterLocation = center;
            ZoomLevel = zoomLevel;
            Route = route;
            Title = title;
            Date = date;
            TimeStarted = timeStarted;
            Distance = distance;
            Duration = duration;
        }
    }
}
