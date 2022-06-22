using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public static class GuildSettings
    {
        private const string collection = "guild_settings";

        private static GuildConfiguration DefaultConfig(ulong guildId)
            => new GuildConfiguration()
            {
                Id = guildId,
                LangCode = "en_US",
                Prefix = null,

                EnableWelcome = false,
                EnableGoodbye = false,
                EnableCache = false,
                EnableLogging = false,
                EnablePrefix = false,
                EnableRoles = false,
                EnableFilter = false,
                EnableStarboard = false,
                EnableNegaStars = false,
                IsDirty = false,

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

                ChannelFilters = new Dictionary<ulong, List<string>>(),

                StarRequirement = 10,
                NegaStarRequirement = 20,

                WelcomeChannel = 0,
                StarboardChannel = 0,
                LogChannel = 0,
                
                WelcomeMessage = null,
                GoodbyeMessage = null
            };

        public static void MarkDirty(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                config.IsDirty = true;
                config.LeaveDate = DateTime.Now;

                AddOrUpdate(config);
            }
        }
        
        public static void MarkClean(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                config.IsDirty = false;

                AddOrUpdate(config);
            }
        }

        public static GuildConfiguration GetGuild(ulong id)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                GuildConfiguration config = configs.Find(conf => conf.Id == id).FirstOrDefault();//.FindAll().FirstOrDefault(conf => conf.Id == id);
                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(id));
                    return GetGuild(id);
                }
                else
                {
                    if(config.NegaStarIgnoredChannels == null)
                    {
                        config.NegaStarIgnoredChannels = new List<ulong>();
                        AddOrUpdate(config);
                    }

                    return config;
                }
            }
        }

        public static List<GuildConfiguration> GetGuilds()
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                return configs.FindAll().ToList();
            }
        }

        public static List<GuildConfiguration> GetDirtyGuilds()
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                return configs.Find(guild => guild.IsDirty).ToList();
            }
        }

        public static void AddOrUpdate(GuildConfiguration config)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                configs.Upsert(config);

                /*if (configs.FindById(config.Id).Equals(default(GuildConfiguration)))
                {
                    configs.Insert(config);
                }
                else
                {
                    configs.Update(config);
                }*/
            }
        }

        public static void DeleteGuildData(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                //if (!configs.Find(conf => conf.Id == guildId).FirstOrDefault().Equals(default(GuildConfiguration)))
                if (!configs.Find(conf => conf.Id == guildId).FirstOrDefault().Equals(default(GuildConfiguration)))
                {
                    configs.Delete(guildId);
                }
            }
        }

        #region Sets
        public static void SetWelcome(string message, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                
                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetWelcome(message, guildId);
                }

                config.WelcomeMessage = message;

                AddOrUpdate(config);
            }
        }

        public static void SetGoodbye(string message, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);

                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();
                
                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetGoodbye(message, guildId);
                }


                config.GoodbyeMessage = message;

                AddOrUpdate(config);
            }
        }

        public static void SetLanguage(string langCode, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetLanguage(langCode, guildId);
                }


                config.LangCode = langCode;

                AddOrUpdate(config);
            }
        }

        public static void SetWelcomeChannel(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (config.Equals(default(GuildConfiguration)))
                {
                    AddOrUpdate(DefaultConfig(guildId));

                    SetWelcomeChannel(channelId, guildId);
                }


                config.WelcomeChannel = channelId;

                AddOrUpdate(config);
            }
        }
        #endregion

        #region Toggles
        public static void ToggleWelcome(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableWelcome = state;

                    AddOrUpdate(config);
                }
            }
        }

        public static void ToggleGoodbye(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableGoodbye = state;

                    AddOrUpdate(config);
                }
            }
        }

        public static void ToggleLogging(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableLogging = state;

                    AddOrUpdate(config);
                }
            }
        }

        public static void ToggleCache(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableCache = state;

                    AddOrUpdate(config);
                }
            }
        }

        public static void ToggleRoles(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableRoles = state;

                    AddOrUpdate(config);
                }
            }
        }

        public static void TogglePrefix(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnablePrefix = state;

                    AddOrUpdate(config);
                }
            }
        }

        public static void ToggleFilter(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableFilter = state;

                    AddOrUpdate(config);
                }
            }
        }

        public static void ToggleStarboard(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableStarboard = state;

                    AddOrUpdate(config);
                }
            }
        }
        
        public static void ToggleNegaStar(ulong guildId, bool state)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.EnableNegaStars = state;

                    AddOrUpdate(config);
                }
            }
        }
        #endregion

        #region Adds

        public static void AddChannelLog(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.LogChannel = channelId;

                    AddOrUpdate(config);
                }
            }
        }

        public static void AddChannelCache(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    if(!config.CacheIgnoredChannels.Contains(channelId))
                    {
                        config.CacheIgnoredChannels.Add(channelId);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void AddRole(ulong roleId, ulong guildId, bool isTeamRole)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if(config.GuildRoles == null)
                    {
                        config.GuildRoles = new List<GuildRole>();
                    }

                    if (!config.GuildRoles.Any(role => role.Id == roleId))
                    {
                        config.GuildRoles.Add(new GuildRole() { Id = roleId, IsTeamRole = isTeamRole });
                    }

                    AddOrUpdate(config);
                }
            }
        }
        
        public static void SetTeamRole(ulong roleId, ulong guildId, bool isTeamRole, string group, bool removableState)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (config.Equals(default(GuildConfiguration)))
                {
                    config = DefaultConfig(guildId);
                }

                if (config.GuildRoles == null)
                {
                    config.GuildRoles = new List<GuildRole>()
                    {
                        new GuildRole()
                        {
                            Id = roleId,
                            Group = group,
                            IsTeamRole = isTeamRole,
                            IsTeamRoleRemovable = removableState,
                            IsAutoRole = false,
                        }
                    };
                }
                else if(!config.GuildRoles.Any(_role => _role.Id == roleId))
                {
                    config.GuildRoles.Add(new GuildRole()
                    {
                        Id = roleId,
                        Group = group,
                        IsTeamRole = isTeamRole,
                        IsTeamRoleRemovable = removableState,
                        IsAutoRole = false,
                    });
                }
                else
                {
                    int index = config.GuildRoles.IndexOf(config.GuildRoles.FirstOrDefault(_role => _role.Id == roleId));

                    GuildRole role = config.GuildRoles[index];
                    role.IsTeamRole = isTeamRole;
                    role.IsTeamRoleRemovable = removableState;
                    role.Group = group;

                    config.GuildRoles[index] = role;
                }

                AddOrUpdate(config);
            }
        }
        
        public static void SetAutoRole(ulong roleId, ulong guildId, bool isAutoRole)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (config.Equals(default(GuildConfiguration)))
                {
                    config = DefaultConfig(guildId);
                }

                if (config.GuildRoles == null)
                {
                    config.GuildRoles = new List<GuildRole>()
                    {
                        new GuildRole()
                        {
                            Id = roleId,
                            Group = "default",
                            IsTeamRole = false,
                            IsTeamRoleRemovable = true,
                            IsAutoRole = isAutoRole,
                        }
                    };
                }
                else if (!config.GuildRoles.Any(_role => _role.Id == roleId))
                {
                    config.GuildRoles.Add(new GuildRole()
                    {
                        Id = roleId,
                        Group = "default",
                        IsTeamRole = false,
                        IsTeamRoleRemovable = true,
                        IsAutoRole = isAutoRole,
                    });
                }
                else
                {
                    int index = config.GuildRoles.IndexOf(config.GuildRoles.FirstOrDefault(_role => _role.Id == roleId));

                    GuildRole role = config.GuildRoles[index];
                    role.IsAutoRole = isAutoRole;

                    config.GuildRoles[index] = role;
                }

                AddOrUpdate(config);
            }
        }

        public static void AddPrefix(string prefix, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    config.Prefix = prefix;

                    AddOrUpdate(config);
                }
            }
        }

        public static void AddRoleModerator(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    if (!config.ModeratorRoles.Contains(roleId))
                    {
                        config.ModeratorRoles.Add(roleId);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void AddRoleAdministrator(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (!config.AdministratorRoles.Contains(roleId))
                    {
                        config.AdministratorRoles.Add(roleId);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void AddCommand(GuildCommand command, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    if (!config.Commands.Any(c => c.Name.ToLower() == command.Name.ToLower()))
                    {
                        config.Commands.Add(command);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void AddFilter(string filter, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    if (config.WordFilter == null)
                    {
                        config.WordFilter = new List<string>();
                    }

                    if (!config.WordFilter.Any(str => str.ToLower() == filter.ToLower()))
                    {
                        config.WordFilter.Add(filter);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void SetStarRequirement(int numStars, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    config.StarRequirement = numStars;

                    AddOrUpdate(config);
                }
            }
        }
        
        public static void SetNegaStarRequirement(int numStars, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    config.NegaStarRequirement = numStars;

                    AddOrUpdate(config);
                }
            }
        }

        public static void SetStarboardChannel(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    config.StarboardChannel = channelId;

                    AddOrUpdate(config);
                }
            }
        }

        public static bool ToggleStarboardInChannel(ulong channelId, ulong guildId)
        {
            bool enabled = false;

            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    enabled = !config.StarboardIgnoredChannels.Contains(channelId);

                    if (!enabled)
                    {
                        config.StarboardIgnoredChannels.Remove(channelId);
                    }
                    else
                    {
                        config.StarboardIgnoredChannels.Add(channelId);
                    }

                    AddOrUpdate(config);
                }
            }

            return enabled;
        }
        
        public static bool ToggleNegastarInChannel(ulong channelId, ulong guildId)
        {
            bool enabled = false;

            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    enabled = !config.NegaStarIgnoredChannels.Contains(channelId);

                    if (!enabled)
                    {
                        config.NegaStarIgnoredChannels.Remove(channelId);
                    }
                    else
                    {
                        config.NegaStarIgnoredChannels.Add(channelId);
                    }

                    AddOrUpdate(config);
                }
            }

            return enabled;
        }

        public static void AddChannelFilter(ulong guildId, ulong channelId, string filter)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.ChannelFilters == null)
                    {
                        config.ChannelFilters = new Dictionary<ulong, List<string>>();
                    }

                    if(!config.ChannelFilters.ContainsKey(channelId))
                    {
                        config.ChannelFilters.Add(channelId, new List<string>());
                    }

                    if (!config.ChannelFilters[channelId].Any(str => str.ToLower() == filter.ToLower()))
                    {
                        config.ChannelFilters[channelId].Add(filter);
                    }

                    AddOrUpdate(config);
                }
            }
        }
        #endregion

        #region Removes
        public static void RemoveChannelCache(ulong channelId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.CacheIgnoredChannels.Contains(channelId))
                    {
                        config.CacheIgnoredChannels.Remove(channelId);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void RemoveChannelLog(ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (config.Equals(default(GuildConfiguration)))
                {
                    if (config.LogChannel != 0)
                    {
                        config.LogChannel = 0;
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void RemoveRole(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if(!config.Equals(default(GuildConfiguration)))
                {
                    if (config.GuildRoles.Any(role => role.Id == roleId))
                    {
                        config.GuildRoles.Remove(config.GuildRoles.First(role => role.Id == roleId));
                    }

                    AddOrUpdate(config);
                }
            }
        }
        
        public static void DropRoleFromOld(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.AssignableRoles.Contains(roleId))
                    {
                        config.AssignableRoles.Remove(roleId);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void RemoveRoleModerator(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.ModeratorRoles.Contains(roleId))
                    {
                        config.ModeratorRoles.Remove(roleId);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void RemoveRoleAdministrator(ulong roleId, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.AdministratorRoles.Contains(roleId))
                    {
                        config.AdministratorRoles.Remove(roleId);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        #endregion

        #region Gets
        public static void RemoveCommand(string command, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.Commands.Any(c => c.Name.ToLower() == command.ToLower()))
                    {
                        config.Commands.Remove(config.Commands.FirstOrDefault(c => c.Name.ToLower() == command.ToLower()));
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void RemoveFilter(int filterIndex, ulong guildId)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if(filterIndex >= 0 && filterIndex < config.WordFilter.Count)
                    {
                        config.WordFilter.RemoveAt(filterIndex);
                    }

                    AddOrUpdate(config);
                }
            }
        }

        public static void RemoveChannelFilter(ulong guildId, ulong channelId, int filterIndex)
        {
            using (LiteDatabase db = new LiteDatabase($"filename={FileDirectories.SettingsDB}; journal=false"))
            {
                LiteCollection<GuildConfiguration> configs = db.GetCollection<GuildConfiguration>(collection);
                GuildConfiguration config = configs.Find(conf => conf.Id == guildId).FirstOrDefault();

                if (!config.Equals(default(GuildConfiguration)))
                {
                    if (config.ChannelFilters == null)
                    {
                        return;
                    }

                    if (!config.ChannelFilters.ContainsKey(channelId))
                    {
                        return;
                    }

                    if (filterIndex >= 0 && filterIndex < config.ChannelFilters[channelId].Count)
                    {
                        config.ChannelFilters[channelId].RemoveAt(filterIndex);
                    }

                    AddOrUpdate(config);
                }
            }
        }
        #endregion

        #region Info

        #endregion
    }
}
