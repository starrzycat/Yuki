using Discord;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Command("autorole")]
        public async Task AutoRoleAsync([Remainder] string args)
        {
            try
            {
                string[] split = args.Split(' ');

                if (!bool.TryParse(split[split.Length - 1], out bool state))
                {
                    await ReplyAsync($"{split[split.Length - 1]} is not a valid bool!");
                }

                string roleName = string.Join(' ', split.Take(split.Length - 1));

                IRole role = Context.Guild.Roles.Where(_role => _role.Name.ToLower() == roleName.ToLower()).FirstOrDefault();
                
                if (role == default)
                {
                    await ReplyAsync(await ReplyAsync(Language.GetString("role_not_found").Replace("{role}", roleName)));
                    return;
                }

                GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);
                GuildSettings.SetAutoRole(role.Id, Context.Guild.Id, state);
                await ReplyAsync(Language.GetString("auto_role_state").Replace("{role}", role.Name).Replace("{status}", state.ToString()));
            }
            catch(Exception e)
            {
                await ReplyAsync(e);
            }
        }
    }
}
