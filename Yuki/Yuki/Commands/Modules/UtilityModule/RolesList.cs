using Qmmands;
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
        [Command("roles")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GetRolesAsync()
        {
            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);

            if (config.EnableRoles)
            {
                List<ulong> guildRoles = Context.Guild.Roles.OrderByDescending(role => role.Position).Select(role => role.Id).ToList();

                foreach(GuildRole role in config.GuildRoles)
                {
                    if(!guildRoles.Contains(role.Id))
                    {
                        GuildSettings.RemoveRole(role.Id, Context.Guild.Id);
                    }
                }

                await PagedReplyAsync("Roles", config.GuildRoles.OrderBy(role => guildRoles.IndexOf(role.Id)).Select(role => Context.Guild.GetRole(role.Id).Name), 20);
            }
            else
            {
                await ReplyAsync(Language.GetString("roles_disabled"));
            }
        }
    }
}
