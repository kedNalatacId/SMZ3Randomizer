using System;
using System.IO;
using Newtonsoft.Json;

namespace Randomizer.CLI {
    public abstract class BaseConfig {
        public string AsarBin { get; set; }
        public bool? AutoIPS { get; set; }
        public string AutoIPSConfig { get; set; }
        public string AutoIPSPath { get; set; }
        public string Goal { get; set; }
        public bool? GoFast { get; set; }
        public string[] Ips { get; set; }
        public int? Loop { get; set; }
        public bool? Multi { get; set; }
        public bool? MysterySeed { get; set; }
        public string OutputFile { get; set; }
        public bool? Patch { get; set; }
        public string PlayerName { get; set; }
        public int Players { get; set; }
        public bool? Playthrough { get; set; }
        public string PythonBin { get; set; }
        public bool? Race { get; set; }
        public string[] Rdc { get; set; }
        public string RdcPath { get; set; }
        public bool? Rom { get; set; }
        public string smFile { get; set; }
        public string SMLogic { get; set; }
        public bool? Spoiler { get; set; }
        public bool? SurpriseMe { get; set; }
        public bool? ToConsole { get; set; }
        public int Verbose { get; set; }

        // public abstract BaseConfig ParseConfig(string ConfigFile);
    }

    public class SMConfig : BaseConfig {
        public string Placement { get; set; }

        public static BaseConfig ParseConfig(string ConfigFile) {
            BaseConfig conf = null;
            using (StreamReader r = File.OpenText(ConfigFile)) {
                string json = r.ReadToEnd();
                conf = JsonConvert.DeserializeObject<SMConfig>(json);
            }

            return conf;
        }
    }

    public class SMZ3Config : BaseConfig {
        public string BossDrops { get; set; }
        public string BottleContents { get; set; }
        public bool? ProgressiveBow { get; set; }
        public string GanonInvincible { get; set; }
        public string Keycards { get; set; }
        public string KeyShuffle { get; set; }
        public bool? LiveDangerously { get; set; }
        public string MorphLocation { get; set; }
        public bool? RandomFlyingTiles { get; set; }
        public string SwordLocation { get; set; }
        public string z3File { get; set; }
        public string Z3HeartColor { get; set; }
        public string Z3Logic { get; set; }

        public static BaseConfig ParseConfig(string ConfigFile) {
            BaseConfig conf = null;
            using (StreamReader r = File.OpenText(ConfigFile)) {
                string json = r.ReadToEnd();
                conf = JsonConvert.DeserializeObject<SMZ3Config>(json);
            }

            return conf;
        }
    }
}
