using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.Brinstar {

    class Red : Region {

        public override string Name => "Brinstar Red";
        public override string Area => "Brinstar";

        public Red(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardMaridiaL1, CardNorfairL1, CardBrinstarBoss };
            Locations = new List<Location> {

                new Location(this, 38, 0x8F8876, LocationType.Chozo, "X-Ray Scope", Major, Logic switch {
                    Normal => items => items.CanUsePowerBombs() && items.CanOpenRedDoors() && (items.Grapple || items.SpaceJump) &&
                        (items.Varia || items.HasEnergyReserves(3)),
                    Medium => items => items.CanUsePowerBombs() && items.CanOpenRedDoors() && (items.Grapple || items.SpaceJump),
                    _ => new Requirement(items => items.CanUsePowerBombs() && items.CanOpenRedDoors() && (
                        items.Grapple || items.SpaceJump ||
                        (items.CanIbj() || items.HiJump && items.SpeedBooster || items.CanSpringBallJump()) &&
                            (items.Varia && items.HasEnergyReserves(3) || items.HasEnergyReserves(5))))
                }),
                // new Location(this, 38, "X-Ray Scope", Chozo, Major, 0x78876, Logic switch

                new Location(this, 39, 0x8F88CA, LocationType.Visible, "Power Bomb (red Brinstar sidehopper room)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanUsePowerBombs() && items.Super)
                }),
                // new Location(this, 39, "Power Bomb (red Brinstar sidehopper room)", Visible, Minor, 0x788CA, Logic switch

                new Location(this, 40, 0x8F890E, LocationType.Chozo, "Power Bomb (red Brinstar spike room)", Minor, Logic switch {
                    Normal => items => (items.CanUsePowerBombs() || items.Ice) && items.Super,
                    _ => new Requirement(items => items.Super)
                }),
                // new Location(this, 40, "Power Bomb (red Brinstar spike room)", Chozo, Minor, 0x7890E, Logic switch

                new Location(this, 41, 0x8F8914, LocationType.Visible, "Missile (red Brinstar spike room)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanUsePowerBombs() && items.Super)
                }),
                // new Location(this, 41, "Missile (red Brinstar spike room)", Visible, Minor, 0x78914, Logic switch

                new Location(this, 42, 0x8F896E, LocationType.Chozo, "Spazer", Major, Logic switch {
                    _ => new Requirement(items => items.CanPassBombPassages() && items.Super)
                }),
                // new Location(this, 42, "Spazer", Chozo, Major, 0x7896E, Logic switch
            };
        }

        public override bool CanEnter(Progression items) {
            return (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.Morph;
        }
    }
}
