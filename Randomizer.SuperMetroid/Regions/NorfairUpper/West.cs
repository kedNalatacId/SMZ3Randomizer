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
            RegionItems = new[] { CardNorfairL2 };

            Locations = new List<Location> {
                new Location(this, 49, 0x8F8AE4, LocationType.Hidden, "Missile (lava room)", Minor, Logic switch {
                    Normal => items => items.Varia && (
                            items.CanOpenRedDoors() && (items.CanFly() || items.HiJump || items.SpeedBooster) ||
                            World.CanEnter("Norfair Upper East", items) && items.CardNorfairL2
                        ) && items.Morph,
                    Medium => items => items.CanHeckRun() && (
                            items.CanOpenRedDoors() && (
                                items.CanFly() || items.HiJump || items.SpeedBooster || items.Varia && items.Ice
                            ) ||
                            World.CanEnter("Norfair Upper East", items) && items.CardNorfairL2
                        ) && items.Morph,
                    _ => new Requirement(items => items.CanHellRun() && (
                            items.CanOpenRedDoors() && (
                                items.CanFly() || items.HiJump || items.SpeedBooster ||
                                items.CanSpringBallJump() || items.Varia && items.Ice
                            ) ||
                            World.CanEnter("Norfair Upper East", items) && items.CardNorfairL2
                        ) && items.Morph),
                }),
                new Location(this, 50, 0x8F8B24, LocationType.Chozo, "Ice Beam", Major, Logic switch {
                    Normal => items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanPassBombPassages() && items.Varia && items.SpeedBooster,
                    Medium => items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanPassBombPassages() && (items.Varia || items.HasEnergyReserves(5)),
                    _ => new Requirement(items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.Morph && (items.Varia || items.HasEnergyReserves(3)))
                }),
                new Location(this, 51, 0x8F8B46, LocationType.Hidden, "Missile (below Ice Beam)", Minor, Logic switch {
                    Normal => items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanUsePowerBombs() && items.Varia && items.SpeedBooster,
                    Medium => items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanPassBombPassages() &&
                        (items.Varia || items.HasEnergyReserves(5)) ||
                        /* Access to Croc's room to get spark */
                        items.CanPassWaveGates(World) && items.Varia && items.SpeedBooster &&
                        (config.UseKeycards ? items.CardNorfairBoss : items.Super) && items.CardNorfairL1,
                    _ => new Requirement(items => (config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanPassBombPassages() &&
                        (items.Varia || items.HasEnergyReserves(3)) ||
                        /* Access to Croc's room to get spark */
                        items.CanPassWaveGates(World) && items.Varia && items.SpeedBooster &&
                        (config.UseKeycards ? items.CardNorfairBoss : items.Super) && items.CardNorfairL1)
                }),
                new Location(this, 53, 0x8F8BAC, LocationType.Chozo, "Hi-Jump Boots", Major, Logic switch {
                    _ => new Requirement(items => items.CanOpenRedDoors() && items.CanPassBombPassages())
                }),
                new Location(this, 55, 0x8F8BE6, LocationType.Visible, "Missile (Hi-Jump Boots)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanOpenRedDoors() && items.Morph)
                }),
                new Location(this, 56, 0x8F8BEC, LocationType.Visible, "Energy Tank (Hi-Jump Boots)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanOpenRedDoors())
                }),
            };
        }

        public override bool CanEnter(Progression items) {
            return (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.Morph;
        }
    }
}
