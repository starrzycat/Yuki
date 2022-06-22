using Qmmands;
using System;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("reverse")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ReverseStringAsync([Remainder]string text)
        {
            char[] c = text.ToCharArray();

            Array.Reverse(c);

            await ReplyAsync(new string(c));
        }
    }
}
