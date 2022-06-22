using LiteDB;
using System;
using System.Collections.Generic;

namespace Yuki.Data.Objects.Database
{
    public struct GuildConfiguration
    {
        [BsonId]
        public ulong Id { get; set; }

        public bool EnableWelcome { get; set; }
        public bool EnableGoodbye { get; set; }
        public bool EnableCache { get; set; }
        public bool EnablePrefix { get; set; }
        public bool EnableRoles { get; set; }
        public bool EnableLogging { get; set; }
        public bool EnableFilter { get; set; }
        public bool EnableStarboard { get; set; }
        public bool EnableNegaStars { get; set; }
        public bool IsDirty { get; set; }

        public string WelcomeMessage { get; set; }
        public string GoodbyeMessage { get; set; }

        public string LangCode { get; set; }

        public string Prefix { get; set; } /* Custom prefix for server */

        public int StarRequirement { get; set; }
        public int NegaStarRequirement { get; set; }

        public ulong WelcomeChannel { get; set; }
        public ulong StarboardChannel { get; set; }
        public ulong LogChannel { get; set; }

        public DateTime LeaveDate { get; set; }

        public List<GuildSetting> Settings { get; set; }
        public List<GuildCommand> Commands { get; set; }

        public List<GuildRole> GuildRoles { get; set; }

        public List<string> WordFilter { get; set; }

        public List<ulong> CacheIgnoredChannels { get; set; }
        public List<ulong> LevelIgnoredChannels { get; set; }
        public List<ulong> StarboardIgnoredChannels { get; set; }
        public List<ulong> NegaStarIgnoredChannels { get; set; }
        public List<ulong> AutoBanUsers { get; set; }    /* Users to ban when they join */
        public List<ulong> AssignableRoles { get; set; }
        public List<ulong> ModeratorRoles { get; set; }
        public List<ulong> AdministratorRoles { get; set; }

        public Dictionary<ulong, List<string>> ChannelFilters { get; set; }
    }
}
