using Randomizer.Shared.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Randomizer.SuperMetroid {

    [DefaultValue(Normal)]
    public enum GameMode {
        [Description("Single player")]
        Normal,
        [Description("Multiworld")]
        Multiworld
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

    [DefaultValue(Split)]
    public enum Placement {
        [Description("Full randomization")]
        Full,
        [Description("Major/Minor split")]
        Split
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

    [DefaultValue(DefeatMB)]
    public enum Goal {
        [Description("Defeat Mother Brain")]
        DefeatMB,
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
        [Description("Keysanity")]
        Keysanity
    }

    public enum RandomizedItemLocation {
        [Description("Home")]
        Home,
        [Description("Anywhere")]
        Anywhere
    }

    public class Config {
        public GameMode GameMode { get; set; } = GameMode.Normal;
        public SMLogic SMLogic { get; set; } = SMLogic.Normal;
        public MorphLocation MorphLocation { get; set; } = MorphLocation.Randomized;
        public Goal Goal { get; set; } = Goal.DefeatMB;
        public Placement Placement { get; set; } = Placement.Split;
        public bool Race { get; set; } = false;
        public bool GoFast { get; set; } = false;
        public bool LiveDangerously { get; set; } = false;
        public Keycards Keycards { get; set; } = Keycards.None;
        public bool UseKeycards => Keycards != Keycards.None;
        public Dictionary<string,RandomizedItemLocation> RandomCards { get; set; }
        public bool MysterySeed { get; set; } = false;

        public Config(IDictionary<string, string> options, Random Rnd) {
            GameMode        = ParseOption(options, GameMode.Normal);
            MysterySeed     = ParseOption(options, "MysterySeed", false);
            Race            = ParseOption(options, "Race", false);

            if (MysterySeed) {
                Goal            = ParseOption(options, Goal.DefeatMB);
                GoFast          = Rnd.Next(100) < 10 ? true : false;
                LiveDangerously = Rnd.Next(100) < 70 ? true : false;
                Keycards        = Keycards.Mystery;

                Placement = Rnd.Next(10) switch {
                    var n when n < 3 => Placement.Split,
                    _ => Placement.Full
                };

                MorphLocation = Rnd.Next(100) switch {
                    var n when n < 5  => MorphLocation.Original,
                    var n when n < 10 => MorphLocation.Early,
                    _ => MorphLocation.Randomized
                };

                SMLogic = Rnd.Next(100) switch {
                    var n when n < 20 => SMLogic.Hard,
                    var n when n < 40 => SMLogic.Normal,
                    _ => SMLogic.Medium
                };
            } else {
                Goal            = ParseOption(options, Goal.DefeatMB);
                GoFast          = ParseOption(options, "GoFast", false);
                Keycards        = ParseOption(options, Keycards.Keysanity);
                LiveDangerously = ParseOption(options, "LiveDangerously", false);
                MorphLocation   = ParseOption(options, MorphLocation.Randomized);
                Placement       = ParseOption(options, Placement.Split);
                SMLogic         = ParseOption(options, SMLogic.Normal);
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

        private TEnum ParseOption<TEnum>(IDictionary<string, string> options, TEnum defaultValue) where TEnum : Enum {
            string enumKey = typeof(TEnum).Name.ToLower();
            if (options.ContainsKey(enumKey)) {
                if (Enum.TryParse(typeof(TEnum), options[enumKey], true, out object enumValue)) {
                    return (TEnum)enumValue;
                }
            }
            return defaultValue;
        }

        private bool ParseOption(IDictionary<string, string> options, string option, bool defaultValue) {
            if (options.ContainsKey(option.ToLower())) {
                return bool.Parse(options[option.ToLower()]);
            }
            else {
                return defaultValue;
            }
        }

        public string GetRandomCardsAsString () {
            if (RandomCards == null)
                return "{}";

            return "{" + string.Join(",", RandomCards.Select(kv => kv.Key + "=" + kv.Value)) + "}";
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
