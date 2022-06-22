using Qmmands;
using Yuki.Commands.Preconditions;

namespace Yuki.Commands.Modules.ServerOwnerModule
{
    [RequireServerOwner]
    [Name("Server Owner")]
    public partial class ServerOwnerModule : YukiModule { }
}
