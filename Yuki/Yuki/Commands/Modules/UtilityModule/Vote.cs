using Discord;
using Discord.WebSocket;
using Interactivity;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Services;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("vote")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task VoteAsync(string pollId, string choice = "")
        {
            try
            {
                if (!(Context.Channel is IDMChannel))
                {
                    await ReplyAsync(Language.GetString("poll_not_in_server"));
                    return;
                }


                Poll poll = PollingService.GetPoll(pollId);

                if (poll == null)
                {
                    await ReplyAsync(Language.GetString("poll_not_found").Replace("{id}", pollId));
                    return;
                }

                if (!poll.UserCanVote(Context.User.Id))
                {
                    await ReplyAsync(Language.GetString("poll_not_in_server"));
                    return;
                }

                if (poll.HasUserVoted(Context.User.Id, out PollItem itemVoted))
                {
                    await ReplyAsync(Language.GetString("poll_already_voted"));

                    InteractivityResult<SocketMessage> result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                    if (result.IsSuccess && result.Value.Content.ToLower() == "yes")
                    {
                        itemVoted.RemoveVote(Context.User.Id);
                        PollingService.Save(poll);
                    }
                    else
                    {
                        return;
                    }
                }

                if (string.IsNullOrEmpty(choice))
                {
                    await ReplyAsync(Language.GetString("poll_vote"), poll.CreateEmbed());

                    InteractivityResult<SocketMessage> result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                    if (result.IsSuccess)
                    {
                        choice = result.Value.Content;
                    }
                    else
                    {
                        return;
                    }
                }

                if (poll.ItemExists(choice, out PollItem item))
                {
                    item.Vote(Context.User.Id);
                    await ReplyAsync(Language.GetString("poll_response_recorded"));
                    PollingService.Save(poll);
                }
                else
                {
                    await ReplyAsync(Language.GetString("poll_unknown_item"));
                }
            }
            catch(Exception e)
            {
                await Logger.LogDebug(e);
            }
        }
    }
}
