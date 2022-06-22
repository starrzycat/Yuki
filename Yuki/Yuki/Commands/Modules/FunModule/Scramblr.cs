using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;
using Yuki.Data;
using Yuki.Data.Objects;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Group("scramblr", "scrambler")]
        public class ScramblrGroup : YukiModule
        {
            [Command]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task Base(IUser user = null)
            {
                try
                {
                    Scramblr scramblr = new Scramblr(Language, Context.User, user);

                    if (UserSettings.CanGetMsgs(Context.User.Id))
                    {
                        if (user != null)
                        {
                            if (UserSettings.CanGetMsgs(user.Id))
                            {
                                await ReplyAsync(scramblr.GetMessage());
                            }
                            else
                            {
                                await ReplyAsync(Language.GetString("scramblr_not_enabled").Replace("{user}", $"{user.Username}#{user.Discriminator}"));
                            }
                        }
                        else
                        {
                            await ReplyAsync(scramblr.GetMessage());
                        }
                    }
                    else
                    {
                        await ReplyAsync(Language.GetString("scramblr_not_enabled").Replace("{user}", $"{Context.User.Username}#{Context.User.Discriminator}"));
                    }
                }
                catch (Exception e)
                {
                    await ReplyAsync("An error occurred!\n```" + e + "```\nPlease let my creator know ASAP");
                }
            }

            [Command("info")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task ScramblrInfo()
            {
                await ReplyAsync(Language.GetString("scramblr_info"));
            }

            [Command("enable")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task ScramblrEnable()
            {
                UserSettings.SetCanGetMessages(Context.User.Id, true, Context.Channel as ITextChannel);
                await ReplyAsync(Language.GetString("scramblr_enabled"));
            }

            [Command("disable")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task ScramblrDisable()
            {
                UserSettings.SetCanGetMessages(Context.User.Id, true, Context.Channel as ITextChannel);
                UserMessageCache.DeleteFromUser(Context.User.Id);

                await ReplyAsync(Language.GetString("scramblr_disabled"));
            }
        }
    }
}
