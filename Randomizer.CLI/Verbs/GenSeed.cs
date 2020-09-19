using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using CommandLine;
using Newtonsoft.Json;
using Randomizer.CLI.FileData;
using Randomizer.Shared.Contracts;
using static Randomizer.CLI.FileHelper;

namespace Randomizer.CLI.Verbs {
    abstract class GenSeedOptions {
        public Dictionary<string, object> defaults { get; set; } = new Dictionary<string, object>() {
            { "ConfigFile", @".\seed_options.json" },

            { "AutoIPS", false },
            { "AutoIPSConfig", "autoips.conf" },
            { "AutoIPSPath", "/tmp" },
            { "GoFast", false },
            { "Ips", new List<string> {} },
            { "Loop", 1 },
            { "Multi", false },
            { "MysterySeed", false },
            { "OutputFile", "" },
            { "Patch", false },
            { "PlayerName", "Player-1" },
            { "Players", 1 },
            { "Playthrough", false },
            { "SMLogic", "Normal" },
            { "smFile", @".\Super_Metroid_JU_.sfc" },
            { "Spoiler", false },
            { "SurpriseMe", false },
            { "Race", false },
            { "Rdc", new List<string> {} },
            { "RdcPath", "" },
            { "Rom", false },
            { "Seed", "" },
            { "ToConsole", false },
            { "Verbose", 0 },
        };

        [Option('c', "config", HelpText = "Options Config File")]
        public string ConfigFile { get; set; } = "";

        [Option(HelpText = "Generate a singleworld mode seed (defaults to single)")]
        public bool? Multi { get; set; }
        public bool? Single => !Multi;

        [Option(
            HelpText = "Set the player's name when single world")]
        public string PlayerName { get; set; }

        [Option("smlogic",
            HelpText = "Super Metroid Logic (default is Normal)")]
        public string SMLogic { get; set; }

        [Option('g', "gofast",
            HelpText = "faster... must go faster... (early boots/speed; default is false)")]
        public bool? GoFast { get; set; }

        [Option('l', "loop",
            HelpText = "Generate seeds repeatedly; takes an integer as argument, zero or less is infinite")]
        public int? Loop { get; set; }

        [Option('m', "mystery",
            HelpText = "Generalized mystery seed; randomize a LOT of values (default: False)")]
        public bool? MysterySeed { get; set; }

        [Option("surpriseme",
            HelpText = "Choose various non-randomization-impacting things at random (sprites, map on/off, new spin, etc)")]
        public bool? SurpriseMe { get; set; }

        [Option('o', "output",
            HelpText = "Where to write the resultant files")]
        public string OutputFile { get; set; }

        [Option('p', "players", Default = 1,
            HelpText = "The number of players for seeds")]
        public int Players { get; set; }

        [Option('r',"race",
            HelpText = "Whether this is a Race seed or not (default is false)")]
        public bool? Race { get; set; }

        [Option("rom",
            HelpText = "Compile rom file of the first world for each seed. Use the ips option to provide all required IPS patchs.")]
        public bool? Rom { get; set; }

        [Option('s', "seed",
            HelpText = "Generate a specific seed")]
        public string Seed { get; set; }

        [Option(
            HelpText = "Specify paths for IPS patches to be applied in the specified order.")]
        public IEnumerable<string> Ips { get; set; }

        [Option("autoips",
            HelpText = "Whether to use auto-IPS compiling or specify IPS manually. Defaults to false")]
        public bool? AutoIPS { get; set; }

        [Option("autoipsconfig",
            HelpText = "JSON config file full of defaults to use when building the AutoIPS")]
        public string AutoIPSConfig { get; set; }

        [Option("autoipspath",
            HelpText = "Path to alttp_sm_combo_randomizer_rom checked out repo with build.py")]
        public string AutoIPSPath { get; set; }

        [Option(
            HelpText = "Specify paths for RDC resources to be applied in the specified order.")]
        public IEnumerable<string> Rdc { get; set; }

        [Option("rdcpath",
            HelpText = "Path to Sprite files (RDCs) used for the surprise me option")]
        public string RdcPath { get; set; }

        [Option('t', "terminal",
            HelpText = "Write patch and playthrough/spoiler to the console instead of directly to files")]
        public bool? ToConsole { get; set; }

        [Option(
            HelpText = "Show json formatted playthrough for each seed")]
        public bool? Playthrough { get; set; }

        [Option(
            HelpText = "Show json formatted spoiler for each seed (turns Playthrough off)")]
        public bool? Spoiler { get; set; }

        [Option(
            HelpText = "Show json formated patch for each world in the seed")]
        public bool? Patch { get; set; }

        [Option("smfile",
            HelpText = "Super Metroid ROM File")]
        public string smFile { get; set; }

        [Option('v', "verbose",
            HelpText = "Verbosity level; defaults to 0 (off)")]
        public int Verbose { get; set; }

        public abstract IRandomizer NewRandomizer();

        public byte[] BaseRom { get; set; }
    }

    [Verb("sm", HelpText = "Generate Super Metroid seeds")]
    class SMSeedOptions : GenSeedOptions {
        [Option(
            HelpText = "Generate seeds with either full or split randomization (default is split)")]
        public string Placement { get; set; }

        [Option('g',"goal",
            HelpText = "Goal of Seed (default is DefeatMB)")]
        public string Goal { get; set; }

        public override IRandomizer NewRandomizer() => new SuperMetroid.Randomizer();

        public SMSeedOptions() {
            defaults.Add("Placement", "split");
            defaults.Add("Goal", "DefeatMB");
        }
    }

    [Verb("smz3", HelpText = "Generate SMZ3 combo seeds")]
    class SMZ3SeedOptions : GenSeedOptions {
        [Option("z3logic",
            HelpText = "Zelda logic (default is Normal)")]
        public string Z3Logic { get; set; }

        [Option("bottle",
            HelpText = "How to fill bottles (defaeult is Empty)")]
        public string BottleContents { get; set; }

        [Option("sword",
            HelpText = "Zelda Sword Location (default is Randomized)")]
        public string SwordLocation { get; set; }

        [Option("morph",
            HelpText = "SM Morph Ball Location (default is Randomized)")]
        public string MorphLocation { get; set; }

        [Option("bow",
            HelpText = "Separate Bow and Silvers, or Progressive (default is Separate)")]
        public string Bow { get; set; }

        [Option('g',"goal",
            HelpText = "Goal of Seed (default is DefeatBoth)")]
        public string Goal { get; set; }

        [Option('k', "keyshuffle",
            HelpText = "What level of key shuffle to use (default is None)")]
        public string KeyShuffle { get; set; }

        [Option("keycards",
            HelpText = "Whether to use KeyCards with Keysanity or not (default is None)")]
        public string Keycards { get; set; }

        [Option("bossdrops",
            HelpText = "Whether to allow bosses to drop dungeon items (default is Randomized)")]
        public string BossDrops { get; set; }

        [Option("ganoninvincible",
            HelpText = "Whether Ganon is invincible at any time (default is Never)")]
        public string GanonInvincible { get; set; }

        [Option("livedangerously",
            HelpText = "Whether to have the Forced Skull Woods key or not (defaults to having it)")]
        public bool? LiveDangerously { get; set; }

        [Option("randomflyingtiles",
            HelpText = "Whether the number of flying tiles in tile rooms should be random or not (default true currently)")]
        public bool? RandomFlyingTiles { get; set; }

        [Option("z3file",
            HelpText = "Zelda: ALTTP ROM File")]
        public string z3File { get; set; }

        public SMZ3SeedOptions() {
            defaults.Add("BossDrops", "Randomized");
            defaults.Add("BottleContents", "Empty");
            defaults.Add("Bow", "Separate");
            defaults.Add("GanonInvincible", "Never");
            defaults.Add("Goal", "DefeatBoth");
            defaults.Add("Keycards", "None");
            defaults.Add("KeyShuffle", "None");
            defaults.Add("LiveDangerously", false);
            defaults.Add("MorphLocation", "Randomized");
            defaults.Add("RandomFlyingTiles", false);
            defaults.Add("SwordLocation", "Randomized");
            defaults.Add("z3File", @".\Zelda_no_Densetsu_-_Kamigami_no_Triforce_Japan.sfc");
            defaults.Add("Z3Logic", "Normal");
        }

        public override IRandomizer NewRandomizer() => new SMZ3.Randomizer();
    }

    static class GenSeed {
        public static void Run(GenSeedOptions opts) {
            var optionList = getOptions(opts);

            try {
                validateOptions(opts);
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

            var start = DateTime.Now;
            while (seed_made == false) {
                try {
                    rando = opts.NewRandomizer();
                    data = rando.GenerateSeed(options, opts.Seed);
                    seed_made = true;
                } catch (CannotFillWorldException) {
                    // If we catch this, just try again
                    // Console.WriteLine(e.Message);
                }
            }

            var end = DateTime.Now;
            var world = data.Worlds.First();
            var filename = ComposeFilename(rando, opts, data.Seed, world.Player);

            Console.WriteLine(string.Join(" - ",
                $"Generated seed: {data.Seed}",
                $"Players: {options["players"]}",
                $"Spheres: {data.Playthrough.Count}",
                $"Generation time: {end - start}"
            ));

            if ((bool)opts.Rom) {
                try {
                    // The IPS file gets applied during writing,
                    // so it has to remain in context after ConstructRom is over.
                    // So we make it here, then delete it before leaving scope.
                    string base_ips = (bool)opts.AutoIPS ? ConstructBaseIps(rando, opts) : opts.Ips.First();

                    ConstructRom(opts, base_ips);
                    var rom = opts.BaseRom;
                    Rom.ApplySeed(rom, world.Patches);

                    AdditionalPatches(rom, (bool)opts.AutoIPS ? opts.Ips : opts.Ips.Skip(1));
                    ApplyRdcResources(rom, (bool)opts.SurpriseMe ? GetRandomSprites(opts) : opts.Rdc);

                    File.WriteAllBytes($"{filename}.sfc", rom);

                    if ((bool)opts.AutoIPS)
                        File.Delete(base_ips);
                } catch (Exception e) {
                    Console.Error.WriteLine(e.Message);
                }
            }

            if ((bool)opts.Playthrough || (bool)opts.Spoiler) {
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

            if ((bool)opts.Patch) {
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
        }

        // ugly, but does what i want... might revert to simpler version that wasn't quite right
        public static List<(string, string)> getOptions(GenSeedOptions opts) {
            BaseConfig conf = null;
            if (!String.IsNullOrEmpty(opts.ConfigFile) && File.Exists(opts.ConfigFile)) {
                if (opts is SMSeedOptions smconf) {
                    conf = SMConfig.ParseConfig(smconf.ConfigFile);
                } else if (opts is SMZ3SeedOptions smz3conf) {
                    conf = SMZ3Config.ParseConfig(smz3conf.ConfigFile);
                }
            }

            // Coalesce options into opts while we're here; this is technically a side-effect
            opts.Multi ??= conf.Multi ?? (bool)opts.defaults["Multi"];
            opts.SurpriseMe ??= conf.SurpriseMe ?? (bool)opts.defaults["SurpriseMe"];
            opts.MysterySeed ??= conf.MysterySeed ?? (bool)opts.defaults["MysterySeed"];
            opts.Race ??= conf.Race ?? (bool)opts.defaults["Race"];
            opts.Rom ??= conf.Rom ?? (bool)opts.defaults["Rom"];
            opts.Loop ??= conf.Loop ?? (int)opts.defaults["Loop"];
            opts.AutoIPS ??= conf.AutoIPS ?? (bool)opts.defaults["AutoIPS"];
            opts.ToConsole ??= conf.ToConsole ?? (bool)opts.defaults["ToConsole"];
            opts.Playthrough ??= conf.Playthrough ?? (bool)opts.defaults["Playthrough"];
            opts.Spoiler ??= conf.Spoiler ?? (bool)opts.defaults["Spoiler"];
            opts.Patch ??= conf.Patch ?? (bool)opts.defaults["Patch"];

            if (String.IsNullOrEmpty(opts.SMLogic))
                opts.SMLogic = !String.IsNullOrEmpty(conf.SMLogic) ? conf.SMLogic : (string)opts.defaults["SMLogic"];
            if (opts.Players == 0)
                opts.Players = conf.Players > 0 ? conf.Players : (int)opts.defaults["Players"];

            if (opts.Verbose == 0)
                opts.Verbose = conf.Verbose > 0 ? conf.Verbose : (int)opts.defaults["Verbose"];

            if (String.IsNullOrEmpty(opts.PlayerName))
                opts.PlayerName = !String.IsNullOrEmpty(conf.PlayerName) ? conf.PlayerName : (string)opts.defaults["PlayerName"];

            if (String.IsNullOrEmpty(opts.AutoIPSConfig))
                opts.AutoIPSConfig = !String.IsNullOrEmpty(conf.AutoIPSConfig) ? conf.AutoIPSConfig : (string)opts.defaults["AutoIPSConfig"];

            if (String.IsNullOrEmpty(opts.AutoIPSPath))
                opts.AutoIPSPath = !String.IsNullOrEmpty(conf.AutoIPSPath) ? conf.AutoIPSPath : (string)opts.defaults["AutoIPSConfig"];

            if (String.IsNullOrEmpty(opts.RdcPath))
                opts.RdcPath = !String.IsNullOrEmpty(conf.RdcPath) ? conf.RdcPath : (string)opts.defaults["RdcPath"];

            if (String.IsNullOrEmpty(opts.OutputFile))
                opts.OutputFile = !String.IsNullOrEmpty(conf.OutputFile) ? conf.OutputFile : (string)opts.defaults["OutputFile"];

            if (String.IsNullOrEmpty(opts.smFile))
                opts.smFile = !String.IsNullOrEmpty(conf.smFile) ? conf.smFile : (string)opts.defaults["smFile"];

            if (opts.Rdc.Count() == 0)
                opts.Rdc = conf.Rdc != null && conf.Rdc.Length > 0 ? conf.Rdc.ToList() : (List<string>)opts.defaults["Rdc"];
            if (opts.Ips.Count() == 0)
                opts.Ips = conf.Ips != null && conf.Ips.Length > 0 ? conf.Ips.ToList() : (List<string>)opts.defaults["Ips"];

            var optionList = new List<(string, string)> {
                ("gamemode", (bool)opts.Multi ? "multiworld" : "single"),
                ("gofast", opts.GoFast.ToString()),
                ("mysteryseed", opts.MysterySeed.ToString()),
                ("surpriseme", opts.SurpriseMe.ToString()),
                ("players", opts.Players.ToString()),
                ("playername", opts.PlayerName),
                ("race", opts.Race.ToString()),
                ("smlogic", opts.SMLogic),
                ("loop", opts.Loop.ToString()),
            };

            if (opts is SMZ3SeedOptions smz3) {
                if (conf is SMZ3Config smz3conf) {
                    if (String.IsNullOrEmpty(smz3.Z3Logic))
                        smz3.Z3Logic = !String.IsNullOrEmpty(smz3conf.Z3Logic) ? smz3conf.Z3Logic : (string)smz3.defaults["Z3Logic"];

                    if (String.IsNullOrEmpty(smz3.BottleContents))
                        smz3.BottleContents = !String.IsNullOrEmpty(smz3conf.BottleContents) ? smz3conf.BottleContents : (string)smz3.defaults["BottleContents"];

                    if (String.IsNullOrEmpty(smz3.SwordLocation))
                        smz3.SwordLocation = !String.IsNullOrEmpty(smz3conf.SwordLocation) ? smz3conf.SwordLocation : (string)smz3.defaults["SwordLocation"];

                    if (String.IsNullOrEmpty(smz3.MorphLocation))
                        smz3.MorphLocation = !String.IsNullOrEmpty(smz3conf.MorphLocation) ? smz3conf.MorphLocation : (string)smz3.defaults["MorphLocation"];

                    if (String.IsNullOrEmpty(smz3.Bow))
                        smz3.Bow = !String.IsNullOrEmpty(smz3conf.Bow) ? smz3conf.Bow : (string)smz3.defaults["Bow"];

                    if (String.IsNullOrEmpty(smz3.KeyShuffle))
                        smz3.KeyShuffle = !String.IsNullOrEmpty(smz3conf.KeyShuffle) ? smz3conf.KeyShuffle : (string)smz3.defaults["KeyShuffle"];

                    if (String.IsNullOrEmpty(smz3.Keycards))
                        smz3.Keycards = !String.IsNullOrEmpty(smz3conf.Keycards) ? smz3conf.Keycards : (string)smz3.defaults["Keycards"];

                    if (String.IsNullOrEmpty(smz3.BossDrops))
                        smz3.BossDrops = !String.IsNullOrEmpty(smz3conf.BossDrops) ? smz3conf.BossDrops : (string)smz3.defaults["BossDrops"];

                    if (String.IsNullOrEmpty(smz3.GanonInvincible))
                        smz3.GanonInvincible = !String.IsNullOrEmpty(smz3conf.GanonInvincible) ? smz3conf.GanonInvincible : (string)smz3.defaults["GanonInvincible"];

                    if (String.IsNullOrEmpty(smz3.Goal))
                        smz3.Goal = !String.IsNullOrEmpty(smz3conf.Goal) ? smz3conf.Goal : (string)smz3.defaults["Goal"];

                    if (String.IsNullOrEmpty(smz3.z3File))
                        smz3.z3File = !String.IsNullOrEmpty(smz3conf.z3File) ? smz3conf.z3File : (string)smz3.defaults["z3File"];

                    smz3.LiveDangerously ??= smz3conf.LiveDangerously ?? (bool)smz3.defaults["LiveDangerously"];
                    smz3.RandomFlyingTiles ??= smz3conf.RandomFlyingTiles ?? (bool)smz3.defaults["RandomFlyingTiles"];
                }

                optionList.AddRange(new[] {
                    ("z3logic", smz3.Z3Logic),
                    ("bottlecontents", smz3.BottleContents),
                    ("swordlocation", smz3.SwordLocation),
                    ("morphlocation", smz3.MorphLocation),
                    ("progressivebow", smz3.Bow == "Progressive" ? "true" : "false"),
                    ("keyshuffle", smz3.KeyShuffle),
                    ("keycards", smz3.Keycards),
                    ("bossdrops", smz3.BossDrops),
                    ("ganoninvincible", smz3.GanonInvincible),
                    ("goal", smz3.Goal),
                    ("livedangerously", smz3.LiveDangerously.ToString()),
                    ("randomflyingtiles", smz3.RandomFlyingTiles.ToString()),
                });
            }

            if (opts is SMSeedOptions sm) {
                if (conf is SMConfig smconf) {
                    if (String.IsNullOrEmpty(sm.Placement))
                        sm.Placement = !String.IsNullOrEmpty(smconf.Placement) ? smconf.Placement : (string)opts.defaults["Placement"];

                    if (String.IsNullOrEmpty(sm.Goal))
                        sm.Goal = !String.IsNullOrEmpty(smconf.Goal) ? smconf.Goal : (string)opts.defaults["Goal"];
                }

                optionList.AddRange(new[] {
                    ("placement", sm.Placement),
                    ("goal", sm.Goal),
                });
            }

            return optionList;
        }

        public static void validateOptions(GenSeedOptions options) {
            if (options.Players < 1 || options.Players > 64)
                throw new ArgumentOutOfRangeException("players", "The players parameter must fall within the range 1-64");

            if (options.AutoIPS == true) {
                if (options.MysterySeed == true)
                    throw new ArgumentException("Must set --autoips=true with --mysteryseed");
                if (String.IsNullOrEmpty(options.AutoIPSPath) || !Directory.Exists(options.AutoIPSPath))
                    throw new DirectoryNotFoundException("--autoipspath must be set when using --autoips");

                // WIP -- have to figure why looping with autoips is creating broken seeds
                // Until then, don't allow both
                if (options.Loop != 1)
                    throw new ArgumentException("AutoIPS does not currently work with the Loop option; use shell looping instead.");
            } else if (options.AutoIPS == false) {
                if (options.Ips.Count() == 0)
                    throw new ArgumentException("Must either set --ips <file> or --autoips");
            }

            foreach (var i in options.Ips) {
                if (!File.Exists(i))
                    throw new ArgumentException($"IPS File {i} doesn't exist.");
            }

            if ((bool)options.Playthrough && (bool)options.Spoiler)
                throw new ArgumentException("Playthrough and Spoiler are mutually exclusive.");

            foreach (var r in options.Rdc) {
                if (!File.Exists(r))
                    throw new ArgumentException($"RDC File {r} doesn't exist.");
            }

            if (String.IsNullOrEmpty(options.smFile) || !File.Exists(options.smFile))
                throw new FileNotFoundException($"Could not find Super Metroid file: {options.smFile}");

            if (options is SMZ3SeedOptions smz3) {
                if (String.IsNullOrEmpty(smz3.z3File) || !File.Exists(smz3.z3File))
                    throw new FileNotFoundException($"Could not find Zelda file: {smz3.z3File}");
            }
        }

        static void AdditionalPatches(byte[] rom, IEnumerable<string> ips) {
            foreach (var patch in ips) {
                using var stream = OpenReadInnerStream(patch);
                Rom.ApplyIps(rom, stream);
            }
        }

        private static IEnumerable<string> GetRandomSprites(GenSeedOptions opts) {
            // personal preference... i don't want to be the below sprites by chance
            // currently using a single blacklist instead of two separate ones
            string[] blacklist = new string[] {
                "grandpoobear", "pug", "eggplant", "vegeta", "poppy", "bailey"
            };
            Random tmp_rnd = new Random();
            IEnumerable<string> sprites = new List<string> {};

            // 20% chance of OG Samus
            if (tmp_rnd.Next(100) <= 80) {
                string[] sm_sprites = Directory.GetFiles(Path.Combine(opts.RdcPath, "sm"), "*.rdc*");
                sm_sprites = sm_sprites.Where(i => !blacklist.Any(e => i.Contains(e))).ToArray();
                sprites = sprites.Concat(new string[] { sm_sprites[tmp_rnd.Next(sm_sprites.Length)] });
            }

            // 20% chance of OG Link
            if (tmp_rnd.Next(100) <= 80) {
                string[] z3_sprites = Directory.GetFiles(Path.Combine(opts.RdcPath, "z3"), "*.rdc*");
                z3_sprites = z3_sprites.Where(i => !blacklist.Any(e => i.Contains(e))).ToArray();
                sprites = sprites.Concat(new string[] { z3_sprites[tmp_rnd.Next(z3_sprites.Length)] });
            }

            return sprites;
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

        private static string ConstructBaseIps(IRandomizer randomizer, GenSeedOptions opts) {
            var ips_file = Path.Join(Path.GetTempPath(), Path.GetRandomFileName());

            string ips_opts = $"build.py --config {opts.AutoIPSConfig} --output {ips_file}";
            if ((bool)opts.SurpriseMe)
                ips_opts += " --surprise_me";

            // The value can be "randomized", so we have to determine what was chosen
            // This allows us to turn on and off keycards based on the seeds chosen values!
            var conf = randomizer.ExportConfig();
            if (conf.ContainsKey("KeyShuffle")) {
                // Because of how ExportConfig currently works, have to deserialize this JSON object
                // TODO: needs fixing :(
                //   This code needs is awful, it needs to be riven
                Dictionary<string, int> myst = new Dictionary<string, int>();
                myst = JsonConvert.DeserializeObject<Dictionary<string, int>>(conf["RandomKeys"]);
                if (conf["KeyShuffle"] == "Randomized") {
                    int val = 0;
                    if (myst["map"] > 0)
                        val += 1;
                    if (myst["compass"] > 0)
                        val += 2;
                    if (myst["small_key"] > 0)
                        val += 4;
                    if (myst["big_key"] > 0)
                        val += 8;
                    ips_opts += $" -k {val}";
                } else if (conf["KeyShuffle"] == "None") {
                    ips_opts += " -k 0";
                }
            }
            if (conf.ContainsKey("Keycards") && conf["Keycards"] == "None") {
                ips_opts += " --no-cards";
            }

            string cur_path = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(opts.AutoIPSPath);

            using (Process p = new Process()) {
                p.StartInfo.FileName = "python3";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.Arguments = ips_opts;

                p.Start();
                p.WaitForExit();
            }

            Directory.SetCurrentDirectory(cur_path);
            return ips_file;
        }

        private static void ConstructRom(GenSeedOptions opts, string base_ips) {
            if (opts.BaseRom != null)
                return;

            Lazy<byte[]> fullRom;
            if (opts is SMSeedOptions sm) {
                fullRom = new Lazy<byte[]>(() => {
                    using var ips = OpenReadInnerStream(base_ips);
                    var rom = File.ReadAllBytes(opts.smFile);
                    FileData.Rom.ApplyIps(rom, ips);
                    return rom;
                });
            } else if (opts is SMZ3SeedOptions smz3) {
                fullRom = new Lazy<byte[]>(() => {
                    using var sm = File.OpenRead(opts.smFile);
                    using var z3 = File.OpenRead(smz3.z3File);
                    using var ips = OpenReadInnerStream(base_ips);
                    var rom = FileData.Rom.CombineSMZ3Rom(sm, z3);
                    FileData.Rom.ApplyIps(rom, ips);
                    return rom;
                });
            } else {
                throw new ArgumentException("Only Seed Modes available are 'sm' and 'smz3'");
            }

            opts.BaseRom = (byte[]) fullRom.Value.Clone();
        }

        static string ComposeFilename(IRandomizer rando, GenSeedOptions opts, string seed, string player) {
            var parts = new[] { new string[] {} };

            if (!String.IsNullOrEmpty(opts.OutputFile)) {
                parts = new[] {
                    new[] { opts.OutputFile },
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
