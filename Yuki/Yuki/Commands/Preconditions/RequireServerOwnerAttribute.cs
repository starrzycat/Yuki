using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Preconditions
{
    public class RequireServerOwnerAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext c)
        {
            YukiCommandContext context = c as YukiCommandContext;

            if (context.Guild.OwnerId == context.User.Id)
            {
                return CheckResult.Successful;
            }

            return CheckResult.Unsuccessful("You must be the server owner to execute this command.");
        }
    }
}
