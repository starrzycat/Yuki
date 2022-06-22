using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.GamblingModule
{
    [Name("Gambling")]
    [RequireUserPermission(Discord.GuildPermission.EmbedLinks, allowDm: true, isBot: true)]
    public partial class GamlingModule : YukiModule { }
}
