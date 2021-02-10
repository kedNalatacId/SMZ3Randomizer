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
            RegionItems = new[] { CardLowerNorfairBoss, CardCrateriaBoss };

            Locations = new List<Location> {
                new Location(this, 74, 0x8F8FCA, LocationType.Visible, "Missile (lower Norfair above fire flea room)", Minor, Logic switch {
                    _ => new Requirement(items => CanExit(items))
                }),
                new Location(this, 75, 0x8F8FD2, LocationType.Visible, "Power Bomb (lower Norfair above fire flea room)", Minor, Logic switch {
                    Normal => items => CanExit(items),
                    _ => new Requirement(items => CanExit(items) && items.CanPassBombPassages())
                }),
                new Location(this, 76, 0x8F90C0, LocationType.Visible, "Power Bomb (Power Bombs of shame)", Minor, Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CanUsePowerBombs())
                }),
                new Location(this, 77, 0x8F9100, LocationType.Visible, "Missile (lower Norfair near Wave Beam)", Minor, Logic switch {
                    Normal => items => CanExit(items),
                    _ => new Requirement(items => CanExit(items) && items.Morph && items.CanDestroyBombWalls())
                }),
                new Location(this, 78, 0x8F9108, LocationType.Hidden, "Energy Tank, Ridley", Major, Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CardLowerNorfairBoss && items.CanUsePowerBombs() && items.Super)
                }),
                new Location(this, 80, 0x8F9184, LocationType.Visible, "Energy Tank, Firefleas", Major, Logic switch {
                    _ => new Requirement(items => CanExit(items))
                })
            };
        }

        bool CanExit(Progression items) {
            return Logic switch {
                Normal => items.CardNorfairL2 /*Bubble Mountain*/ ||
                    /* Volcano Room and Blue Gate */
                    items.Gravity && items.CanPassWaveGates(World) &&
                    /*Spikey Acid Snakes and Croc Escape*/
                    (items.Grapple || items.SpaceJump),
                Medium => (
                    /*Vanilla LN Escape*/
                    items.Morph && (
                        items.CardNorfairL2 /*Bubble Mountain*/ || items.CanPassWaveGates(World) && 
                        /*Frog Speedway or Croc Escape*/
                        (items.SpeedBooster || items.CanFly() || items.Grapple || items.HiJump && items.Ice)
                    ) ||
                    /*Reverse Amphitheater*/
                    items.HasEnergyReserves(7)
                ),
                _ => (
                    /*Vanilla LN Escape*/
                    items.Morph && (
                        items.CardNorfairL2 /*Bubble Mountain*/ || items.CanPassWaveGates(World) && 
                        /*Frog Speedway or Croc Escape*/
                        (items.SpeedBooster || items.CanFly() || items.Grapple || items.HiJump && (items.CanSpringBallJump() || items.Ice))
                    ) ||
                    /*Reverse Amphitheater*/
                    items.HasEnergyReserves(5)
                )
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal =>
                    items.Varia && items.CardLowerNorfairL1 && items.SpaceJump && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.Gravity
                    ),
                Medium =>
                    items.Varia && items.CardLowerNorfairL1 && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.SpaceJump && items.Gravity
                    ) &&
                    (items.CanFly() || items.HiJump || items.Ice && items.Charge) &&
                    (items.CanPassBombPassages() || items.ScrewAttack && items.SpaceJump),
                _ =>
                    items.Varia && items.CardLowerNorfairL1 && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && (items.HiJump || items.Gravity)
                    ) &&
                    (items.CanFly() || items.HiJump || items.CanSpringBallJump() || items.Ice && items.Charge) &&
                    (items.CanPassBombPassages() || items.ScrewAttack && items.SpaceJump)
            };
        }

        public bool CanComplete(Progression items) {
            return Locations.Get("Energy Tank, Ridley").Available(items);
        }
    }
}
