using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Randomizer.Shared.Contracts;

namespace Randomizer.SMZ3 {

    [DefaultValue(Normal)]
    public enum GameMode {
        [Description("Single player")]
        Normal,
        [Description("Multiworld")]
        Multiworld
    }

    // TODO: Medium/Hard are actually "advanced item placement"; change them
    [DefaultValue(Normal)]
    public enum Z3Logic {
        [Description("Normal")]
        Normal,
        [Description("Medium")]
        Medium,
        [Description("Hard")]
        Hard,
        [Description("No-Major Glitches")]
        Nmg,
        [Description("Over-World Glitches")]
        Owg,
    }

    [DefaultValue(Normal)]
    public enum SMLogic {
        [Description("Normal")]
        Normal,
        [Description("Medium")]
        Medium,
        [Description("Hard")]
        Hard,
    }

    [DefaultValue(Randomized)]
    public enum SwordLocation {
        [Description("Randomized")]
        Randomized,
        [Description("Early")]
        Early,
        [Description("Uncle assured")]
        Uncle
    }    

    [DefaultValue(Randomized)]
    public enum MorphLocation {
        [Description("Randomized")]
        Randomized,
        [Description("Early")]
        Early,
        [Description("Original location")]
        Original
    }

    [DefaultValue(Empty)]
    public enum BottleContents {
        [Description("Empty")]
        Empty,
        [Description("Randomized")]
        Randomized
    }

    [DefaultValue(DefeatBoth)]
    public enum Goal {
        [Description("Defeat Ganon and Mother Brain")]
        DefeatBoth,
    }

    [DefaultValue(None)]
    public enum KeyShuffle {
        [Description("None")]
        None,
        [Description("Randomized")]
        Randomized,
        [Description("Mystery")]
        Mystery,
        [Description("WithinWorld")]
        WithinWorld,
        [Description("Outsiders")]
        Outsiders,
        [Description("AlienOverlords")]
        AlienOverlords,
        [Description("Keysanity")]
        Keysanity
    }

    [DefaultValue(None)]
    public enum Keycards {
        [Description("None")]
        None,
        [Description("Randomized")]
        Randomized,
        [Description("Mystery")]
        Mystery,
        [Description("Exist")]
        Exist,
        [Description("WithinWorld")]
        WithinWorld,
        [Description("Outsiders")]
        Outsiders,
        [Description("AlienOverlords")]
        AlienOverlords,
        [Description("Keysanity")]
        Keysanity
    }

    public enum RandomizedItemLocation {
        [Description("Home")]
        Home,
        [Description("Local")]
        Local,
        [Description("Away")]
        Away,
        [Description("Anywhere")]
        Anywhere
    }

    [DefaultValue(Randomized)]
    public enum BossDrops {
        [Description("Randomized")]
        Randomized,
        [Description("Non-Dungeon")]
        NonDungeon
    }

    [DefaultValue(Never)]
    public enum GanonInvincible {
        [Description("Never")]
        Never,
        [Description("Before Crystals")]
        BeforeCrystals,
        [Description("Before All Dungeons")]
        BeforeAllDungeons,
        [Description("Always")]
        Always,
    }

    public class Config {
        public int Seed { get; set; }
        public GameMode GameMode { get; set; } = GameMode.Normal;
        public string PlayerName { get; set; } = "Player";
        public string SMControls { get; set; } = "";
        public Z3Logic Z3Logic { get; set; } = Z3Logic.Normal;
        public SMLogic SMLogic { get; set; } = SMLogic.Normal;
        public SwordLocation SwordLocation { get; set; } = SwordLocation.Randomized;
        public MorphLocation MorphLocation { get; set; } = MorphLocation.Randomized;
        public Goal Goal { get; set; } = Goal.DefeatBoth;
        public KeyShuffle KeyShuffle { get; set; } = KeyShuffle.None;
        public bool Keysanity => KeyShuffle != KeyShuffle.None;
        public Dictionary<string,RandomizedItemLocation> RandomKeys { get; set; }
        public Keycards Keycards { get; set; } = Keycards.None;
        public bool UseKeycards => Keycards != Keycards.None;
        public Dictionary<string,RandomizedItemLocation> RandomCards { get; set; }
        public bool ProgressiveBow { get; set; } = false;
        public bool Race { get; set; } = false;
        public BossDrops BossDrops { get; set; } = BossDrops.Randomized;
        public int TowerCrystals { get; set; } = 7;
        public int GanonCrystals { get; set; } = 7;
        public GanonInvincible GanonInvincible { get; set; } = GanonInvincible.BeforeCrystals;
        public BottleContents BottleContents { get; set; } = BottleContents.Empty;
        public bool LiveDangerously { get; set; } = false;
        public bool GoFast { get; set; } = false;
        public bool MysterySeed { get; set; } = false;
        public bool RandomFlyingTiles { get; set; } = false;

        public Config(IDictionary<string, string> options, int randoSeed, Random Rnd) {
            // The things that aren't mystery
            Seed            = randoSeed;
            GameMode        = ParseOption(options, GameMode.Normal);
            MysterySeed     = ParseOption(options, "MysterySeed", false);
            PlayerName      = options.ContainsKey("playername") ? options["playername"] : "Player";
            SMControls      = options.ContainsKey("smcontrols") ? options["smcontrols"] : "";
            Race            = ParseOption(options, "Race", false);

            if (MysterySeed) {
                BossDrops         = Rnd.Next(100) < 80 ? BossDrops.Randomized : BossDrops.NonDungeon;
                BottleContents    = Rnd.Next(100) < 80 ? BottleContents.Randomized : BottleContents.Empty;
                Goal              = ParseOption(options, Goal.DefeatBoth);
                GoFast            = Rnd.Next(100) < 10 ? true : false;
                LiveDangerously   = Rnd.Next(100) < 70 ? true : false;
                ProgressiveBow    = Rnd.Next(100) < 60 ? true : false;
                KeyShuffle        = KeyShuffle.Mystery;
                Keycards          = Keycards.Mystery;
                RandomFlyingTiles = Rnd.Next(100) < 80 ? true : false;

                TowerCrystals     = Rnd.Next(100) < 30 ? Rnd.Next(3, 7) : 7;
                if (TowerCrystals < 7) {
                    GanonCrystals = Rnd.Next(TowerCrystals, 7);
                    // done this way because "Never" or "Always" may be options in the future
                    GanonInvincible = Rnd.Next(100) switch {
                        var n when n < 20 => GanonInvincible.BeforeAllDungeons,
                        _ => GanonInvincible.BeforeCrystals,
                    };
                } else {
                    GanonCrystals = 7;
                    GanonInvincible = GanonInvincible.BeforeCrystals;
                }

                MorphLocation = Rnd.Next(100) switch {
                    var n when n < 5  => MorphLocation.Original,
                    var n when n < 10 => MorphLocation.Early,
                    _ => MorphLocation.Randomized
                };

                SwordLocation = Rnd.Next(100) switch {
                    var n when n < 5  => SwordLocation.Uncle,
                    var n when n < 10 => SwordLocation.Early,
                    _ => SwordLocation.Randomized
                };

                SMLogic = Rnd.Next(100) switch {
                    var n when n < 20 => SMLogic.Hard,
                    var n when n < 40 => SMLogic.Normal,
                    _ => SMLogic.Medium
                };

                Z3Logic = Rnd.Next(100) switch {
                    var n when n < 20 => Z3Logic.Hard,
                    var n when n < 40 => Z3Logic.Normal,
                    _ => Z3Logic.Medium
                };
            } else {
                BossDrops         = ParseOption(options, BossDrops.Randomized);
                BottleContents    = ParseOption(options, BottleContents.Empty);
                Goal              = ParseOption(options, Goal.DefeatBoth);
                GoFast            = ParseOption(options, "GoFast", false);
                LiveDangerously   = ParseOption(options, "LiveDangerously", false);
                RandomFlyingTiles = ParseOption(options, "RandomFlyingTiles", false);
                ProgressiveBow    = ParseOption(options, "ProgressiveBow", false);
                MorphLocation     = ParseOption(options, MorphLocation.Randomized);
                SMLogic           = ParseOption(options, SMLogic.Normal);
                SwordLocation     = ParseOption(options, SwordLocation.Randomized);
                Z3Logic           = ParseOption(options, Z3Logic.Normal);
                TowerCrystals     = ParseOption(options, "TowerCrystals", 7);
                GanonCrystals     = ParseOption(options, "GanonCrystals", 7);
                GanonInvincible   = ParseOption(options, GanonInvincible.BeforeCrystals);

                // Have to be parsed in order (dependency)
                KeyShuffle      = ParseOption(options, KeyShuffle.None);
                Keycards        = ParseOption(options, Keysanity ? Keycards.Keysanity : Keycards.None);
            }

            while (KeyShuffle == KeyShuffle.Mystery) {
                Array orig_shuffle_types = Enum.GetValues(typeof(KeyShuffle));
                Array shuffle_types = Array.CreateInstance(typeof(KeyShuffle), orig_shuffle_types.Length + 6);
                Array.Copy(orig_shuffle_types, shuffle_types, orig_shuffle_types.Length);
                for (int i = 0; i < (shuffle_types.Length - orig_shuffle_types.Length); i++)
                    shuffle_types.SetValue(KeyShuffle.Mystery, orig_shuffle_types.Length + i);
                KeyShuffle = (KeyShuffle)shuffle_types.GetValue(Rnd.Next(shuffle_types.Length));
            }

            if (KeyShuffle == KeyShuffle.Randomized) {
                Array rando_types = Enum.GetValues(typeof(RandomizedItemLocation));
                RandomKeys = new Dictionary<string, RandomizedItemLocation>() {
                    {"map", (RandomizedItemLocation)rando_types.GetValue(Rnd.Next(rando_types.Length))},
                    {"compass", (RandomizedItemLocation)rando_types.GetValue(Rnd.Next(rando_types.Length))},
                    {"small_key", (RandomizedItemLocation)rando_types.GetValue(Rnd.Next(rando_types.Length))},
                    {"big_key", (RandomizedItemLocation)rando_types.GetValue(Rnd.Next(rando_types.Length))}
                };
            }

            while (Keycards == Keycards.Mystery) {
                Array orig_card_types = Enum.GetValues(typeof(Keycards));
                Array card_types = Array.CreateInstance(typeof(Keycards), orig_card_types.Length + 6);
                Array.Copy(orig_card_types, card_types, orig_card_types.Length);
                for (int i = 0; i < (card_types.Length - orig_card_types.Length); i++)
                    card_types.SetValue(Keycards.Mystery, orig_card_types.Length + i);
                Keycards = (Keycards)card_types.GetValue(Rnd.Next(card_types.Length));
            }

            if (Keycards == Keycards.Randomized) {
                Array rando_types = Enum.GetValues(typeof(RandomizedItemLocation));
                RandomCards = new Dictionary<string, RandomizedItemLocation>() {
                    {"one", (RandomizedItemLocation)rando_types.GetValue(Rnd.Next(rando_types.Length))},
                    {"two", (RandomizedItemLocation)rando_types.GetValue(Rnd.Next(rando_types.Length))},
                    {"boss", (RandomizedItemLocation)rando_types.GetValue(Rnd.Next(rando_types.Length))}
                };
            }
        }

        private TEnum ParseOption<TEnum>(IDictionary<string, string> options, TEnum defaultValue) where TEnum: Enum {
            string enumKey = typeof(TEnum).Name.ToLower();
            if (options.ContainsKey(enumKey)) {
                if (Enum.TryParse(typeof(TEnum), options[enumKey], true, out object enumValue)) {
                    return (TEnum)enumValue;
                }
            }
            return defaultValue;
        }

        private T ParseOption<T>(IDictionary<string, string> options, string option, T defaultValue) {
            if (options.ContainsKey(option.ToLower())) {
                return (T) Convert.ChangeType(options[option.ToLower()], typeof(T));
            } else {
                return defaultValue;
            }
        }

        public string GetRandomCardsAsString () {
            if (RandomCards == null)
                return "{}";

            return "{" + string.Join(",", RandomCards.Select(kv => kv.Key + "=" + kv.Value)) + "}";
        }

        public string GetRandomKeysAsString () {
            if (RandomKeys == null)
                return "{}";

            return "{" + string.Join(",", RandomKeys.Select(kv => kv.Key + "=" + kv.Value)) + "}";
        }

        public static RandomizerOption GetRandomizerOption<T>(string description, string defaultOption = "") where T : Enum {
            var enumType = typeof(T);
            var values = Enum.GetValues(enumType).Cast<Enum>();

            return new RandomizerOption {
                Key = enumType.Name.ToLower(),
                Description = description,
                Type = RandomizerOptionType.Dropdown,
                Default = string.IsNullOrEmpty(defaultOption) ? GetDefaultValue<T>().ToLString() : defaultOption,
                Values = values.ToDictionary(k => k.ToLString(), v => v.GetDescription())
            };
        }

        public static RandomizerOption GetRandomizerOption(string name, string description, bool defaultOption = false) {
            return new RandomizerOption {
                Key = name.ToLower(),
                Description = description,
                Type = RandomizerOptionType.Checkbox,
                Default = defaultOption.ToString().ToLower(),
                Values = new Dictionary<string, string>()
            };
        }

        public static TEnum GetDefaultValue<TEnum>() where TEnum : Enum {
            Type t = typeof(TEnum);
            var attributes = (DefaultValueAttribute[])t.GetCustomAttributes(typeof(DefaultValueAttribute), false);
            if ((attributes?.Length ?? 0) > 0) {
                return (TEnum)attributes.First().Value;
            }
            else {
                return default;
            }
        }
    }
}
