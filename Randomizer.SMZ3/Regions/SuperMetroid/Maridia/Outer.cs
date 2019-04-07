﻿using System.Collections.Generic;
using static Randomizer.SMZ3.Logic;

namespace Randomizer.SMZ3.Regions.SuperMetroid.Maridia {

    class Outer : Region {

        public override string Name => "Maridia Outer";
        public override string Area => "Maridia";

        public Outer(World world, Config config) : base(world, config) {
            Locations = new List<Location> {
                new Location(this, 136, 0x7C437, LocationType.Visible, "Missile (green Maridia shinespark)", Config.Logic switch {
                    Casual => items => items.SpeedBooster,
                    _ => new Requirement(items => items.Gravity && items.SpeedBooster)
                }),
                new Location(this, 137, 0x7C43D, LocationType.Visible, "Super Missile (green Maridia)"),
                new Location(this, 138, 0x7C47D, LocationType.Visible, "Energy Tank, Mama turtle", Config.Logic switch {
                    Casual => items => items.CanFly() || items.SpeedBooster || items.Grapple,
                    _ => new Requirement(items => items.CanFly() || items.SpeedBooster || items.Grapple ||
                        items.CanSpringBallJump() && (items.Gravity || items.HiJump))
                }),
                new Location(this, 139, 0x7C483, LocationType.Hidden, "Missile (green Maridia tatori)"),
            };
        }

        public override bool CanEnter(Progression items) {
            return Config.Logic switch {
                Casual => (
                        World.CanEnter("Norfair Upper West", items) && items.CanUsePowerBombs() ||
                        items.CanAccessMaridiaPortal(World)
                    ) && items.Gravity,
                _ =>
                    World.CanEnter("Norfair Upper West", items) && items.CanUsePowerBombs() &&
                    (items.Gravity || items.HiJump && (items.CanSpringBallJump() || items.Ice))
                    || items.CanAccessMaridiaPortal(World)
            };
        }

    }

}
