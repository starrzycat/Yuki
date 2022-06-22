using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.OwnerModule
{
    [Name("Bot Owner")]
    [RequireBotOwner]
    public partial class BotOwnerModule : YukiModule { }
}