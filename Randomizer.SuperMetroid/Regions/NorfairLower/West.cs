using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.NorfairLower {

    class West : Region {

        public override string Name => "Norfair Lower West";
        public override string Area => "Norfair Lower";

        public West(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardLowerNorfairBoss };

            Locations = new List<Location> {
                new Location(this, 70, 0x8F8E6E, LocationType.Visible, "Missile (Gold Torizo)", Minor, Logic switch {
                    Normal => items => items.CanUsePowerBombs() && items.SpaceJump && items.Super,
                    _ => new Requirement(items => items.CanUsePowerBombs() && items.SpaceJump && items.Varia && (items.HiJump || items.Gravity))
                }),
                // new Location(this, 70, "Missile (Gold Torizo)", Visible, Minor, 0x78E6E, Logic switch

                new Location(this, 71, 0x8F8E74, LocationType.Hidden, "Super Missile (Gold Torizo)", Minor, Logic switch {
                    Normal => items => items.CanDestroyBombWalls() && (items.Super || items.Charge) && items.SpaceJump && items.CanUsePowerBombs(),
                    _ => new Requirement(items => items.CanDestroyBombWalls() && items.Varia && (items.Super || items.Charge))
                }),
                // new Location(this, 71, "Super Missile (Gold Torizo)", Hidden,  Minor, 0x78E74, Logic switch

                new Location(this, 79, 0x8F9110, LocationType.Chozo, "Screw Attack", Major, Logic switch {
                    Normal => items => items.CanDestroyBombWalls() && items.SpaceJump && items.CanUsePowerBombs(),
                    _ => new Requirement(items => items.CanDestroyBombWalls() && items.Varia)
                }),
                // new Location(this, 79, "Screw Attack", Chozo, Major, 0x79110, Logic switch
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal => items.Varia && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.SpaceJump && items.Gravity &&
                        (items.CardNorfairL1 && items.SpeedBooster || items.CardNorfairL2 || items.Wave && items.SpeedBooster)
                    ),
                Medium => items.Varia && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.Gravity && (items.HiJump || items.SpaceJump) &&
                        (items.CardNorfairL1 && items.SpeedBooster || items.CardNorfairL2 || items.Wave && items.SpeedBooster)
                    ),
                _ => World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.Varia && (items.HiJump || items.Gravity)
            };
        }
    }
}
