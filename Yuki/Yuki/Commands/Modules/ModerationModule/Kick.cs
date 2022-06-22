using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Command("kick")]
        [RequireModerator]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task KickAsync(IUser user)
        {
            await (await Context.Guild.GetUserAsync(user.Id)).KickAsync();
            await ReplyAsync(Language.GetString("user_kicked"));
        }
    }
}
