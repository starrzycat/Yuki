using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("channelfilter", "channelfilters", "cfilter", "cfilters")]
        public class ChannelWordFilter : YukiModule
        {
            [Command("add")]
            public async Task AddChannelFilterAsync([Remainder] string filter)
            {
                string channelName = filter.Split(' ')[0];
                if (MentionUtils.TryParseChannel(channelName, out ulong channelId)) { }
                else
                {
                    foreach (ITextChannel channel in (await Context.Guild.GetTextChannelsAsync()))
                    {
                        if (channel.Name.ToLower() == channelName.ToLower())
                        {
                            channelId = channel.Id;
                            break;
                        }
                    }
                }

                if (channelId == 0)
                {
                    channelId = Context.Channel.Id;
                }
                else
                {
                    filter = filter.Replace(channelName + " ", "");
                }

                GuildSettings.AddChannelFilter(Context.Guild.Id, channelId, $@"\b{filter}\b");

                await ReplyAsync(Language.GetString("filter_added"));
            }

            [Command("addregex")]
            public async Task AddChannelFilterRegexAsync([Remainder] string filter)
            {
                string channelName = filter.Split(' ')[0];
                if (MentionUtils.TryParseChannel(channelName, out ulong channelId)) { }
                else
                {
                    foreach (ITextChannel channel in (await Context.Guild.GetTextChannelsAsync()))
                    {
                        if (channel.Name.ToLower() == channelName.ToLower())
                        {
                            channelId = channel.Id;
                            break;
                        }
                    }
                }

                if (channelId == 0)
                {
                    channelId = Context.Channel.Id;
                }
                else
                {
                    filter = filter.Replace(channelName + " ", "");
                }

                GuildSettings.AddChannelFilter(Context.Guild.Id, channelId, filter);

                await ReplyAsync(Language.GetString("filter_added"));
            }

            [Command("list")]
            public async Task ListChannelFiltersAsync(ITextChannel channel, int page = 0)
            {
                string[] filters = GuildSettings.GetGuild(Context.Guild.Id).ChannelFilters[channel.Id].ToArray();

                await PagedReplyAsync("Filters", filters, 20, showNumbers: true);
            }
            
            [Command("list")]
            public async Task ListChannelFiltersAsync(int page = 0)
            {
                string[] filters = GuildSettings.GetGuild(Context.Guild.Id).ChannelFilters[Context.Channel.Id].ToArray();

                await PagedReplyAsync("Filters", filters, 20, showNumbers: true);
            }

            [Command("remove", "rem")]
            public async Task RemoveChannelFilterAsync(ITextChannel channel, int filterIndex)
            {
                GuildSettings.RemoveChannelFilter(Context.Guild.Id, channel.Id, --filterIndex);

                await ReplyAsync(Language.GetString("filter_removed"));
            }
            
            [Command("remove", "rem")]
            public async Task RemoveChannelFilterAsync(int filterIndex)
            {
                GuildSettings.RemoveChannelFilter(Context.Guild.Id, Context.Channel.Id, --filterIndex);

                await ReplyAsync(Language.GetString("filter_removed"));
            }
        }
    }
}
