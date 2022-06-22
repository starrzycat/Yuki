using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Services;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("viewpoll")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ViewPollAsync(string pollId)
        {
            Poll poll = PollingService.GetPoll(pollId);

            if (poll == null)
            {
                await ReplyAsync(Language.GetString("poll_not_found").Replace("{id}", pollId));
            }

            if (!poll.UserCanVote(Context.User.Id))
            {
                await ReplyAsync(Language.GetString("poll_not_in_server"));
                return;
            }

            await ReplyAsync(poll.CreateEmbed(!(Context.Channel is IDMChannel) && Context.UserHasPermission(GuildPermission.ManageMessages)));
        }
    }
}
