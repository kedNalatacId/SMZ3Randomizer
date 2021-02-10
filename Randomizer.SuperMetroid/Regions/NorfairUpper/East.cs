using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.NorfairUpper {
    class East : Region {
        public override string Name => "Norfair Upper East";
        public override string Area => "Norfair Upper";

        public East(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardNorfairBoss };

            Locations = new List<Location> {
                new Location(this, 61, 0x8F8C3E, LocationType.Chozo, "Reserve Tank, Norfair", Major, Logic switch {
                    Normal => items => items.CardNorfairL2 && items.Morph && (
                        items.SpaceJump ||
                        items.Grapple && (items.SpeedBooster || items.CanPassBombPassages()) ||
                        items.HiJump || items.Ice
                    ),
                    _ => new Requirement(items => items.CardNorfairL2 && items.Morph && items.Super)
                }),
                new Location(this, 62, 0x8F8C44, LocationType.Hidden, "Missile (Norfair Reserve Tank)", Minor, Logic switch {
                    Normal => items => items.CardNorfairL2 && items.Morph && (
                        items.SpaceJump ||
                        items.Grapple && (items.SpeedBooster || items.CanPassBombPassages()) ||
                        items.HiJump || items.Ice
                    ),
                    _ => new Requirement(items => items.CardNorfairL2 && items.Morph && items.Super)
                }),
                new Location(this, 63, 0x8F8C52, LocationType.Visible, "Missile (bubble Norfair green door)", Minor, Logic switch {
                    Normal => items => items.CardNorfairL2 && (
                        items.SpaceJump ||
                        items.Grapple && items.Morph && (items.SpeedBooster || items.CanPassBombPassages()) ||
                        items.HiJump || items.Ice
                    ),
                    _ => new Requirement(items => items.CardNorfairL2 && items.Super)
                }),
                new Location(this, 64, 0x8F8C66, LocationType.Visible, "Missile (bubble Norfair)", Minor, Logic switch {
                    _ => new Requirement(items => items.CardNorfairL2)
                }),
                new Location(this, 65, 0x8F8C74, LocationType.Hidden, "Missile (Speed Booster)", Minor, Logic switch {
                    Normal => items => items.CardNorfairL2 && (
                        items.SpaceJump ||
                        items.Morph && (items.SpeedBooster || items.CanPassBombPassages()) ||
                        items.HiJump || items.Ice
                    ),
                    _ => new Requirement(items => items.CardNorfairL2 && items.Super)
                }),
                new Location(this, 66, 0x8F8C82, LocationType.Chozo, "Speed Booster", Major, Logic switch {
                    Normal => items => items.CardNorfairL2 && (
                        items.SpaceJump ||
                        items.Morph && (items.SpeedBooster || items.CanPassBombPassages()) ||
                        items.HiJump || items.Ice
                    ),
                    _ => new Requirement(items => items.CardNorfairL2 && items.Super)
                }),
                new Location(this, 67, 0x8F8CBC, LocationType.Visible, "Missile (Wave Beam)", Minor, Logic switch {
                    Normal => items => items.CardNorfairL2 && (
                        items.SpaceJump ||
                        items.Morph && (items.SpeedBooster || items.CanPassBombPassages()) ||
                        items.HiJump || items.Ice
                    ) || (
                        items.SpeedBooster && items.Wave && items.Morph && items.Super
                    ),
                    _ => new Requirement(items => items.CanOpenRedDoors() && (items.CardNorfairL2 || items.Varia))
                }),
                new Location(this, 68, 0x8F8CCA, LocationType.Chozo, "Wave Beam", Major, Logic switch {
                    Normal => items => items.CardNorfairL2 && (
                        items.SpaceJump ||
                        items.Morph && (items.SpeedBooster || items.CanPassBombPassages()) ||
                        items.HiJump || items.Ice
                    ) || (
                        items.SpeedBooster && items.Wave && items.Morph && items.Super
                    ),
                    _ => new Requirement(items => items.CanOpenRedDoors() && (items.CardNorfairL2 || items.Varia) &&
                        (items.Morph || items.Grapple || items.HiJump && items.Varia || items.SpaceJump))
                }),
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal => (
                        (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.Morph
                    ) && items.Varia && items.Super && (
                        /* Cathedral */
                        items.CanOpenRedDoors() && (Config.UseKeycards ? items.CardNorfairL2 : items.Super) &&
                            (items.CanFly() || items.HiJump) ||
                        /* Frog Speedway */
                        items.SpeedBooster && (items.CardNorfairL2 || items.Wave) && items.CanUsePowerBombs()
                    ),
                Medium => (
                        // Norfair Main Street Access
                        (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.Morph
                    ) &&
                    // "Heck" Run and Green Door
                    items.CanHeckRun() && (
                        items.CanOpenRedDoors() && (Config.UseKeycards ? items.CardNorfairL2 : items.Super) && (
                            /* Cathedral */
                            items.CanFly() || items.HiJump || items.SpeedBooster || items.Varia && items.Ice
                        ) ||
                        /* Frog Speedway */
                        items.SpeedBooster && (items.CardNorfairL2 || items.Super || items.Wave) && items.CanUsePowerBombs()
                    ),
                _ => (
                        // Norfair Main Street Access
                        (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.Morph
                    ) &&
                    // Hell Run and Green Door
                    items.CanHellRun() && (
                        /* Cathedral */
                        items.CanOpenRedDoors() && (Config.UseKeycards ? items.CardNorfairL2 : items.Super) && (
                            items.CanFly() || items.HiJump || items.SpeedBooster || items.CanSpringBallJump() || items.Varia && items.Ice
                        ) ||
                        /* Frog Speedway */
                        items.SpeedBooster && (items.CardNorfairL2 || items.Missile || items.Super || items.Wave) && items.CanUsePowerBombs()
                    ),
            };
        }
    }
}
