using Discord;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ServerOwnerModule
{
    public partial class ServerOwnerModule
    {
        [Group("moderatorrole", "modrole")]
        public class ModerationRole : YukiModule
        {
            [Command("add")]
            public async Task AddModRoleAsync([Remainder] string roleName)
            {

                ulong roleId = 0;

                if (!MentionUtils.TryParseRole(roleName, out roleId))
                {
                    roleId = Context.Guild.Roles.FirstOrDefault(role => role.Name.ToLower() == roleName.ToLower()).Id;
                }

                GuildSettings.AddRoleModerator(roleId, Context.Guild.Id);

                await ReplyAsync(Language.GetString("moderator_role_added").Replace("{rolename}", roleName));
            }

            [Command("remove", "rem")]
            public async Task RemoveModRoleAsync([Remainder] string roleName)
            {
                ulong roleId = 0;

                if(!MentionUtils.TryParseRole(roleName, out roleId))
                {
                    roleId = Context.Guild.Roles.FirstOrDefault(role => role.Name.ToLower() == roleName.ToLower()).Id;
                }

                GuildSettings.RemoveRoleModerator(roleId, Context.Guild.Id);

                await ReplyAsync(Language.GetString("moderator_role_removed").Replace("{rolename}", roleName));
            }
        }
    }
}
