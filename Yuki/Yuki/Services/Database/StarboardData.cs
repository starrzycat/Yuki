using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public static class StarboardData
    {
        private const string collection = "posts";

        private static List<StarboardPost> Posts = new List<StarboardPost>();

        public static async Task UpdatePosts()
        {
            for(int i = 0; i < Posts.Count; i++)
            {
                try
                {
                    DateTimeOffset offset = (await YukiBot.Discord.Client.GetGuild(Posts[i].GuildId).GetTextChannel(Posts[i].ChannelId).GetMessageAsync(Posts[i].Id)).Timestamp;

                    await Core.Logger.LogInfo("Timestamp for post with id " + Posts[i].Id + ": " + offset);

                    Posts[i] = new StarboardPost
                    {
                        Id = Posts[i].Id,
                        GuildId = Posts[i].GuildId,
                        ChannelId = Posts[i].ChannelId,
                        PostId = Posts[i].PostId,
                        Timestamp = offset,
                        Score = Posts[i].Score,
                    };
                }
                catch (Exception)
                {
                    await Core.Logger.LogInfo("Timestamp for post with id " + Posts[i].Id + " is null! Removing..");
                    Posts.RemoveAt(i);
                }
            }
        }

        public static void AddOrUpdatePost(StarboardPost post)
        {
            StarboardPost found = Posts.Find(starPost => starPost.Id == post.Id);

            if (found.Equals(default(StarboardPost)))
            {
                Posts.Add(post);
            }
            else
            {
                Posts[Posts.IndexOf(found)] = post;
            }
        }
        
        public static void RemovePostById(ulong messageId)
        {
            StarboardPost found = Posts.Find(starPost => starPost.Id == messageId);

            if (!found.Equals(default(StarboardPost)))
            {
                Posts.Remove(found);
            }
        }

        public static StarboardPost GetPostFor(ulong guildId, ulong messageId)
        {
            StarboardPost[] posts = GetPostsFromGuild(guildId);

            for(int i = 0; i < posts.Length; i++)
            {
                if(posts[i].Id == messageId)
                {
                    return posts[i];
                }
            }

            return default;
        }

        public static bool MessageIsStarPost(ulong guildId, ulong messageId)
        {
            StarboardPost[] posts = GetPostsFromGuild(guildId);

            for (int i = 0; i < posts.Length; i++)
            {
                if (posts[i].PostId == messageId)
                {
                    return true;
                }
            }

            return false;
        }

        public static StarboardPost[] GetPostsFromGuild(ulong guildId)
        {
            return Posts.Where(p => p.GuildId == guildId).ToArray();
        }

        public static void RemovePostsFromGuild(ulong guildId)
        {
            StarboardPost[] posts = GetPostsFromGuild(guildId);

            for(int i = 0; i < posts.Length; i++)
            {
                RemovePostById(posts[i].Id);
            }
        }

        public static void Load()
        {
            if(File.Exists(FileDirectories.Starboard))
            {
                Posts = JsonConvert.DeserializeObject<List<StarboardPost>>(File.ReadAllText(FileDirectories.Starboard));
            }
        }

        public static void Save()
        {
            File.WriteAllText(FileDirectories.Starboard, JsonConvert.SerializeObject(Posts, Formatting.Indented));
        }
    }
}
