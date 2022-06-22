using Discord;
using Qmmands;
using System.Diagnostics;
using System.Threading.Tasks;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("ping")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task PingAsync()
        {
            EmbedBuilder embedBuilder = Context.CreateEmbedBuilder(Language.GetString("ping_pinging"));

            Stopwatch watch = Stopwatch.StartNew();
            IUserMessage msg = await embedBuilder.SendToAsync(Context.Channel);
            watch.Stop();

            await msg.ModifyAsync(emb =>
            {
                embedBuilder.WithDescription(
                    Language.GetString("ping_pong") + "\n" +
                    Language.GetString("ping_latency").Replace("{latency}", watch.ElapsedMilliseconds.ToString()) + "\n" +
                    Language.GetString("ping_api_latency").Replace("{latency}", Context.Client.Latency.ToString())
                );
                emb.Embed = embedBuilder.Build();
            });
        }
    }
}
