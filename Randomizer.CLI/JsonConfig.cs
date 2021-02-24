using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Randomizer.CLI {
    public class SMConfig {
        public string Keycards { get; set; }
        public string Placement { get; set; }
        public string smFile { get; set; }
        public string SMLogic { get; set; }
        public string MorphLocation { get; set; }
    }

    public class Z3Config {
        public string BossDrops { get; set; }
        public string BottleContents { get; set; }
        public bool? ProgressiveBow { get; set; }
        public string GanonInvincible { get; set; }
        public string KeyShuffle { get; set; }
        public bool? RandomFlyingTiles { get; set; }
        public string SwordLocation { get; set; }
        public string Z3HeartColor { get; set; }
        public string z3File { get; set; }
        public string Z3Logic { get; set; }
    }

    public class JsonConfig {
        public string AsarBin { get; set; }
        public bool? AutoIPS { get; set; }
        public string AutoIPSConfig { get; set; }
        public string AutoIPSPath { get; set; }
        public string Goal { get; set; }
        public bool? GoFast { get; set; }
        public string[] Ips { get; set; }
        public bool? LiveDangerously { get; set; }
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
        public bool? Rom { get; set; }
        public bool? Spoiler { get; set; }
        public string[] Sprites { get; set; }
        public string[] AvoidSprites { get; set; }
        public string SpriteCachePath { get; set; }
        public string SpriteURL { get; set; }
        public string SpriteSomethingBin { get; set; }
        public bool? SurpriseMe { get; set; }
        public bool? ToConsole { get; set; }
        public int Verbose { get; set; }

        public SMConfig Metroid { get; set; }
        public Z3Config Zelda { get; set; }

        public static JsonConfig ParseConfig(string ConfigFile) {
            JsonConfig conf = null;
            using (StreamReader r = File.OpenText(ConfigFile)) {
                string json = r.ReadToEnd();
                try {
                    conf = JsonConvert.DeserializeObject<JsonConfig>(json);
                } catch (Exception e) {
                    Console.WriteLine("Improper JSON config:");
                    Console.WriteLine(JsonConvert.SerializeObject(e, Formatting.Indented));
                }
            }

            return conf;
        }
    }

    // The below are Used for parsing sprite inventory data
    public class SpriteTags {
        public string character { get; set; }
        public string game { get; set; }
        public string gender { get; set; }
        public string universe { get; set; }
        public string[] type { get; set; }
    }

    public abstract class SpriteEntry {
        public string name { get; set; }
        public string author { get; set; }
        public int version { get; set; }
        public string file { get; set; }
        public bool demonetized { get; set; }
        public string[] note { get; set; }
        public string[] usage { get; set; }
    }

    public class SMSpriteEntry : SpriteEntry {
        public SpriteTags tags { get; set; }
    }

    public class Z3SpriteEntry : SpriteEntry {
        public string[] tags { get; set; }
    }

    public abstract class SpriteEntryClass {
        // public virtual Dictionary<string, SpriteEntry> denied { get; set; }
        // public virtual Dictionary<string, SpriteEntry> approved { get; set; }
        // public virtual Dictionary<string, SpriteEntry> waiting { get; set; }
    }

    public class SMSpriteEntryClass : SpriteEntryClass {
        public Dictionary<string, SMSpriteEntry> denied { get; set; }
        public Dictionary<string, SMSpriteEntry> approved { get; set; }
        public Dictionary<string, SMSpriteEntry> waiting { get; set; }
    }

    public class Z3SpriteEntryClass : SpriteEntryClass {
        public Dictionary<string, Z3SpriteEntry> denied { get; set; }
        public Dictionary<string, Z3SpriteEntry> approved { get; set; }
        public Dictionary<string, Z3SpriteEntry> waiting { get; set; }
    }

    public class SpriteInventory {
        public Z3SpriteEntryClass z3 { get; set; }
        public SMSpriteEntryClass m3 { get; set; }
    }
}
