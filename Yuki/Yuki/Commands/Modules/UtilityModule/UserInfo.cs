using Discord;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        /* Basic perms that just fill up the perm list, no need to list them */
        GuildPermission[] blacklisterPerms = new GuildPermission[]
        {
            GuildPermission.AddReactions,
            GuildPermission.AttachFiles,
            GuildPermission.ChangeNickname,
            GuildPermission.Connect,
            GuildPermission.CreateInstantInvite,
            GuildPermission.EmbedLinks,
            GuildPermission.PrioritySpeaker,
            GuildPermission.ReadMessageHistory,
            GuildPermission.SendMessages,
            GuildPermission.SendTTSMessages,
            GuildPermission.Speak,
            GuildPermission.Stream,
            GuildPermission.UseExternalEmojis,
            GuildPermission.UseVAD,
            GuildPermission.ViewAuditLog,
            GuildPermission.ViewChannel
        };

        [Command("userinfo")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GetUserInfoAsync(string userString = null)
        {
            try
            {
                IUser user = Context.User;

                if (userString != null && MentionUtils.TryParseUser(userString, out ulong userId))
                {
                    user = Context.Client.GetUser(userId);
                }

                string activity = (user.Activity != null && user.Activity.Type.ToString() != "4") ? Language.GetString(user.Activity.Type.ToString().ToLower()) : Language.GetString("activity");

                string game = user.Activity != null ? user.Activity.Name.ToString().ToLower() : Language.GetString("none");

                EmbedBuilder embed = new EmbedBuilder()
                    .WithColor(Colors.Pink)
                    .WithImageUrl(user.GetAvatarUrl())
                    .WithAuthor(new EmbedAuthorBuilder()
                    {
                        IconUrl = user.GetAvatarUrl(),
                        Name = user.Username + "#" + user.Discriminator
                    })
                    .AddField(Language.GetString("uinf_id"), user.Id, true)
                    .AddField(activity, game, true)
                    .AddField(Language.GetString("uinf_status"), Language.GetString(user.Status.ToString().ToLower()), true)
                    .AddField(Language.GetString("uinf_acc_create"), user.CreatedAt.DateTime.ToPrettyTime(false, false), true)
                    .WithCurrentTimestamp();

                if (user is IGuildUser guildUser)
                {
                    string perms = (!guildUser.GuildPermissions.ToList().Equals(null) && guildUser.GuildPermissions.ToList().Count > 0) ?
                        string.Join(", ", guildUser.GuildPermissions.ToList().Where(perm => !blacklisterPerms.Contains(perm))) : Language.GetString("none");

                    string roles = (guildUser.RoleIds != null && guildUser.RoleIds.Count > 0) ?
                                    string.Join(", ", Context.Guild.Roles.Where(role => guildUser.RoleIds.Contains(role.Id) && role != Context.Guild.EveryoneRole).Select(role => role.Name))
                                    : Language.GetString("none");

                    if(guildUser.RoleIds.Count > 0 && guildUser.HighestRole().Color != Color.Default)
                    {
                        embed.WithColor(guildUser.HighestRole().Color);
                    }

                    embed.AddField(Language.GetString("uinf_acc_join"), guildUser.JoinedAt.Value.DateTime.ToPrettyTime(false, false), true);


                    if(!string.IsNullOrWhiteSpace(perms))
                    {
                        embed.AddField(Language.GetString("uinf_permissions"), string.Join(", ", perms), true);
                    }

                    if(!string.IsNullOrWhiteSpace(roles))
                    {
                        embed.AddField(Language.GetString("uinf_roles").Replace("{count}", guildUser.RoleIds.Count.ToString()), roles, true);
                    }

                    if (guildUser.PremiumSince.HasValue)
                    {
                        embed.AddField(Language.GetString("booster_since"), guildUser.PremiumSince.Value.DateTime.ToPrettyTime(false, false), true);
                    }
                }

                await ReplyAsync(embed);
            }
            catch(Exception e)
            {
                await Logger.LogError(e);
            }
        }
    }
}
