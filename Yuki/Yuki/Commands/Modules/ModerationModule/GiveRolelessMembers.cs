using Discord;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Command("giverolelessmembers")]
        public async Task ASDFGAsync([Remainder] string roleName)
        {
            /*IRole role = Context.Guild.Roles.Where(_role => _role.Name.ToLower() == roleName.ToLower()).FirstOrDefault();

            if(!role.Equals(default(IRole)))
            {
                await Context.Guild.DownloadUsersAsync();
                IGuildUser[] users = (await Context.Guild.GetUsersAsync()).Where(u => !u.RoleIds.Contains((ulong)214896300422987779)).ToArray();

                await ReplyAsync($"Found {users.Length} users without role `214896300422987779 ({Context.Guild.GetRole(214896300422987779).Name})`");

                for(int i = 0; i < users.Length; i++)
                {
                    await users[i].AddRoleAsync(role);
                }

                await ReplyAsync($"Gave {users.Length} users the role `{role.Name}`");
            }*/
        }
    }
}
