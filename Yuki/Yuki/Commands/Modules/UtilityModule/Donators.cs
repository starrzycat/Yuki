using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("donators")]
        public async Task DonatorsAsync()
        {
            ulong[] donators = UserSettings.GetPatrons();

            if(donators == null)
            {
                await ReplyAsync("donators_none");
                return;
            }

            string content = "";
            
            for(int i = 0; i < donators.Length; i++)
            {
                IUser user = Context.Client.GetUser(donators[i]);

                if(user != null)
                {
                    content += user.Username + "#" + user.Discriminator + "\n";
                }
                else
                {
                    content += donators[i];
                }
            }

            EmbedBuilder embed = Context.CreateColoredEmbed(Color.Purple, new EmbedAuthorBuilder() { Name = Language.GetString("donators_title") }, content)
                                        .WithFooter(Language.GetString("donators_footer"));

            await ReplyAsync(embed);
        }
    }
}
