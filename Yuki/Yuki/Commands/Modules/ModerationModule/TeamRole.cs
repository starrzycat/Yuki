using Discord;
using Discord.WebSocket;
using Interactivity;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Command("teamrole")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task SetTeamRoleAsync([Remainder] string args)
        {
            string[] split = args.Split(' ');

            string group = "";
            bool removableState = true;

            if (!bool.TryParse(split[split.Length-1], out bool state))
            {
                await ReplyAsync($"{split[split.Length - 1]} is not a valid bool!");
            }

            string roleName = string.Join(' ', split.Take(split.Length - 1));
            IRole role = Context.Guild.Roles.Where(_role => _role.Name.ToLower() == roleName.ToLower()).FirstOrDefault();

            if(role == default)
            {
                await ReplyAsync(await ReplyAsync(Language.GetString("role_not_found").Replace("{role}", roleName)));
                return;
            }

            if (state)
            {
                IUserMessage _msg = await ReplyAsync(Language.GetString("group_role_create"));
                InteractivityResult<SocketMessage> result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                if (result.IsSuccess)
                {
                    group = result.Value.Content.ToLower();

                    if(string.IsNullOrWhiteSpace(group) || group == "d")
                    {
                        group = "default";
                    }
                }
                else
                {
                    return;
                }

                _msg = await ReplyAsync(Language.GetString("role_removable"));
                result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                if(result.IsSuccess)
                {
                    string val = result.Value.Content.ToLower();
                    removableState = val == "y"; // not checking if its "n" here but oh well
                }
            }

            GuildConfiguration config = GuildSettings.GetGuild(Context.Guild.Id);
            GuildSettings.SetTeamRole(role.Id, Context.Guild.Id, state, group, removableState);
            await ReplyAsync(Language.GetString("team_role_state").Replace("{role}", role.Name).Replace("{status}", state.ToString()).Replace("{group}", group));

            /*if (config.GuildRoles != null && config.GuildRoles.Any(_role => _role.Id == role.Id))
            {
                
            }
            else
            {
                await ReplyAsync(Language.GetString("role_not_found").Replace("{role}", role.Name));
            }*/
        }
    }
}
