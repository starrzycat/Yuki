using Discord;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        private string[] gn = new[]
            {
                "https://b.catgirlsare.sexy/B8fn.gif",
                "https://b.catgirlsare.sexy/fm61.gif",
                "https://b.catgirlsare.sexy/o7KS.gif",
                "https://b.catgirlsare.sexy/qGeT.gif",
                "https://b.catgirlsare.sexy/TIQN.gif",
                "https://b.catgirlsare.sexy/zIIR.gif",
                "https://b.catgirlsare.sexy/-sCU.gif",
                "https://b.catgirlsare.sexy/sTEC.gif",
                "https://b.catgirlsare.sexy/f6lx.gif",
                "https://b.catgirlsare.sexy/2KwZ.gif",
                "https://b.catgirlsare.sexy/MTWN.gif",
                "https://b.catgirlsare.sexy/bYaW.gif",
                "https://b.catgirlsare.sexy/t2-o.gif",
                "https://b.catgirlsare.sexy/OWVo.gif",
                "https://b.catgirlsare.sexy/_Zd-.gif",
                "https://b.catgirlsare.sexy/4liQ.gif",
                "https://b.catgirlsare.sexy/lrTe.gif"
            };

        [Command("goodnight", "gn")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GoodnightAsync(params IUser[] mentioned)
        {
            EmbedBuilder embed;

            if (mentioned != null && mentioned.Length > 0)
            {
                embed = Context.CreateEmbedBuilder(Language.GetString("goodnight_title_mentioned").Replace("{user}", Context.User.Username)
                                .Replace("{mentioned_user}", string.Join(", ", mentioned.Select(user => user.Username))))
                                .WithImageUrl(gn[new Random().Next(gn.Length)]);
            }
            else
            {
                embed = Context.CreateEmbedBuilder(Language.GetString("goodnight_title").Replace("{user}", Context.User.Username))
                                .WithImageUrl(gn[new Random().Next(gn.Length)]);
            }

            await ReplyAsync(embed);
        }
    }
}
