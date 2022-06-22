using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.FunModule
{
    [Name("Fun")]
    [RequireUserPermission(Discord.GuildPermission.EmbedLinks, allowDm: true, isBot: true)]
    public partial class FunModule : YukiModule { }
}
