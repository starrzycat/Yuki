using Qmmands;
using System.Threading.Tasks;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("calculate", "calc")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task CalculateAsync([Remainder] string expression)
        {
            await ReplyAsync(expression.Calculate());
        }
    }
}
