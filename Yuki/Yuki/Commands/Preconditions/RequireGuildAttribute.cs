using Discord;
using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Preconditions
{
    public class RequireGuildAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext c)
        {
            YukiCommandContext context = c as YukiCommandContext;

            if (context.Channel is IDMChannel)
            {
                return CheckResult.Unsuccessful("This command can only be executed inside a guild channel.");
            }
            else
            {
                return CheckResult.Successful;
            }
        }
    }
}
