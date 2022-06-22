using System.Collections.Generic;
using System.IO;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public class GuildSettingsRewrite
    {
        private static string directory = Path.Combine(FileDirectories.DataRoot, "storage", "guilds");

        private static GuildConfiguration NewGuildConfig(ulong guildId)
        {
            return new GuildConfiguration()
            {
                Id = guildId,
                LangCode = "en_US",

                // Lists
                GuildRoles = new List<GuildRole>(),
                StarboardIgnoredChannels = new List<ulong>(),
                AutoBanUsers = new List<ulong>(),
                CacheIgnoredChannels = new List<ulong>(),
                LevelIgnoredChannels = new List<ulong>(),
                ModeratorRoles = new List<ulong>(),
                AdministratorRoles = new List<ulong>(),
                NegaStarIgnoredChannels = new List<ulong>(),
                Commands = new List<GuildCommand>(),
                Settings = new List<GuildSetting>(),
                WordFilter = new List<string>(),

                // Dicts
                ChannelFilters = new Dictionary<ulong, List<string>>(),

                // Integers
                StarRequirement = 10,
                NegaStarRequirement = 20,
                WelcomeChannel = 0,
                StarboardChannel = 0,
                LogChannel = 0,
            };
        }

        public static GuildConfiguration GetGuild(ulong id)
        {
            if(!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            //using(FileStream strea)
            return NewGuildConfig(id);
        }
    }
}
