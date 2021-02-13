using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Newtonsoft.Json;
using Randomizer.Shared.Contracts;
using Randomizer.CLI.Verbs;
using static Randomizer.CLI.FileHelper;

namespace Randomizer.CLI.FileData {
    class Ips {
        public static void AdditionalPatches(byte[] rom, IEnumerable<string> ips) {
            foreach (var patch in ips) {
                using var stream = OpenReadInnerStream(patch);
                Rom.ApplyIps(rom, stream);
            }
        }

        // when using AutoIPS
        public static string ConstructBaseIps(IRandomizer randomizer, GenSeedOptions opts, string[] authors) {
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
    }
}
