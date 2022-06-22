using Discord;
using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Command("ban")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task BanAsync(IUser user)
        {
            await Context.Guild.AddBanAsync(user);
            await ReplyAsync(Language.GetString("user_banned"));
        }
    }
}
