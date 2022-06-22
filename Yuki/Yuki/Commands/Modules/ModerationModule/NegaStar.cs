using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("negastar")]
        public class NegaStar : YukiModule
        {
            [Command("requirement")]
            public async Task SetNegaStarRequirementAsync(int stars)
            {
                if (stars > 0 && stars < 101)
                {
                    GuildSettings.SetNegaStarRequirement(stars, Context.Guild.Id);
                    await ReplyAsync(Language.GetString("negastar_requirement_set"));
                }
                else
                {
                    await ReplyAsync(Language.GetString("negastar_invalid_requirement"));
                }
            }

            [Command("ignore")]
            public async Task NegaStarIgnoreChannelAsync()
            {
                ulong channelId = Context.Channel.Id;

                bool enabled = GuildSettings.ToggleStarboardInChannel(channelId, Context.Guild.Id);

                string status = enabled ? Language.GetString("negastar_enabled_in_channel") : Language.GetString("negastar_disabled_in_channel");

                await ReplyAsync(Language.GetString("negastar_channel_ignored"));
            }
        }
    }
}
