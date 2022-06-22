using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Command("enable")]
        public async Task EnableAsync([Remainder] string option)
        {
            bool found = true;

            ulong guildId = Context.Guild.Id;

            switch(option.ToLower())
            {
                case "welcome":
                    GuildSettings.ToggleWelcome(guildId, true);
                    break;
                case "goodbye":
                    GuildSettings.ToggleGoodbye(guildId, true);
                    break;
                case "logging":
                    GuildSettings.ToggleLogging(guildId, true);
                    break;
                case "message cache":
                case "messagecache":
                    GuildSettings.ToggleCache(guildId, true);
                    break;
                case "selfrole":
                case "roles":
                    GuildSettings.ToggleRoles(guildId, true);
                    break;
                case "filter":
                    GuildSettings.ToggleFilter(guildId, true);
                    break;
                case "starboard":
                    GuildSettings.ToggleStarboard(guildId, true);
                    break;
                case "negastar":
                    GuildSettings.ToggleNegaStar(guildId, true);
                    break;
                default:
                    found = false;
                    break;
            }

            if(found)
            {
                await ReplyAsync(Language.GetString("setting_enabled").Replace("{settingname}", option));
            }
        }
    }
}
