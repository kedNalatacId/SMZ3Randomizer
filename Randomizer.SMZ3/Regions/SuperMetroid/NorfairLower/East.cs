﻿using System.Collections.Generic;
using static Randomizer.SMZ3.SMLogic;
using static Randomizer.SMZ3.ItemType;

namespace Randomizer.SMZ3.Regions.SuperMetroid.NorfairLower {

    class East : SMRegion, IReward {

        public override string Name => "Norfair Lower East";
        public override string Area => "Norfair Lower";

        public RewardType Reward { get; set; } = RewardType.GoldenFourBoss;

        public East(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardLowerNorfairBoss };

            Locations = new List<Location> {
                new Location(this, 73, 0x8F8F30, LocationType.Visible, "Missile (Mickey Mouse room)", Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.Morph && items.CanDestroyBombWalls()),
                }),
                new Location(this, 74, 0x8F8FCA, LocationType.Visible, "Missile (lower Norfair above fire flea room)", Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1)
                }),
                new Location(this, 75, 0x8F8FD2, LocationType.Visible, "Power Bomb (lower Norfair above fire flea room)", Logic switch {
                    Normal => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1),
                    _ => items => CanExit(items) && items.CardLowerNorfairL1 && items.CanPassBombPassages()
                }),
                new Location(this, 76, 0x8F90C0, LocationType.Visible, "Power Bomb (Power Bombs of shame)", Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1 && items.CanUsePowerBombs())
                }),
                new Location(this, 77, 0x8F9100, LocationType.Visible, "Missile (lower Norfair near Wave Beam)", Logic switch {
                    Normal => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1),
                    _ => items => CanExit(items) && items.CardLowerNorfairL1 && items.Morph && items.CanDestroyBombWalls()
                }),
                new Location(this, 78, 0x8F9108, LocationType.Hidden, "Energy Tank, Ridley", Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1 && items.CardLowerNorfairBoss && items.CanUsePowerBombs() && items.Super)
                }),
                new Location(this, 80, 0x8F9184, LocationType.Visible, "Energy Tank, Firefleas", Logic switch {
                    _ => new Requirement(items => CanExit(items) && items.CardLowerNorfairL1)
                })
            };
        }

        bool CanExit(Progression items) {
            return CanExitThroughNorfair(items) || CanExitThroughPortal(items);
        }

        bool CanExitThroughNorfair(Progression items) {
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

        // Todo: no guarantee of PBs on Hard can block green gate by falling at Mickey Mouse (but "solved" through PB front fill)
        bool CanExitThroughPortal(Progression items) {
            return Logic switch {
                /* Through Acid Statue Room, then fight GT */
                Normal => items.CanUsePowerBombs() && items.SpaceJump && (items.Super || items.Charge),
                _ => /* GGG directly back to the LN portal */
                    items.Super /* Green Gate */ ||
                    items.CanUsePowerBombs() && items.SpaceJump && (items.Super || items.Charge),
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal =>
                    items.Varia && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.SpaceJump && items.Gravity ||
                        items.CanAccessNorfairLowerPortal() && items.CanDestroyBombWalls() && items.Super && items.CanUsePowerBombs() && items.CanFly()
                    ),
                Medium =>
                    items.Varia && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && items.SpaceJump && items.Gravity ||
                        items.CanAccessNorfairLowerPortal() && items.CanDestroyBombWalls() && items.Super && items.CanUsePowerBombs() && items.CanFly()
                    ),
                _ =>
                    items.Varia && (
                        World.CanEnter("Norfair Upper East", items) && items.CanUsePowerBombs() && (items.HiJump || items.Gravity) ||
                        items.CanAccessNorfairLowerPortal() && items.CanDestroyBombWalls() && items.Super && (items.CanFly() || items.CanSpringBallJump() || items.SpeedBooster)
                    ) &&
                    (items.CanFly() || items.HiJump || items.CanSpringBallJump() || items.Ice && items.Charge) &&
                    (items.CanPassBombPassages() || items.ScrewAttack && items.SpaceJump) &&
                    (items.Morph || items.HasEnergyReserves(5))
            };
        }

        public bool CanComplete(Progression items) {
            return Locations.Get("Energy Tank, Ridley").Available(items);
        }
    }
}
