using Qmmands;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Yuki.Data;

namespace Yuki.Commands.Modules.ImageModule
{
    public partial class ImageModule
    {
        [Command("person")]
        public async Task GetNonExistentPersonAsync()
        {
            WebClient client = new WebClient();
            byte[] img = client.DownloadData("http://thispersondoesnotexist.com/image");
            
            await Context.Channel.SendFileAsync(new MemoryStream(img), $"{Passphrase.Generate(5)}.jpg");
        }
    }
}
