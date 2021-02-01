using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.Brinstar {

    class Blue : Region {

        public override string Name => "Blue Brinstar";
        public override string Area => "Brinstar";

        public Blue(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardCrateriaL1, CardCrateriaBoss };

            Locations = new List<Location> {
                new Location(this, 26, 0x8F86EC, LocationType.Visible, "Morphing Ball", Major),
                // new Location(this, 26, "Morphing Ball", Visible, Major, 0x786DE),

                new Location(this, 27, 0x8F874C, LocationType.Visible, "Power Bomb (blue Brinstar)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanUsePowerBombs())
                }),
                // new Location(this, 27, "Power Bomb (blue Brinstar)", Visible, Minor, 0x7874C, Logic switch

                new Location(this, 28, 0x8F8798, LocationType.Visible, "Missile (blue Brinstar middle)", Minor, Logic switch {
                    _ => new Requirement(items => items.CardBrinstarL1 && items.Morph)
                }),
                // new Location(this, 28, "Missile (blue Brinstar middle)", Visible, Minor, 0x78798, Logic switch

                new Location(this, 29, 0x8F879E, LocationType.Hidden, "Energy Tank, Brinstar Ceiling", Major, Logic switch {
                    Normal => items => items.CardBrinstarL1 && (items.CanFly() || items.HiJump ||items.SpeedBooster || items.Ice),
                    _ => new Requirement(items => items.CardBrinstarL1)
                }),
                // new Location(this, 29, "Energy Tank, Brinstar Ceiling", Hidden, Major, 0x7879E, Logic switch

                new Location(this, 34, 0x8F8802, LocationType.Chozo, "Missile (blue Brinstar bottom)", Minor, Logic switch {
                    _ => new Requirement(items => items.Morph)
                }),

                new Location(this, 36, 0x8F8836, LocationType.Visible, "Missile (blue Brinstar top)", Minor, Logic switch {
                    _ => new Requirement(items => items.CardBrinstarL1 && items.CanUsePowerBombs())
                }),
                // new Location(this, 36, "Missile (blue Brinstar top)", Visible, Minor, 0x78836, Logic switch

                new Location(this, 37, 0x8F883C, LocationType.Hidden, "Missile (blue Brinstar behind missile)", Minor, Logic switch {
                    _ => new Requirement(items => items.CardBrinstarL1 && items.CanUsePowerBombs())
                }),
                // new Location(this, 37, "Missile (blue Brinstar behind missile)", Hidden, Minor, 0x7883C, Logic switch
            };
        }
    }
}
