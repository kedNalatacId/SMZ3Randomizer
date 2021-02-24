using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using CommandLine;
using Newtonsoft.Json;
using Randomizer.CLI.FileData;
using Randomizer.Shared.Contracts;

namespace Randomizer.CLI.Verbs {

    [Verb("sm", HelpText = "Generate Super Metroid seeds")]
    class SMSeedOptions : SMSpecifics {
        public SMSeedOptions() {
            defaults.Add("Placement", "split");
            defaults.Add("Goal", "DefeatMB");
            defaults.Add("MorphLocation","Randomized");
            defaults.Add("Keycards", "None");
        }
    }

    [Verb("smz3", HelpText = "Generate SMZ3 combo seeds")]
    class SMZ3SeedOptions : SMZ3Specifics {
        public SMZ3SeedOptions() {
            defaults.Add("BossDrops", "Randomized");
            defaults.Add("BottleContents", "Empty");
            defaults.Add("ProgressiveBow", false);
            defaults.Add("GanonInvincible", "Never");
            defaults.Add("Goal", "DefeatBoth");
            defaults.Add("KeyShuffle", "None");
            defaults.Add("Keycards", "None");
            defaults.Add("LiveDangerously", false);
            defaults.Add("MorphLocation", "Randomized");
            defaults.Add("RandomFlyingTiles", false);
            defaults.Add("SwordLocation", "Randomized");
            defaults.Add("Z3Logic", "Normal");
        }
    }

    static class GenSeed {
        public static void Run(GenSeedOptions opts) {
            List<(string, string)> optionList = null;

            try {
                optionList = GenSeedOptions.getOptions(opts);
                GenSeedOptions.validateOptions(opts);
            } catch (Exception e) {
                Console.WriteLine("Failed to validate options:\n\t{0}", e.Message);
                Environment.Exit(0);
            }

            var players = from n in Enumerable.Range(0, opts.Players)
                          select ($"player-{n}", $"Player {n + 1}");
            var options = optionList.Concat(players).ToDictionary(x => x.Item1, x => x.Item2);

            try {
                int seeds = (int)opts.Loop;
                int cnt = 0;
                // zero or negative is "infinite"
                while (seeds <= 0 || cnt++ < seeds) {
                    MakeSeed(options, opts);
                }
            } catch (Exception e) {
                Console.Error.WriteLine(e.Message);
            }
        }

        static void MakeSeed(Dictionary<string, string> options, GenSeedOptions opts) {
            ISeedData data = null;
            IRandomizer rando = null;
            bool seed_made = false;
            int seed_fails = 0;

            var start = DateTime.Now;
            while (seed_made == false) {
                try {
                    rando = opts.NewRandomizer();
                    data = rando.GenerateSeed(options, opts.Seed);
                    seed_made = true;
                } catch (CannotFillWorldException) {
                    if (++seed_fails > 99) {
                        Console.WriteLine("Failed to create a seed too many times. Aborting.");
                        Environment.Exit(1);
                    }
                }
            }

            var end = DateTime.Now;
            var filename = ComposeFilename(rando, opts, data.Seed, data.Worlds.First().Player);

            Console.WriteLine(string.Join(" - ",
                $"Generated seed: {data.Seed}",
                $"Players: {options["players"]}",
                $"Spheres: {data.Playthrough.Count}",
                $"Generation time: {end - start}"
            ));

            if ((bool)opts.Rom) {
                try {
                    WriteRom(filename, rando, data, options, opts);
                } catch (Exception e) {
                    Console.Error.WriteLine(e.Message);
                    // Don't write the spoiler if we don't write the rom (jic)
                    Environment.Exit(1);
                }
            }

            if ((bool)opts.Playthrough || (bool)opts.Spoiler) {
                WritePlaythrough(filename, data, opts);
            }

            if ((bool)opts.Patch) {
                WritePatches(filename, data, opts);
            }
        }

        static void WriteRom(string filename, IRandomizer rando, ISeedData data, Dictionary<string, string> options, GenSeedOptions opts) {
            IEnumerable<string> sprites = new List<string> {};
            IEnumerable<SpriteMetaData> RandoSprites = new List<SpriteMetaData> {};
            var world = data.Worlds.First();

            // Get the sprites ahead of time so we can get the authors
            if ((bool)opts.SurpriseMe) {
                RandoSprites = Rdc.GetRandomSprites(options);
                sprites = sprites.Concat(RandoSprites.Select(x => x.SpriteData).ToList());
            } else {
                RandoSprites = Rdc.GetSpecificSprites(options, opts.Sprites);

                sprites = opts.Sprites;
                foreach (var spr in RandoSprites) {
                    sprites = sprites.Where(x => x != spr.SearchTerm).ToList();
                    sprites = sprites.Concat(new List<string> { spr.SpriteData });
                }
            }

            // The IPS file gets applied during writing,
            // so it has to remain in context after ConstructRom is over.
            // So we make it here, then delete it before leaving scope.
            string base_ips = (bool)opts.AutoIPS ? Ips.ConstructBaseIps(rando, opts, RandoSprites) : opts.Ips.First();

            var rom = Rom.ConstructBaseRom(opts.smFile, opts.z3File, base_ips);
            Rom.ApplySeed(rom, world.Patches);
            Ips.AdditionalPatches(rom, (bool)opts.AutoIPS ? opts.Ips : opts.Ips.Skip(1));
            Rdc.ApplyRdcResources(rom, sprites);

            File.WriteAllBytes($"{filename}.sfc", rom);

            if ((bool)opts.AutoIPS)
                File.Delete(base_ips);
        }

        static void WritePlaythrough(string filename, ISeedData data, GenSeedOptions opts) {
            var text = "";

            if ((bool)opts.Spoiler) {
                text = data.Spoiler;
            } else {
                text = JsonConvert.SerializeObject(data.Playthrough, Formatting.Indented);
            }

            if ((bool)opts.ToConsole) {
                Console.WriteLine(text);
                Console.ReadLine();
            } else {
                string data_type = (bool)opts.Spoiler ? "spoiler" : "playthrough";
                File.WriteAllText($"{filename}-{data_type}.json", text);
            }
        }

        static void WritePatches(string filename, ISeedData data, GenSeedOptions opts) {
            var text = JsonConvert.SerializeObject(
                data.Worlds.ToDictionary(x => x.Player, x => x.Patches), Formatting.Indented,
                new PatchWriteConverter()
            );

            if ((bool)opts.ToConsole) {
                Console.WriteLine(text);
                Console.ReadLine();
            } else {
                File.WriteAllText($"{filename}-patch.json", text);
            }
        }

        static string ComposeFilename(IRandomizer rando, GenSeedOptions opts, string seed, string player) {
            var parts = new[] { new string[] {} };
            string dir  = "";
            string name = "";

            if (!String.IsNullOrEmpty(opts.OutputFile)) {
                // If they set to just a directory (regardless of trailing slash)
                if (Directory.Exists(opts.OutputFile)) {
                    dir = opts.OutputFile;
                } else {
                    dir  = Path.GetDirectoryName(opts.OutputFile);
                    name = Path.GetFileName(opts.OutputFile);
                }
            }

            if (!String.IsNullOrEmpty(name)) {
                parts = new[] {
                    new[] { name },
                    new[] {
                        seed,
                        (bool)opts.Multi ? player : null,
                    }
                };
            } else {
                parts = new[] {
                    new[] {
                        rando.Id.ToUpper(),
                        $"V{rando.Version}",
                    },
                    opts is SMZ3SeedOptions smz3 ? new[] {
                        $"ZLn+SL{smz3.SMLogic[0]}",
                        smz3.SwordLocation != "randomized" ? $"S{smz3.SwordLocation[0]}" : null,
                        smz3.MorphLocation != "randomized" ? $"M{smz3.MorphLocation[0]}" : null,
                        (smz3.KeyShuffle == "keysanity" ? "Kk" : smz3.KeyShuffle == "withinworld" ? "Kw" : null),
                        (smz3.Keycards == "keysanity" ? "Cc" : smz3.KeyShuffle == "withinworld" ? "Cw" : null)
                    } : opts is SMSeedOptions sm ? new[] {
                        $"L{sm.SMLogic[0]}",
                        $"I{sm.Placement[0]}",
                    } : new string[] { },
                    new[] {
                        seed,
                        (bool)opts.Multi ? player : null,
                    }
                };
            }

            /* Flatten, then keep non-null parts */
            return Path.Combine(dir, string.Join("-", parts.SelectMany(x => x).Where(x => x != null)));
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
