using Discord;
using Discord.WebSocket;
using Qmmands;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Core;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("serverinfo")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task ServerInfoAsync()
        {

            SocketGuild guild = Context.Client.GetShardFor(Context.Guild).GetGuild(Context.Guild.Id);

            SocketGuildUser owner = guild.Owner;

            int voiceCount = guild.VoiceChannels.Count;
            int textCount = guild.TextChannels.Count;

            string channels = textCount + " " + Language.GetString("serverinfo_channels_text") + "\n" +
                              voiceCount + " " + Language.GetString("serverinfo_channels_voice") + "\n\n" +
                              guild.CategoryChannels.Count + " " + Language.GetString("serverinfo_categories") + "\n";

            Embed embed = new EmbedBuilder()
                .WithAuthor(new EmbedAuthorBuilder()
                {
                    IconUrl = guild.IconUrl,
                    Name = guild.Name
                })
                .WithThumbnailUrl(guild.IconUrl)
                .WithColor(Colors.Pink)
                .AddField(Language.GetString("serverinfo_owner"), $"{owner.Username}#{owner.Discriminator} ({owner.Id})")
                .AddField("ID", guild.Id, true)
                .AddField("Shard", Context.Client.GetShardFor(Context.Guild).ShardId, true)
                .AddField(Language.GetString("serverinfo_region"), guild.VoiceRegionId, true)
                .AddField(Language.GetString("serverinfo_verification_level"), guild.VerificationLevel, true)
                .AddField(Language.GetString("serverinfo_channels") + $" [{textCount + voiceCount}]", channels, true)
                .AddField(Language.GetString("serverinfo_members") + $" [{guild.MemberCount}]", $"{guild.Users.Where(user => user.Status != UserStatus.Offline).Count()} {Language.GetString("serverinfo_online")}", true)
                .AddField(Language.GetString("serverinfo_roles") + $" [{guild.Roles.Count}]", Language.GetString("serverinfo_roles_view"), true)
                .WithFooter(Language.GetString("serverinfo_created") + ": " + guild.CreatedAt.DateTime.ToPrettyTime(false, false))
                .Build();

            await ReplyAsync(embed);
        }
    }
}
