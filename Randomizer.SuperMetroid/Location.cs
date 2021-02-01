using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using Newtonsoft.Json;

namespace Randomizer.SuperMetroid {

    enum LocationType {
        Visible,
        Chozo,
        Hidden
    }

    delegate bool Requirement(Progression items);
    delegate bool Verification(Item item, Progression items);

    class Location {

        public int Id { get; set; }
        public string Name { get; set; }
        public LocationType Type { get; set; }
        public ItemClass Class { get; set; }
        public int Address { get; set; }
        public Item Item { get; set; }
        [JsonIgnore]
        public Region Region { get; set; }

        public int Weight {
            get { return weight ?? Region.Weight; }
        }

        readonly Requirement canAccess;
        Verification alwaysAllow;
        Verification allow;
        int? weight;

        public bool ItemIs(ItemType type, World world) => Item?.Is(type, world) ?? false;
        public bool ItemIsNot(ItemType type, World world) => !ItemIs(type, world);

        public Location(Region region, int id, int address, LocationType type, string name, ItemClass locationClass)
            : this(region, id, address, type, name, locationClass, items => true) {
        }

        public Location(Region region, int id, int address, LocationType type, string name, ItemClass locationClass, Requirement access) {
            Region = region;
            Id = id;
            Name = name;
            Type = type;
            Address = address;
            Class = locationClass;
            canAccess = access;
        }

        public Location Weighted(int? weight) {
            this.weight = weight;
            return this;
        }

        public Location AlwaysAllow(Verification allow) {
            alwaysAllow = allow;
            return this;
        }

        public Location Allow(Verification allow) {
            this.allow = allow;
            return this;
        }

        public bool Available(Progression items) {
            return Region.CanEnter(items) && canAccess(items);
        }

        public bool CanFill(Item item, Progression items) {
            var oldItem = Item;
            Item = item;
            // bool fillable = Available(items);
            bool fillable = Region.CanFill(item, items) && Available(items);
            Item = oldItem;
            return fillable;
        }
    }

    static class LocationListExtensions {

        public static Location Get(this IEnumerable<Location> locations, string name) {
            var location = locations.FirstOrDefault(l => l.Name == name);
            if (location == null)
                throw new ArgumentException($"Could not find location name {name}", nameof(name));
            return location;
        }

        public static IEnumerable<Location> Empty(this IEnumerable<Location> locations) {
            return locations.Where(l => l.Item == null);
        }

        public static IEnumerable<Location> Filled(this IEnumerable<Location> locations) {
            return locations.Where(l => l.Item != null);
        }

        public static IEnumerable<Location> AvailableWithinWorld(this IEnumerable<Location> locations, IEnumerable<Item> items) {
            return locations.Select(x => x.Region.World).Distinct().SelectMany(world =>
                locations.Where(l => l.Region.World == world).Available(items.Where(i => i.World == world)));
        }

        public static IEnumerable<Location> Available(this IEnumerable<Location> locations, IEnumerable<Item> items) {
            var progression = new Progression(items);
            return locations.Where(l => l.Available(progression));
        }

        /* public static IEnumerable<Location> Available(this IEnumerable<Location> locations, IEnumerable<Item> items, bool restrictWorld = false) {
            var progression = new Progression(items);
            if (restrictWorld) {
                return locations.Where(l => l.Available(progression.Where(i => i.World = l.Region.World)));
            }
            else {
                return locations.Where(l => l.Available(progression));
            }
        } */

        public static IEnumerable<Location> CanFillWithinWorld(this IEnumerable<Location> locations, Item item, IEnumerable<Item> items) {
            var itemWorldProgression = new Progression(items.Where(i => i.World == item.World).Append(item));
            var worldProgression = locations.Select(x => x.Region.World).Distinct().ToDictionary(world => world.Id, world => new Progression(items.Where(i => i.World == world)));

            return locations.Where(l =>
                l.CanFill(item, worldProgression[l.Region.World.Id]) &&
                item.World.Locations.Find(ll => ll.Id == l.Id).Available(itemWorldProgression));
        }
    }
}
