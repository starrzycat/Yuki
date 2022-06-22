using Discord;
using Qmmands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("clear")]
        [RequireModerator]
        public class ClearCommand : YukiModule
        {
            [Command]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task Base(int amount)
            {
                IEnumerable<IMessage> messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();

                foreach(IMessage message in messages)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                }

                await ReplyAsync(Language.GetString("clear_result").Replace("{amount}", amount.ToString()));
            }

            [Command("from")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task ClearMessagesFromUserAsync(IUser user, int amount = 100)
            {
                IEnumerable<IMessage> messages = (await Context.Channel.GetMessagesAsync(1000, CacheMode.AllowDownload, null).FlattenAsync())
                                                        .Where(msg => msg.Author.Id == user.Id).Take(amount + 1);

                foreach (IMessage message in messages)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                }

                await ReplyAsync(Language.GetString("clear_result").Replace("{amount}", amount.ToString()));
            }

            [Command("with")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task ClearMessagesFromUserAsync(int amount, [Remainder] string str)
            {
                IEnumerable<IMessage> messages = (await Context.Channel.GetMessagesAsync(1000, CacheMode.AllowDownload, null).FlattenAsync())
                                                        .Where(msg => msg.Content.ToLower().Contains(str.ToLower())).Take(amount + 1);

                foreach (IMessage message in messages)
                {
                    await Context.Channel.DeleteMessageAsync(message);
                }

                await ReplyAsync(Language.GetString("clear_result").Replace("{amount}", amount.ToString()));
            }
        }
    }
}
