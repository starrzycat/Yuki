using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.OwnerModule
{
    public partial class BotOwnerModule
    {
        [Command("upgraderoles")]
        public async Task UpgradeRolesAsync()
        {
            try
            {
                await ReplyAsync("attempting to migrate...");
                foreach (ulong guildId in GuildSettings.GetGuilds().Select(g => g.Id).ToArray())
                {
                    foreach (ulong roleId in GuildSettings.GetGuild(guildId).AssignableRoles.ToArray())
                    {
                        GuildSettings.AddRole(roleId, guildId, false);
                        GuildSettings.DropRoleFromOld(roleId, guildId);
                    }
                }

                await ReplyAsync("migrated!");
            }
            catch(Exception e)
            {
                await ReplyAsync(e);
            }
        }
    }
}
