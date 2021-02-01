using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.NorfairUpper {

    class West : Region {

        public override string Name => "Norfair Upper West";
        public override string Area => "Norfair Upper";

        public West(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardLowerNorfairL1, CardNorfairL2, CardNorfairBoss };

            Locations = new List<Location> {
                new Location(this, 50, 0x8F8B24, LocationType.Chozo, "Ice Beam", Major, Logic switch {
                    Normal => items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanPassBombPassages() && items.Varia && items.SpeedBooster,
                    Medium => items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanPassBombPassages() && (items.Varia || items.HasEnergyReserves(5)),
                    _ => new Requirement(items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.Morph && (items.Varia || items.HasEnergyReserves(3)))
                }),
                // new Location(this, 50, "Ice Beam", Chozo, Major, 0x78B24, Logic switch

                new Location(this, 51, 0x8F8B46, LocationType.Hidden, "Missile (below Ice Beam)", Minor, Logic switch {
                    Normal => items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanUsePowerBombs() && items.Varia && items.SpeedBooster,
                    Medium => items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanUsePowerBombs() && (items.Varia || items.HasEnergyReserves(5)),
                    _ => new Requirement(items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanUsePowerBombs() &&
                        (items.Varia || items.HasEnergyReserves(3)) ||
                        items.Varia && items.SpeedBooster && items.Super && items.CardNorfairL1)
                }),
                // new Location(this, 51, "Missile (below Ice Beam)", Hidden, Minor, 0x78B46, Logic switch

                new Location(this, 53, 0x8F8BAC, LocationType.Chozo, "Hi-Jump Boots", Major, Logic switch {
                    _ => new Requirement(items => items.CanOpenRedDoors() && items.CanPassBombPassages())
                }),
                // new Location(this, 53, "Hi-Jump Boots", Chozo, Major, 0x78BAC, Logic switch

                new Location(this, 55, 0x8F8BE6, LocationType.Visible, "Missile (Hi-Jump Boots)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanOpenRedDoors() && items.Morph)
                }),
                // new Location(this, 55, "Missile (Hi-Jump Boots)", Visible, Minor, 0x78BE6, Logic switch

                new Location(this, 56, 0x8F8BEC, LocationType.Visible, "Energy Tank (Hi-Jump Boots)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanOpenRedDoors())
                }),
                // new Location(this, 56, "Energy Tank (Hi-Jump Boots)", Visible, Minor, 0x78BEC, Logic switch
            };
        }

        public override bool CanEnter(Progression items) {
            return (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.Morph;
        }
    }
}
