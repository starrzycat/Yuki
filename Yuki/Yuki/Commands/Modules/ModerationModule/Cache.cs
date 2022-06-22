using Discord;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("cache")]
        public class Cache : YukiModule
        {
            [Command("ignore")]
            public async Task IgnoreChannelAsync([Remainder] string channelName)
            {
                ulong channelId = default;

                if (MentionUtils.TryParseChannel(channelName, out channelId)) { }
                else
                {
                    channelId = (await Context.Guild.GetTextChannelsAsync()).Where(ch => ch.Name.ToLower() == channelName.ToLower()).FirstOrDefault().Id;
                }

                if (channelId == default)
                {
                    await ReplyAsync(Language.GetString("channel_not_found").Replace("{channelname}", channelName));
                }
                else
                {
                    GuildSettings.AddChannelCache(channelId, Context.Guild.Id);

                    UserMessageCache.DeleteWithChannelId(channelId);

                    await ReplyAsync(Language.GetString("cache_channel_ignored").Replace("{channelname}", MentionUtils.MentionChannel(channelId)));
                }
            }

            [Command("notice")]
            public async Task NoticeChannelAsync([Remainder] string channelName)
            {
                ulong channelId = 0;

                if (MentionUtils.TryParseChannel(channelName, out channelId)) { }
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
                    await ReplyAsync(Language.GetString("channel_not_found").Replace("{channel}", channelName));
                }
                else
                {
                    GuildSettings.RemoveChannelCache(channelId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("cache_channel_noticed").Replace("{channel}", MentionUtils.MentionChannel(channelId)));
                }
            }
        }
    }
}
