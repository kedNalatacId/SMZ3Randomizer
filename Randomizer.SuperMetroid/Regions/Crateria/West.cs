using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.Crateria {

    class West : Region {

        public override string Name => "Crateria West";
        public override string Area => "Crateria";

        public West(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardCrateriaL2 };
            Locations = new List<Location> {

                new Location(this, 8, 0x8F8432, LocationType.Visible, "Energy Tank, Terminator", Major),
                // new Location(this, 8, "Energy Tank, Terminator", Visible, Major, 0x78432),

                new Location(this, 5, 0x8F8264, LocationType.Visible, "Energy Tank, Gauntlet", Major, Logic switch {
                    Normal => items => CanEnterAndLeaveGauntlet(items) && items.HasEnergyReserves(1),
                    _ => new Requirement(items => CanEnterAndLeaveGauntlet(items))
                }),
                // new Location(this, 5, "Energy Tank, Gauntlet", Visible, Major, 0x78264, Logic switch

                new Location(this, 9, 0x8F8464, LocationType.Visible, "Missile (Crateria gauntlet right)", Minor, Logic switch {
                    Normal => items => CanEnterAndLeaveGauntlet(items) && items.CanPassBombPassages() && items.HasEnergyReserves(2),
                    _ => new Requirement(items => CanEnterAndLeaveGauntlet(items) && items.CanPassBombPassages())
                }),
                // new Location(this, 9, "Missile (Crateria gauntlet right)", Visible, Minor, 0x78464, Logic switch

                new Location(this, 10, 0x8F846A, LocationType.Visible, "Missile (Crateria gauntlet left)", Minor, Logic switch {
                    Normal => items => CanEnterAndLeaveGauntlet(items) && items.CanPassBombPassages() && items.HasEnergyReserves(2),
                    _ => new Requirement(items => CanEnterAndLeaveGauntlet(items) && items.CanPassBombPassages())
                })
                // new Location(this, 10, "Missile (Crateria gauntlet left)", Visible, Minor, 0x7846A, Logic switch
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal => items.CanDestroyBombWalls(),
                _ =>items.CanDestroyBombWalls() || items.SpeedBooster,
            };
        }

        private bool CanEnterAndLeaveGauntlet(Progression items) {
            return Logic switch {
                Normal =>
                    items.CardCrateriaL1 && items.Morph && (items.SpaceJump || items.SpeedBooster) &&
                        (items.CanIbj() || items.CanUsePowerBombs() && items.TwoPowerBombs || items.ScrewAttack),
                Medium =>
                    items.CardCrateriaL1 && items.Morph && (items.CanFly() || items.SpeedBooster) &&
                        (items.CanIbj() || items.CanUsePowerBombs() && items.TwoPowerBombs || items.ScrewAttack),
                _ =>
                    items.CardCrateriaL1 && (
                        (items.Morph && (items.Bombs || items.TwoPowerBombs)) ||
                        items.ScrewAttack || items.SpeedBooster && items.CanUsePowerBombs() && items.HasEnergyReserves(2)
                    )
            };
        }
    }
}
