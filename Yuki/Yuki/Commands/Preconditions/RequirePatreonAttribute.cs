using Qmmands;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Preconditions
{
    public class RequirePatreonAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext c)
        {
            YukiCommandContext context = c as YukiCommandContext;

            if(UserSettings.IsPatron(context.User.Id))
            {
                return CheckResult.Successful;
            }
            else
            {
                return CheckResult.Unsuccessful("You must be a patron to execute this command.");
            }
        }
    }
}
