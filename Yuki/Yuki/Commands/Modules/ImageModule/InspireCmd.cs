using Qmmands;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.ImageModule
{
    public partial class ImageModule
    {
        [Command("inspire")]
        public async Task InspireAsync()
        {
            WebClient client = new WebClient();
            string downloadString = client.DownloadString("https://inspirobot.me/api?generate=true");

            byte[] imageBytes = client.DownloadData(downloadString);

            await Context.Channel.SendFileAsync(new MemoryStream(imageBytes), Path.GetFileName(downloadString));
        }
    }
}
