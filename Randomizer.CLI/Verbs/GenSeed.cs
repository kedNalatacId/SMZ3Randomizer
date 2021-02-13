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

            { "AsarBin", "asar" },
            { "AutoIPS", false },
            { "AutoIPSConfig", "" },
            { "AutoIPSPath", "/tmp" },
            { "GoFast", false },
            { "Ips", new List<string> {} },
            { "Loop", 1 },
            { "Multi", false },
            { "MysterySeed", false },
            { "OutputFile", "" },
            { "Patch", false },
            { "PlayerName", "Player" },
            { "Players", 1 },
            { "Playthrough", false },
            { "PythonBin", "python" },
            { "SMLogic", "Normal" },
            { "smFile", @".\Super_Metroid_JU_.sfc" },
            { "Spoiler", false },
            { "SurpriseMe", false },
            { "Race", false },
            { "Sprites", new List<string> {} },
            { "Rom", false },
            { "Seed", "" },
            { "SpriteCachePath", "/tmp" },
            { "SpriteSomethingBin", "SpriteSomething.py" },
            { "AvoidSprites", new List<string> {} },
            { "SpriteURL", "http://smalttpr.mymm1.com/sprites/" },
            { "ToConsole", false },
            { "Verbose", 0 },
            { "z3File", @".\Zelda_no_Densetsu_-_Kamigami_no_Triforce_Japan.sfc" },
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

        [Option("asar",
            HelpText = "Path to asar executable; used with autoips. Possibly obviates the need for autoipsconfig")]
        public string AsarBin { get; set; }

        [Option("python",
            HelpText = "Name/Path to python executable; used with autoips. Defaults to 'python'")]
        public string PythonBin { get; set; }

        [Option(
            HelpText = "Specify paths for RDC resources to be applied in the specified order.")]
        public IEnumerable<string> Sprites { get; set; }

        [Option("spritecachepath",
            HelpText = "Where to store downloaded sprites temporarily")]
        public string SpriteCachePath { get; set; }

        [Option("spritesomething",
            HelpText = "Where the SpriteSomething repo is checked out")]
        public string SpriteSomethingBin { get; set; }

        [Option("avoidsprites",
            HelpText = "Sprites you don't want to see in --surpriseme mode; one list works for both SM and Z3")]
        public IEnumerable<string> AvoidSprites { get; set; }

        [Option("spriteurl",
            HelpText = "Mike Trethewey's available Sprite list; work from approved smz3 sprites only!")]
        public string SpriteURL { get; set; }

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

        [Option("z3file",
            HelpText = "Zelda: ALTTP ROM File")]
        public string z3File { get; set; }

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

        [Option("morph",
            HelpText = "SM Morph Ball Location (default is Randomized)")]
        public string MorphLocation { get; set; }

        [Option("keycards",
            HelpText = "Whether to use KeyCards with Keysanity or not (default is None)")]
        public string Keycards { get; set; }

        public override IRandomizer NewRandomizer() => new SuperMetroid.Randomizer();

        public SMSeedOptions() {
            defaults.Add("Placement", "split");
            defaults.Add("Goal", "DefeatMB");
            defaults.Add("MorphLocation","Randomized");
            defaults.Add("Keycards", "None");
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

        [Option("progressivebow",
            HelpText = "Separate Bow and Silvers, or Progressive (default is false)")]
        public bool? ProgressiveBow { get; set; }

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
            HelpText = "Removes various first sphere items from all games")]
        public bool? LiveDangerously { get; set; }

        [Option("randomflyingtiles",
            HelpText = "Whether the number of flying tiles in tile rooms should be random or not (default true currently)")]
        public bool? RandomFlyingTiles { get; set; }

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

        public override IRandomizer NewRandomizer() => new SMZ3.Randomizer();
    }

    static class GenSeed {
        public static void Run(GenSeedOptions opts) {
            List<(string, string)> optionList = null;

            try {
                optionList = getOptions(opts);
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
                        Environment.Exit(0);
                    }
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
                    string[] authors = new string[2];
                    IEnumerable<string> sprites = new List<string> {};

                    // Get the sprites ahead of time so we can get the authors
                    if ((bool)opts.SurpriseMe) {
                        (authors, sprites) = Rdc.GetRandomSprites(options);
                    } else {
                        sprites = opts.Sprites;
                    }

                    // The IPS file gets applied during writing,
                    // so it has to remain in context after ConstructRom is over.
                    // So we make it here, then delete it before leaving scope.
                    string base_ips = (bool)opts.AutoIPS ? ConstructBaseIps(rando, opts, authors) : opts.Ips.First();

                    ConstructRom(opts, base_ips);
                    var rom = opts.BaseRom;
                    Rom.ApplySeed(rom, world.Patches);

                    AdditionalPatches(rom, (bool)opts.AutoIPS ? opts.Ips : opts.Ips.Skip(1));
                    ApplyRdcResources(rom, sprites);

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
            JsonConfig conf = null;
            if (!String.IsNullOrEmpty(opts.ConfigFile) && File.Exists(opts.ConfigFile)) {
                conf = JsonConfig.ParseConfig(opts.ConfigFile);
            }

            if (conf.Metroid == null)
                throw new ArgumentException("You haven't specified a 'Metroid' configuration section. Please reference the sample_conifg.json");
            if (conf.Zelda == null)
                throw new ArgumentException("You haven't specified a 'Zelda' configuration section. Please reference the sample_config.json");

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
            opts.GoFast ??= conf.GoFast ?? (bool)opts.defaults["GoFast"];

            if (String.IsNullOrEmpty(opts.SMLogic))
                opts.SMLogic = !String.IsNullOrEmpty(conf.Metroid.SMLogic) ? conf.Metroid.SMLogic : (string)opts.defaults["SMLogic"];

            if (opts.Players == 0)
                opts.Players = conf.Players > 0 ? conf.Players : (int)opts.defaults["Players"];

            if (opts.Verbose == 0)
                opts.Verbose = conf.Verbose > 0 ? conf.Verbose : (int)opts.defaults["Verbose"];

            if (String.IsNullOrEmpty(opts.PlayerName))
                opts.PlayerName = !String.IsNullOrEmpty(conf.PlayerName) ? conf.PlayerName : (string)opts.defaults["PlayerName"];

            if (String.IsNullOrEmpty(opts.AsarBin))
                opts.AsarBin = !String.IsNullOrEmpty(conf.AsarBin) ? conf.AsarBin : (string)opts.defaults["AsarBin"];

            if (String.IsNullOrEmpty(opts.AutoIPSConfig))
                opts.AutoIPSConfig = !String.IsNullOrEmpty(conf.AutoIPSConfig) ? conf.AutoIPSConfig : (string)opts.defaults["AutoIPSConfig"];

            if (String.IsNullOrEmpty(opts.AutoIPSPath))
                opts.AutoIPSPath = !String.IsNullOrEmpty(conf.AutoIPSPath) ? conf.AutoIPSPath : (string)opts.defaults["AutoIPSPath"];

            if (String.IsNullOrEmpty(opts.SpriteURL))
                opts.SpriteURL = !String.IsNullOrEmpty(conf.SpriteURL) ? conf.SpriteURL : (string)opts.defaults["SpriteURL"];

            if (String.IsNullOrEmpty(opts.SpriteCachePath))
                opts.SpriteCachePath = !String.IsNullOrEmpty(conf.SpriteCachePath) ? conf.SpriteCachePath : (string)opts.defaults["SpriteCachePath"];

            if (String.IsNullOrEmpty(opts.SpriteSomethingBin))
                opts.SpriteSomethingBin = !String.IsNullOrEmpty(conf.SpriteSomethingBin) ? conf.SpriteSomethingBin : (string)opts.defaults["SpriteSomethingBin"];

            if (String.IsNullOrEmpty(opts.OutputFile))
                opts.OutputFile = !String.IsNullOrEmpty(conf.OutputFile) ? conf.OutputFile : (string)opts.defaults["OutputFile"];

            if (String.IsNullOrEmpty(opts.PythonBin))
                opts.PythonBin = !String.IsNullOrEmpty(conf.PythonBin) ? conf.PythonBin : (string)opts.defaults["PythonBin"];

            if (String.IsNullOrEmpty(opts.smFile))
                opts.smFile = !String.IsNullOrEmpty(conf.Metroid.smFile) ? conf.Metroid.smFile : (string)opts.defaults["smFile"];

            if (String.IsNullOrEmpty(opts.z3File))
                opts.z3File = !String.IsNullOrEmpty(conf.Zelda.z3File) ? conf.Zelda.z3File : (string)opts.defaults["z3File"];

            if (opts.Sprites.Count() == 0)
                opts.Sprites = conf.Sprites != null && conf.Sprites.Length > 0 ? conf.Sprites.ToList() : (List<string>)opts.defaults["Sprites"];
            if (opts.AvoidSprites.Count() == 0)
                opts.AvoidSprites = conf.AvoidSprites != null && conf.AvoidSprites.Length > 0 ? conf.AvoidSprites.ToList() : (List<string>)opts.defaults["AvoidSprites"];
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

                // These options are passed this way when getting a random sprite (easier)
                ("SpriteURL", opts.SpriteURL),
                ("SpriteCachePath", opts.SpriteCachePath),
                ("SpriteSomethingBin", opts.SpriteSomethingBin),
                ("AvoidSprites", String.Join(',', opts.AvoidSprites)),
                ("PythonBin", opts.PythonBin),
            };

            if (opts is SMZ3SeedOptions smz3) {
                if (String.IsNullOrEmpty(smz3.Z3Logic))
                    smz3.Z3Logic = !String.IsNullOrEmpty(conf.Zelda.Z3Logic) ? conf.Zelda.Z3Logic : (string)smz3.defaults["Z3Logic"];

                if (String.IsNullOrEmpty(smz3.BottleContents))
                    smz3.BottleContents = !String.IsNullOrEmpty(conf.Zelda.BottleContents) ? conf.Zelda.BottleContents : (string)smz3.defaults["BottleContents"];

                if (String.IsNullOrEmpty(smz3.SwordLocation))
                    smz3.SwordLocation = !String.IsNullOrEmpty(conf.Zelda.SwordLocation) ? conf.Zelda.SwordLocation : (string)smz3.defaults["SwordLocation"];

                if (String.IsNullOrEmpty(smz3.MorphLocation))
                    smz3.MorphLocation = !String.IsNullOrEmpty(conf.Metroid.MorphLocation) ? conf.Metroid.MorphLocation : (string)smz3.defaults["MorphLocation"];

                if (String.IsNullOrEmpty(smz3.KeyShuffle))
                    smz3.KeyShuffle = !String.IsNullOrEmpty(conf.Zelda.KeyShuffle) ? conf.Zelda.KeyShuffle : (string)smz3.defaults["KeyShuffle"];

                if (String.IsNullOrEmpty(smz3.Keycards))
                    smz3.Keycards = !String.IsNullOrEmpty(conf.Metroid.Keycards) ? conf.Metroid.Keycards : (string)smz3.defaults["Keycards"];

                if (String.IsNullOrEmpty(smz3.BossDrops))
                    smz3.BossDrops = !String.IsNullOrEmpty(conf.Zelda.BossDrops) ? conf.Zelda.BossDrops : (string)smz3.defaults["BossDrops"];

                if (String.IsNullOrEmpty(smz3.GanonInvincible))
                    smz3.GanonInvincible = !String.IsNullOrEmpty(conf.Zelda.GanonInvincible) ? conf.Zelda.GanonInvincible : (string)smz3.defaults["GanonInvincible"];

                if (String.IsNullOrEmpty(smz3.Goal))
                    smz3.Goal = !String.IsNullOrEmpty(conf.Goal) ? conf.Goal : (string)smz3.defaults["Goal"];

                smz3.ProgressiveBow ??= conf.Zelda.ProgressiveBow ?? (bool)smz3.defaults["ProgressiveBow"];
                smz3.LiveDangerously ??= conf.LiveDangerously ?? (bool)smz3.defaults["LiveDangerously"];
                smz3.RandomFlyingTiles ??= conf.Zelda.RandomFlyingTiles ?? (bool)smz3.defaults["RandomFlyingTiles"];

                optionList.AddRange(new[] {
                    ("z3logic", smz3.Z3Logic),
                    ("bottlecontents", smz3.BottleContents),
                    ("swordlocation", smz3.SwordLocation),
                    ("morphlocation", smz3.MorphLocation),
                    ("keyshuffle", smz3.KeyShuffle),
                    ("keycards", smz3.Keycards),
                    ("bossdrops", smz3.BossDrops),
                    ("ganoninvincible", smz3.GanonInvincible),
                    ("goal", smz3.Goal),
                    ("progressivebow", smz3.ProgressiveBow.ToString()),
                    ("livedangerously", smz3.LiveDangerously.ToString()),
                    ("randomflyingtiles", smz3.RandomFlyingTiles.ToString()),
                });
            }

            if (opts is SMSeedOptions sm) {
                if (String.IsNullOrEmpty(sm.Placement))
                    sm.Placement = !String.IsNullOrEmpty(conf.Metroid.Placement) ? conf.Metroid.Placement : (string)sm.defaults["Placement"];

                if (String.IsNullOrEmpty(sm.Goal))
                    sm.Goal = !String.IsNullOrEmpty(conf.Goal) ? conf.Goal : (string)sm.defaults["Goal"];

                if (String.IsNullOrEmpty(sm.MorphLocation))
                    sm.MorphLocation = !String.IsNullOrEmpty(conf.Metroid.MorphLocation) ? conf.Metroid.MorphLocation : (string)sm.defaults["MorphLocation"];

                if (String.IsNullOrEmpty(sm.Keycards))
                    sm.Keycards = !String.IsNullOrEmpty(conf.Metroid.Keycards) ? conf.Metroid.Keycards : (string)sm.defaults["Keycards"];

                optionList.AddRange(new[] {
                    ("placement", sm.Placement),
                    ("goal", sm.Goal),
                    ("morphlocation", sm.MorphLocation),
                    ("keycards", sm.Keycards),
                });
            }

            return optionList;
        }

        public static void validateOptions(GenSeedOptions options) {
            if (options.Players < 1 || options.Players > 64)
                throw new ArgumentOutOfRangeException("players", "The players parameter must fall within the range 1-64");

            if (options.AutoIPS == true) {
                if (String.IsNullOrEmpty(options.AutoIPSPath))
                    throw new ArgumentException("--autoipspath must be set when using --autoips");
                if (!Directory.Exists(options.AutoIPSPath))
                    throw new DirectoryNotFoundException($"Could not find --autoipspath {options.AutoIPSPath}");

                // WIP -- have to figure why looping with autoips is creating broken seeds
                // Until then, don't allow both
                if (options.Loop != 1)
                    throw new ArgumentException("AutoIPS does not currently work with the Loop option; use shell looping instead.");
            } else if (options.AutoIPS == false) {
                if (options.MysterySeed == true)
                    throw new ArgumentException("Must set --autoips=true with --mysteryseed");
                if (options.Ips.Count() == 0)
                    throw new ArgumentException("Must either set --ips <file> or --autoips");
            }

            foreach (var i in options.Ips) {
                if (!File.Exists(i))
                    throw new ArgumentException($"IPS File {i} doesn't exist.");
            }

            if ((bool)options.Playthrough && (bool)options.Spoiler)
                throw new ArgumentException("Playthrough and Spoiler are mutually exclusive.");

            foreach (var r in options.Sprites) {
                if (!File.Exists(r))
                    throw new ArgumentException($"RDC File {r} doesn't exist.");
            }

            if (String.IsNullOrEmpty(options.smFile) || !File.Exists(options.smFile))
                throw new FileNotFoundException($"Could not find Super Metroid file: {options.smFile}");

            if (String.IsNullOrEmpty(options.z3File) || !File.Exists(options.z3File))
                throw new FileNotFoundException($"Could not find Zelda file: {options.z3File}");
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

        private static string ConstructBaseIps(IRandomizer randomizer, GenSeedOptions opts, string[] authors) {
            var ips_file = Path.Join(Path.GetTempPath(), Path.GetRandomFileName());

            string ips_opts = $"build.py --output {ips_file}";
            if (!String.IsNullOrEmpty(opts.AutoIPSConfig) && File.Exists(opts.AutoIPSConfig))
                ips_opts += $" --config {opts.AutoIPSConfig}";
            if (!String.IsNullOrEmpty(opts.AsarBin) && File.Exists(opts.AsarBin))
                ips_opts += $" --asar {opts.AsarBin}";
            if (authors.Length > 0) {
                if (!String.IsNullOrEmpty(authors[0]))
                    ips_opts += $" --smspriteauthor {authors[0]}";
                if (authors.Length > 1 && !String.IsNullOrEmpty(authors[1]))
                    ips_opts += $" --z3spriteauthor {authors[1]}";
            }
            if ((bool)opts.SurpriseMe)
                ips_opts += " --surprise_me";
            if (opts is SMSeedOptions)
                ips_opts += " --mode sm";

            // The value can be "randomized", so we have to determine what was chosen
            // This allows us to turn on and off keycards based on the seeds chosen values!
            var conf = randomizer.ExportConfig();
            if (conf.ContainsKey("KeyShuffle")) {
                // Because of how ExportConfig currently works, have to deserialize this JSON object
                // TODO: needs fixing :(
                //   This code is awful, it needs to be riven
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
            if (conf.ContainsKey("Keycards") && conf["Keycards"] == "None")
                ips_opts += " --no-cards";
            if (conf.ContainsKey("Seed") && Int32.Parse(conf["Seed"]) > 0)
                ips_opts += $" --seed {conf["Seed"]}";

            string cur_path = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(opts.AutoIPSPath);

            using (Process p = new Process()) {
                p.StartInfo.FileName = opts.PythonBin;
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
            fullRom = new Lazy<byte[]>(() => {
                using var sm = File.OpenRead(opts.smFile);
                using var z3 = File.OpenRead(opts.z3File);
                var rom = FileData.Rom.CombineSMZ3Rom(sm, z3);
                using var ips = OpenReadInnerStream(base_ips);
                FileData.Rom.ApplyIps(rom, ips);
                return rom;
            });

            opts.BaseRom = (byte[]) fullRom.Value.Clone();
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
