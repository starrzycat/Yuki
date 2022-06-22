using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Data.Objects.Database;
using Yuki.Events;
using Yuki.Extensions;
using Yuki.Services.Database;

namespace Yuki.Data
{
    public class MessageCacheData
    {
        public ulong Version = 002;
        public Dictionary<ulong, List<CachedMessage>> Data = new Dictionary<ulong, List<CachedMessage>>();
    }

    public static class UserMessageCache
    {
        public static readonly int MaxMessages = 1000;

        private static List<CacheableMessage> MessagesOld = new List<CacheableMessage>();

        private static MessageCacheData Messages;

        public static void Update()
        {
            Messages = new MessageCacheData();
            MessagesOld = JsonConvert.DeserializeObject<List<CacheableMessage>>(File.ReadAllText(FileDirectories.Messages));

            for(int i = 0; i < MessagesOld.Count; i++)
            {
                if(!Messages.Data.ContainsKey(MessagesOld[i].AuthorId))
                {
                    Messages.Data.Add(MessagesOld[i].AuthorId, new List<CachedMessage>());
                }

                Messages.Data[MessagesOld[i].AuthorId].Add(new CachedMessage()
                {
                    Id = MessagesOld[i].Id,
                    ChannelId = MessagesOld[i].ChannelId,
                    SendDate = MessagesOld[i].SendDate,
                    Content = MessagesOld[i].Content
                });
            }

            MessagesOld.Clear();
            SaveToFile();
        }

        public static void AddOrUpdate(SocketMessage message, GuildConfiguration config)
        {
            string messageContent = message.Cleanse();

            if(message.Channel is IDMChannel || message.Author.IsBot)
            {
                return;
            }

            if(message.Content.HasUrl(out int[] indexes))
            {
                List<string> split = message.Content.Split(' ').ToList();

                for(int i = 0; i < indexes.ToArray().Length; i++)
                {
                    split.RemoveAt(indexes[i]);
                }

                messageContent = string.Join(" ", split);
            }

            if (config.CacheIgnoredChannels.Contains(message.Channel.Id))
            {
                return;
            }

            if(DiscordSocketEventHandler.HasPrefix(message as SocketUserMessage, config, out string _prefix))
            {
                return;
            }

            if(UserSettings.CanGetMsgs(message.Author.Id))
            {
                DateTime sendDate = new DateTime(message.Timestamp.UtcTicks);

                CachedMessage foundMsg = default;

                if(!Messages.Data.ContainsKey(message.Author.Id))
                {
                    Messages.Data.Add(message.Author.Id, new List<CachedMessage>());
                }

                foundMsg = Messages.Data[message.Author.Id].FirstOrDefault(msg => msg.Id == message.Id || msg.Content.ToLower() == messageContent.ToLower());

                // is the msg already saved? if it is, update it
                if(!foundMsg.Equals(default(CachedMessage)))
                {
                    if(foundMsg.Id == message.Id)
                    {
                        int index = Messages.Data[message.Author.Id].IndexOf(foundMsg);
                        foundMsg.Content = messageContent;

                        Messages.Data[message.Author.Id][index] = foundMsg;
                    }
                }
                else
                {
                    Messages.Data[message.Author.Id].Add(new CachedMessage()
                    {
                        Id = message.Id,
                        ChannelId = message.Channel.Id,
                        SendDate = sendDate,
                        Content = messageContent,
                    });
                }

                // clear messages if we have more than the max allowed
                if (Messages.Data[message.Author.Id].Count > MaxMessages)
                {
                    int messagesToRemove = Messages.Data[message.Author.Id].Count - MaxMessages;
                    
                    Messages.Data[message.Author.Id].RemoveRange(0, messagesToRemove);
                }
            }
        }

        public static List<CachedMessage> GetMessagesFromUser(ulong userId)
        {
            // nullreference checks be like :
            if(Messages != null && Messages.Data != null && Messages.Data.ContainsKey(userId))
            {
                return Messages.Data[userId];
            }
            else
            {
                return new List<CachedMessage>();
            }
        }

        public static int UserMessageCount(ulong userId)
        {
            return Messages.Data[userId].Count;
        }

        public static void Delete(SocketMessage message)
        {
            Delete(message.Author.Id, message.Id);
        }
        
        public static void Delete(ulong userId, ulong messageId)
        {
            if(Messages.Data.ContainsKey(userId))
            {
                CachedMessage _message = Messages.Data[userId].FirstOrDefault(msg => msg.Id == messageId);

                if (!_message.Equals(default(CacheableMessage)))
                {
                    Messages.Data[userId].Remove(_message);
                }
            }
        }
        
        public static void Delete(ulong userId, CachedMessage msg)
        {
            if(Messages.Data[userId].Contains(msg))
            {
                Messages.Data[userId].Remove(msg);
            }
        }

        public static void DeleteWithChannelId(ulong channelId)
        {
            for(int u = 0; u < Messages.Data.Count; u++)
            {
                ulong userId = Messages.Data.Keys.ElementAt(u);

                for (int msg = 0; msg < Messages.Data[userId].Count; msg++)
                {
                    if(Messages.Data[userId][msg].ChannelId == channelId)
                    {
                        Messages.Data[userId].RemoveAt(msg);
                    }
                }
            }
        }

        public static void DeleteFromUser(ulong id)
        {
            if(Messages.Data.ContainsKey(id))
            {
                Messages.Data[id].Clear();
            }
        }

        public static void LoadFromFile()
        {
            string json = File.ReadAllText(FileDirectories.Messages);

            if (File.Exists(FileDirectories.Messages))
            {
                try
                {
                    Messages = JsonConvert.DeserializeObject<MessageCacheData>(Encryption.Decrypt(json, Config.GetConfig().encryption_key));
                }
                catch(Exception) // file not encrypted
                {
                    Messages = JsonConvert.DeserializeObject<MessageCacheData>(json);
                }
            }

            if (Messages == null)
            {
                Messages = new MessageCacheData();
            }
        }

        public static void SaveToFile()
        {
            File.WriteAllText(FileDirectories.Messages, Encryption.Encrypt(JsonConvert.SerializeObject(Messages, Formatting.Indented), Config.GetConfig().encryption_key));
        }
    }
}
