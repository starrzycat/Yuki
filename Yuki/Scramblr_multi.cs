using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Yuki.Data.Objects.Database;
using Yuki.Extensions;
using Yuki.Services.Database;

namespace Yuki.Data.Objects
{
    public class Scramblr
    {
        private const int MaxLikeMessages = 500;
        private const int MaxLoops = 25;

        /// <summary>
        /// Get a scrambled message
        /// </summary>
        /// <param name="user1"></param>
        /// <param name="user2"></param>
        /// <param name="loopCount"></param>
        /// <returns></returns>
        public string GetMessage(params IUser[] users)
        {
            return GetMessageInternal(0, users);
        }

        private string GetMessageInternal(int loops = 0, params IUser[] users)
        {
            List<ScramblrData> similarMessages = new List<ScramblrData>();
            CachedMessage[] firstUserMessages = UserMessageCache.GetMessagesFromUser(users[0].Id).ToArray();

            Random random = new Random();

            for(int i = 0; i < users.Length; i++)
            {
                if(users[i].IsBot)
                {
                    return "I can't scramble bots, silly!";
                }
            }

            if(firstUserMessages.Length <= 3)
            {
                return "Sorry! I couldn't find enough/any messages from you :(";
            }

            if(users.Length > 1)
            {
                for(int i = 1; i < users.Length; i++)
                {
                    similarMessages.AddRange(GetLikeMessages(firstUserMessages, UserMessageCache.GetMessagesFromUser(users[i].Id).ToArray()));
                }
            }
            else
            {
                similarMessages.AddRange(GetLikeMessages(firstUserMessages, firstUserMessages));
            }

            if (similarMessages.Count < 1)
                return "I couldn't find any like words :/";

            if (users.Length > 1)
            {
                return ScrambledMessage(similarMessages[random.Next(similarMessages.Count)]);
            }

            string scrambled = ScrambledMessage(similarMessages[random.Next(similarMessages.Count)]);

            //we want to make sure that if the user didnt mention anyone, the message the bot generates isnt the same as a message the user has sent.   
            for (int i = 0; i < firstUserMessages.Length; i++)
            {
                if (firstUserMessages[i].Content == scrambled || firstUserMessages[i].Content.Length > 2000)
                {
                    if (loops < MaxLoops)
                        return GetMessageInternal(loops++, users);
                    else
                    {
                        if (firstUserMessages[i].Content.Length > 2000)
                        {
                            firstUserMessages[i].Content = firstUserMessages[i].Content.Substring(0, 2000);
                        }

                        break;
                    }
                }
            }

            return scrambled;
        }

        /// <summary>
        /// Scramble the message!
        /// </summary>
        /// <param name="data"></param>
        /// <param name="guild"></param>
        /// <param name="user2"></param>
        /// <returns></returns>
        private string ScrambledMessage(ScramblrData data)
        {
            //split the messages each into 2 separate strings where the like word is
            string[] firstDataSanitized = Regex.Split(data.Message1.Content, $@"\b{Regex.Escape(data.likeWord)}\b");
            string[] secondDataSanitized = Regex.Split(data.Message2.Content, $@"\b{Regex.Escape(data.likeWord)}\b");

            string scrambled(string[] dat1, string[] dat2)
            {
                string str = dat1[0];

                str += data.likeWord;

                if (dat2.Length > 1)
                {
                    str += dat2[1];
                }
                else
                {
                    str += dat2[0];
                }

                return str;
            }

            if (new Random().NextDouble() <= .5)
            {
                return scrambled(firstDataSanitized, secondDataSanitized);
            }
            else
            {
                return scrambled(secondDataSanitized, firstDataSanitized);
            }
        }

        /// <summary>
        /// Get messages that have at least one word in common
        /// </summary>
        /// <param name="dat1"></param>
        /// <param name="dat2"></param>
        /// <returns></returns>
        private ScramblrData[] GetLikeMessages(CachedMessage[] firstUser, CachedMessage[] secondUser)
        {
            List<ScramblrData> data = new List<ScramblrData>();

            for (int i = 0; i < firstUser.Length; i++)
            {
                for (int j = 0; j < secondUser.Length; j++)
                {
                    GetLikeWords(ref data, firstUser[i], secondUser[j]);
                }
            }

            return data.ToArray();
        }

        /// <summary>
        /// Adds like words to our list
        /// </summary>
        /// <param name="data"></param>
        /// <param name="msg1"></param>
        /// <param name="msg2"></param>
        private void GetLikeWords(ref List<ScramblrData> data, CachedMessage msg1, CachedMessage msg2)
        {
            if (msg1.Id != msg2.Id)
            {
                //Split the message at every space, selecting every substring that isn't a url
                string[] firstMsg = msg1.Content.Split(' ').ToList().Where(_str => !_str.IsUrl()).ToArray();
                string[] secondMsg = msg2.Content.Split(' ').ToList().Where(_str => !_str.IsUrl()).ToArray();

                for (int i = 0; i < firstMsg.Length; i++)
                {
                    for (int j = 0; j < secondMsg.Length; j++)
                    {
                        if (firstMsg[i].Equals(secondMsg[j]))
                        {
                            ScramblrData scramblrData = null;

                            //verify the like word isnt at the beginning of the string
                            if (msg1.Content.IndexOf(firstMsg[i]) != 0 && msg2.Content.IndexOf(firstMsg[i]) != 0)
                                scramblrData = new ScramblrData(firstMsg[i], msg1, msg2);

                            if (scramblrData != null && !data.Contains(scramblrData) && data.Count < MaxLikeMessages)
                                data.Add(scramblrData);
                        }
                    }
                }
            }
        }
    }

    public class ScramblrData
    {
        public CachedMessage Message1;
        public CachedMessage Message2;

        public string likeWord;

        public ScramblrData(string word, CachedMessage msg1, CachedMessage msg2)
        {
            likeWord = word;

            Message1 = msg1;
            Message2 = msg2;
        }
    }
}
