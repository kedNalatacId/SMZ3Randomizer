using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.Brinstar {
    class Kraid : Region {
        public override string Name => "Brinstar Kraid";
        public override string Area => "Brinstar";

        public Kraid(World world, Config config) : base(world, config) {
            Locations = new List<Location> {
                new Location(this, 43, 0x8F899C, LocationType.Hidden, "Energy Tank, Kraid", Major, items => items.CardBrinstarBoss),
                new Location(this, 48, 0x8F8ACA, LocationType.Chozo, "Varia Suit", Major,  items => items.CardBrinstarBoss),
                new Location(this, 44, 0x8F89EC, LocationType.Hidden, "Missile (Kraid)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanUsePowerBombs())
                }),
            };
        }

        public override bool CanEnter(Progression items) {
            return (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.CanPassBombPassages();
        }

        public bool CanComplete(Progression items) {
            return Locations.Get("Varia Suit").Available(items);
        }
    }
}
