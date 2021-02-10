using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;


namespace Randomizer.SuperMetroid.Regions.Brinstar {
    class Green : Region {
        public override string Name => "Brinstar Green";
        public override string Area => "Brinstar";

        public Green(World world, Config config) : base(world, config) {
            Weight = -6;

            RegionItems = new[] { CardBrinstarL2 };

            Locations = new List<Location> {
                new Location(this, 13, 0x8F84AC, LocationType.Chozo, "Power Bomb (green Brinstar bottom)", Minor, Logic switch {
                    _ => new Requirement(items => items.CardBrinstarL2 && items.CanUsePowerBombs())
                }),
                // morph can be here if you can get out with bombs
                new Location(this, 15, 0x8F8518, LocationType.Visible, "Missile (green Brinstar below super missile)", Minor, Logic switch {
                    Hard => items => items.CanOpenRedDoors(),
                    _ => new Requirement(items => items.CanOpenRedDoors() && items.CanPassBombPassages()),
                }).AlwaysAllow((item, items) => item.Is(Morph, World) && (items.Bombs || items.PowerBomb)),
                new Location(this, 16, 0x8F851E, LocationType.Visible, "Super Missile (green Brinstar top)", Minor, Logic switch {
                    Normal => items => items.CanOpenRedDoors() && items.SpeedBooster,
                    _ => new Requirement(items => items.CanOpenRedDoors() && (items.Morph || items.SpeedBooster))
                }),
                new Location(this, 17, 0x8F852C, LocationType.Chozo, "Reserve Tank, Brinstar", Major, Logic switch {
                    Normal => items => items.CanOpenRedDoors() && items.SpeedBooster,
                    _ => new Requirement(items => items.CanOpenRedDoors() && (items.Morph || items.SpeedBooster))
                }),
                new Location(this, 18, 0x8F8532, LocationType.Hidden, "Missile (green Brinstar behind missile)", Minor, Logic switch {
                    Normal => items => items.SpeedBooster && items.CanPassBombPassages() && items.CanOpenRedDoors(),
                    Medium => new Requirement(items => items.CanPassBombPassages() && items.CanOpenRedDoors()),
                    _ => new Requirement(items => (items.CanPassBombPassages() || items.Morph && items.ScrewAttack) && items.CanOpenRedDoors())
                }),
                new Location(this, 19, 0x8F8538, LocationType.Visible, "Missile (green Brinstar behind reserve tank)", Minor, Logic switch {
                    Normal => items => items.SpeedBooster && items.CanOpenRedDoors() && items.Morph,
                    _ => new Requirement(items => items.CanOpenRedDoors() && items.Morph)
                }),
                new Location(this, 30, 0x8F87C2, LocationType.Visible, "Energy Tank, Etecoons", Major, Logic switch {
                    _ => new Requirement(items => items.CardBrinstarL2 && items.CanUsePowerBombs())
                }),
                new Location(this, 31, 0x8F87D0, LocationType.Visible, "Super Missile (green Brinstar bottom)", Minor, Logic switch {
                    _ => new Requirement(items => items.CardBrinstarL2 && items.CanUsePowerBombs() && items.Super)
                }),
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal => items.CanDestroyBombWalls(),
                _ => items.CanDestroyBombWalls() || items.SpeedBooster
            };
        }
    }
}
