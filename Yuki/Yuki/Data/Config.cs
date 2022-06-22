using Nett;
using System.Collections.Generic;

namespace Yuki.Data
{
    public class Config
    {
        public string default_lang { get; set; } = "en_US";
        public List<string> prefix { get; set; } = new List<string>() { "y!" };
        public int playing_seconds { get; set; } = 300;
        public int command_timeout_seconds { get; set; } = 10;

        public List<ulong> owners { get; set; } = new List<ulong>() { 0 };

        public string token { get; set; } = "YOUR TOKEN HERE";
        public string encryption_key { get; set; } = "YOUR KEY HERE";
        
        private static Config Instance;

        public static Config GetConfig(bool reload = false)
        {
            if(Instance == null || reload)
            {
                Instance = Toml.ReadFile<Config>(FileDirectories.ConfigFile);
            }

            return Instance;
        }
    }
}
