using System;
using System.Collections.Generic;
using System.Linq;
using Randomizer.Shared.Contracts;
using static Randomizer.SuperMetroid.ItemType;
using Newtonsoft.Json;

namespace Randomizer.SuperMetroid {

    /* TODO: This is currently very heavy; could use improvement */
    class Spoiler {

        readonly List<IWorldData> worlds;
        readonly Config config;
        readonly IDictionary<string, string> options;
        readonly Dictionary<int, IItemTypeData> items;
        readonly Dictionary<int, ILocationTypeData> locations;

        public Spoiler(List<IWorldData> worlds, Config config, IDictionary<string, string> opts, Dictionary<int, IItemTypeData> items, Dictionary<int, ILocationTypeData> locations) {
            this.worlds    = worlds;
            this.config    = config;
            this.options   = opts;
            this.items     = items;
            this.locations = locations;
        }

        public class SpoilerLocationData {
            public int LocationId { get; set; }
            public string LocationName { get; set; }
            public string LocationType { get; set; }
            public string LocationArea { get; set; }
            public string LocationRegion { get; set; }
            public int ItemId { get; set; }
            public string ItemName { get; set; }
            public int WorldId { get; set; }
            public int ItemWorldId { get; set; }
        }

        public class SpoilerData {
            public IDictionary<string, string> Options { get; set; }
            public Dictionary<string, string> Config { get; set; }
            public List<Dictionary<string, string>> Playthrough { get; set; }
            public List<SpoilerLocationData> Locations { get; set; }
        }

        public SpoilerData Generate(List<Dictionary<string, string>> playthru, Dictionary<string, string> conf) {
            var spoilerLocationData = new List<SpoilerLocationData>();
            foreach (var world in worlds) {
                foreach(var location in world.Locations) {
                    spoilerLocationData.Add(new SpoilerLocationData {
                        LocationId = location.LocationId,
                        LocationName = locations[location.LocationId].Name,
                        LocationType = locations[location.LocationId].Type,
                        LocationRegion = locations[location.LocationId].Region,
                        LocationArea = locations[location.LocationId].Area,

                        ItemId = location.ItemId,
                        ItemName = items[location.ItemId].Name,

                        WorldId = world.Id,
                        ItemWorldId = location.ItemWorldId,
                    });
                }
            }

            // Over-write these values with something more readable
            conf["RandomCards"] = config.GetRandomCardsAsString();

            return new SpoilerData { Options = options, Config = conf, Playthrough = playthru, Locations = spoilerLocationData };
        }
    }
}
