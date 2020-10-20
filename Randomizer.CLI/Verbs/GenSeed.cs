using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using Randomizer.CLI.FileData;
using Randomizer.Shared.Contracts;
using static Randomizer.CLI.FileHelper;

namespace Randomizer.CLI.Verbs {

    abstract class GenSeedOptions {

        [Option('c', "config",
            Default  = @".\seed_options.json",
            HelpText = "Options Config File")]
        public string ConfigFile { get; set; }

        [Option(
            HelpText = "Generate a singleworld mode seed (defaults to single)")]
        public bool Multi { get; set; }
        public bool Single => !Multi;

        [Option('p', "players", Default = 1,
            HelpText = "The number of players for seeds")]
        public int Players { get; set; }

        [Option("smlogic",
            Default="Normal",
            HelpText = "Super Metroid Logic (default is Normal)")]
        public string SMLogic { get; set; }

        [Option('r',"race",
            HelpText = "Whether this is a Race seed or not (default is false)")]
        public bool Race { get; set; }

        [Option('l', "loop",
            HelpText = "Generate seeds repeatedly")]
        public bool Loop { get; set; }

        [Option('s', "seed",
            HelpText = "Generate a specific seed")]
        public string Seed { get; set; } = string.Empty;

        [Option("rom",
            HelpText = "Compile rom file of the first world for each seed. Use the ips option to provide all required IPS patchs.")]
        public bool Rom { get; set; }

        [Option('o', "output",
            HelpText = "Where to write the resultant files")]
        public string OutputFile { get; set; }

        [Option(
            HelpText = "Specify paths for IPS patches to be applied in the specified order.")]
        public IEnumerable<string> Ips { get; set; }

        [Option(
            HelpText = "Specify paths for RDC resources to be applied in the specified order.")]
        public IEnumerable<string> Rdc { get; set; }

        [Option('t', "terminal",
            HelpText = "Write patch and playthrough to the console instead of directly to files")]
        public bool ToConsole { get; set; }

        [Option(
            HelpText = "Show json formatted playthrough for each seed")]
        public bool Playthrough { get; set; }

        [Option(
            HelpText = "Show json formatted spoiler for each seed (turns Playthrough off)")]
        public bool Spoiler { get; set; }

        [Option(
            HelpText = "Show json formated patch for each world in the seed")]
        public bool Patch { get; set; }

        [Option("smfile",
            Default = @".\Super_Metroid_JU_.sfc",
            HelpText = "Super Metroid ROM File")]
        public string smFile { get; set; }

        [Option('v', "verbose",
            Default = 0,
            HelpText = "Verbosity level; defaults to 0 (off)")]
        public int Verbose { get; set; }

        public abstract IRandomizer NewRandomizer();

        public byte[] BaseRom { get; set; }
    }

    [Verb("sm", HelpText = "Generate Super Metroid seeds")]
    class SMSeedOptions : GenSeedOptions {

        [Option(
            Default = "split",
            HelpText = "Generate seeds with either full or split randomization (default is split)")]
        public string Placement { get; set; }

        [Option('g',"goal",
            Default="DefeatMB",
            HelpText = "Goal of Seed (default is DefeatMB)")]
        public string Goal { get; set; }

        public override IRandomizer NewRandomizer() => new SuperMetroid.Randomizer();

        public SMSeedOptions() { }

    }

    [Verb("smz3", HelpText = "Generate SMZ3 combo seeds")]
    class SMZ3SeedOptions : GenSeedOptions {

        [Option("z3logic",
            Default="Normal",
            HelpText = "Zelda logic (default is Normal)")]
        public string Z3Logic { get; set; }

        [Option("sword",
            Default="Randomized",
            HelpText = "Zelda Sword Location (default is Randomized)")]
        public string SwordLocation { get; set; }

        [Option("morph",
            Default="Randomized",
            HelpText = "SM Morph Ball Location (default is Randomized)")]
        public string MorphLocation { get; set; }

        [Option('g',"goal",
            Default="DefeatBoth",
            HelpText = "Goal of Seed (default is DefeatBoth)")]
        public string Goal { get; set; }

        [Option('k', "keyshuffle",
            Default="none",
            HelpText = "What level of key shuffle to use (default is None)")]
        public string KeyShuffle { get; set; }

        [Option("keycards",
            Default="yes",
            HelpText = "Whether to use KeyCards with Keysanity or not (yes or no; default is yes)")]
        public string Keycards { get; set; }

        [Option("ganoninvincible",
            Default="Never",
            HelpText = "Whether Ganon is invincible at any time (default is Never)")]
        public string GanonInvincible { get; set; }

        [Option("z3file",
            Default = @".\Zelda_no_Densetsu_-_Kamigami_no_Triforce_Japan.sfc",
            HelpText = "Zelda: ALTTP ROM File")]
        public string z3File { get; set; }

        public SMZ3SeedOptions() { }

        public override IRandomizer NewRandomizer() => new SMZ3.Randomizer();

    }

    static class GenSeed {
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
            public List<Dictionary<string, string>> Playthrough { get; set; }
            public List<SpoilerLocationData> Locations { get; set; }
        }

        public static void Run(GenSeedOptions opts) {
            if (!String.IsNullOrEmpty(opts.ConfigFile) && File.Exists(opts.ConfigFile)) {
                ParseConfig(opts);
            }

            if (opts.Players < 1 || opts.Players > 64)
                throw new ArgumentOutOfRangeException("players", "The players parameter must fall within the range 1-64");

            var optionList = new List<(string, string)> {
                ("gamemode", opts.Multi ? "multiworld" : "normal"),
                ("players", opts.Players.ToString()),
                ("race", opts.Race.ToString()),
                ("smlogic", opts.SMLogic),
            };

            if (opts is SMZ3SeedOptions smz3) {
                optionList.AddRange(new[] {
                    ("z3logic", smz3.Z3Logic),
                    ("swordlocation", smz3.SwordLocation),
                    ("morphlocation", smz3.MorphLocation),
                    ("keyshuffle", smz3.KeyShuffle),
                    ("keycards", smz3.Keycards),
                    ("ganoninvincible", smz3.GanonInvincible),
                    ("goal", smz3.Goal),
                });
            }
            if (opts is SMSeedOptions sm) {
                optionList.AddRange(new[] {
                    ("placement", sm.Placement),
                    ("goal", sm.Goal),
                });
            }

            var players = from n in Enumerable.Range(0, opts.Players)
                          select ($"player-{n}", $"Player {n + 1}");
            var options = optionList.Concat(players).ToDictionary(x => x.Item1, x => x.Item2);

            try {
                while (true) {
                    MakeSeed(options, opts);
                    if (!opts.Loop) break;
                }
            } catch (Exception e) {
                Console.Error.WriteLine(e.Message);
            }
        }

        static void MakeSeed(Dictionary<string, string> options, GenSeedOptions opts) {
            var rando = opts.NewRandomizer();
            var start = DateTime.Now;
            var data = rando.GenerateSeed(options, opts.Seed);
            var end = DateTime.Now;
            var world = data.Worlds.First();
            var filename = ComposeFilename(rando, opts, data.Seed, world.Player);

            Console.WriteLine(string.Join(" - ",
                $"Generated seed: {data.Seed}",
                $"Players: {options["players"]}",
                $"Spheres: {data.Playthrough.Count}",
                $"Generation time: {end - start}"
            ));

            if (opts.Rom) {
                try {
                    ConstructRom(opts);
                    var rom = opts.BaseRom;
                    Rom.ApplySeed(rom, world.Patches);
                    AdditionalPatches(rom, opts.Ips.Skip(1));
                    ApplyRdcResources(rom, opts.Rdc);
                    File.WriteAllBytes($"{filename}.sfc", rom);
                } catch (Exception e) {
                    Console.Error.WriteLine(e.Message);
                }
            }
            if (opts.Playthrough || opts.Spoiler) {
                var text = "";
                if (opts.Spoiler) {
                    text = JsonConvert.SerializeObject(GenerateSpoiler(data, rando), Formatting.Indented);
                } else {
                    text = JsonConvert.SerializeObject(data.Playthrough, Formatting.Indented);
                }
                if (opts.ToConsole) {
                    Console.WriteLine(text);
                    Console.ReadLine();
                } else {
                    string data_type = opts.Spoiler ? "spoiler" : "playthrough";
                    File.WriteAllText($"{filename}-{data_type}.json", text);
                }
            }
            if (opts.Patch) {
                var text = JsonConvert.SerializeObject(
                    data.Worlds.ToDictionary(x => x.Player, x => x.Patches), Formatting.Indented,
                    new PatchWriteConverter()
                );
                if (opts.ToConsole) {
                    Console.WriteLine(text);
                    Console.ReadLine();
                } else {
                    File.WriteAllText($"{filename}-patch.json", text);
                }
            }
        }

        static void AdditionalPatches(byte[] rom, IEnumerable<string> ips) {
            foreach (var patch in ips) {
                using var stream = OpenReadInnerStream(patch);
                Rom.ApplyIps(rom, stream);
            }
        }

        static void ApplyRdcResources(byte[] rom, IEnumerable<string> rdc) {
            foreach (var resource in rdc) {
                using var stream = OpenReadInnerStream(resource);
                var content = Rdc.Parse(stream);
                if (content.TryParse<LinkSprite>(stream, out var block))
                    (block as DataBlock)?.Apply(rom);
                if (content.TryParse<SamusSprite>(stream, out block))
                    (block as DataBlock)?.Apply(rom);
            }
        }

        private static void ConstructRom(GenSeedOptions opts) {
            if (opts.BaseRom != null)
                return;

            Lazy<byte[]> fullRom;
            if (opts is SMSeedOptions sm) {
                fullRom = new Lazy<byte[]>(() => {
                    using var ips = OpenReadInnerStream(opts.Ips.First());
                    var rom = File.ReadAllBytes(opts.smFile);
                    FileData.Rom.ApplyIps(rom, ips);
                    return rom;
                });
            } else if (opts is SMZ3SeedOptions smz3) {
                fullRom = new Lazy<byte[]>(() => {
                    using var sm = File.OpenRead(opts.smFile);
                    using var z3 = File.OpenRead(smz3.z3File);
                    using var ips = OpenReadInnerStream(opts.Ips.First());
                    var rom = FileData.Rom.CombineSMZ3Rom(sm, z3);
                    FileData.Rom.ApplyIps(rom, ips);
                    return rom;
                });
            } else {
                throw new Exception("Only Seed Modes available are 'sm' and 'smz3'");
            }

            opts.BaseRom = (byte[]) fullRom.Value.Clone();
        }

        /* TODO -- this is ugly as sin */
        private static void ParseConfig(GenSeedOptions opts) {
            Dictionary<string, object> conf = new Dictionary<string, object>();

            using (StreamReader r = File.OpenText(opts.ConfigFile)) {
                string json = r.ReadToEnd();
                conf = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            }

            opts.Ips = conf.ContainsKey("Ips")
                ? ((Newtonsoft.Json.Linq.JArray)conf["Ips"]).ToObject<List<string>>()
                : new List<string>{};
            opts.Rdc = conf.ContainsKey("Rdc")
                ? ((Newtonsoft.Json.Linq.JArray)conf["Rdc"]).ToObject<List<string>>()
                : new List<string>{};

            opts.Multi = conf.ContainsKey("Multi") ? (bool)conf["Multi"] : false;
            opts.Players = conf.ContainsKey("Players") ? (int)conf["Players"] : 1;
            opts.Race = conf.ContainsKey("Race") ? (bool)conf["Race"] : false;
            opts.Rom = conf.ContainsKey("Rom") ? (bool)conf["Rom"] : false;
            opts.OutputFile = conf.ContainsKey("OutputFile") ? (string)conf["OutputFile"] : "";
            opts.Playthrough = conf.ContainsKey("Playthrough") ? (bool)conf["Playthrough"] : false;
            opts.Spoiler = conf.ContainsKey("Spoiler") ? (bool)conf["Spoiler"] : false;
            opts.smFile = conf.ContainsKey("smFile") ? (string)conf["smFile"] : "";
            opts.Verbose = conf.ContainsKey("Verbose") ? (int)conf["Verbose"] : 0;

            if (opts is SMZ3SeedOptions smz3) {
                smz3.SwordLocation = conf.ContainsKey("SwordLocation") ? (string)conf["SwordLocation"] : "Randomized";
                smz3.MorphLocation = conf.ContainsKey("MorphLocation") ? (string)conf["Morphlocation"] : "Randomized";
                smz3.Goal = conf.ContainsKey("Goal") ? (string)conf["Goal"] : "DefeatBoth";
                smz3.KeyShuffle = conf.ContainsKey("KeyShuffle") ? (string)conf["KeyShuffle"] : "none";
                smz3.Keycards = conf.ContainsKey("Keycards") ? (string)conf["Keycards"] : "yes";
                smz3.GanonInvincible = conf.ContainsKey("GanonInvincible") ? (string)conf["GanonInvincible"] : "Never";
                smz3.z3File = conf.ContainsKey("z3File") ? (string)conf["z3File"] : "";
            }

            if (opts is SMSeedOptions sm) {
                sm.Goal = conf.ContainsKey("Goal") ? (string)conf["Goal"] : "DefeatMB";
                sm.Placement = conf.ContainsKey("Placement") ? (string)conf["Placement"] : "split";
            }
        }

        static SpoilerData GenerateSpoiler(ISeedData seedData, IRandomizer randomizer) {
            var itemData = randomizer.GetItems();
            var locationData = randomizer.GetLocations();

            var spoilerLocationData = new List<SpoilerLocationData>();
            foreach(var world in seedData.Worlds) {
                foreach(var location in world.Locations) {
                    spoilerLocationData.Add(new SpoilerLocationData {
                        LocationId = location.LocationId,
                        LocationName = locationData[location.LocationId].Name,
                        LocationType = locationData[location.LocationId].Type,
                        LocationRegion = locationData[location.LocationId].Region,
                        LocationArea = locationData[location.LocationId].Area,

                        ItemId = location.ItemId,
                        ItemName = itemData[location.ItemId].Name,

                        WorldId = world.Id,
                        ItemWorldId = location.ItemWorldId,
                    });
                }
            }

            return new SpoilerData { Playthrough = seedData.Playthrough, Locations = spoilerLocationData };
        }

        static string ComposeFilename(IRandomizer rando, GenSeedOptions opts, string seed, string player) {
            if (!String.IsNullOrEmpty(opts.OutputFile))
                return opts.OutputFile;

            var parts = new[] {
                new[] {
                    rando.Id.ToUpper(),
                    $"V{rando.Version}",
                },
                opts is SMZ3SeedOptions smz3 ? new[] {
                    $"ZLn+SL{smz3.SMLogic[0]}",
                    smz3.SwordLocation != "randomized" ? $"S{smz3.SwordLocation[0]}" : null,
                    smz3.MorphLocation != "randomized" ? $"M{smz3.MorphLocation[0]}" : null,
                    smz3.KeyShuffle == "keysanity" ? "Kk" : null,
                    smz3.Keycards == "yes" ? "Cc" : null,
                } : opts is SMSeedOptions sm ? new[] {
                    $"L{sm.SMLogic[0]}",
                    $"I{sm.Placement[0]}",
                } : new string[] { },
                new[] {
                    seed,
                    opts.Multi ? player : null,
                }
            };
            /* Flatten, then keep non-null parts */
            return string.Join("-", parts.SelectMany(x => x).Where(x => x != null));
        }

        public class PatchWriteConverter : JsonConverter<IDictionary<int, byte[]>> {

            public override void WriteJson(JsonWriter writer, IDictionary<int, byte[]> value, JsonSerializer serializer) {
                writer.WriteStartObject();
                foreach (var (address, bytes) in value) {
                    var width = address > 0x00FFFFFF ? 8 : address > 0x0000FFFF ? 6 : 4;
                    writer.WritePropertyName(address.ToString($"X{width}"));
                    writer.WriteValue(string.Join("", bytes.Select(b => b.ToString("X2"))));
                }
                writer.WriteEndObject();
            }

            #region Can not read

            public override bool CanRead => false;

            public override IDictionary<int, byte[]> ReadJson(JsonReader reader, Type objectType, IDictionary<int, byte[]> existingValue, bool hasExistingValue, JsonSerializer serializer) {
                throw new NotImplementedException("This converter can not read data");
            }

            #endregion

        }

    }

}
