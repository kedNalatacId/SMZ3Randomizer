using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.Crateria {
    class East : Region {
        public override string Name => "Crateria East";
        public override string Area => "Crateria";

        public East(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardWreckedShipBoss };

            Locations = new List<Location> {
                new Location(this, 1, 0x8F81E8, LocationType.Visible, "Missile (outside Wrecked Ship bottom)", Minor, Logic switch {
                    Normal => items => items.Morph && (
                        items.Grapple || items.SpaceJump || (items.Gravity && items.HiJump) || World.CanEnter("Wrecked Ship", items)),
                    Medium => items => items.Morph && (
                        items.SpeedBooster || items.Grapple || items.SpaceJump || items.Gravity || World.CanEnter("Wrecked Ship", items)),
                    _ => new Requirement(items => items.Morph)
                }),
                new Location(this, 2, 0x8F81EE, LocationType.Hidden, "Missile (outside Wrecked Ship top)", Minor, Logic switch {
                    _ => new Requirement(items => World.CanEnter("Wrecked Ship", items) && (!Config.UseKeycards || items.CardWreckedShipBoss) && items.CanPassBombPassages())
                }),
                new Location(this, 3, 0x8F81F4, LocationType.Visible, "Missile (outside Wrecked Ship middle)", Minor, Logic switch {
                    _ => new Requirement(items => World.CanEnter("Wrecked Ship", items) && (!Config.UseKeycards || items.CardWreckedShipBoss) && items.CanPassBombPassages())
                }),
                new Location(this, 4, 0x8F8248, LocationType.Visible, "Missile (Crateria moat)", Minor, Logic switch {
                    Normal => items => items.SpaceJump || items.SpeedBooster || items.Grapple,
                    _ => new Requirement(items => true)
                }),
            };
        }

        public override bool CanEnter(Progression items) {
            return items.Super && (Config.UseKeycards ? items.CardCrateriaL2 : items.CanUsePowerBombs());
        }
    }
}
