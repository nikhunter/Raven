using Microsoft.Maps.MapControl.WPF;

namespace Raven {
    public class Tile {
        public int RowId { get; set; }
        public Location StartLocation { get; set; }
        public Location EndLocation { get; set; }
        public LocationRect Bounds { get; set; }
        public MapLayer Route { get; set; }
        public string Title { get; set; }
        public string Date { get; set; }
        public double Distance { get; set; }
        public string Duration { get; set; }

        public Tile(int rowId, Location startLocation, Location endLocation, LocationRect bounds, MapLayer route, string title, string date, double distance, string duration) {
            RowId = rowId;
            StartLocation = startLocation;
            EndLocation = endLocation;
            Bounds = bounds;
            Route = route;
            Title = title;
            Date = date;
            Distance = distance;
            Duration = duration;
        }
    }
}