using Qmmands;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("filter", "filters")]
        public class WordFilter : YukiModule
        {
            [Command("add")]
            public async Task AddFilterAsync(string filter)
            {
                GuildSettings.AddFilter($@"\b{filter}\b", Context.Guild.Id);

                await ReplyAsync(Language.GetString("filter_added"));
            }
            
            [Command("addregex")]
            public async Task AddFilterRegexAsync(string filter)
            {
                GuildSettings.AddFilter(filter, Context.Guild.Id);

                await ReplyAsync(Language.GetString("filter_added"));
            }
            
            [Command("list")]
            public async Task ListFiltersAsync(int page = 0)
            {
                string[] filters = GuildSettings.GetGuild(Context.Guild.Id).WordFilter.ToArray();

                await PagedReplyAsync("Filters", filters, 20, showNumbers: true);
            }

            [Command("remove", "rem")]
            public async Task RemoveFilterAsync(int filterIndex)
            {
                GuildSettings.RemoveFilter(--filterIndex, Context.Guild.Id);

                await ReplyAsync(Language.GetString("filter_removed"));
            }
        }
    }
}
