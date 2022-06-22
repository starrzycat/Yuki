using Qmmands;
using System;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.GamblingModule
{
    public partial class GamlingModule
    {
        [Command("roll")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task DiceRollAsync([Remainder] string text = "")
        {
            int dice1 = new Random().Next(1, 6);
            int dice2 = new Random().Next(1, 6);

            await ReplyAsync(Language.GetString("roll_rolled").Replace("{diceval}", (dice1 + dice2).ToString()));
        }
    }
}
