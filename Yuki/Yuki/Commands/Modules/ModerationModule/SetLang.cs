using Qmmands;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Command("setlang", "lang")]
        public async Task SetLangAsync(string langCode)
        {
            if(langCode == "default")
            {
                langCode = "en_US";
            }

            Language found = Localization.GetLanguage(langCode);

            if (found.Code.ToLower() != langCode.ToLower())
            {
                await ReplyAsync(Language.GetString("language_not_found"));
            }
            else
            {
                GuildSettings.SetLanguage(found.Code, Context.Guild.Id);
                await ReplyAsync(Language.GetString("lang_set_to").Replace("{lang}", GuildSettings.GetGuild(Context.Guild.Id).LangCode));
            }
        }
    }
}
