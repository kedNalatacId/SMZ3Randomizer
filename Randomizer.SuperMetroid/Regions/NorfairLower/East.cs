using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.NorfairLower {

    class East : Region {

        public override string Name => "Norfair Lower East";
        public override string Area => "Norfair Lower";

        public East(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardLowerNorfairBoss };

            Locations = new List<Location> {
                new Location(this, 73, 0x8F8F30, LocationType.Visible, "Missile (Mickey Mouse room)", Minor, Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.Morph && items.CanDestroyBombWalls()),
                }),
                // new Location(this, 73, "Missile (Mickey Mouse room)", Visible, Minor, 0x78F30, Logic switch

                new Location(this, 74, 0x8F8FCA, LocationType.Visible, "Missile (lower Norfair above fire flea room)", Minor, Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1)
                }),
                // new Location(this, 74, "Missile (lower Norfair above fire flea room)", Visible, Minor, 0x78FCA),

                new Location(this, 75, 0x8F8FD2, LocationType.Visible, "Power Bomb (lower Norfair above fire flea room)", Minor, Logic switch {
                    Normal => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1),
                    _ => items => CanExit(items) && items.CardLowerNorfairL1 && items.CanPassBombPassages()
                }),
                // new Location(this, 75, "Power Bomb (lower Norfair above fire flea room)", Visible, Minor, 0x78FD2, Logic switch

                new Location(this, 76, 0x8F90C0, LocationType.Visible, "Power Bomb (Power Bombs of shame)", Minor, Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1 && items.CanUsePowerBombs())
                }),
                // new Location(this, 76, "Power Bomb (Power Bombs of shame)", Visible, Minor, 0x790C0, Logic switch

                new Location(this, 77, 0x8F9100, LocationType.Visible, "Missile (lower Norfair near Wave Beam)", Minor, Logic switch {
                    Normal => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1),
                    _ => items => CanExit(items) && items.CardLowerNorfairL1 && items.Morph && items.CanDestroyBombWalls()
                }),
                // new Location(this, 77, "Missile (lower Norfair near Wave Beam)", Visible, Minor, 0x79100, Logic switch

                new Location(this, 78, 0x8F9108, LocationType.Hidden, "Energy Tank, Ridley", Major, Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1 && items.CardLowerNorfairBoss && items.CanUsePowerBombs() && items.Super)
                }),
                // new Location(this, 78, "Energy Tank, Ridley", Hidden, Major, 0x79108, Logic switch

                new Location(this, 80, 0x8F9184, LocationType.Visible, "Energy Tank, Firefleas", Major, Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1)
                })
                // new Location(this, 80, "Energy Tank, Firefleas", Visible, Major, 0x79184)
            };
        }

        bool CanExit(Progression items) {
            return Logic switch {
                Normal =>
                    /* Varia and Gravity for Reverse Lava Dive if missing LN-1 card */
                    (items.CardLowerNorfairL1 || items.Gravity) && items.CardNorfairL2 ||
                    items.Gravity && items.Wave /* Blue Gate */ && items.SpeedBooster,
                _ =>
                    items.CardNorfairL2 ||
                    /* Without UN-2 we need to check for Morph due to Reverse Amphitheater */
                    items.Morph && (items.Missile || items.Super || items.Wave /* Blue Gate */) && items.SpeedBooster,
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Hard =>
                    items.Varia && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && (items.HiJump || items.Gravity)
                    ) &&
                    (items.CanFly() || items.HiJump || items.CanSpringBallJump() || items.Ice && items.Charge) &&
                    (items.CanPassBombPassages() || items.ScrewAttack && items.SpaceJump) &&
                    (items.Morph || items.HasEnergyReserves(5)),
                _ => items.Varia && World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.SpaceJump && items.Gravity
            };
        }

        public bool CanComplete(Progression items) {
            return Locations.Get("Energy Tank, Ridley").Available(items);
        }
    }
}
