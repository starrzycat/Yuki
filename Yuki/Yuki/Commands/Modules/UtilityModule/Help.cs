using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("help")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task HelpAsync([Remainder] string commandStr = "")
        {
            Help help = new Help();

            await help.Get(Context, commandStr);
        }
    }
}
