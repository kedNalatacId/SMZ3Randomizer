﻿using System.Collections.Generic;
using static Randomizer.SMZ3.SMLogic;
using static Randomizer.SMZ3.ItemType;

namespace Randomizer.SMZ3.Regions.SuperMetroid.NorfairLower {

    class West : SMRegion {

        public override string Name => "Norfair Lower West";
        public override string Area => "Norfair Lower";

        public West(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardLowerNorfairBoss };

            Locations = new List<Location> {
                new Location(this, 70, 0x8F8E6E, LocationType.Visible, "Missile (Gold Torizo)",
                    Logic switch {
                        Normal => items => items.CanUsePowerBombs() && items.SpaceJump && items.Super,
                        _ => new Requirement(items => items.CanUsePowerBombs() && items.SpaceJump && items.Varia && (
                            items.HiJump || items.Gravity ||
                            items.CanAccessNorfairLowerPortal() && items.Super &&
                                (items.CanFly() || items.CanSpringBallJump() || items.SpeedBooster)
                            )
                        )
                    }
                ),
                new Location(this, 71, 0x8F8E74, LocationType.Hidden, "Super Missile (Gold Torizo)", Logic switch {
                    Normal => items => items.CanDestroyBombWalls() && (items.Super || items.Charge) &&
                        (items.CanAccessNorfairLowerPortal() || items.SpaceJump && items.CanUsePowerBombs()),
                    _ => new Requirement(items => items.CanDestroyBombWalls() && items.Varia && (items.Super || items.Charge))
                }),
                new Location(this, 79, 0x8F9110, LocationType.Chozo, "Screw Attack", Logic switch {
                    Normal => items => items.CanDestroyBombWalls() && (items.SpaceJump && items.CanUsePowerBombs() || items.CanAccessNorfairLowerPortal()),
                    _ => new Requirement(items => items.CanDestroyBombWalls() && (items.Varia || items.CanAccessNorfairLowerPortal()))
                }),
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal =>
                    items.Varia && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.SpaceJump && items.Gravity &&
                            (items.CardNorfairL1 && items.SpeedBooster || items.CardNorfairL2 || items.Wave && items.SpeedBooster) ||
                            items.CanAccessNorfairLowerPortal() && items.CanDestroyBombWalls()),
                Medium =>
                    items.Varia && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.Gravity && (items.HiJump || items.SpaceJump) &&
                            (items.CardNorfairL1 && items.SpeedBooster || items.CardNorfairL2 || items.Wave && items.SpeedBooster) ||
                            items.CanAccessNorfairLowerPortal() && items.CanDestroyBombWalls()),
                _ =>
                    World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.Varia && (items.HiJump || items.Gravity) ||
                    items.CanAccessNorfairLowerPortal() && items.CanDestroyBombWalls()
            };
        }
    }
}
