using Qmmands;
using System.Threading.Tasks;
using Yuki.Data;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("userreset")]
        public async Task ResetDataAsync()
        {
            UserSettings.Remove(Context.User.Id);
            UserMessageCache.DeleteFromUser(Context.User.Id);

            await ReplyAsync(Language.GetString("user_data_deleted"));
        }
    }
}
