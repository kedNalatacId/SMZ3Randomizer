using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.Brinstar {
    class Pink : Region {
        public override string Name => "Brinstar Pink";
        public override string Area => "Brinstar";

        public Pink(World world, Config config) : base(world, config) {
            Weight = -4;

            RegionItems = new[] { CardNorfairL1, CardBrinstarBoss };

            Locations = new List<Location> {
                new Location(this, 14, 0x8F84E4, LocationType.Chozo, "Super Missile (pink Brinstar)", Minor, Logic switch {
                    Normal => items => items.CardBrinstarBoss && items.CanPassBombPassages() && items.Super,
                    _ => new Requirement(items => (items.CardBrinstarBoss || items.CardBrinstarL2) && items.CanPassBombPassages() && items.Super)
                }),
                new Location(this, 21, 0x8F8608, LocationType.Visible, "Missile (pink Brinstar top)", Minor, Logic switch {
                    Normal => items => items.CanFly() || items.Grapple,
                    _ => items => true,
                }),
                new Location(this, 22, 0x8F860E, LocationType.Visible, "Missile (pink Brinstar bottom)", Minor),
                new Location(this, 23, 0x8F8614, LocationType.Chozo, "Charge Beam", Major, Logic switch {
                    _ => new Requirement(items => items.CanPassBombPassages())
                }),
                new Location(this, 24, 0x8F865C, LocationType.Visible, "Power Bomb (pink Brinstar)", Minor, Logic switch {
                    Hard => items => items.CanUsePowerBombs() && items.Super,
                    _ => new Requirement(items => items.CanUsePowerBombs() && items.Super && items.HasEnergyReserves(1)),
                }),
                new Location(this, 25, 0x8F8676, LocationType.Visible, "Missile (green Brinstar pipe)", Minor, Logic switch {
                    _ => new Requirement(items => items.Morph && (items.PowerBomb || items.Super))
                }),

                // Adding HiJump for Medium, because getting out of the water is tricky
                new Location(this, 33, 0x8F87FA, LocationType.Visible, "Energy Tank, Waterway", Major, Logic switch {
                    Normal => new Requirement(items => items.CanUsePowerBombs() && items.CanOpenRedDoors() && items.SpeedBooster && items.Gravity),
                    Medium => new Requirement(items => items.CanUsePowerBombs() && items.CanOpenRedDoors() && items.SpeedBooster &&
                        (items.Gravity || (items.HiJump && items.HasEnergyReserves(1)))),
                    _ => new Requirement(items => items.CanUsePowerBombs() && items.CanOpenRedDoors() && items.SpeedBooster &&
                        (items.Gravity || items.HasEnergyReserves(1))),
                }),

                // Adding HiJump for Medium, because doing the door jump is tricky
                new Location(this, 35, 0x8F8824, LocationType.Visible, "Energy Tank, Brinstar Gate", Major, Logic switch {
                    Normal => items => items.CardBrinstarL2 && items.CanUsePowerBombs() && items.Wave && items.HasEnergyReserves(1),
                    Medium => items => items.CardBrinstarL2 && items.CanUsePowerBombs() && (items.Wave || (items.Super && items.HiJump)) && items.HasEnergyReserves(1),
                    _ => new Requirement(items => items.CardBrinstarL2 && items.CanUsePowerBombs() && (items.Wave || items.Super))
                }),
            };
        }

        public override bool CanEnter(Progression items) {
            return items.CanOpenRedDoors() && (items.CanDestroyBombWalls() || items.SpeedBooster) || items.CanUsePowerBombs();
        }
    }
}
