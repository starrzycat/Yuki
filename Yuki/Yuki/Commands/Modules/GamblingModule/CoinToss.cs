using Qmmands;
using System;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.GamblingModule
{
    public partial class GamlingModule
    {
        [Command("toss")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task CoinTossAsync([Remainder] string text = "")
        {
            string face = (new Random().Next(1, 100)) > 50 ?
                            Language.GetString("coin_heads") : Language.GetString("coin_tails");

            await ReplyAsync(Context.CreateEmbed(face, new Discord.EmbedAuthorBuilder()
            {
                IconUrl = Context.User.GetAvatarUrl(),
                Name = Language.GetString("coin_flipped").Replace("{user}", Context.User.Username)
            }));
        }
    }
}
