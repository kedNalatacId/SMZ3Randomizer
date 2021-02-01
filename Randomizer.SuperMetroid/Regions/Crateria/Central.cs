using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.Crateria {

    class Central : Region {

        public override string Name => "Crateria Central";
        public override string Area => "Crateria";

        public Central(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardCrateriaBoss };

            Locations = new List<Location> {
                new Location(this, 0, 0x8F81CC, LocationType.Visible, "Power Bomb (Crateria surface)", Minor, Logic switch {
                    _ => new Requirement(items => (config.UseKeycards ? items.CardCrateriaL1 : items.CanUsePowerBombs()) && (items.SpeedBooster || items.CanFly()))
                }),
                // new Location(this, 0, "Power Bomb (Crateria surface)", Visible, Minor, 0x781CC, Logic switch

                new Location(this, 12, 0x8F8486, LocationType.Visible, "Missile (Crateria middle)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanPassBombPassages())
                }),
                // new Location(this, 12, "Missile (Crateria middle)", Visible, Minor, 0x78486, Logic switch

                new Location(this, 6, 0x8F83EE, LocationType.Visible, "Missile (Crateria bottom)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanDestroyBombWalls())
                }),
                // new Location(this, 6, "Missile (Crateria bottom)", Visible, Minor, 0x783EE, Logic switch

                // QoL: add Ice Beam to Normal; add either more ETanks or a viable exit strat
                new Location(this, 11, 0x8F8478, LocationType.Visible, "Super Missile (Crateria)", Minor, Logic switch {
                    Normal => new Requirement(items => items.CanUsePowerBombs() && items.SpeedBooster && items.Ice &&
                        (items.HasEnergyReserves(4) || (items.HasEnergyReserves(3) && (items.Grapple || items.SpaceJump)))),
                    Medium => new Requirement(items => items.CanUsePowerBombs() && items.SpeedBooster &&
                        (items.HasEnergyReserves(3) || (items.HasEnergyReserves(2) && (items.Grapple || items.SpaceJump)))),
                    _ => new Requirement(items => items.CanUsePowerBombs() && items.HasEnergyReserves(2) && items.SpeedBooster)
                }),
                // new Location(this, 11, "Super Missile (Crateria)", Visible, Minor, 0x78478, Logic switch

                // Exiting wall jumps are hard only
                new Location(this, 7, 0x8F8404, LocationType.Chozo, "Bombs", Major, Logic switch {
                    Hard => items => (config.UseKeycards ? items.CardCrateriaBoss : items.CanOpenRedDoors()) && items.Morph,
                    _ => new Requirement(items => (config.UseKeycards ? items.CardCrateriaBoss : items.CanOpenRedDoors()) && items.CanPassBombPassages())
                })
                // new Location(this, 7, "Bombs", Chozo, Major, 0x78404, Logic switch
            };
        }
    }
}
