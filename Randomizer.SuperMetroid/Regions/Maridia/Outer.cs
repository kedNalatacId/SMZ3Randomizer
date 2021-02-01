using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.Maridia {

    class Outer : Region {

        public override string Name => "Maridia Outer";
        public override string Area => "Maridia";

        public Outer(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardMaridiaL1 };

            Locations = new List<Location> {
                new Location(this, 136, 0x8FC437, LocationType.Visible, "Missile (green Maridia shinespark)", Minor, Logic switch {
                    _ => new Requirement(items => items.Gravity && items.SpeedBooster)
                }),
                // new Location(this, 136, "Missile (green Maridia shinespark)", Visible, Minor, 0x7C437, Logic switch

                new Location(this, 137, 0x8FC43D, LocationType.Visible, "Super Missile (green Maridia)", Minor),
                // new Location(this, 137, "Super Missile (green Maridia)", Visible, Minor, 0x7C43D),

                new Location(this, 138, 0x8FC47D, LocationType.Visible, "Energy Tank, Mama turtle", Major, Logic switch {
                    Normal => items => items.Gravity && items.CanOpenRedDoors() && (items.SpaceJump || items.SpeedBooster || items.Grapple),
                    Medium => items => items.Gravity && items.CanOpenRedDoors() && (items.CanFly() || items.SpeedBooster || items.Grapple),
                    _ => new Requirement(items => items.CanOpenRedDoors() && (
                        items.CanFly() || items.SpeedBooster || items.Grapple ||
                        items.CanSpringBallJump() && (items.Gravity || items.HiJump)
                    ))
                }),
                // new Location(this, 138, "Energy Tank, Mama turtle", Visible, Major, 0x7C47D, Logic switch

                new Location(this, 139, 0x8FC483, LocationType.Hidden, "Missile (green Maridia tatori)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanOpenRedDoors())
                }),
                // new Location(this, 139, "Missile (green Maridia tatori)", Hidden, Minor, 0x7C483),
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal => items.Gravity && World.CanEnter("Norfair Upper West", items) && items.CanUsePowerBombs(),
                _ =>
                    World.CanEnter("Norfair Upper West", items) && items.CanUsePowerBombs() &&
                        (items.Gravity || items.HiJump && (items.CanSpringBallJump() || items.Ice))
            };
        }
    }
}
