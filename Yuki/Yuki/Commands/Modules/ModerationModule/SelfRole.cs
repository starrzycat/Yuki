using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("selfrole")]
        public class SelfRole : YukiModule
        {
            [Command("add")]
            public async Task AddSelfRoleAsync([Remainder] string roleName)
            {
                ulong roleId = 0;

                if (MentionUtils.TryParseRole(roleName, out ulong id))
                {
                    roleId = id;
                }
                else
                {
                    foreach (IRole irole in Context.Guild.Roles)
                    {
                        if (irole.Name.ToLower() == roleName.ToLower())
                        {
                            roleId = irole.Id;
                            break;
                        }
                    }
                }

                if (roleId == 0)
                {
                    await ReplyAsync(Language.GetString("role_not_found").Replace("{rolename}", roleName).Replace("{user}", Context.User.Username));
                }
                else
                {
                    GuildSettings.AddRole(roleId, Context.Guild.Id, false);

                    await ReplyAsync(Language.GetString("role_added").Replace("{rolename}", roleName));
                }
            }


            [Command("remove", "rem")]
            public async Task RemoveSelfRoleAsync([Remainder] string roleName)
            {
                ulong roleId = 0;

                if (MentionUtils.TryParseRole(roleName, out ulong id))
                {
                    roleId = id;
                }
                else
                {
                    foreach (IRole irole in Context.Guild.Roles)
                    {
                        if (irole.Name.ToLower() == roleName.ToLower())
                        {
                            roleId = irole.Id;
                            break;
                        }
                    }
                }

                if (roleId == 0)
                {
                    await ReplyAsync(Language.GetString("role_not_found").Replace("{rolename}", roleName).Replace("{user}", Context.User.Username));
                }
                else
                {
                    GuildSettings.RemoveRole(roleId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("role_removed").Replace("{rolename}", roleName));
                }
            }
        }
    }
}
