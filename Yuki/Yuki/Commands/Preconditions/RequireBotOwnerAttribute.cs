using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data;

namespace Yuki.Commands.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireBotOwnerAttribute : CheckAttribute
    {
        public override ValueTask<CheckResult> CheckAsync(CommandContext context)
        {
            if (Config.GetConfig().owners.Any(o => o == ((YukiCommandContext)context).User.Id))
            {
                return CheckResult.Successful;
            }
            else
            {
                return CheckResult.Unsuccessful("You must be a bot owner to run this command.");
            }
        }
    }
}