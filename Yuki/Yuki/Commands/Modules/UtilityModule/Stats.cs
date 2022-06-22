using ByteSizeLib;
using Discord;
using Qmmands;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("stats")]
        public async Task StatsAsync()
        {
            int shardId = 0;

            if (Context.Channel is IGuildChannel)
            {
                shardId = YukiBot.Discord.Client.GetShardIdFor(Context.Guild);
            }

            EmbedBuilder embed = new EmbedBuilder()
                .WithAuthor(Language.GetString("bot_stats_title"), Context.Client.CurrentUser.GetAvatarUrl())
                .WithColor(Colors.Pink)
                .AddField(Language.GetString("bot_stats_users_unique"),
                          Context.Client.Guilds.SelectMany(guild => guild.Users).GroupBy(user => user.Id).Select(user => user.First()).Count(), true)
                .AddField(Language.GetString("bot_stats_guilds"), Context.Client.Guilds.Count, true)
                .AddField(Language.GetString("bot_stats_creator"), "Vee#0003", true)
                .AddField(Language.GetString("bot_stats_uptime"), Process.GetCurrentProcess().StartTime.ToUniversalTime().ToPrettyTime(true, false), true)
                .AddField(Language.GetString("bot_stats_shard"), $"{shardId}/{YukiBot.Discord.ShardCount - 1}", true)
                .AddField(Language.GetString("bot_stats_memory"), ByteSize.FromBytes(Convert.ToDouble(GC.GetTotalMemory(true))), true);

            await ReplyAsync(embed);
        }
    }
}
