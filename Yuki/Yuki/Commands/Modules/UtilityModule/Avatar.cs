using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("avatar")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GetAvatarAsync(IUser user = null)
        {
            if (user == null)
            {
                user = Context.User;
            }

            string title = Language.GetString("avatar_user_avatar").Replace("{user}", $"{user.Username}#{user.Discriminator}");

            await ReplyAsync(Context.CreateImageEmbedBuilder(title, user.GetBigAvatarUrl()));
        }
    }
}
