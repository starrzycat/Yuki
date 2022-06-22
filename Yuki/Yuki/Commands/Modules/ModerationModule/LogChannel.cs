using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("logchannel")]
        public class LogChannel : YukiModule
        {
            [Command("set")]
            public async Task SetLogChannelAsync([Remainder] string channelName)
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
                    await ReplyAsync(Language.GetString("channel_not_found").Replace("{channelname}", channelName));
                }
                else
                {
                    GuildSettings.AddChannelLog(channelId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("log_channel_added").Replace("{channelname}", MentionUtils.MentionChannel(channelId)));
                }
            }

            [Command("remove", "rem")]
            public async Task RemoveLogChannelAsync()
            {
                GuildSettings.RemoveChannelLog(Context.Guild.Id);

                await ReplyAsync(Language.GetString("log_channel_removed"));
            }
        }
    }
}
