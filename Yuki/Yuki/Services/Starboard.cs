using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Extensions;
using Yuki.Services.Database;

namespace Yuki.Services
{
    public class Starboard
    {
        public const string Emote = "⭐";

        private const int TOPCOUNT = 5;

        private static List<ulong> guildsSent = new List<ulong>(); // TEMPORARY FIX

        private static bool running = false;

        public static async Task Manage(IUserMessage message, ITextChannel channel, SocketReaction reaction, bool isDeleteCheck)
        {
            IGuild guild = channel.Guild;
            IGuildUser user = await guild.GetUserAsync(message.Author.Id);

            GuildConfiguration config = GuildSettings.GetGuild(guild.Id);

            IUserMessage starPost = default;

            if (!config.Equals(null) && config.EnableStarboard)
            {
                if (!reaction.User.Value.IsBot && message.Author.Id != reaction.UserId)
                {
                    if (config.StarboardIgnoredChannels != null && !config.StarboardIgnoredChannels.Contains(message.Channel.Id))
                    {
                        // make sure the message wasn't sent more than a week ago
                        if ((DateTime.Now - message.Timestamp).Days <= 7)
                        {
                            if(StarboardData.MessageIsStarPost(guild.Id, message.Id))
                            {
                                return;
                            }

                            StarboardPost foundPost = StarboardData.GetPostFor(guild.Id, message.Id);
                            
                            if(reaction.Emote.Name != new Emoji(Emote).Name)
                            {
                                return;
                            }

                            if (isDeleteCheck)
                            {
                                foundPost.Score--;
                            }
                            else
                            {
                                foundPost.Score++;
                            }

                            int stars = message.Reactions.FirstOrDefault(r => r.Key.Name == Emote).Value.ReactionCount;

                            if (foundPost.Score > stars)
                            {
                                foundPost.Score = stars;
                            }
                            else if(foundPost.Score < stars - 1)
                            {
                                foundPost.Score = stars - 1;
                            }

                            // if the post is found, get the message
                            if (!foundPost.Equals(default(StarboardPost)) && foundPost.PostId > 0)
                            {
                                starPost = await (await guild.GetTextChannelAsync(config.StarboardChannel)).GetMessageAsync(foundPost.PostId) as IUserMessage;
                            }

                            if (foundPost.Equals(default(StarboardPost)) || starPost == null || foundPost.PostId <= 0)
                            {
                                if (foundPost.Score >= config.StarRequirement)
                                {
                                    starPost = await (await guild.GetTextChannelAsync(config.StarboardChannel)).SendMessageAsync("", false, CreateEmbed(message, config, foundPost.Score).Build());

                                    foundPost.PostId = starPost.Id;
                                }
                            }
                            else
                            {
                                await starPost.ModifyAsync(a =>
                                {
                                    a.Embed = CreateEmbed(message, config, foundPost.Score).Build();
                                });
                            }

                            if (isDeleteCheck && foundPost.Score < 1)
                            {
                                StarboardData.RemovePostById(foundPost.Id);

                                if (starPost != default || starPost != null)
                                {
                                    await starPost.DeleteAsync();
                                }
                            }
                            else
                            {
                                StarboardData.AddOrUpdatePost(new StarboardPost()
                                {
                                    Id = message.Id,
                                    GuildId = guild.Id,
                                    ChannelId = channel.Id,
                                    PostId = foundPost.PostId,
                                    Score = foundPost.Score,
                                    Timestamp = DateTime.Now
                                });
                            }
                        }
                    }
                }
            }
        }

        public static async Task ClearStars(IUserMessage message, ITextChannel channel)
        {
            IGuild guild = channel.Guild;
            IGuildUser user = await guild.GetUserAsync(message.Author.Id);

            GuildConfiguration config = GuildSettings.GetGuild(guild.Id);

            if (!config.Equals(null) && config.EnableStarboard)
            {
                if (config.StarboardIgnoredChannels != null && !config.StarboardIgnoredChannels.Contains(message.Channel.Id))
                {
                    // make sure the message wasn't sent more than a week ago
                    if ((DateTime.Now - message.Timestamp).Days <= 7)
                    {
                        if (StarboardData.MessageIsStarPost(guild.Id, message.Id))
                        {
                            return;
                        }

                        StarboardPost foundPost = StarboardData.GetPostFor(guild.Id, message.Id);

                        if (!foundPost.Equals(default(StarboardPost)) && foundPost.PostId > 0)
                        {
                            IUserMessage starPost = await (await guild.GetTextChannelAsync(config.StarboardChannel)).GetMessageAsync(foundPost.PostId) as IUserMessage;
                            
                            StarboardData.RemovePostById(foundPost.Id);

                            if (starPost != default || starPost != null)
                            {
                                await starPost.DeleteAsync();
                            }
                        }
                    }
                }
            }
        }

        private static EmbedBuilder CreateEmbed(IUserMessage message, GuildConfiguration config, int starCount)
        {
            Language lang = Localization.GetLanguage(config.LangCode);

            string content = message.Content;

            if (string.IsNullOrWhiteSpace(content))
            {
                content = lang.GetString("starboard_jump_to");
            }

            EmbedBuilder embed = new EmbedBuilder()
                    .WithAuthor(lang.GetString("starboard_title"))
                    .WithDescription($"[{content}]({message.GetJumpUrl()})")
                    .AddField(lang.GetString("starboard_field_author"), message.Author.Mention, true)
                    .AddField(lang.GetString("starboard_field_channel"), ((ITextChannel)message.Channel).Mention, true)
                    .WithFooter($"{Emote} {starCount} {lang.GetString("starboard_stars")} ({message.Id})").WithCurrentTimestamp()
                    .WithColor(Color.Gold);

            if (message.Attachments != null && message.Attachments.Count > 0)
            {
                string attachments = string.Empty;
                string imageUrl = null;

                IAttachment[] _attachments = message.Attachments.ToArray();

                foreach (string str in message.Content.Split(' '))
                {
                    if (str.IsMedia())
                    {
                        if (imageUrl == null)
                        {
                            imageUrl = str;
                        }

                        attachments += $"[{Path.GetFileName(message.Content)}]({str})\n";
                    }
                }

                if (imageUrl == null)
                {
                    imageUrl = _attachments.FirstOrDefault(img => img.ProxyUrl.IsMedia())?.ProxyUrl;
                }

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

            return embed;
        }

        public static async Task CheckTopPosts()
        {
            try
            {
                if (running)
                {
                    return;
                }

                DateTime now = DateTime.Now;
                //DateTime nextMonth = new DateTime(now.AddMonths(1).Year, now.AddMonths(1).Month, 1, 13, 0, 0);

                if (!File.Exists(FileDirectories.SBEvent))
                {
                    DateTime lastMonth = now.AddMonths(-1);

                    File.WriteAllText(FileDirectories.SBEvent, lastMonth.ToShortDateString());
                }

                string line = File.ReadAllLines(FileDirectories.SBEvent)[0];

                DateTime savedDate = DateTime.Parse(line);


                //if (now.Day == 12)
                //if (now.Day == 31)
                if (now.Month > savedDate.Month)
                {
                    if (File.Exists(FileDirectories.SBEvent))
                    {
                        await Logger.LogDebug(line + " " + now.ToShortDateString());
                        if (savedDate >= now)
                        {
                            return;
                        }
                    }

                    await Logger.LogDebug("Sending top posts...");
                    await SendTopPosts();
                    await Logger.LogDebug("Writing file....");
                    File.WriteAllText(FileDirectories.SBEvent, now.ToShortDateString());
                    await Logger.LogDebug("done");
                }

                guildsSent.Clear();
                //Logger.Write(LogLevel.Debug, guildsSent.Count);
            }
            catch(Exception e)
            {
                await ((ITextChannel)YukiBot.Discord.Client.GetGuild(267732080564240395).GetChannel(267734473595027456)).SendMessageAsync($"an error occurred:\n{e}");
            }
        }

        private static async Task SendTopPosts()
        {
            running = true;
            //Logger.Write(LogLevel.Debug, "test");
            GuildConfiguration[] guilds = GuildSettings.GetGuilds().ToArray(); //new GuildConfiguration[] { GuildSettings.GetGuild(267732080564240395) };

            DateTime lastMonth = DateTime.Now.AddMonths(-1);

            for (int i = 0; i < guilds.Length; i++)
            {
                if (guildsSent.Contains(guilds[i].Id))
                {
                    continue;
                }
                
                if (guilds[i].EnableStarboard)
                {
                    //Logger.Write(LogLevel.Debug, guilds[i].Id);

                    StarboardPost[] posts = StarboardData.GetPostsFromGuild(guilds[i].Id).Where(p => p.Timestamp.Month == lastMonth.Month)
                                                .OrderByDescending(p => p.Score).Take(TOPCOUNT).Reverse().ToArray();

                    Language lang = Localization.GetLanguage(guilds[i].LangCode);
                    string content = "";

                    for (int j = 0; j < posts.Length; j++)
                    {
                        if (posts[j].ChannelId != 0)
                        {
                            try
                            {
                                IUserMessage msg = await (await (YukiBot.Discord.Client.GetGuild(guilds[i].Id) as IGuild).GetTextChannelAsync(posts[j].ChannelId)).GetMessageAsync(posts[j].Id) as IUserMessage;

                                string msgContent = new string(msg.Content.Take(377).ToArray());

                                if (string.IsNullOrWhiteSpace(msgContent) && msg.Attachments.Count > 0)
                                {
                                    msgContent = new string(msg.Attachments.First().Filename.Take(377).ToArray());
                                }

                                if (msgContent.Length < msg.Content.Length)
                                {
                                    msgContent += "...";
                                }

                                content += $"**{TOPCOUNT - j}.** [{msgContent}]({msg.GetJumpUrl()})  | {lang.GetString("star_count").Replace("{stars}", posts[j].Score.ToString())}\n";
                            }
                            catch(Exception) { }
                        }
                        //catch (NullReferenceException) { }
                    }

                    EmbedBuilder embed = new EmbedBuilder()
                        .WithAuthor(lang.GetString("top_posts").Replace("{count}", TOPCOUNT.ToString()).Replace("{month}", lastMonth.ToString("MMMM")))
                        .WithDescription(content)
                        .WithColor(Color.Gold);

                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        //await YukiBot.Discord.Client.GetGuild(guilds[i].Id).GetTextChannel(guilds[i].StarboardChannel).SendMessageAsync("", false, embed.Build());
                        await Logger.LogDebug("posted top posts in guild " + YukiBot.Discord.Client.GetGuild(guilds[i].Id).Name);
                        guildsSent.Add(guilds[i].Id);
                    }
                }
            }

            running = false;
        }
    }
}