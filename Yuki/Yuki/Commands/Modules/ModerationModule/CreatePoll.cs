using Discord;
using Discord.WebSocket;
using Interactivity;
using Qmmands;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects;
using Yuki.Extensions;
using Yuki.Services;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Command("createpoll")]
        [RequireModerator]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task CreatePollAsync(string pollName = "")
        {
            try
            {
                string[] items = new string[] { };
                DateTime deadline = default;
                bool? showVotes = null;

                EmbedBuilder builder = Context.CreateEmbedBuilder(Language.GetString("poll_create_creating"))
                    .AddField(Language.GetString("poll_creating_title_str"), string.IsNullOrEmpty(pollName) ? "none" : pollName)
                    .AddField(Language.GetString("poll_creating_items_str"), Language.GetString("poll_create_items_desc"))
                    .AddField(Language.GetString("poll_creating_deadline_str"), Language.GetString("poll_create_deadline_desc"))
                    .AddField(Language.GetString("poll_create_show_vote_str"), Language.GetString("poll_create_show_vote_desc"));

                IUserMessage message = await builder.SendToAsync(Context.Channel);
                InteractivityResult<SocketMessage> result;

                if (string.IsNullOrEmpty(pollName))
                {
                    IUserMessage _msg = await ReplyAsync(Language.GetString("poll_create_title"));
                    result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                    if (result.IsSuccess)
                    {
                        pollName = result.Value.Content;

                        await Context.TryDeleteAsync(result.Value);
                        await Context.TryDeleteAsync(_msg);
                    }
                    else
                    {
                        return;
                    }
                }

                if (pollName.Length > 300)
                {
                    await ReplyAsync(Language.GetString("poll_create_title_long"));
                    return;
                }

                await message.ModifyAsync(emb =>
                {
                    builder.Fields.Find(x => x.Name == Language.GetString("poll_creating_title_str")).WithValue(pollName);

                    emb.Embed = builder.Build();
                });

                while (items.Length < 2)
                {
                    IUserMessage _msg = await ReplyAsync(Language.GetString("poll_create_items"));
                    result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                    if (result.IsSuccess)
                    {
                        items = Regex.Split(result.Value.Content, @"\s*[|]\s*");

                        if (items.Length < 2)
                        {
                            await ReplyAsync(Language.GetString("poll_create_items_short"));
                        }

                        await Context.TryDeleteAsync(result.Value);
                        await Context.TryDeleteAsync(_msg);
                    }
                    else
                    {
                        return;
                    }
                }

                await message.ModifyAsync(emb =>
                {
                    builder.Fields.Find(field => field.Name == Language.GetString("poll_creating_items_str")).WithValue(string.Join(", ", items));

                    emb.Embed = builder.Build();
                });

                while (deadline == default)
                {
                    IUserMessage _msg = await ReplyAsync(Language.GetString("poll_create_deadline"));
                    result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                    if (result.IsSuccess)
                    {
                        deadline = result.Value.Content.ToDateTime();

                        if (deadline == default)
                        {
                            await ReplyAsync(Language.GetString("poll_create_deadline_invalid"));
                        }
                        else if ((deadline.TimeOfDay.TotalDays / 7) > 2)
                        {
                            await ReplyAsync(Language.GetString("poll_create_deadline_long"));
                        }

                        await Context.TryDeleteAsync(result.Value);
                        await Context.TryDeleteAsync(_msg);
                    }
                    else
                    {
                        return;
                    }
                }

                await message.ModifyAsync(emb =>
                {
                    builder.Fields.Find(field => field.Name == Language.GetString("poll_creating_deadline_str")).WithValue(deadline.ToPrettyTime(false, true) + " UTC");

                    emb.Embed = builder.Build();
                });

                while (showVotes == null)
                {
                    IUserMessage _msg = await ReplyAsync(Language.GetString("poll_create_allow_view"));
                    result = await Interactivity.NextMessageAsync(msg => msg.Author == Context.User && msg.Channel == Context.Channel);

                    if (result.IsSuccess)
                    {
                        if (result.Value.Content.ToLower() == "y" || result.Value.Content == "n")
                        {
                            showVotes = result.Value.Content.ToLower() == "y";
                        }

                        await Context.TryDeleteAsync(result.Value);
                        await Context.TryDeleteAsync(_msg);
                    }
                    else
                    {
                        return;
                    }
                }

                if (showVotes is bool ShowVotes)
                {
                    Poll poll = Poll.Create(pollName, items, Context.Guild.Id, deadline, ShowVotes);
                    PollingService.Save(poll);

                    await message.ModifyAsync(emb =>
                    {
                        builder.Fields.Find(field => field.Name == Language.GetString("poll_create_show_vote_str")).WithValue(showVotes);

                        builder.WithAuthor(new EmbedAuthorBuilder()
                        {
                            IconUrl = Context.User.GetAvatarUrl(),
                            Name = Context.User.Username + " | " + Language.GetString("poll_create_created")
                        })
                        .WithFooter(Language.GetString("poll_created_id") + $": {poll.Id}");

                        emb.Embed = builder.Build();
                    });

                    await ReplyAsync(Language.GetString("poll_create_created") + $" ID: {poll.Id}");
                }
                else
                {
                    await ReplyAsync("Uh oh! Something bad happened!");
                }
            }
            catch(Exception e)
            {
                await ReplyAsync(e);
            }
        }
    }
}