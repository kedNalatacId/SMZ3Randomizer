using System.Collections.Generic;
using System.ComponentModel;

namespace Randomizer.SuperMetroid {

    abstract class Region {

        public virtual string Name { get; }
        public virtual string Area { get; }

        public List<Location> Locations { get; set; }
        public World World { get; set; }
        public int Weight { get; set; } = 0;

        public Config Config { get; set; }
        protected IList<ItemType> RegionItems { get; set; } = new List<ItemType>();

        public SMLogic Logic => Config.SMLogic;

        public Region(World world, Config config) {
            Config = config;
            World = world;
        }

        public bool IsRegionItem(Item item) {
            return RegionItems.Contains(item.Type);
        }

        public virtual bool CanEnter(Progression items) {
            return true;
        }

        public virtual bool CanFill(Item item, Progression items) {
            if (!item.IsKeycard) {
                return true;
            } else {
                if (Config.UseKeycards) {
                    if (Config.Keycards == Keycards.Keysanity) {
                        return true;
                    } else if (Config.Keycards == Keycards.Randomized) {
                        if (item.IsBossKeycard) {
                            return Config.RandomCards["boss"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                _ => true
                            };
                        } else if (item.IsCardOne) {
                            return Config.RandomCards["one"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                _ => true
                            };
                        } else if (item.IsCardTwo) {
                            return Config.RandomCards["two"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                _ => true
                            };
                        } else {
                            return Config.RandomCards["area"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                _ => true
                            };
                        }
                    }
                }

                // "Exist" just falls through here
                return IsRegionItem(item);
            }
        }
    }
}
