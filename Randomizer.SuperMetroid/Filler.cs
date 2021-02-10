using System;
using System.Collections.Generic;
using Randomizer.Shared.Contracts;
using System.Linq;
using Newtonsoft.Json;

namespace Randomizer.SuperMetroid {
    class Filler {
        List<World> Worlds { get; set; }
        Config Config { get; set; }
        Random Rnd { get; set; }

        public Filler(List<World> worlds, Config config, Random rnd) {
            Worlds = worlds;
            Config = config;
            Rnd = rnd;
        }

        public void Fill() {
            var progressionItems = new List<Item>();
            var baseItems = new List<Item>();

            foreach (var world in Worlds) {
                var progression = Item.CreateProgressionPool(world);

                /* If not using keycards then add keycards to the "base items" list.
                    This simplifies logic programming since it assumes that the character has the keycard */
                if (Config.UseKeycards) {
                    progression.AddRange(Item.CreateKeycards(world));
                } else {
                    baseItems = baseItems.Concat(Item.CreateKeycards(world)).ToList();
                }

                InitialFillInOwnWorld(progression, world);

                progressionItems.AddRange(progression);
            }

            progressionItems = progressionItems.Shuffle(Rnd);

            var niceItems = Worlds.SelectMany(world => Item.CreateNicePool(world)).Shuffle(Rnd);
            var junkItems = Worlds.SelectMany(world => Item.CreateJunkPool(world)).Shuffle(Rnd);

            var locations = Worlds.SelectMany(x => x.Locations).Empty().Shuffle(Rnd);
            if (Config.GameMode != GameMode.Multiworld) {
                locations = ApplyLocationWeighting(locations).ToList();
            } else {
                var r = Rnd.Next(2);
                ApplyItemBias(progressionItems, new[] {
                    (ItemType.Varia, (.20 + r * .20)),
                    (ItemType.Gravity, (.20 + (1 - r) * .20)),
                });
            }

            AssumedFill(progressionItems, baseItems, locations, Worlds);

            FastFill(niceItems, locations);
            FastFill(junkItems, locations);
        }

        void ApplyItemBias(List<Item> itemPool, IEnumerable<(ItemType type, double weight)> reorder) {
            var n = itemPool.Count;

            /* Gather all items that are being biased */
            var items = reorder.ToDictionary(x => x.type, x => itemPool.FindAll(item => item.Type == x.type));
            itemPool.RemoveAll(item => reorder.Any(x => x.type == item.Type));

            /* Insert items from each biased type such that their lowest index
             * is based on their weight on the original pool size
             */
            foreach (var (type, weight) in reorder.OrderByDescending(x => x.weight)) {
                var i = (int)(n * (1 - weight));
                if (i >= itemPool.Count)
                    throw new InvalidOperationException($"Too many items are being biased which makes the tail portion for {type} too big");
                foreach (var item in items[type]) {
                    var k = Rnd.Next(i, itemPool.Count);
                    itemPool.Insert(k, item);
                }
            }
        }

        IEnumerable<Location> ApplyLocationWeighting(IEnumerable<Location> locations) {
            return from location in locations.Select((x, i) => (x, i: i - x.Weight))
                   orderby location.i select location.x;
        }

        void AssumedFill(List<Item> itemPool, List<Item> baseItems, IEnumerable<Location> locations, IEnumerable<World> worlds) {
            var assumedItems = new List<Item>(itemPool);
            int fail_counter = 0;

            /* Place items until progression item pool is empty */
            while (assumedItems.Count > 0) {
                var item = assumedItems.First();
                assumedItems.Remove(item);

                var inventory = CollectItems(assumedItems.Concat(baseItems).ToList(), worlds);
                var location = Config.Placement switch {
                    Placement.Split => locations.Empty().Where(x => x.Class == item.Class).ToList().CanFillWithinWorld(item, inventory).FirstOrDefault(),
                    _ => locations.Empty().CanFillWithinWorld(item, inventory).FirstOrDefault()
                };

                if (location == null) {
                    assumedItems.Add(item);
                    if (++fail_counter > locations.Empty().Count()) {
                        throw new CannotFillWorldException("Cannot fill world");
                    }
                    continue;
                }

                location.Item = item;
                itemPool.Remove(item);
            }
        }

        public List<Item> CollectItems(List<Item> items, IEnumerable<World> worlds) {
            var myItems = new List<Item>(items);
            var availableLocations = worlds.SelectMany(l => l.Locations).Where(x => x.Item != null).ToList();
            while (true) {
                var searchLocations = availableLocations.AvailableWithinWorld(myItems).ToList();
                availableLocations = availableLocations.Except(searchLocations).ToList();
                var foundItems = searchLocations.Select(x => x.Item).ToList();
                if (foundItems.Count == 0)
                    break;

                myItems = myItems.Concat(foundItems).ToList();
            }

            return myItems;
        }

        public void FastFill(List<Item> items, IEnumerable<Location> locations) {
            foreach (var (location, item) in locations.Empty().Zip(items, (l, i) => (l, i)).ToList()) {
                location.Item = item;
                items.Remove(item);
            }
        }

        void InitialFillInOwnWorld(List<Item> progressionItems, World world) {
            if (Config.MorphLocation == MorphLocation.Original)
                FillItemAtLocation(progressionItems, ItemType.Morph, world.Locations.Get("Morphing Ball"));

            // Spice things up every so often
            // We still have to place a way to break blocks directly after this...
            if (Config.SMLogic != SMLogic.Normal && Rnd.Next(8) < 1)
                FrontFillItemInWorld(world, progressionItems, ItemType.CardBrinstarL1, true);

            /* Place a way to break bomb blocks. */
            int choice = Rnd.Next(Config.SMLogic == SMLogic.Normal ? 1 : 2);

            FrontFillItemInWorld(world, progressionItems, choice switch {
                0 => ItemType.ScrewAttack,
                1 => ItemType.Morph,
                _ => ItemType.SpeedBooster,
            }, true);

            /* Place a way to break bomb blocks.
             * If split placement is used with normal logic we must place a powerbomb since there are no more major item locations available */
            if (choice == 1 && Config.Placement == Placement.Split && Config.SMLogic == SMLogic.Normal) {
                FrontFillItemInWorld(world, progressionItems, ItemType.PowerBomb, true);
            }

            FrontFillItemInWorld(world, progressionItems, Rnd.Next(2) == 0 ? ItemType.Missile : ItemType.Super, true);

            // With split placement, we'll run into problem with placement if progression minors aren't available from the start
            if (Config.Placement == Placement.Split) {
                // If missile was placed, also place a super
                // This is about as much as LiveDangerously can effect without seriously causing logic issues
                if (world.Locations.Filled().Where(l => l.ItemIs(ItemType.Super, world)).ToList().Count == 0 && !Config.LiveDangerously) {
                    FrontFillItemInWorld(world, progressionItems, ItemType.Super, true);
                }

                // If no power bomb was placed, place one
                if (world.Locations.Filled().Where(l => l.ItemIs(ItemType.PowerBomb, world)).ToList().Count == 0) {
                    FrontFillItemInWorld(world, progressionItems, ItemType.PowerBomb, true);
                }
            }

            // if they said early morph, place it now; it'll happen soon anyway...
            if (choice != 1 && Config.MorphLocation == MorphLocation.Early)
                FrontFillItemInWorld(world, progressionItems, ItemType.Morph, true);
        }

        private void FrontFillItemInWorld(World world, List<Item> itemPool, ItemType itemType, bool restrictWorld = false) {
            /* Get a shuffled list of available locations to place this item in */
            Item item = restrictWorld ? itemPool.Get(itemType, world) : itemPool.Get(itemType);
            var availableLocations = Config.Placement switch {
                Placement.Split => world.Locations.Empty().Where(x => x.Class == item.Class).ToList().Available(world.Items).Shuffle(Rnd),
                _ => world.Locations.Empty().ToList().Available(world.Items).Shuffle(Rnd)
            };

            if (availableLocations.Count > 0) {
                var locationToFill = availableLocations.First();
                locationToFill.Item = item;
                itemPool.Remove(item);
            } else {
                throw new CannotFillWorldException("No location to place item:" + item.Name);
            }
        }

        void FillItemAtLocation(List<Item> itemPool, ItemType itemType, Location location) {
            var itemToPlace = itemPool.Get(itemType);
            location.Item = itemToPlace ?? throw new InvalidOperationException($"Tried to place item {itemType} at {location.Name}, but there is no such item in the item pool");
            itemPool.Remove(itemToPlace);
        }
    }
}
