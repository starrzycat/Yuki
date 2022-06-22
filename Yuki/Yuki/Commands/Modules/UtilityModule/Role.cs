using Discord;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands.Preconditions;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("role", "r")]
        [RequireGuild]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task GiveRoleAsync([Remainder] string roleString)
        {
            try
            {
                IRole queriedRole = default;

                if (MentionUtils.TryParseRole(roleString, out ulong guildRole))
                {
                    queriedRole = Context.Guild.GetRole(guildRole);
                }
                else
                {
                    foreach (IRole role in Context.Guild.Roles)
                    {
                        if (role.Name.ToLower() == roleString.ToLower())
                        {
                            queriedRole = role;
                            break;
                        }
                    }

                    if (queriedRole == default)
                    {
                        await ReplyAsync(Language.GetString("role_not_found").Replace("{role}", roleString).Replace("{user}", Context.User.Username));
                        return;
                    }
                }

                GuildRole assignedRole = GuildSettings.GetGuild(Context.Guild.Id).GuildRoles.FirstOrDefault(role => role.Id == queriedRole.Id);
                
                if (!assignedRole.Equals(default(GuildRole)))
                {
                    if (!((IGuildUser)Context.User).RoleIds.Contains(queriedRole.Id))
                    {
                        bool hasTeamRole = false;
                        GuildRole foundRole = default;

                        foreach (ulong roleId in (await Context.Guild.GetUserAsync(Context.User.Id)).RoleIds)
                        {
                            // needs clean-up possibly?
                            GuildRole role = GuildSettings.GetGuild(Context.Guild.Id).GuildRoles.FirstOrDefault(_role => _role.Id == roleId);


                            if(!role.Equals(default(GuildRole)))
                            {
                                // if there are any team roles not part of a group, set them to the default group
                                if (string.IsNullOrWhiteSpace(role.Group))
                                {
                                    role.Group = "default"; // so we can use it in the next if statement w/out fetching it again
                                    GuildSettings.SetTeamRole(role.Id, Context.Guild.Id, role.IsTeamRole, role.Group, role.IsTeamRoleRemovable);
                                }

                                if (!role.Equals(default(GuildRole)) && role.IsTeamRole && role.Id != assignedRole.Id && role.Group == assignedRole.Group)
                                {
                                    hasTeamRole = true;
                                    foundRole = role;
                                    break;
                                }
                            }
                        }

                        if (hasTeamRole && assignedRole.IsTeamRole)
                        {
                            if(!foundRole.IsTeamRoleRemovable)
                            {
                                return;
                            }

                            await ((IGuildUser)Context.User).RemoveRoleAsync(Context.Guild.GetRole(foundRole.Id));
                        }

                        await ((IGuildUser)Context.User).AddRoleAsync(Context.Guild.GetRole(queriedRole.Id));
                        await ReplyAsync(Language.GetString("role_given").Replace("{role}", queriedRole.Name).Replace("{user}", Context.User.Username));
                    }
                    else
                    {
                        if (!assignedRole.IsTeamRole)
                        {
                            await (await Context.Guild.GetUserAsync(Context.User.Id)).RemoveRoleAsync(Context.Guild.GetRole(queriedRole.Id));

                            await ReplyAsync(Language.GetString("role_taken").Replace("{role}", queriedRole.Name).Replace("{user}", Context.User.Username));
                        }
                    }
                }
                else
                {
                    await ReplyAsync(Language.GetString("role_not_found").Replace("{role}", roleString).Replace("{user}", Context.User.Username));
                }
            }
            catch(Exception e)
            {
                await ReplyAsync(e);
            }
        }
    }
}
