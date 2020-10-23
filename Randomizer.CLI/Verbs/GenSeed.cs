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

        [Option('m', "multi",
            HelpText = "Generate a multiworld mode seed (defaults to single)")]
        public bool Multi { get; set; }

        [Option('p', "players", Default = 1,
            HelpText = "The number of players for seeds")]
        public int Players { get; set; }

        [Option("smlogic",
            Default="Normal",
            HelpText = "SM logic (default is Normal)")]
        public string SMLogic { get; set; }

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
            Default="None",
            HelpText = "What level of key shuffle to use (default is None)")]
        public string KeyShuffle { get; set; }

        [Option("keycards",
            Default="None",
            HelpText = "Whether to use KeyCards with Keysanity or not (default is None)")]
        public string Keycards { get; set; }

        [Option('r',"race",
            Default=false,
            HelpText = "Whether this is a Race seed or not (default is false)")]
        public bool Race { get; set; }

        [Option("ganoninvincible",
            Default="Never",
            HelpText = "Whether Ganon is invincible at any time (default is Never)")]
        public string GanonInvincible { get; set; }

        [Option('l', "loop",
            HelpText = "Generate seeds repeatedly")]
        public bool Loop { get; set; }

        [Option('s', "seed",
            HelpText = "Generate a specific seed")]
        public string Seed { get; set; } = string.Empty;

        [Option("rom",
            HelpText = "Compile rom file of the first world for each seed. Use the ips option to provide all required IPS patchs.")]
        public bool Rom { get; set; }

        [Option(
            HelpText = "Specify paths for IPS patches to be applied in the specified order.")]
        public IEnumerable<string> Ips { get; set; }

        [Option(
            HelpText = "Specify paths for RDC resources to be applied in the specified order.")]
        public IEnumerable<string> Rdc { get; set; }

        [Option(
            HelpText = "Write patch and playthrough to the console instead of directly to files")]
        public bool ToConsole { get; set; }

        [Option(
            HelpText = "Show json formated playthrough for each seed")]
        public bool Playthrough { get; set; }

        [Option(
            HelpText = "Show json formated patch for each world in the seed")]
        public bool Patch { get; set; }

        [Option("smfile",
            Default = @".\Super_Metroid_JU_.sfc",
            HelpText = "Super Metroid ROM File")]
        public string smFile { get; set; }

        [Option("z3file",
            Default = @".\Zelda_no_Densetsu_-_Kamigami_no_Triforce_Japan.sfc",
            HelpText = "Zelda: ALTTP ROM File")]
        public string z3File { get; set; }

        public virtual string LogicName { get; }
        public virtual string LogicValue { get; }
        public virtual string SeedMode { get; }

        public abstract IRandomizer NewRandomizer();

        public byte[] BaseRom { get; set; }
        public bool BaseRomSet { get; set; }

    }

    [Verb("sm", HelpText = "Generate Super Metroid seeds")]
    class SMSeedOptions : GenSeedOptions {

        public override string LogicName     => "logic";
        public override string LogicValue    => this switch {
            var o when o.SMLogic == "Hard"   => "tournament",
            var o when o.SMLogic == "Normal" => "casual",
            _ => "casual",
        };
        public override string SeedMode => "sm";

        public SMSeedOptions() { }

        public override IRandomizer NewRandomizer() => new SuperMetroid.Randomizer();

    }

    [Verb("smz3", HelpText = "Generate SMZ3 combo seeds")]
    class SMZ3SeedOptions : GenSeedOptions {

        public override string LogicName     => "smlogic";
        public override string LogicValue    => this switch {
            var o when o.SMLogic == "Hard"   => "hard",
            var o when o.SMLogic == "Normal" => "normal",
            _ => "Normal"
        };
        public override string SeedMode => "smz3";

        public SMZ3SeedOptions() { }

        public override IRandomizer NewRandomizer() => new SMZ3.Randomizer();

    }

    static class GenSeed {
        public static void Run(GenSeedOptions opts) {
            if (opts.Players < 1 || opts.Players > 64)
                throw new ArgumentOutOfRangeException("players", "The players parameter must fall within the range 1-64");

            var optionList = new[] {
                ("gamemode", opts.Multi ? "multiworld" : "normal"),
                (opts.LogicName, opts.LogicValue),
                ("players", opts.Players.ToString()),
                ("swordlocation", opts.SwordLocation),
                ("morphlocation", opts.MorphLocation),
                ("goal", opts.Goal),
                ("keyshuffle", opts.KeyShuffle),
                ("keycards", opts.Keycards),
                ("race", opts.Race.ToString()),
                ("ganoninvincible", opts.GanonInvincible),
            };
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
            Console.WriteLine(string.Join(" - ",
                $"Generated seed: {data.Seed}",
                $"Players: {options["players"]}",
                $"Spheres: {data.Playthrough.Count}",
                $"Generation time: {end - start}"
            ));
            if (opts.Rom) {
                try {
                    var world = data.Worlds.First();
                    ConstructRom(opts);
                    var rom = opts.BaseRom;
                    Rom.ApplySeed(rom, world.Patches);
                    AdditionalPatches(rom, opts.Ips.Skip(1));
                    ApplyRdcResources(rom, opts.Rdc);
                    File.WriteAllBytes($"{data.Game} {data.Logic} - {data.Seed}{(opts.Multi ? $" - {world.Player}" : "")}.sfc", rom);
                } catch (Exception e) {
                    Console.Error.WriteLine(e.Message);
                }
            }
            if (opts.Playthrough) {
                var text = JsonConvert.SerializeObject(data.Playthrough, Formatting.Indented);
                if (opts.ToConsole) {
                    Console.WriteLine(text);
                    Console.ReadLine();
                } else {
                    File.WriteAllText($"playthrough-{data.Logic}-{data.Seed}.json", text);
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
                    File.WriteAllText($"patch-{data.Logic}-{data.Seed}.json", text);
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
            if (opts.BaseRomSet) {
                return;
            }

            Lazy<byte[]> fullRom;
            if (opts.SeedMode == "sm") {
                fullRom = new Lazy<byte[]>(() => {
                    using var ips = OpenReadInnerStream(opts.Ips.First());
                    var rom = File.ReadAllBytes(opts.smFile);
                    FileData.Rom.ApplyIps(rom, ips);
                    return rom;
                });
            } else if (opts.SeedMode == "smz3") {
                fullRom = new Lazy<byte[]>(() => {
                    using var sm = File.OpenRead(opts.smFile);
                    using var z3 = File.OpenRead(opts.z3File);
                    using var ips = OpenReadInnerStream(opts.Ips.First());
                    var rom = FileData.Rom.CombineSMZ3Rom(sm, z3);
                    FileData.Rom.ApplyIps(rom, ips);
                    return rom;
                });
            } else {
                throw new Exception("Only Seed Modes available are 'sm' and 'smz3'");
            }

            opts.BaseRom = (byte[]) fullRom.Value.Clone();
            opts.BaseRomSet = true;
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
