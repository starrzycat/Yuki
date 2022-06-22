using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Events;

namespace Yuki.Services
{
    public static class WordFilter
    {
        public static Task CheckFilter(SocketUserMessage message, GuildConfiguration config)
        {
            IGuildChannel guildChannel = (message.Channel as IGuildChannel);

            Language lang = Localization.GetLanguage(config.LangCode);
            
            if (config.EnableFilter)
            {
                for(int i = 0; i < config.WordFilter.Count; i++)
                {
                    if (Regex.IsMatch(message.Content, config.WordFilter[i], RegexOptions.IgnoreCase))
                    {
                        HandleMessage(guildChannel, message, config, lang);
                        break;
                    }
                }

                ulong channel = message.Channel.Id;
                
                if (config.ChannelFilters != null && config.ChannelFilters.ContainsKey(channel))
                {
                    for(int i = 0; i < config.ChannelFilters[channel].Count; i++)
                    {
                        if (Regex.IsMatch(message.Content, config.ChannelFilters[channel][i], RegexOptions.IgnoreCase))
                        {
                            HandleMessage(guildChannel, message, config, lang);
                            break;
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static void HandleMessage(IGuildChannel guildChannel, SocketUserMessage message, GuildConfiguration config, Language lang)
        {
            if (guildChannel.Guild.GetUserAsync(message.Author.Id).Result.RoleIds.Any(config.ModeratorRoles.Contains) || guildChannel.Guild.OwnerId == message.Author.Id)
            {
                /* Add exclamation mark emote */
                message.AddReactionAsync(new Emoji("❗"));
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                .WithAuthor(lang.GetString("event_filter_triggered"), message.Author.GetAvatarUrl())
                .WithDescription($"{lang.GetString("event_message_id")}: {message.Id}\n" +
                                 $"{lang.GetString("event_message_channel")}: {MentionUtils.MentionChannel(guildChannel.Id)} ({guildChannel.Id})\n" +
                                 $"{lang.GetString("event_message_author")}: {MentionUtils.MentionUser(message.Author.Id)}")
                .AddField(lang.GetString("message_content"), message.Content)
                .WithColor(Color.Purple);

                DiscordSocketEventHandler.LogMessage(embed, guildChannel.GuildId).GetAwaiter().GetResult();
                message.DeleteAsync();
            }
        }

        private static string Sanitize(string str)
        {
            str = Regex.Replace(str, @"[^\w\s]*", "");

            Dictionary<string, string> leetRules = new Dictionary<string, string>();

            leetRules.Add("4", "A");
            leetRules.Add(@"/\", "A");
            leetRules.Add("@", "A");
            leetRules.Add("^", "A");

            leetRules.Add("13", "B");
            leetRules.Add("/3", "B");
            leetRules.Add("|3", "B");
            leetRules.Add("8", "B");

            leetRules.Add("><", "X");

            leetRules.Add("<", "C");
            leetRules.Add("(", "C");

            leetRules.Add("|)", "D");
            leetRules.Add("|>", "D");

            leetRules.Add("3", "E");

            leetRules.Add("6", "G");

            leetRules.Add("/-/", "H");
            leetRules.Add("[-]", "H");
            leetRules.Add("]-[", "H");

            leetRules.Add("!", "I");

            leetRules.Add("|_", "L");

            leetRules.Add("_/", "J");
            leetRules.Add("_|", "J");

            leetRules.Add("1", "L");

            leetRules.Add("0", "O");

            leetRules.Add("5", "S");

            leetRules.Add("7", "T");

            leetRules.Add(@"\/\/", "W");
            leetRules.Add(@"\/", "V");

            leetRules.Add("2", "Z");

            foreach(KeyValuePair<string, string> pair in leetRules)
            {
                str = str.Replace(pair.Key, pair.Value);
            }

            return str;
        }
    }
}
