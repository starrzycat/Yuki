using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.API;

namespace Yuki.Commands.Modules.ImageModule
{
    public partial class ImageModule
    {
        [Command("cat")]
        public async Task CatCmdAsync()
        {
            EmbedBuilder img = Context.CreateImageEmbedBuilder(Language.GetString("cat_title"), await CatApi.GetImage());

            await ReplyAsync(img);
        }
    }
}
