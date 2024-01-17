namespace Yuki {
    public class Config {
        public string? Token { get; set; }
        public ulong DevGuild { get; set; }

        public static Config FromFile(string filePath) {
            var text = File.ReadAllText(filePath);
            var conf = Tomlyn.Toml.ToModel<Config>(text);
            
            return conf;
        }
    }
}