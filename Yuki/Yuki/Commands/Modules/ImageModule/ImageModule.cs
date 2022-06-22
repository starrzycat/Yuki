using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ImageModule
{
    [Name("Image")]
    [RequireUserPermission(Discord.GuildPermission.EmbedLinks, allowDm: true, isBot: true)]
    public partial class ImageModule : YukiModule { }
}
