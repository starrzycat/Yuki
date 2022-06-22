using Discord;
using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("donate")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task DonateAsync()
        {
            EmbedBuilder embed = Context.CreateEmbedBuilder(Language.GetString("donate_title"))
                .WithDescription(Language.GetString("donate_desc").Replace("{patreon}", YukiBot.PatronUrl).Replace("{paypal}", YukiBot.PayPalUrl));

            await ReplyAsync(embed);
        }
    }
}
