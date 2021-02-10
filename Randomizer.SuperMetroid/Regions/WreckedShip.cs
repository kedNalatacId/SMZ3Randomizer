using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions {
    class WreckedShip : Region {
        public override string Name => "Wrecked Ship";
        public override string Area => "Wrecked Ship";

        public WreckedShip(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardWreckedShipL1 };

            Locations = new List<Location> {
                new Location(this, 128, 0x8FC265, LocationType.Visible, "Missile (Wrecked Ship middle)", Minor, Logic switch {
                    _ => new Requirement(items => items.CanPassBombPassages())
                }),
                new Location(this, 129, 0x8FC2E9, LocationType.Chozo, "Reserve Tank, Wrecked Ship", Major, Logic switch {
                    Normal => items => CanUnlockShip(items) && items.CardWreckedShipL1 && items.SpeedBooster && items.CanUsePowerBombs() &&
                        (items.Grapple || items.SpaceJump || items.Varia && items.HasEnergyReserves(2) || items.HasEnergyReserves(3)),
                    _ => new Requirement(items => CanUnlockShip(items) && items.CardWreckedShipL1 && items.CanUsePowerBombs() && items.SpeedBooster &&
                        (items.Varia || items.HasEnergyReserves(2)))
                }),
                new Location(this, 130, 0x8FC2EF, LocationType.Visible, "Missile (Gravity Suit)", Minor, Logic switch {
                    Normal => items => CanUnlockShip(items) && items.CardWreckedShipL1 &&
                        (items.Grapple || items.SpaceJump || items.Varia && items.HasEnergyReserves(2) || items.HasEnergyReserves(3)),
                    _ => new Requirement(items => CanUnlockShip(items) && items.CardWreckedShipL1 && (items.Varia || items.HasEnergyReserves(1)))
                }),
                new Location(this, 131, 0x8FC319, LocationType.Visible, "Missile (Wrecked Ship top)", Minor, items => CanUnlockShip(items)),
                new Location(this, 132, 0x8FC337, LocationType.Visible, "Energy Tank, Wrecked Ship", Major, Logic switch {
                    Normal => items => CanUnlockShip(items) &&
                        (items.HiJump || items.SpaceJump || items.SpeedBooster || items.Gravity),
                    _ => new Requirement(items => CanUnlockShip(items) && (items.Bombs || items.PowerBomb || items.CanSpringBallJump() ||
                        items.HiJump || items.SpaceJump || items.SpeedBooster || items.Gravity))
                }),
                new Location(this, 133, 0x8FC357, LocationType.Visible, "Super Missile (Wrecked Ship left)", Minor, items => CanUnlockShip(items)),
                new Location(this, 134, 0x8FC365, LocationType.Visible, "Right Super, Wrecked Ship", Major, items => CanUnlockShip(items)),
                new Location(this, 135, 0x8FC36D, LocationType.Chozo, "Gravity Suit", Major, Logic switch {
                    Normal => items => CanUnlockShip(items) && items.CardWreckedShipL1 &&
                        (items.Grapple || items.SpaceJump || items.Varia && items.HasEnergyReserves(2) || items.HasEnergyReserves(3)),
                    Medium => items => CanUnlockShip(items) && items.CardWreckedShipL1 &&
                        (items.Grapple || items.SpaceJump || items.Varia && items.HasEnergyReserves(1) || items.HasEnergyReserves(2)),
                    _ => new Requirement(items => CanUnlockShip(items) && items.CardWreckedShipL1 && (items.Varia || items.HasEnergyReserves(1)))
                })
            };
        }

        bool CanUnlockShip(Progression items) {
            return items.CardWreckedShipBoss && items.CanPassBombPassages();
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal =>
                    items.Super && (
                        /* Over the Moat */
                        (Config.UseKeycards ? items.CardCrateriaL2 : items.CanUsePowerBombs()) && (
                            items.Grapple || items.SpaceJump || (items.Gravity && items.CanIbj())
                        ) ||
                        /* Through Maridia -> Forgotten Highway */
                        items.CanUsePowerBombs() && items.Gravity
                    ),
                Medium =>
                    items.Super && (
                        /* Over the Moat */
                        (Config.UseKeycards ? items.CardCrateriaL2 : items.CanUsePowerBombs()) && (
                            items.SpeedBooster || items.Grapple || items.SpaceJump || items.Gravity
                        ) ||
                        /* Through Maridia -> Forgotten Highway */
                        items.CanUsePowerBombs() && (
                            items.Gravity ||
                            /* Climb Mt. Everest */
                            items.HiJump && items.Ice && items.Grapple && items.CardMaridiaL1
                        )
                    ),
                _ =>
                    items.Super && (
                        /* Over the Moat */
                        (Config.UseKeycards ? items.CardCrateriaL2 : items.CanUsePowerBombs()) ||
                        /* Through Maridia -> Forgotten Highway */
                        items.CanUsePowerBombs() && (
                            items.Gravity ||
                            /* Climb Mt. Everest */
                            items.HiJump && (items.Ice || items.CanSpringBallJump()) && items.Grapple && items.CardMaridiaL1
                        )
                    )
            };
        }

        public bool CanComplete(Progression items) {
            return CanEnter(items) && CanUnlockShip(items);
        }
    }
}
