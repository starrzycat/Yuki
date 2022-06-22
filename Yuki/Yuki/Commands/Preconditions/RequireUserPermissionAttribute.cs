using Discord;
using Qmmands;
using System;
using System.Threading.Tasks;

/*
 * Modification of foxbot's RequireUserPermission attribute to work with qmmands
 * Credit: foxbot
 * https://github.com/discord-net/Discord.Net/blob/dev/src/Discord.Net.Commands/Attributes/Preconditions/RequireUserPermissionAttribute.cs
*/

namespace Yuki.Commands.Preconditions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class RequireUserPermissionAttribute : CheckAttribute
    {
        public GuildPermission? GuildPermission { get; }

        public ChannelPermission? ChannelPermission { get; }

        public bool IsBot { get; }
        public bool AllowDM { get; }

        public RequireUserPermissionAttribute(GuildPermission permission, bool allowDm, bool isBot = false)
        {
            GuildPermission = permission;
            ChannelPermission = null;
            IsBot = isBot;
            AllowDM = allowDm;
        }

        public RequireUserPermissionAttribute(ChannelPermission permission, bool allowDm, bool isBot = false)
        {
            ChannelPermission = permission;
            GuildPermission = null;
            IsBot = isBot;
            AllowDM = allowDm;
        }

        public override ValueTask<CheckResult> CheckAsync(CommandContext _context)
        {
            YukiCommandContext context = (YukiCommandContext)_context;

            if(context.Channel is IDMChannel)
            {
                if(AllowDM)
                {
                    return CheckResult.Successful;
                }
                else
                {
                    return CheckResult.Unsuccessful("Command must be used in a guild channel.");
                }
            }

            IUser user;

            if (!IsBot)
            {
                user = context.User;
            }
            else
            {
                user = context.Client.CurrentUser;
            }

            IGuildUser guildUser = context.Guild.GetUserAsync(user.Id).Result;

            if (GuildPermission.HasValue)
            {
                if (!guildUser.GuildPermissions.Has(GuildPermission.Value))
                {
                    return CheckResult.Unsuccessful($"Sorry, {context.User.Username}! {(IsBot ? "I" : "you")} require the guild permission {GuildPermission.Value}");
                }
            }

            if (ChannelPermission.HasValue)
            {
                ChannelPermissions perms;

                if (context.Channel is IGuildChannel guildChannel)
                {
                    perms = guildUser.GetPermissions(guildChannel);
                }
                else
                {
                    perms = ChannelPermissions.All(context.Channel);
                }

                if (!perms.Has(ChannelPermission.Value))
                {
                    return CheckResult.Unsuccessful($"Sorry, {context.User.Username}! {(IsBot ? "I" : "you")} require the channel permission {ChannelPermission.Value}");
                }
            }

            return CheckResult.Successful;
        }
    }
}
