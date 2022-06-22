using Qmmands;
using System;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("8ball")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task MagicEightBall([Remainder] string args = "")
        {
            await ReplyAsync(Language.GetString("eightball_response_" + new Random().Next(1, 20)));
        }
    }
}
