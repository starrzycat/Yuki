using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.OwnerModule
{
    public partial class BotOwnerModule
    {
        [Command("addpatroncmd")]
        public async Task AddPatronCmdAsync(ulong userId, string cmdName, [Remainder]string result)
        {
            Patreon.AddCommand(new PatronCommand()
            {
                UserId = userId,
                Name = cmdName,
                Response = result
            });

            await ReplyAsync(Language.GetString("patron_cmd_added"));
        }
    }
}
