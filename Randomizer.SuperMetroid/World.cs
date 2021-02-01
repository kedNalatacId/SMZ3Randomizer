using System;
using System.Collections.Generic;
using System.Linq;

namespace Randomizer.SuperMetroid {

    class World {

        public List<Location> Locations { get; set; }
        public List<Region> Regions { get; set; }
        public Config Config { get; set; }
        public string Player { get; set; }
        public string Guid { get; set; }
        public int Id { get; set; }

        public IEnumerable<Item> Items {
            get { return Locations.Select(l => l.Item).Where(i => i != null); }
        }

        public World(Config config, string player, int id, string guid) {
            Id = id;
            Config = config;
            Player = player;
            Guid = guid;

            Regions = new List<Region> {
                new Regions.Crateria.Central(this, Config),
                new Regions.Crateria.West(this, Config),
                new Regions.Crateria.East(this, Config),
                new Regions.Brinstar.Blue(this, Config),
                new Regions.Brinstar.Green(this, Config),
                new Regions.Brinstar.Kraid(this, Config),
                new Regions.Brinstar.Pink(this, Config),
                new Regions.Brinstar.Red(this, Config),
                new Regions.Maridia.Outer(this, Config),
                new Regions.Maridia.Inner(this, Config),
                new Regions.NorfairUpper.West(this, Config),
                new Regions.NorfairUpper.East(this, Config),
                new Regions.NorfairUpper.Crocomire(this, Config),
                new Regions.NorfairLower.West(this, Config),
                new Regions.NorfairLower.East(this, Config),
                new Regions.WreckedShip(this, Config)
            };

            Locations = Regions.SelectMany(x => x.Locations).ToList();
        }

        public bool CanEnter(string regionName, Progression items) {
            var region = Regions.Find(r => r.Name == regionName);
            if (region == null)
                throw new ArgumentException($"World.CanEnter: Invalid region name {regionName}", nameof(regionName));
            return region.CanEnter(items);
        }
    }
}
