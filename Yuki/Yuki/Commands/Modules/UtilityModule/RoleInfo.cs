using Discord;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("roleinfo", "rinfo")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GetRoleInfoAsync([Remainder] string roleName)
        {
            await Context.Guild.DownloadUsersAsync();

            IRole role = Context.Guild.Roles.Where(r => r.Name.ToLower() == roleName.ToLower()).FirstOrDefault();

            if (!role.Equals(default))
            {
                EmbedBuilder embed = new EmbedBuilder()
                    .WithColor(role.Color)
                    .WithAuthor(role.Name)
                    .AddField(Language.GetString("roleinfo_id"), role.Id, true)
                    .AddField(Language.GetString("roleinfo_position"), role.Position, true)
                    .AddField(Language.GetString("roleinfo_created"), role.CreatedAt.DateTime.ToPrettyTime(false, true), true)
                    .AddField(Language.GetString("roleinfo_hoisted"), Language.GetString($"_{role.IsHoisted.ToString().ToLower()}"), true)
                    .AddField(Language.GetString("roleinfo_mentionable"), Language.GetString($"_{role.IsMentionable.ToString().ToLower()}"), true)
                    .AddField(Language.GetString("roleinfo_managed"), Language.GetString($"_{role.IsManaged.ToString().ToLower()}"), true)
                    .AddField(Language.GetString("roleinfo_count"), (await Context.Guild.GetUsersAsync()).Where(user => user.RoleIds.Contains(role.Id)).Count(), true)
                    .AddField(Language.GetString("roleinfo_permissions"), string.Join(", ", role.Permissions), true);
                
                await ReplyAsync(embed);
            }
        }
    }
}
