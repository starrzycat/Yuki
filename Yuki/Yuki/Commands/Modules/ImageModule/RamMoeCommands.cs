using Qmmands;
using System.Threading.Tasks;
using Yuki.API;

namespace Yuki.Commands.Modules.ImageModule
{
    public partial class ImageModule
    {
        [Command("hug")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task HugAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "hug", str);
        }

        [Command("pat")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task PatAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "pat", str);
        }

        [Command("slap")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task SlapAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "slap", str);
        }
        
        [Command("tickle")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task TickleAsync([Remainder] string str = null)
        {
            await RamMoe.SendImageAsync(Context, Language, "tickle", str);
        }
    }
}