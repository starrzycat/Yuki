using Discord;
using Discord.WebSocket;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Core;
using Yuki.Data;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Extensions;
using Yuki.Services;
using Yuki.Services.Database;

namespace Yuki.Events
{
    public static class DiscordSocketEventHandler
    {
        private static Language GetLanguage(ISocketMessageChannel channel)
        {
            return GetLanguage((channel as IGuildChannel).GuildId);
        }

        private static Language GetLanguage(ulong guildId)
        {
            return Localization.GetLanguage(GuildSettings.GetGuild(guildId).LangCode);
        }

        public static bool HasPrefix(SocketUserMessage message, GuildConfiguration config, out string output)
        {
            output = string.Empty;
            if (!(message.Channel is IDMChannel))
            {
                if (config.EnablePrefix && config.Prefix != null)
                {
                    return CommandUtilities.HasPrefix(message.Content.ToLower(), config.Prefix.ToLower(), out output);
                }
            }

            return CommandUtilities.HasAnyPrefix(message.Content.ToLower(), Config.GetConfig().prefix.Select(p => p.ToLower()), out string prefix, out output);
        }


        public static async Task MessageReceived(SocketMessage socketMessage)
        {
            try
            {
                if (!(socketMessage is SocketUserMessage message))
                    return;
                if (message.Source != MessageSource.User)
                    return;

                DiscordSocketClient shard = (message.Channel is IGuildChannel) ?
                                                YukiBot.Discord.Client.GetShardFor(((IGuildChannel)message.Channel).Guild) :
                                                YukiBot.Discord.Client.GetShard(0);

                GuildConfiguration config = GuildSettings.GetGuild((message.Channel as IGuildChannel).GuildId);

                bool hasPrefix = HasPrefix(message, config, out string trimmedContent);


                if (!(message.Channel is IDMChannel))
                {
                    await WordFilter.CheckFilter(message, config);
                }

                if (!hasPrefix)
                {
                    UserMessageCache.AddOrUpdate(socketMessage, config);

                    return;
                }

                IResult result = YukiBot.Discord.CommandService
                                    .ExecuteAsync(trimmedContent, new YukiCommandContext(
                                            YukiBot.Discord.Client, socketMessage as IUserMessage, YukiBot.Discord.Services)).Result;

                if (result is FailedResult failedResult)
                {
                    if (!(failedResult is CommandNotFoundResult) && failedResult is ChecksFailedResult checksFailed)
                    {
                        if (checksFailed.FailedChecks.Count == 1)
                        {
                            await message.Channel.SendMessageAsync(checksFailed.FailedChecks[0].Result.Reason);
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync($"The following checks failed:\n\n" +
                                    $"{string.Join("\n", checksFailed.FailedChecks.Select(check => check.Result.Reason))}");
                        }
                    }
                }

                if (UserSettings.IsPatron(message.Author.Id))
                {
                    PatronCommand cmd = Patreon.GetCommand(message.Author.Id, trimmedContent.Split(' ')[0].ToLower());

                    if (!cmd.Equals(default))
                    {
                        YukiContextMessage msg = new YukiContextMessage(message.Author, (message.Author as IGuildUser).Guild);

                        await message.Channel.SendMessageAsync(StringReplacements.GetReplacement(trimmedContent, cmd.Response, msg));

                        return;
                    }
                }

                if (message.Channel is IDMChannel)
                {
                    return;
                }

                GuildCommand execCommand = GuildSettings.GetGuild((message.Channel as IGuildChannel).GuildId).Commands
                                                        .FirstOrDefault(cmd => cmd.Name.ToLower() == trimmedContent.Split(' ')[0].ToLower());

                if (!execCommand.Equals(null) && !execCommand.Equals(default) && !execCommand.Equals(null) && !string.IsNullOrEmpty(execCommand.Response))
                {
                    YukiContextMessage msg = new YukiContextMessage(message.Author, (message.Author as IGuildUser).Guild);

                    if (execCommand.Response.IsMedia())
                    {
                        await message.Channel.SendMessageAsync("", false, new EmbedBuilder().WithImageUrl(execCommand.Response).WithColor((message.Author as IGuildUser).HighestRole().Color).Build());
                    }
                    else
                    {
                        await message.Channel.SendMessageAsync(StringReplacements.GetReplacement(trimmedContent, execCommand.Response, msg));
                    }
                }
            }
            catch(Exception e)
            {
                await Logger.LogWarning(e);
            }
        }

        public static async Task MessageUpdated(Cacheable<IMessage, ulong> messageOld, SocketMessage current, ISocketMessageChannel channel)
        {
            if (current.Content != null)
            {

                if(channel is IDMChannel)
                {
                    return;
                }

                GuildConfiguration config = GuildSettings.GetGuild((current.Channel as IGuildChannel).GuildId);

                await WordFilter.CheckFilter(current as SocketUserMessage, config);

                UserMessageCache.AddOrUpdate(current, config);

                if (!current.Author.IsBot && messageOld.Value != null && current.Content != messageOld.Value.Content)
                {
                    string oldContent = messageOld.Value.Content;
                    string newContent = current.Content;

                    if (oldContent.Length > 1000)
                    {
                        oldContent = oldContent.Substring(0, 997) + "...";
                    }

                    if (newContent.Length > 1000)
                    {
                        newContent = newContent.Substring(0, 997) + "...";
                    }

                    Language lang = GetLanguage(channel);

                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(lang.GetString("event_message_updated"), current.Author.GetAvatarUrl())
                        .WithDescription($"{lang.GetString("event_message_id")}: {current.Id}\n" +
                                         $"{lang.GetString("event_message_channel")}: {MentionUtils.MentionChannel(channel.Id)} ({channel.Id})\n" +
                                         $"{lang.GetString("event_message_author")}: {MentionUtils.MentionUser(current.Author.Id)}")
                        .AddField(lang.GetString("event_message_old"), oldContent)
                        .AddField(lang.GetString("event_message_new"), newContent)
                        .WithColor(Color.Gold);

                    await LogMessage(embed, (channel as IGuildChannel).GuildId);
                }
            }
        }

        public static async Task MessageDeleted(Cacheable<IMessage, ulong> _message, ISocketMessageChannel channel)
        {
            if (_message.HasValue)
            {
                IMessage message = _message.Value;

                UserMessageCache.Delete(message.Author.Id, message.Id);

                if (!message.Author.IsBot)
                {
                    string content = message.Content;

                    if (content.Length > 1000)
                    {
                        content = content.Substring(0, 997) + "...";
                    }

                    Language lang = GetLanguage(channel);

                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(lang.GetString("event_message_deleted"), message.Author.GetAvatarUrl())
                        .WithDescription($"{lang.GetString("event_message_id")}: {message.Id}\n" +
                                         $"{lang.GetString("event_message_channel")}: {MentionUtils.MentionChannel(channel.Id)} ({channel.Id})\n" +
                                         $"{lang.GetString("event_message_author")}: {MentionUtils.MentionUser(message.Author.Id)}")
                        .WithColor(Color.Red);

                    if (!string.IsNullOrEmpty(content))
                    {
                        embed.AddField(lang.GetString("message_content"), content);
                    }

                    if (message.Attachments != null && message.Attachments.Count > 0)
                    {
                        string attachments = string.Empty;
                        string imageUrl = null;

                        IAttachment[] _attachments = message.Attachments.ToArray();

                        imageUrl = _attachments.FirstOrDefault(img => img.ProxyUrl.IsMedia())?.ProxyUrl;

                        for (int i = 0; i < _attachments.Length; i++)
                        {
                            attachments += $"[{_attachments[i].Filename}]({_attachments[i].ProxyUrl})\n";
                        }

                        if (imageUrl != null)
                        {
                            embed.WithImageUrl(imageUrl);
                        }

                        embed.AddField(lang.GetString("message_attachments"), attachments);
                    }

                    await LogMessage(embed, ((IGuildChannel)channel).GuildId);
                }
            }
        }

        public static async Task ReactionAdded(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            IUserMessage msg = message.GetOrDownloadAsync().Result;

            /*if (!message.HasValue || msg.Equals(null) || !reaction.User.IsSpecified)
            {
                return;
            }*/


            if (channel is IDMChannel)
            {
                return;
            }

            await NegaStars.Manage(msg, channel as ITextChannel, reaction);
            await Starboard.Manage(msg, channel as ITextChannel, reaction, isDeleteCheck: false);
        }

        public static async Task ReactionRemoved(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            IUserMessage msg = message.GetOrDownloadAsync().Result;

            if (channel is IDMChannel)
            {
                return;
            }


            await Starboard.Manage(msg, channel as ITextChannel, reaction, isDeleteCheck: true);
        }

        public static async Task ReactionsCleared(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel)
        {
            IUserMessage msg = message.GetOrDownloadAsync().Result;

            if (channel is IDMChannel)
            {
                return;
            }


            await Starboard.ClearStars(msg, channel as ITextChannel);
        }

        public static async Task UserBanned(SocketUser user, SocketGuild guild)
        {
            Language lang = GetLanguage(guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithAuthor(lang.GetString("event_user_banned"))
                .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

            await LogMessage(embed, guild.Id);
        }

        public static async Task UserJoined(SocketGuildUser user)
        {
            try
            {
                Language lang = GetLanguage(user.Guild.Id);

                EmbedBuilder embed = new EmbedBuilder()
                    .WithColor(Color.Green)
                    .WithAuthor(lang.GetString("event_user_join").Replace("{user}", $"{user.Username}#{user.Discriminator}"))
                    .AddField(lang.GetString("event_user_name"), $"{user.Id}", true)
                    .AddField(lang.GetString("uinf_acc_create"), user.CreatedAt, true);

                await LogMessage(embed, user.Guild.Id);

                GuildConfiguration guild = GuildSettings.GetGuild(user.Guild.Id);

                if (guild.EnableWelcome && !string.IsNullOrWhiteSpace(guild.WelcomeMessage) && user.Guild.Channels.ToList().Any(ch => ch.Id == guild.WelcomeChannel))
                {
                    YukiContextMessage msg = new YukiContextMessage(user, user.Guild);

                    await user.Guild.GetTextChannel(guild.WelcomeChannel).SendMessageAsync(StringReplacements.GetReplacement(null, guild.WelcomeMessage, msg));
                }

                if (!GuildSettings.GetGuild(guild.Id).Equals(default) && GuildSettings.GetGuild(guild.Id).GuildRoles != null)
                {
                    GuildRole[] roles = GuildSettings.GetGuild(guild.Id).GuildRoles.Where(r => r.IsAutoRole).ToArray();

                    for (int i = 0; i < roles.Length; i++)
                    {
                        IRole role = user.Guild.GetRole(roles[i].Id);

                        if (role != default)
                        {
                            await user.AddRoleAsync(role);
                        }
                    }
                }
            }
            catch(Exception e)
            {
                await ((ITextChannel)YukiBot.Discord.Client.GetChannel(674991918647869461)).SendMessageAsync($"An error occurred:\n{e}");
            }
        }

        public static async Task UserLeft(SocketGuildUser user)
        {
            Language lang = GetLanguage(user.Guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Red)
                .WithAuthor(lang.GetString("event_user_leave"))
                .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

            await LogMessage(embed, user.Guild.Id);

            GuildConfiguration guild = GuildSettings.GetGuild(user.Guild.Id);

            if (guild.EnableGoodbye && !string.IsNullOrWhiteSpace(guild.GoodbyeMessage) && user.Guild.Channels.ToList().Any(ch => ch.Id == guild.WelcomeChannel))
            {
                YukiContextMessage msg = new YukiContextMessage(user, user.Guild);

                await user.Guild.GetTextChannel(guild.WelcomeChannel).SendMessageAsync(StringReplacements.GetReplacement(null, guild.GoodbyeMessage, msg));
            }
        }

        public static async Task UserUnbanned(SocketUser user, SocketGuild guild)
        {
            Language lang = GetLanguage(guild.Id);

            EmbedBuilder embed = new EmbedBuilder()
                .WithColor(Color.Orange)
                .WithAuthor(lang.GetString("event_user_unban"))
                .AddField(lang.GetString("event_user_name"), $"{user.Username}#{user.Discriminator} ({user.Id})");

            await LogMessage(embed, guild.Id);
        }

        public static Task RoleDeleted(SocketRole socketRole)
        {
            GuildConfiguration config = GuildSettings.GetGuild(socketRole.Guild.Id);

            if (config.GuildRoles.Any(role => role.Id == socketRole.Id))
            {
                GuildSettings.RemoveRole(socketRole.Id, socketRole.Guild.Id);
            }

            return Task.CompletedTask;
        }

        public static Task JoinedGuild(SocketGuild guild)
        {
            GuildConfiguration guildConfig = GuildSettings.GetGuild(guild.Id);

            if(!guildConfig.Equals(default(GuildConfiguration)) && guildConfig.IsDirty)
            {
                GuildSettings.MarkClean(guild.Id);
            }
            
            return Task.CompletedTask;
        }

        public static Task LeftGuild(SocketGuild guild)
        {
            //GuildSettings.DeleteGuildData(guild.Id);
            //StarboardData.RemovePostsFromGuild(guild.Id);

            GuildSettings.MarkDirty(guild.Id);

            return Task.CompletedTask;
        }

        public static async Task LogMessage(EmbedBuilder embed, ulong guildId)
        {
            GuildConfiguration config = GuildSettings.GetGuild(guildId);

            if (!config.Equals(null) && config.EnableLogging && config.LogChannel != 0 && YukiBot.Discord.Client.GetGuild(guildId).TextChannels.Any(c => c.Id == config.LogChannel))
            {
                await YukiBot.Discord.Client.GetGuild(guildId).GetTextChannel(config.LogChannel).SendMessageAsync("", false, embed.WithCurrentTimestamp().Build());
            }
        }
    }
}
