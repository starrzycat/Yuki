using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ServerOwnerModule
{
    public partial class ServerOwnerModule
    {
        [Command("resetguild")]
        public async Task ResetGuildData()
        {
            GuildSettings.DeleteGuildData(Context.Guild.Id);
            await ReplyAsync(Language.GetString("guild_data_deleted"));
        }
    }
}
