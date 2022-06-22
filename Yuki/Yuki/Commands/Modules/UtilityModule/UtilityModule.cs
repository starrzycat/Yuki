using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.UtilityModule
{
    [Name("Utility")]
    [RequireUserPermission(Discord.GuildPermission.EmbedLinks, allowDm: true, isBot: true)]
    public partial class UtilityModule : YukiModule { }
}
