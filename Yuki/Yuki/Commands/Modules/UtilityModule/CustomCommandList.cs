using Discord;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("commandlist", "ccommands")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task CustomCommandsList(int page = 0)
        {
            try
            {
                if (!(Context.Channel is IDMChannel))
                {
                    List<GuildCommand> commands = GuildSettings.GetGuild(Context.Guild.Id).Commands;

                    if (commands.Count > 0)
                    {
                        await PagedReplyAsync("Custom Commands", commands.Select(c => c.Name).ToArray(), 20);
                    }
                    else
                    {
                        await ReplyAsync(Language.GetString("no_custom_commands"));
                    }
                }

            }
            catch (Exception e) { await ReplyAsync(e); }
        }
    }
}
