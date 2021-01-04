﻿using System.Collections.Generic;

namespace Randomizer.SMZ3.Regions.Zelda.LightWorld.DeathMountain {

    class East : Z3Region {

        public override string Name => "Light World Death Mountain East";
        public override string Area => "Death Mountain";

        public East(World world, Config config) : base(world, config) {
            Locations = new List<Location> {
                new Location(this, 256+4, 0x308141, LocationType.Regular, "Floating Island",
                    items => items.Mirror && items.MoonPearl && items.CanLiftHeavy()),
                new Location(this, 256+5, 0x1E9BF, LocationType.Regular, "Spiral Cave"),
                new Location(this, 256+6, 0x1EB39, LocationType.Regular, "Paradox Cave Upper - Left"),
                new Location(this, 256+7, 0x1EB3C, LocationType.Regular, "Paradox Cave Upper - Right"),
                new Location(this, 256+8, 0x1EB2A, LocationType.Regular, "Paradox Cave Lower - Far Left"),
                new Location(this, 256+9, 0x1EB2D, LocationType.Regular, "Paradox Cave Lower - Left"),
                new Location(this, 256+10, 0x1EB36, LocationType.Regular, "Paradox Cave Lower - Middle"),
                new Location(this, 256+11, 0x1EB30, LocationType.Regular, "Paradox Cave Lower - Right"),
                new Location(this, 256+12, 0x1EB33, LocationType.Regular, "Paradox Cave Lower - Far Right"),
                new Location(this, 256+13, 0x1E9C5, LocationType.Regular, "Mimic Cave",
                    items => items.Mirror && items.KeyTR >= 2 && World.CanEnter("Turtle Rock", items)),
            };
        }

        public override bool CanEnter(Progression items) {
            return World.CanEnter("Light World Death Mountain West", items) && (
                items.Hammer && items.Mirror ||
                items.Hookshot
            );
        }
    }
}
