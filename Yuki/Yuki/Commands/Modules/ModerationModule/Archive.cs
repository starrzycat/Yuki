using ByteSizeLib;
using Discord;
using Discord.WebSocket;
using Qmmands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Yuki.Extensions;

namespace Yuki.Commands.Modules.ModerationModule
{
    public class ArchivalFilterManager
    {
        public bool ExcludeLinks = false;
        public bool ExcludeImages = false;
        public bool ExcludeEmotes = false;
        
        public IUser FilteredByUser = null;

        public ArchivalFilterManager() { }

        public ArchivalFilterManager(string[] filters, SocketGuild guild)
        {
            for(int i = 0; i < filters.Length; i++)
            {
                string[] filter = filters[i].ToLower().Split('=');

                switch (filter[0])
                {
                    case "excludelinks":
                    {
                        if (bool.TryParse(filter[1], out bool _val))
                        {
                            ExcludeLinks = _val;
                        }
                        break;
                    }
                    case "excludeimages":
                    {
                        if (bool.TryParse(filter[1], out bool _val))
                        {
                            ExcludeImages = _val;
                        }
                        break;
                    }
                    case "excludeemotes":
                    {
                        if (bool.TryParse(filter[1], out bool _val))
                        {
                            ExcludeEmotes = _val;
                        }
                        break;
                    }
                    case "filterbyuser":
                    {
                        FilteredByUser = guild.Users.FirstOrDefault(user => user.Mention == filters[i].Split('=')[1]);

                        break;
                    }
                }
            }
        }

        public List<IMessage> Filter(List<IMessage> messages)
        {
            if(ExcludeLinks)
            {
                messages = messages.Where(msg => !msg.Content.IsUrl()).ToList();// messages.Where(msg => msg.Content.IsUrl())).ToList();
            }

            if(ExcludeImages)
            {
                messages = messages.Where(msg => !string.IsNullOrWhiteSpace(msg.Content)).ToList();
            }

            if(ExcludeEmotes)
            {
                messages = messages.Where(msg => !Regex.IsMatch(msg.Content, @"<a?:(?<name>\w+):\d+>")).ToList();
            }

            if(FilteredByUser != null)
            {
                messages = messages.Where(msg => msg.Author == FilteredByUser).ToList();
            }

            return messages;
        }
    }

    public partial class ModerationModule
    {
        [Group("archive")]
        public class Archive : YukiModule
        {
            [Command]
            public async Task ArchiveChannelAsync(params string[] filters)
            {
                ArchivalFilterManager filter = new ArchivalFilterManager(filters, Context.Guild as SocketGuild);
                
                await ReplyAsync(Language.GetString("archiving"));

                try
                {
                    List<IMessage> messages = (await Context.Channel.GetMessagesAsync(int.MaxValue, CacheMode.AllowDownload, RequestOptions.Default).FlattenAsync()).Reverse().ToList();
                    
                    List<string> files = new List<string>();

                    string filename = Path.Combine(FileDirectories.Temp, $"{Context.Channel.Name}_{Context.Channel.Id}.txt");

                    ByteSize uploadLimit = default;

                    switch(Context.Guild.PremiumTier)
                    {
                        case PremiumTier.None:
                        case PremiumTier.Tier1:
                            uploadLimit = ByteSize.FromMegaBytes(8);
                            break;
                        case PremiumTier.Tier2:
                            uploadLimit = ByteSize.FromMegaBytes(50);
                            break;
                        case PremiumTier.Tier3:
                            uploadLimit = ByteSize.FromMegaBytes(100);
                            break;
                    }

                    ByteSize maxSize = uploadLimit.Subtract(ByteSize.FromKiloBytes(100));

                    messages = filter.Filter(messages);
                    Console.WriteLine(messages.Count);

                    while (messages.Count() > 0)
                    {
                        using (StreamWriter file = new StreamWriter(filename))
                        {
                            foreach (IMessage message in messages.ToArray())
                            {
                                string attachments = string.Join("\n", message.Attachments.Select(att => att.Url));

                                file.WriteLine($"[{message.Timestamp.UtcDateTime}]{ (message.EditedTimestamp.HasValue ? $" [{Language.GetString("edited")} {message.EditedTimestamp.Value.UtcDateTime}]" : "")} <{message.Author}> {message.Content}{(!string.IsNullOrEmpty(attachments) ? $"\n{Language.GetString("message_attachments")}: {attachments})" : "")}");
                                file.Flush();

                                messages.Remove(message);

                                if (file.BaseStream.Length > maxSize.Bytes)
                                {
                                    files.Add(filename);
                                    filename = Path.Combine(FileDirectories.Temp, $"{Context.Channel.Name}_{Context.Channel.Id}_{files.Count}.txt");

                                    break;
                                }
                            }
                        }
                    }

                    files.Add(filename);

                    for (int i = 0; i < files.Count; i++)
                    {
                        if (i == 0)
                        {
                            SendFileAsync(files[i], Language.GetString("archiving_done")).Wait();
                        }
                        else
                        {
                            SendFileAsync(files[i], embed: null).Wait();
                        }

                        File.Delete(files[i]);
                    }
                }
                catch (Exception e)
                {
                    await ReplyAsync(e);
                }
            }
            
            [Command("pins")]
            public async Task ArchivePinsInChannelAsync()
            {
                await ReplyAsync(Language.GetString("archiving"));
                IEnumerable<IMessage> messages = (await Context.Channel.GetPinnedMessagesAsync()).Reverse();

                string filename = Path.Combine(FileDirectories.Temp, $"pins_{Context.Channel.Name}_{Context.Channel.Id}).txt");

                using (StreamWriter file = new StreamWriter(filename))
                {
                    foreach (IMessage message in messages)
                    {
                        string attachments = string.Join("\n", message.Attachments.Select(att => att.Url));

                        file.WriteLine($"[{message.Timestamp.UtcDateTime}]{ (message.EditedTimestamp.HasValue ? $" [{Language.GetString("edited")} {message.EditedTimestamp.Value.UtcDateTime}]" : "")} <{message.Author}> {message.Content}{(!string.IsNullOrEmpty(attachments) ? $"\n{Language.GetString("message_attachments")}: {attachments})" : "")}");
                    }
                }
                await SendFileAsync(filename, Language.GetString("archiving_done"));
                File.Delete(filename);
            }
        }
    }
}
