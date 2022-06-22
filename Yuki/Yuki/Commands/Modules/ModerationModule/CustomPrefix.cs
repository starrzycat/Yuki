using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Command("prefix")]
        public async Task CustomPrefixAsync([Remainder] string prefix)
        {
            if(prefix.Length > 5)
            {
                await ReplyAsync(Language.GetString("prefix_less_than_five"));
                return;
            }

            GuildSettings.AddPrefix(prefix, Context.Guild.Id);
        }
    }
}
