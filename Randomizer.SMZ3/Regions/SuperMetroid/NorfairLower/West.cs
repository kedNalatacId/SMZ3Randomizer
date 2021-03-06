﻿using System.Collections.Generic;
using static Randomizer.SMZ3.SMLogic;
using static Randomizer.SMZ3.ItemType;

namespace Randomizer.SMZ3.Regions.SuperMetroid.NorfairLower {
    class West : SMRegion {
        public override string Name => "Norfair Lower West";
        public override string Area => "Norfair Lower";

        public West(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardLowerNorfairL1 };

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
                new Location(this, 73, 0x8F8F30, LocationType.Visible, "Missile (Mickey Mouse room)", Logic switch {
                    Normal => items => items.SpaceJump && items.Morph && items.Super &&
                        /*Exit to Upper Norfair*/
                        (
                            (items.CardLowerNorfairL1 || items.Gravity /*Vanilla or Reverse Lava Dive*/) && items.CardNorfairL2 /*Bubble Mountain*/ ||
                            items.Gravity && items.Wave /* Volcano Room and Blue Gate */ && (items.Grapple || items.SpaceJump) /*Spikey Acid Snakes and Croc Escape*/ ||
                            /*Exit via GT fight and Portal*/
                            (items.CanUsePowerBombs() && items.SpaceJump && items.Super)
                        ),
                    Medium => items => 
                        items.Morph && items.Varia && items.Super && (
                            items.CanFly() || items.SpeedBooster && (items.HiJump && items.CanUsePowerBombs() || items.Charge && items.Ice)
                        ) &&
                        /*Exit to Upper Norfair*/
                        (items.CardNorfairL2 || (items.SpeedBooster || items.CanFly() || items.Grapple || items.HiJump && items.Ice)) ||
                        /*Return to Portal*/
                        (items.CanUsePowerBombs() && items.CanFly() && (items.Super || items.Charge)),
                    _ => new Requirement(items => 
                        items.Morph && items.Varia && items.Super && (
                            (items.CanFly() || items.CanSpringBallJump() || items.SpeedBooster && (items.HiJump && items.CanUsePowerBombs() || items.Charge && items.Ice)
                        ) &&
                        /*Exit to Upper Norfair*/
                        (items.CardNorfairL2 || (items.SpeedBooster || items.CanFly() || items.Grapple || items.HiJump && (items.CanSpringBallJump() || items.Ice))) ||
                        /*Return to Portal*/
                        items.CanUsePowerBombs()))
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
