using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.OwnerModule
{
    public partial class BotOwnerModule
    {
        [Command("removepatron")]
        public async Task RemovePatronAsync(ulong userId)
        {
            UserSettings.RemovePatron(userId);

            await ReplyAsync(Language.GetString("patron_removed"));
        }
    }
}
