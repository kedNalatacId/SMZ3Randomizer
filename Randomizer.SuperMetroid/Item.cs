using System;
using System.Linq;
using System.Collections.Generic;
using static Randomizer.SuperMetroid.ItemType;
using static Randomizer.SuperMetroid.SMLogic;
using static Randomizer.SuperMetroid.ItemClass;
using System.Text.RegularExpressions;
using System.ComponentModel;
using Newtonsoft.Json;

namespace Randomizer.SuperMetroid {

    public enum ItemType : byte {
        [Description("Nothing")] 
        Nothing,

        [Description("Crateria Level 1 Keycard")]
        CardCrateriaL1 = 0xD0,
        [Description("Crateria Level 2 Keycard")]
        CardCrateriaL2 = 0xD1,
        [Description("Crateria Boss Keycard")]
        CardCrateriaBoss = 0xD2,
        [Description("Brinstar Level 1 Keycard")]
        CardBrinstarL1 = 0xD3,
        [Description("Brinstar Level 2 Keycard")]
        CardBrinstarL2 = 0xD4,
        [Description("Brinstar Boss Keycard")]
        CardBrinstarBoss = 0xD5,
        [Description("Norfair Level 1 Keycard")]
        CardNorfairL1 = 0xD6,
        [Description("Norfair Level 2 Keycard")]
        CardNorfairL2 = 0xD7,
        [Description("Norfair Boss Keycard")]
        CardNorfairBoss = 0xD8,
        [Description("Maridia Level 1 Keycard")]
        CardMaridiaL1 = 0xD9,
        [Description("Maridia Level 2 Keycard")]
        CardMaridiaL2 = 0xDA,
        [Description("Maridia Boss Keycard")]
        CardMaridiaBoss = 0xDB,
        [Description("Wrecked Ship Level 1 Keycard")]
        CardWreckedShipL1 = 0xDC,
        [Description("Wrecked Ship Boss Keycard")]
        CardWreckedShipBoss = 0xDD,
        [Description("Lower Norfair Level 1 Keycard")]
        CardLowerNorfairL1 = 0xDE,
        [Description("Lower Norfair Boss Keycard")]
        CardLowerNorfairBoss = 0xDF,

        [Description("Missile")]
        Missile = 0xC2,
        [Description("Super Missile")]
        Super = 0xC3,
        [Description("Power Bomb")]
        PowerBomb = 0xC4,
        [Description("Grappling Beam")]
        Grapple = 0xB0,
        [Description("X-Ray Scope")]
        XRay = 0xB1,
        [Description("Energy Tank")]
        ETank = 0xC0,
        [Description("Reserve Tank")]
        ReserveTank = 0xC1,
        [Description("Charge Beam")]
        Charge = 0xBB,
        [Description("Ice Beam")]
        Ice = 0xBC,
        [Description("Wave Beam")]
        Wave = 0xBD,
        [Description("Spazer")]
        Spazer = 0xBE,
        [Description("Plasma Beam")]
        Plasma = 0xBF,
        [Description("Varia Suit")]
        Varia = 0xB2,
        [Description("Gravity Suit")]
        Gravity = 0xB6,
        [Description("Morphing Ball")]
        Morph = 0xB4,
        [Description("Morph Bombs")]
        Bombs = 0xB9,
        [Description("Spring Ball")]
        SpringBall = 0xB3,
        [Description("Screw Attack")]
        ScrewAttack = 0xB5,
        [Description("Hi-Jump Boots")]
        HiJump = 0xB7,
        [Description("Space Jump")]
        SpaceJump = 0xB8,
        [Description("Speed Booster")]
        SpeedBooster = 0xBA,
    }

    /* public enum ItemType : byte {
        Missile = 1,
        Super = 2,
        PowerBomb = 3,
        Grapple = 16,
        XRay = 14,
        ETank = 0,
        ReserveTank = 20,
        Charge = 5,
        Ice = 6,
        Wave = 9,
        Spazer = 10,
        Plasma = 15,
        Varia = 12,
        Gravity = 13,
        Morph = 19,
        Bombs = 4,
        SpringBall = 11,
        ScrewAttack = 18,
        HiJump = 7,
        SpaceJump = 17,
        SpeedBooster = 8
    } */

    enum ItemClass {
        Minor,
        Major
    }

    class Item {
        public string Name { get; set; }
        public ItemType Type { get; set; }
        public bool Progression { get; set; }
        public ItemClass Class { get; set; } = Minor;
        [JsonIgnore]
        public World World { get; set; }

        static readonly Regex keycard  = new Regex("^Card");
        static readonly Regex cardone  = new Regex("^Card.*L1");
        static readonly Regex cardtwo  = new Regex("^Card.*L2");
        static readonly Regex bosscard = new Regex("^Card.*Boss$");

        public bool IsKeycard     => keycard.IsMatch(Type.ToString());
        public bool IsCardOne     => cardone.IsMatch(Type.ToString());
        public bool IsCardTwo     => cardtwo.IsMatch(Type.ToString());
        public bool IsBossKeycard => bosscard.IsMatch(Type.ToString());

        public bool Is(ItemType type, World world) => Type == type && World == world;
        public bool IsNot(ItemType type, World world) => !Is(type, world);

        public static ItemType[] minorItems = new[] {
            ItemType.Missile,
            ItemType.Super,
            ItemType.PowerBomb,
        };

        public Item(ItemType itemType) {
            Name = itemType.GetDescription();
            Type = itemType;
        }

        public Item(ItemType itemType, World world) : this(itemType) {
            World = world;
        }

        public static Item Nothing(World world) {
            return new Item(ItemType.Nothing, world);
        }

        public static List<Item> CreateProgressionPool(World world) {
            var itemPool = new List<Item> {
                new Item(Grapple),
                new Item(Charge),
                new Item(Ice),
                new Item(Wave),
                new Item(Plasma),
                new Item(Varia),
                new Item(Gravity),
                new Item(Morph),
                new Item(Bombs),
                new Item(SpringBall),
                new Item(ScrewAttack),
                new Item(HiJump),
                new Item(SpaceJump),
                new Item(SpeedBooster),

                new Item(Missile),
                new Item(Super),
                new Item(PowerBomb),
                new Item(PowerBomb),
                new Item(ETank),
                new Item(ETank),
                new Item(ETank),
                new Item(ETank),
            };

            foreach (var item in itemPool) {
                item.Progression = true;
                item.World = world;
                if (!minorItems.Contains(item.Type))
                    item.Class = Major;
            }

            return itemPool;
        }

        public static List<Item> CreateNicePool(World world) {
            var itemPool = new List<Item> {
                new Item(Spazer),
                new Item(XRay),
            };
            
            foreach (var item in itemPool) item.World = world;

            return itemPool;
        }

        public static List<Item> CreateJunkPool(World world) {
            var itemPool = new List<Item>();

            itemPool.AddRange(Copies(10,  () => new Item(ETank)));
            itemPool.AddRange(Copies(4,  () => new Item(ReserveTank)));

            // TODO: futz with numbers here
            itemPool.AddRange(Copies(world.Config.UseKeycards ? 28 : 39, () => new Item(Missile)));
            itemPool.AddRange(Copies(world.Config.UseKeycards ? 12 : 15, () => new Item(Super)));
            itemPool.AddRange(Copies(world.Config.UseKeycards ? 6 : 8,  () => new Item(PowerBomb)));

            foreach (var item in itemPool) item.World = world;

            return itemPool;
        }

        public static List<Item> CreateKeycards(World world) {
            var itemPool = new List<Item> {
                new Item(CardCrateriaL1),
                new Item(CardCrateriaL2),
                new Item(CardCrateriaBoss),
                new Item(CardBrinstarL1),
                new Item(CardBrinstarL2),
                new Item(CardBrinstarBoss),
                new Item(CardNorfairL1),
                new Item(CardNorfairL2),
                new Item(CardNorfairBoss),
                new Item(CardMaridiaL1),
                new Item(CardMaridiaL2),
                new Item(CardMaridiaBoss),
                new Item(CardWreckedShipL1),
                new Item(CardWreckedShipBoss),
                new Item(CardLowerNorfairL1),
                new Item(CardLowerNorfairBoss),
            };

            foreach (var item in itemPool) {
                item.Progression = true;
                item.World = world;
                item.Class = Major;
            }

            return itemPool;
        }

        static List<Item> Copies(int nr, Func<Item> template) {
            return Enumerable.Range(1, nr).Select(i => template()).ToList();
        }
    }

    static class ItemListExtensions {
        public static Item Get(this List<Item> items, ItemType itemType) {
            var item = items.FirstOrDefault(i => i.Type == itemType);
            if (item == null)
                throw new InvalidOperationException($"Could not find an item of type {itemType}");
            return item;
        }

        public static Item Get(this List<Item> items, ItemType itemType, World world) {
            var item = items.FirstOrDefault(i => i.Is(itemType, world));
            if (item == null)
                throw new InvalidOperationException($"Could not find an item of type {itemType} in world {world.Id}");
            return item;
        }
    }

    class Progression {
        public bool CardCrateriaL1 { get; private set; }
        public bool CardCrateriaL2 { get; private set; }
        public bool CardCrateriaBoss { get; private set; }
        public bool CardBrinstarL1 { get; private set; }
        public bool CardBrinstarL2 { get; private set; }
        public bool CardBrinstarBoss { get; private set; }
        public bool CardNorfairL1 { get; private set; }
        public bool CardNorfairL2 { get; private set; }
        public bool CardNorfairBoss { get; private set; }
        public bool CardMaridiaL1 { get; private set; }
        public bool CardMaridiaL2 { get; private set; }
        public bool CardMaridiaBoss { get; private set; }
        public bool CardWreckedShipL1 { get; private set; }
        public bool CardWreckedShipBoss { get; private set; }
        public bool CardLowerNorfairL1 { get; private set; }
        public bool CardLowerNorfairBoss { get; private set; }

        public bool Grapple { get; private set; }
        public bool Charge { get; private set; }
        public bool Ice { get; private set; }
        public bool Wave { get; private set; }
        public bool Plasma { get; private set; }
        public bool Varia { get; private set; }
        public bool Gravity { get; private set; }
        public bool Morph { get; private set; }
        public bool Bombs { get; private set; }
        public bool SpringBall { get; private set; }
        public bool ScrewAttack { get; private set; }
        public bool HiJump { get; private set; }
        public bool SpaceJump { get; private set; }
        public bool SpeedBooster { get; private set; }
        public bool Missile { get; private set; }
        public bool Super { get; private set; }
        public bool PowerBomb { get; private set; }
        public bool TwoPowerBombs { get; private set; }
        public int ETank { get; private set; }
        public int ReserveTank { get; private set; }

        public Progression(IEnumerable<Item> items) {
            Add(items);
        }

        public void Add(IEnumerable<Item> items) {
            foreach (var item in items) {
                bool done = item.Type switch {
                    ItemType.CardCrateriaL1 => CardCrateriaL1 = true,
                    ItemType.CardCrateriaL2 => CardCrateriaL2 = true,
                    ItemType.CardCrateriaBoss => CardCrateriaBoss = true,
                    ItemType.CardBrinstarL1 => CardBrinstarL1 = true,
                    ItemType.CardBrinstarL2 => CardBrinstarL2 = true,
                    ItemType.CardBrinstarBoss => CardBrinstarBoss = true,
                    ItemType.CardNorfairL1 => CardNorfairL1 = true,
                    ItemType.CardNorfairL2 => CardNorfairL2 = true,
                    ItemType.CardNorfairBoss => CardNorfairBoss = true,
                    ItemType.CardMaridiaL1 => CardMaridiaL1 = true,
                    ItemType.CardMaridiaL2 => CardMaridiaL2 = true,
                    ItemType.CardMaridiaBoss => CardMaridiaBoss = true,
                    ItemType.CardWreckedShipL1 => CardWreckedShipL1 = true,
                    ItemType.CardWreckedShipBoss => CardWreckedShipBoss = true,
                    ItemType.CardLowerNorfairL1 => CardLowerNorfairL1 = true,
                    ItemType.CardLowerNorfairBoss => CardLowerNorfairBoss = true,

                    ItemType.Grapple => Grapple = true,
                    ItemType.Charge => Charge = true,
                    ItemType.Ice => Ice = true,
                    ItemType.Wave => Wave = true,
                    ItemType.Plasma => Plasma = true,
                    ItemType.Varia => Varia = true,
                    ItemType.Gravity => Gravity = true,
                    ItemType.Morph => Morph = true,
                    ItemType.Bombs => Bombs = true,
                    ItemType.SpringBall => SpringBall = true,
                    ItemType.ScrewAttack => ScrewAttack = true,
                    ItemType.HiJump => HiJump = true,
                    ItemType.SpaceJump => SpaceJump = true,
                    ItemType.SpeedBooster => SpeedBooster = true,
                    ItemType.Missile => Missile = true,
                    ItemType.Super => Super = true,
                    _ => false
                };

                if (done)
                    continue;

                switch (item.Type) {
                    case ItemType.ETank: ETank += 1; break;
                    case ItemType.ReserveTank: ReserveTank += 1; break;
                    case ItemType.PowerBomb:
                        TwoPowerBombs = PowerBomb;
                        PowerBomb = true;
                        break;
                }
            }
        }
    }

    static class ProgressionExtensions {

        public static bool CanIbj(this Progression items) {
            return items.Morph && items.Bombs;
        }

        public static bool CanFly(this Progression items) {
            return items.SpaceJump || items.CanIbj();
        }

        public static bool CanUsePowerBombs(this Progression items) {
            return items.Morph && items.PowerBomb;
        }

        public static bool CanPassBombPassages(this Progression items) {
            return items.Morph && (items.Bombs || items.PowerBomb);
        }

        public static bool CanDestroyBombWalls(this Progression items) {
            return items.CanPassBombPassages() || items.ScrewAttack;
        }

        public static bool CanSpringBallJump(this Progression items) {
            return items.Morph && items.SpringBall;
        }

        public static bool CanHeckRun(this Progression items) {
            return items.Varia || items.HasEnergyReserves(7);
        }

        public static bool CanHellRun(this Progression items) {
            return items.Varia || items.HasEnergyReserves(5);
        }

        public static bool HasEnergyReserves(this Progression items, int amount) {
            return (items.ETank + items.ReserveTank) >= amount;
        }

        public static bool CanOpenRedDoors(this Progression items) {
            return items.Missile || items.Super;
        }
    }
}
