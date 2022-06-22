using Discord;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Commands;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Data.Objects.API;

namespace Yuki.API
{
    public static class RamMoe
    {
        private static string[] badImages = new[]
        {
            "rkshJ3Sjx.gif"
        };

        public static async Task<string> GetImageAsync(string type)
        {
            using (HttpClient http = new HttpClient())
            {
                string url = "https://rra.ram.moe/i/r?type=" + type + "&nsfw=false";

                /* Read the json, return the received string */
                using (StreamReader reader = new StreamReader(await http.GetStreamAsync(url)))
                {
                    RamMoeFile moe = JsonConvert.DeserializeObject<RamMoeFile>(reader.ReadToEnd());

                    string moeImage = moe.path.Remove(0, 3);

                    if(!badImages.Contains(moeImage))
                    {
                        return "https://cdn.ram.moe/" + moeImage;
                    }
                    else
                    {
                        return await GetImageAsync(type);
                    }
                }
            }
        }

        public static async Task SendImageAsync(YukiCommandContext context, Language lang, string imgType, string mentionStr)
        {
            try
            {
                List<IUser> mentionedUsers = new List<IUser>();

                string embedStringTitle = "rammoe_" + imgType;

                string userString = null;

                if (mentionStr != null)
                {
                    foreach (string substr in mentionStr.Split(' '))
                    {
                        if (MentionUtils.TryParseUser(substr, out ulong userId))
                        {
                            mentionedUsers.Add(context.Client.GetUser(userId));
                        }
                    }

                    if(mentionedUsers.Count < 1)
                    {
                        userString = mentionStr;
                    }
                }
                else
                {
                    if(lang.GetString(embedStringTitle + "_alt") != embedStringTitle + "_alt")
                    {
                        embedStringTitle += "_alt";

                        mentionedUsers.Add(context.Client.CurrentUser);
                    }
                }

                if(userString == null)
                {
                    if(mentionedUsers.Count == 1)
                    {
                        userString = mentionedUsers[0].Username;
                    }
                    else if(mentionedUsers.Count == 2)
                    {
                        userString = $"{mentionedUsers[0].Username} and {mentionedUsers[1].Username}";
                    }
                    else
                    {
                        for(int i = 0; i < mentionedUsers.Count; i++)
                        {
                            if(i == mentionedUsers.Count - 1)
                            {
                                userString += "and " + mentionedUsers[i].Username;
                            }
                            else
                            {
                                userString += mentionedUsers[i].Username + ", ";
                            }
                        }
                    }
                }

                string translatedTitle = lang.GetString(embedStringTitle)
                    .Replace("{executor}", context.User.Username)
                    .Replace("{user}", userString);

                await context.ReplyAsync(context.CreateImageEmbedBuilder(translatedTitle, await GetImageAsync(imgType)));
            }
            catch (Exception e)
            {
                await Logger.LogDebug(e);
            }
        }
    }
}
