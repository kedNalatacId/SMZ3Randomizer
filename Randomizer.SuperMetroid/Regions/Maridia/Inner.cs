using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.LocationType;
using static Randomizer.SuperMetroid.ItemClass;

namespace Randomizer.SuperMetroid.Regions.Maridia {
    class Inner : Region {
        public override string Name => "Maridia Inner";
        public override string Area => "Maridia";

        public Inner(World world, Config config) : base(world, config) {
            RegionItems = new[] { CardMaridiaL2, CardMaridiaBoss };

            Locations = new List<Location> {
                new Location(this, 140, 0x8FC4AF, LocationType.Visible, "Super Missile (yellow Maridia)", Minor, Logic switch {
                    Normal => items => items.CardMaridiaL1 && items.CanPassBombPassages(),
                    Medium => items => items.CardMaridiaL1 && items.CanPassBombPassages() && (items.Gravity || items.Ice),
                    _ => new Requirement(items => items.CardMaridiaL1 && items.CanPassBombPassages() &&
                        (items.Gravity || items.Ice || items.HiJump && items.CanSpringBallJump()))
                }),
                new Location(this, 141, 0x8FC4B5, LocationType.Visible, "Missile (yellow Maridia super missile)", Minor, Logic switch {
                    Normal => items => items.CardMaridiaL1 && items.CanPassBombPassages(),
                    Medium => items => items.CardMaridiaL1 && items.CanPassBombPassages() && (items.Gravity || items.Ice),
                    _ => new Requirement(items => items.CardMaridiaL1 && items.CanPassBombPassages() &&
                        (items.Gravity || items.Ice || items.HiJump && items.CanSpringBallJump()))
                }),
                new Location(this, 142, 0x8FC533, LocationType.Visible, "Missile (yellow Maridia false wall)", Minor, Logic switch {
                    Normal => items => items.CardMaridiaL1 && items.CanPassBombPassages(),
                    Medium => items => items.CardMaridiaL1 && items.CanPassBombPassages() && (items.Gravity || items.Ice),
                    _ => new Requirement(items => items.CardMaridiaL1 && items.CanPassBombPassages() &&
                        (items.Gravity || items.Ice || items.HiJump && items.CanSpringBallJump()))
                }),
                new Location(this, 143, 0x8FC559, LocationType.Chozo, "Plasma Beam", Major, Logic switch {
                    Normal => items => CanDefeatDraygon(items) && (items.ScrewAttack || items.Plasma) && items.SpaceJump,
                    Medium => items => CanDefeatDraygon(items) && (items.ScrewAttack || items.Plasma) &&
                        (items.HiJump || items.CanFly() || items.SpeedBooster),
                    _ => new Requirement(items => CanDefeatDraygon(items) &&
                        (items.Charge && items.HasEnergyReserves(3) || items.ScrewAttack || items.Plasma || items.SpeedBooster) &&
                        (items.HiJump || items.CanSpringBallJump() || items.CanFly() || items.SpeedBooster))
                }),
                new Location(this, 144, 0x8FC5DD, LocationType.Visible, "Missile (left Maridia sand pit room)", Minor, Logic switch {
                    Normal => items => CanReachAqueduct(items) && items.Super && items.CanPassBombPassages(),
                    Medium => new Requirement(items => CanReachAqueduct(items) && items.Super &&
                        (items.HiJump && items.SpaceJump || items.Gravity)),
                    _ => new Requirement(items => CanReachAqueduct(items) && items.Super &&
                        (items.HiJump && (items.SpaceJump || items.CanSpringBallJump()) || items.Gravity))
                }),
                new Location(this, 145, 0x8FC5E3, LocationType.Chozo, "Reserve Tank, Maridia", Major, Logic switch {
                    Normal => items => CanReachAqueduct(items) && items.Super && items.CanPassBombPassages(),
                    Medium => items => CanReachAqueduct(items) && items.Super && (items.HiJump && items.SpaceJump || items.Gravity),
                    _ => new Requirement(items => CanReachAqueduct(items) && items.Super &&
                        (items.HiJump && (items.SpaceJump || items.CanSpringBallJump()) || items.Gravity))
                }),
                new Location(this, 146, 0x8FC5EB, LocationType.Visible, "Missile (right Maridia sand pit room)", Minor, Logic switch {
                    Normal => new Requirement(items => CanReachAqueduct(items) && items.Super),
                    _ => items => CanReachAqueduct(items) && items.Super && (items.HiJump || items.Gravity)
                }),
                new Location(this, 147, 0x8FC5F1, LocationType.Visible, "Power Bomb (right Maridia sand pit room)", Minor, Logic switch {
                    Normal => new Requirement(items => CanReachAqueduct(items) && items.Super),
                    Medium => items => CanReachAqueduct(items) && items.Super && items.Gravity,
                    _ => items => CanReachAqueduct(items) && items.Super && (items.HiJump && items.CanSpringBallJump() || items.Gravity)
                }),
                new Location(this, 148, 0x8FC603, LocationType.Visible, "Missile (pink Maridia)", Minor, Logic switch {
                    Hard => items => CanReachAqueduct(items) && items.Gravity,
                    _ => new Requirement(items => CanReachAqueduct(items) && items.Gravity && items.SpeedBooster)
                }),
                new Location(this, 149, 0x8FC609, LocationType.Visible, "Super Missile (pink Maridia)", Minor, Logic switch {
                    Hard => items => CanReachAqueduct(items) && items.Gravity,
                    _ => new Requirement(items => CanReachAqueduct(items) && items.SpeedBooster)
                }),
                new Location(this, 150, 0x8FC6E5, LocationType.Chozo, "Spring Ball", Major, Logic switch {
                    Normal => items => items.Super && items.Grapple && items.CanUsePowerBombs() && (items.SpaceJump || items.HiJump),
                    Medium => items => items.Super && items.Grapple && items.CanUsePowerBombs() && (items.CanFly() || items.HiJump),
                    _ => new Requirement(items => items.Super && items.Grapple && items.CanUsePowerBombs() && (
                        items.Gravity && (items.CanFly() || items.HiJump) ||
                        items.Ice && items.HiJump && items.CanSpringBallJump() && items.SpaceJump))
                }),
                new Location(this, 151, 0x8FC74D, LocationType.Hidden, "Missile (Draygon)", Minor, Logic switch {
                    Normal => items => items.CardMaridiaL1 && items.CardMaridiaL2 && CanDefeatBotwoon(items),
                    _ => new Requirement(items => (items.CardMaridiaL1 && items.CardMaridiaL2 && CanDefeatBotwoon(items)) && items.Gravity)
                }),
                new Location(this, 152, 0x8FC755, LocationType.Visible, "Energy Tank, Botwoon", Major, Logic switch {
                    _ => new Requirement(items => items.CardMaridiaL1 && items.CardMaridiaL2 && CanDefeatBotwoon(items))
                }),
                new Location(this, 154, 0x8FC7A7, LocationType.Chozo, "Space Jump", Major, Logic switch {
                    _ => new Requirement(items => CanDefeatDraygon(items))
                })
            };
        }

        bool CanReachAqueduct(Progression items) {
            return items.CardMaridiaL1;
        }

        bool CanDefeatDraygon(Progression items) {
            return Logic switch {
                Normal => items.CardMaridiaL1 && items.CardMaridiaL2 && CanDefeatBotwoon(items) &&
                    items.CardMaridiaBoss && items.Gravity && items.SpaceJump,
                Medium => items.CardMaridiaL1 && items.CardMaridiaL2 && CanDefeatBotwoon(items) &&
                    items.CardMaridiaBoss && items.Gravity && (items.SpeedBooster && items.HiJump || items.CanFly()),
                _ => items.CardMaridiaL1 && items.CardMaridiaL2 && CanDefeatBotwoon(items) &&
                    items.CardMaridiaBoss && items.Gravity
            };
        }

        bool CanDefeatBotwoon(Progression items) {
            return Logic switch {
                Hard => items.Ice || items.SpeedBooster && items.Gravity,
                _ => items.SpeedBooster && items.Gravity
            };
        }

        public override bool CanEnter(Progression items) {
            return Logic switch {
                Normal => items.Gravity && (
                    World.CanEnter("Norfair Upper West", items) && items.Super && items.CanUsePowerBombs() &&
                        (items.SpaceJump || items.SpeedBooster || items.Grapple)
                ),
                Medium => items.Gravity && (
                    World.CanEnter("Norfair Upper West", items) && items.Super && items.CanUsePowerBombs() &&
                        (items.CanFly() || items.SpeedBooster || items.Grapple)
                ),
                _ =>
                    items.Super && World.CanEnter("Norfair Upper West", items) && items.CanUsePowerBombs() &&
                        (items.Gravity || items.HiJump && (items.Ice || items.CanSpringBallJump()) && items.Grapple)
            };
        }

        public bool CanComplete(Progression items) {
            return Locations.Get("Space Jump").Available(items);
        }
    }
}
