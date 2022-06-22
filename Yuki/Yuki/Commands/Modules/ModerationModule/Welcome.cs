using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("welcome")]
        public class Welcome : YukiModule
        {
            [Command("setchannel", "setc")]
            public async Task SetWelcomeChannelAsync([Remainder] string channelName)
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
                    await ReplyAsync(Language.GetString("channel_not_found").Replace("{channelname}", channelName).Replace("{user}", Context.User.Username));
                }
                else
                {
                    GuildSettings.SetWelcomeChannel(channelId, Context.Guild.Id);

                    await ReplyAsync(Language.GetString("welcome_channel_set").Replace("{channelname}", MentionUtils.MentionChannel(channelId)));
                }
            }

            [Command("removechannel", "remc")]
            public async Task RemoveWelcomeChannelAsync()
            {
                GuildSettings.SetWelcomeChannel(0, Context.Guild.Id);

                await ReplyAsync(Language.GetString("welcome_channel_removed"));
            }

            [Command("setmsg")]
            public async Task SetWelcomeMessageAsync([Remainder] string msg)
            {
                GuildSettings.SetWelcome(msg, Context.Guild.Id);

                await ReplyAsync(Language.GetString("welcome_message_set").Replace("{msg}", msg));
            }
        }
    }
}
