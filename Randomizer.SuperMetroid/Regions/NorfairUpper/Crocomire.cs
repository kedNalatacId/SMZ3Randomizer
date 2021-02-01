using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.NorfairUpper {

    class Crocomire : Region {

        public override string Name => "Norfair Upper Crocomire";
        public override string Area => "Norfair Upper";

        public Crocomire(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardLowerNorfairL1, CardCrateriaBoss };

            Locations = new List<Location> {
                new Location(this, 52, 0x8F8BA4, LocationType.Visible, "Energy Tank, Crocomire", Major, Logic switch {
                    Normal => items => CanAccessCrocomire(items) && (items.HasEnergyReserves(1) || items.SpaceJump || items.Grapple),
                    _ => new Requirement(items => CanAccessCrocomire(items))
                }),
                // new Location(this, 52, "Energy Tank, Crocomire", Visible, Major, 0x78BA4, Logic switch

                new Location(this, 54, 0x8F8BC0, LocationType.Visible, "Missile (above Crocomire)", Minor, Logic switch {
                    Normal => items => items.SpaceJump || items.Grapple || items.HiJump && items.SpeedBooster,
                    Medium => items => (items.CanFly() || items.Grapple || items.HiJump &&
                        (items.SpeedBooster || items.Varia && items.Ice) && items.CanHeckRun()),
                    _ => new Requirement(items => (items.CanFly() || items.Grapple || items.HiJump &&
                        (items.SpeedBooster || items.CanSpringBallJump() || items.Varia && items.Ice)) && items.CanHellRun())
                }),
                // new Location(this, 54, "Missile (above Crocomire)", Visible, Minor, 0x78BC0, Logic switch

                new Location(this, 57, 0x8F8C04, LocationType.Visible, "Power Bomb (Crocomire)", Minor, Logic switch {
                    Normal => items => CanAccessCrocomire(items) && (items.CanFly() || items.HiJump || items.Grapple),
                    _ => new Requirement(items => CanAccessCrocomire(items))
                }),
                // new Location(this, 57, "Power Bomb (Crocomire)", Visible, Minor, 0x78C04, Logic switch

                new Location(this, 58, 0x8F8C14, LocationType.Visible, "Missile (below Crocomire)", Minor, Logic switch {
                    _ => new Requirement(items => CanAccessCrocomire(items) && items.Morph)
                }),
                // new Location(this, 58, "Missile (below Crocomire)", Visible, Minor, 0x78C14, Logic switch

                new Location(this, 59, 0x8F8C2A, LocationType.Visible, "Missile (Grappling Beam)", Minor, Logic switch {
                    Normal => items => CanAccessCrocomire(items) && items.Morph && (items.SpaceJump || items.SpeedBooster && items.CanUsePowerBombs()),
                    Medium => items => CanAccessCrocomire(items) && items.Morph && (items.CanFly() || items.SpeedBooster && items.CanUsePowerBombs()),
                    _ => new Requirement(items => CanAccessCrocomire(items) && (items.SpeedBooster || items.Morph && (items.CanFly() || items.Grapple)))
                }),
                // new Location(this, 59, "Missile (Grapple Beam)", Visible, Minor, 0x78C2A, Logic switch

                new Location(this, 60, 0x8F8C36, LocationType.Chozo, "Grappling Beam", Major, Logic switch {
                    Normal => items => CanAccessCrocomire(items) && items.Morph && (items.SpaceJump || items.SpeedBooster && items.CanUsePowerBombs()),
                    Medium => items => CanAccessCrocomire(items) && items.Morph && (items.CanFly() || items.SpeedBooster && items.CanUsePowerBombs()),
                    _ => new Requirement(items => CanAccessCrocomire(items) && (items.CanFly() || items.Morph || items.Grapple || items.HiJump && items.SpeedBooster))
                }),
                // new Location(this, 60, "Grapple Beam", Chozo, Major, 0x78C36, Logic switch
            };
        }

        bool CanAccessCrocomire(Progression items) {
            return Config.UseKeycards ? items.CardNorfairBoss : items.Super;
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal => (
                        (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.Morph
                    ) &&
                    items.Varia && (
                        /* Ice Beam -> Croc Speedway */
                        (Config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanUsePowerBombs() && items.SpeedBooster ||
                        /* Frog Speedway */
                        items.SpeedBooster && items.Wave ||
                        /* Cathedral -> through the floor or Vulcano */
                        items.CanOpenRedDoors() && (Config.UseKeycards ? items.CardNorfairL2 : items.Super) &&
                            (items.CanFly() || items.HiJump) && (items.CanPassBombPassages() || items.Gravity && items.Morph) && items.Wave
                    ),
                Medium => (
                        (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.Morph
                    ) && (
                        /* Ice Beam -> Croc Speedway */
                        (Config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanUsePowerBombs() &&
                            items.SpeedBooster && (items.HasEnergyReserves(5) || items.Varia) ||
                        /* Frog Speedway */
                        items.SpeedBooster && (items.HasEnergyReserves(4) || items.Varia) &&
                            (items.Super || items.Wave) /* Blue Gate */ ||
                        /* Cathedral -> through the floor or Vulcano */
                        items.CanHeckRun() && items.CanOpenRedDoors() && (Config.UseKeycards ? items.CardNorfairL2 : items.Super) &&
                            (items.CanFly() || items.HiJump || items.SpeedBooster || items.Varia && items.Ice) &&
                            (items.CanPassBombPassages() || items.Varia && items.Morph) &&
                            (items.Super || items.Wave) /* Blue Gate */
                    ),
                _ => (
                        (items.CanDestroyBombWalls() || items.SpeedBooster) && items.Super && items.Morph
                    ) && (
                        /* Ice Beam -> Croc Speedway */
                        (Config.UseKeycards ? items.CardNorfairL1 : items.Super) && items.CanUsePowerBombs() &&
                            items.SpeedBooster && (items.HasEnergyReserves(3) || items.Varia) ||
                        /* Frog Speedway */
                        items.SpeedBooster && (items.HasEnergyReserves(2) || items.Varia) &&
                            (items.Missile || items.Super || items.Wave) /* Blue Gate */ ||
                        /* Cathedral -> through the floor or Vulcano */
                        items.CanHellRun() && items.CanOpenRedDoors() && (Config.UseKeycards ? items.CardNorfairL2 : items.Super) &&
                            (items.CanFly() || items.HiJump || items.SpeedBooster || items.CanSpringBallJump() || items.Varia && items.Ice) &&
                            (items.CanPassBombPassages() || items.Varia && items.Morph) &&
                            (items.Missile || items.Super || items.Wave) /* Blue Gate */
                    )
            };
        }
    }
}
