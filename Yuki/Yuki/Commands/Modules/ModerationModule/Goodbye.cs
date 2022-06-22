using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("goodbye")]
        public class Goodbye : YukiModule
        {
            [Command("set")]
            public async Task SetGoodbyeMessageAsync([Remainder] string msg)
            {
                GuildSettings.SetGoodbye(msg, Context.Guild.Id);

                await ReplyAsync(Language.GetString("goodbye_message_set").Replace("{msg}", msg));
            }

            [Command("remove")]
            public async Task RemoveGoodbyeMessageAsync()
            {
                GuildSettings.SetGoodbye(null, Context.Guild.Id);

                await ReplyAsync(Language.GetString("goodbye_message_removed"));
            }
        }
    }
}
