﻿using System.Collections.Generic;
using static Randomizer.SMZ3.SMLogic;
using static Randomizer.SMZ3.ItemType;

namespace Randomizer.SMZ3.Regions.SuperMetroid.Crateria {

    class East : SMRegion {

        public override string Name => "Crateria East";
        public override string Area => "Crateria";

        public East(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardWreckedShipL1 };
            Locations = new List<Location> {
                new Location(this, 1, 0x8F81E8, LocationType.Visible, "Missile (outside Wrecked Ship bottom)", Logic switch {
                    Normal => items => items.Morph && (
                        items.SpeedBooster || items.Grapple || items.SpaceJump ||
                        items.Gravity && (items.CanFly() || items.HiJump) ||
                        World.CanEnter("Wrecked Ship", items)),
                    Medium => items => items.Morph && (
                        items.SpeedBooster || items.Grapple || items.SpaceJump ||
                        items.Gravity || World.CanEnter("Wrecked Ship", items)),
                    _ => new Requirement(items => items.Morph)
                }),
                new Location(this, 2, 0x8F81EE, LocationType.Hidden, "Missile (outside Wrecked Ship top)", Logic switch {
                    _ => new Requirement(items => World.CanEnter("Wrecked Ship", items) && (!Config.UseKeycards || items.CardWreckedShipBoss) && items.CanPassBombPassages())
                }),
                new Location(this, 3, 0x8F81F4, LocationType.Visible, "Missile (outside Wrecked Ship middle)", Logic switch {
                    _ => new Requirement(items => World.CanEnter("Wrecked Ship", items) && (!Config.UseKeycards || items.CardWreckedShipBoss) && items.CanPassBombPassages())
                }),
                new Location(this, 4, 0x8F8248, LocationType.Visible, "Missile (Crateria moat)", Logic switch {
                    _ => new Requirement(items => true)
                }),
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal =>
                    ((Config.UseKeycards && items.CardCrateriaL2) || (!Config.UseKeycards && items.CanUsePowerBombs())) && items.Super ||
                    ((Config.UseKeycards && items.CardCrateriaL2) || (!Config.UseKeycards && items.CanUsePowerBombs())) && items.CanAccessNorfairUpperPortal() && items.CanUsePowerBombs() && (items.Ice || items.HiJump || items.SpaceJump) ||
                    items.CanAccessMaridiaPortal(World) && items.Gravity && items.Super &&
                        ((items.CanDestroyBombWalls() && items.CardMaridiaL2) || World.Locations.Get("Space Jump").Available(items)),
                Medium =>
                    ((Config.UseKeycards && items.CardCrateriaL2) || (!Config.UseKeycards && items.CanUsePowerBombs())) && items.Super ||
                    ((Config.UseKeycards && items.CardCrateriaL2) || (!Config.UseKeycards && items.CanUsePowerBombs())) && items.CanAccessNorfairUpperPortal() && items.CanUsePowerBombs() && (items.Ice || items.HiJump || items.CanFly()) ||
                    items.CanAccessMaridiaPortal(World) && items.Gravity && items.Super &&
                        ((items.CanDestroyBombWalls() && items.CardMaridiaL2) || World.Locations.Get("Space Jump").Available(items)),
                _ =>
                    ((Config.UseKeycards && items.CardCrateriaL2) || (!Config.UseKeycards && items.CanUsePowerBombs())) && items.Super ||
                    ((Config.UseKeycards && items.CardCrateriaL2) || (!Config.UseKeycards && items.CanUsePowerBombs())) && items.CanAccessNorfairUpperPortal() && items.CanUsePowerBombs() && (items.Ice || items.HiJump || items.CanFly() || items.CanSpringBallJump()) ||
                    items.CanAccessMaridiaPortal(World) && (
                        items.CardMaridiaL2 && items.Super && items.HiJump && items.CanPassBombPassages() ||
                        items.Gravity && (items.CanDestroyBombWalls() || World.Locations.Get("Space Jump").Available(items))
                    )
            };
        }

    }

}
