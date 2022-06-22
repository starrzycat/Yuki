using Discord;
using Qmmands;
using System.Threading.Tasks;
using Yuki.API;

namespace Yuki.Commands.Modules.ImageModule
{
    public partial class ImageModule
    {
        [Command("dog")]
        public async Task DogCmdAsync()
        {
            EmbedBuilder img = Context.CreateImageEmbedBuilder(Language.GetString("dog_title"), await DogApi.GetImage());

            await ReplyAsync(img);
        }
    }
}
