using System.Collections.Generic;
using System.ComponentModel;

namespace Randomizer.SMZ3 {

    enum RewardType {
        [Description("None")]
        None,
        [Description("Agahnim")]
        Agahnim,
        [Description("Green Pendant")]
        PendantGreen,
        [Description("Blue/Red Pendant")]
        PendantNonGreen,
        [Description("Blue Crystal")]
        CrystalBlue,
        [Description("Red Crystal")]
        CrystalRed,
        [Description("Golden Four Boss")]
        GoldenFourBoss
    }

    interface IReward {
        RewardType Reward { get; set; }
        bool CanComplete(Progression items);
    }

    interface IMedallionAccess {
        ItemType Medallion { get; set; }
    }

    abstract class SMRegion : Region {
        public SMLogic Logic => Config.SMLogic;
        public SMRegion(World world, Config config) : base(world, config) { }
    }

    abstract class Z3Region : Region {
        public Z3Logic Logic => Config.Z3Logic;
        public Z3Region(World world, Config config) : base(world, config) { }
    }

    abstract class Region {

        public virtual string Name { get; }
        public virtual string Area => Name;

        public List<Location> Locations { get; set; }
        public World World { get; set; }
        public int Weight { get; set; } = 0;

        public Config Config { get; set; }
        protected IList<ItemType> RegionItems { get; set; } = new List<ItemType>();

        public Region(World world, Config config) {
            Config = config;
            World = world;
        }

        public bool IsRegionItem(Item item) {
            return RegionItems.Contains(item.Type);
        }

        public virtual bool CanFill(Item item, Progression items) {
            if (!item.IsDungeonItem && !item.IsKeycard) {
                return true;
            }

            if (item.IsKeycard) {
                if (Config.UseKeycards) {
                    if (Config.Keycards == Keycards.Keysanity) {
                        return true;
                    } else if (Config.Keycards == Keycards.WithinWorld) {
                        return this is SMRegion;
                    } else if (Config.Keycards == Keycards.Outsiders) {
                        return this is Z3Region;
                    } else if (Config.Keycards == Keycards.AlienOverlords) {
                        if (item.IsBossKeycard)
                            return this is Z3Region;
                        return this is SMRegion;
                    } else if (Config.Keycards == Keycards.Randomized) {
                        if (item.IsBossKeycard) {
                            return Config.RandomCards["boss"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                RandomizedItemLocation.Local => this is Z3Region,
                                RandomizedItemLocation.Away  => this is SMRegion,
                                _ => true
                            };
                        } else if (item.IsCardOne) {
                            return Config.RandomCards["one"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                RandomizedItemLocation.Local => this is Z3Region,
                                RandomizedItemLocation.Away  => this is SMRegion,
                                _ => true
                            };
                        } else if (item.IsCardTwo) {
                            return Config.RandomCards["two"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                RandomizedItemLocation.Local => this is Z3Region,
                                RandomizedItemLocation.Away  => this is SMRegion,
                                _ => true
                            };
                        } else {
                            return Config.RandomCards["area"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                RandomizedItemLocation.Local => this is Z3Region,
                                RandomizedItemLocation.Away  => this is SMRegion,
                                _ => true
                            };
                        }
                    }
                }

                // "Exist" just falls through here
                return IsRegionItem(item);
            } else if (item.IsDungeonItem) {
                if (Config.Keysanity) {
                    if (Config.KeyShuffle == KeyShuffle.Keysanity) {
                        return true;
                    } else if (Config.KeyShuffle == KeyShuffle.WithinWorld) {
                        return this is Z3Region;
                    } else if (Config.KeyShuffle == KeyShuffle.Outsiders) {
                        // maps/compasses inside dungeons, everything else everywhere
                        if (item.IsKey || item.IsBigKey)
                            return this is SMRegion;
                        return true;
                    } else if (Config.KeyShuffle == KeyShuffle.AlienOverlords) {
                        if (item.IsBigKey)
                            return this is SMRegion;
                        return this is Z3Region;
                    } else if (Config.KeyShuffle == KeyShuffle.Randomized) {
                        if (item.IsMap) {
                            return Config.RandomKeys["map"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                RandomizedItemLocation.Local => this is SMRegion,
                                RandomizedItemLocation.Away  => this is Z3Region,
                                _ => true
                            };
                        } else if (item.IsCompass) {
                            return Config.RandomKeys["compass"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                RandomizedItemLocation.Local => this is SMRegion,
                                RandomizedItemLocation.Away  => this is Z3Region,
                                _ => true
                            };
                        } else if (item.IsKey) {
                            return Config.RandomKeys["small_key"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                RandomizedItemLocation.Local => this is SMRegion,
                                RandomizedItemLocation.Away  => this is Z3Region,
                                _ => true
                            };
                        } else if (item.IsBigKey) {
                            return Config.RandomKeys["big_key"] switch {
                                RandomizedItemLocation.Home  => IsRegionItem(item),
                                RandomizedItemLocation.Local => this is SMRegion,
                                RandomizedItemLocation.Away  => this is Z3Region,
                                _ => true
                            };
                        }
                    }
                }

                return IsRegionItem(item);

                // This was the original line (for reference)
                // return Config.Keysanity || !item.IsDungeonItem || IsRegionItem(item);
            }

            return true;
        }

        public virtual bool CanEnter(Progression items) {
            return true;
        }

    }

}
