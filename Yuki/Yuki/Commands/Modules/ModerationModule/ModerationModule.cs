using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ModerationModule
{
    [Name("Moderation")]
    [RequireGuild]
    [RequireModerator]
    [RequireUserPermission(Discord.GuildPermission.EmbedLinks, allowDm: true, isBot: true)]
    public partial class ModerationModule : YukiModule { }
}
