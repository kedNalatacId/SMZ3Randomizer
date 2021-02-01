using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;


namespace Randomizer.SuperMetroid.Regions.Brinstar {

    class Green : Region {

        public override string Name => "Brinstar Green";
        public override string Area => "Brinstar";

        public Green(World world, Config config) : base(world, config) {
            Weight = -6;

            RegionItems = new[] { CardBrinstarL1, CardBrinstarL2, CardCrateriaBoss };

            Locations = new List<Location> {
                new Location(this, 13, 0x8F84AC, LocationType.Chozo, "Power Bomb (green Brinstar bottom)", Minor, Logic switch {
                    _ => new Requirement(items => items.CardBrinstarL2 && items.CanUsePowerBombs())
                }),
                // new Location(this, 13, "Power Bomb (green Brinstar bottom)", Chozo, Minor, 0x784AC, Logic switch

                // morph can be here if you can get out with bombs
                new Location(this, 15, 0x8F8518, LocationType.Visible, "Missile (green Brinstar below super missile)", Minor, Logic switch {
                    Hard => items => items.CanOpenRedDoors(),
                    _ => new Requirement(items => items.CanOpenRedDoors() && items.CanPassBombPassages()),
                }).AlwaysAllow((item, items) => item.Is(Morph, World) && (items.Bombs || items.PowerBomb)),
                // new Location(this, 15, "Missile (green Brinstar below super missile)", Visible, Minor, 0x78518, Logic switch

                new Location(this, 16, 0x8F851E, LocationType.Visible, "Super Missile (green Brinstar top)", Minor, Logic switch {
                    Normal => items => items.CanOpenRedDoors() && items.SpeedBooster,
                    _ => new Requirement(items => items.CanOpenRedDoors() && (items.Morph || items.SpeedBooster))
                }),
                // new Location(this, 16, "Super Missile (green Brinstar top)", Visible, Minor, 0x7851E, Logic switch

                new Location(this, 17, 0x8F852C, LocationType.Chozo, "Reserve Tank, Brinstar", Major, Logic switch {
                    Normal => items => items.CanOpenRedDoors() && items.SpeedBooster,
                    _ => new Requirement(items => items.CanOpenRedDoors() && (items.Morph || items.SpeedBooster))
                }),
                // new Location(this, 17, "Reserve Tank, Brinstar", Chozo, Major, 0x7852C, Logic switch

                new Location(this, 18, 0x8F8532, LocationType.Hidden, "Missile (green Brinstar behind missile)", Minor, Logic switch {
                    Normal => items => items.SpeedBooster && items.CanPassBombPassages() && items.CanOpenRedDoors(),
                    Medium => new Requirement(items => items.CanPassBombPassages() && items.CanOpenRedDoors()),
                    _ => new Requirement(items => (items.CanPassBombPassages() || items.Morph && items.ScrewAttack) && items.CanOpenRedDoors())
                }),
                // new Location(this, 18, "Missile (green Brinstar behind missile)", Hidden, Minor, 0x78532, Logic switch

                new Location(this, 19, 0x8F8538, LocationType.Visible, "Missile (green Brinstar behind reserve tank)", Minor, Logic switch {
                    Normal => items => items.SpeedBooster && items.CanOpenRedDoors() && items.Morph,
                    _ => new Requirement(items => items.CanOpenRedDoors() && items.Morph)
                }),
                // new Location(this, 19, "Missile (green Brinstar behind reserve tank)", Visible, Minor, 0x78538, Logic switch

                new Location(this, 30, 0x8F87C2, LocationType.Visible, "Energy Tank, Etecoons", Major, Logic switch {
                    _ => new Requirement(items => items.CardBrinstarL2 && items.CanUsePowerBombs())
                }),
                // new Location(this, 30, "Energy Tank, Etecoons", Visible, Major, 0x787C2, Logic switch

                new Location(this, 31, 0x8F87D0, LocationType.Visible, "Super Missile (green Brinstar bottom)", Minor, Logic switch {
                    _ => new Requirement(items => items.CardBrinstarL2 && items.CanUsePowerBombs() && items.Super)
                }),
                // new Location(this, 31, "Super Missile (green Brinstar bottom)", Visible, Minor, 0x787D0, Logic switch
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal => items.CanDestroyBombWalls(),
                _ => items.CanDestroyBombWalls() || items.SpeedBooster
            };
        }
    }
}
