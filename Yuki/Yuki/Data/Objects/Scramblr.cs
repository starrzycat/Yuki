using Discord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Yuki.Data.Objects
{
    public class Scramblr
    {
        private IUser main;
        private IUser other;

        private Language language;

        private string[] SplitAtWordIndex(string str, int index)
        {
            List<string> array = new List<string>();

            string[] split = str.Split(' ');

            string value = string.Empty;

            for(int i = 0; i < split.Length; i++)
            {
                if(i == index)
                {
                    if(value.Length > 0)
                    {
                        value = value.Remove(value.Length - 1);
                    }

                    array.Add(value);
                    value = string.Empty;
                    continue;
                }

                value += split[i] + " ";
            }

            if (value.Length > 0)
            {
                value = value.Remove(value.Length - 1);
            }
            
            array.Add(value);
            
            return array.ToArray();
        }

        private int[] WordIndexesOf(string str, string value)
        {
            List<int> indexes = new List<int>();

            string[] split = str.ToLower().Split(' ');

            for(int i = 0; i < split.Length; i++)
            {
                if(split[i] == value.ToLower())
                {
                    indexes.Add(i);
                }
            }

            return indexes.ToArray();
        }

        private string Scramble(string msg1, string msg2, string similarWord)
        {
            int[] indexesOfSimilar1 = WordIndexesOf(msg1, similarWord);
            int[] indexesOfSimilar2 = WordIndexesOf(msg2, similarWord);

            string[] split1 = SplitAtWordIndex(msg1, indexesOfSimilar1[new Random().Next(indexesOfSimilar1.Length)]).ToArray();
            string[] split2 = SplitAtWordIndex(msg2, indexesOfSimilar2[new Random().Next(indexesOfSimilar2.Length)]).ToArray();

            string firstPart;
            string secondPart;

            double value = new Random().NextDouble();

            if (value < .5D)
            {
                firstPart = split1[0];

                if(split2.Length > 1)
                {
                    secondPart = split2[1];
                }
                else
                {
                    secondPart = split2[0];
                }
            }
            else
            {
                firstPart = split2[0];

                if (split1.Length > 1)
                {
                    secondPart = split1[1];
                }
                else
                {
                    secondPart = split1[0];
                }
            }

            return firstPart + " " + similarWord + " " + secondPart;
        }

        public Scramblr(Language lang, IUser main) : this(lang, main, main) { }

        public Scramblr(Language lang, IUser main, IUser other)
        {
            this.main = main;

            if(other != null)
            {
                this.other = other;
            }
            else
            {
                this.other = main;
            }

            language = lang;
        }

        public string GetMessage()
        {
            if(main.IsBot || other.IsBot)
            {
                return language.GetString("cant_scramble_bots");
            }

            List<CachedMessage> mainMessages = UserMessageCache.GetMessagesFromUser(main.Id);
            List<CachedMessage> otherMessages = null;

            if(main.Id != other.Id)
            {
                otherMessages = UserMessageCache.GetMessagesFromUser(other.Id);
            }

            if (mainMessages == null || mainMessages.Count <= 3)
            {
                return language.GetString("main_few_msgs").Replace("{user}", main.Username);
            }

            if(otherMessages == null || otherMessages.Count <= 3)
            {
                return language.GetString("other_few_msgs").Replace("{user}", other.Username);
            }

            int tries = 0;

            CachedMessage chosenMsg1;
            CachedMessage chosenMsg2;

            string lastMsg = null;

            while(tries < 25)
            {
                chosenMsg1 = mainMessages[new Random().Next(0, mainMessages.Count)];

                if(otherMessages != null)
                {
                    chosenMsg2 = otherMessages[new Random().Next(0, otherMessages.Count)];
                }
                else
                {
                    chosenMsg2 = mainMessages[new Random().Next(0, mainMessages.Count)];
                }

                if(chosenMsg1.Id == chosenMsg2.Id)
                {
                    continue;
                }

                string[] firstSplit = chosenMsg1.Content.ToLower().Split(' ');
                string[] secondSplit = chosenMsg2.Content.ToLower().Split(' ');

                List<string> similarWords = new List<string>();

                for(int i = 0; i < firstSplit.Length; i++)
                {
                    for(int j = 0; j < secondSplit.Length; j++)
                    {
                        if(firstSplit[i] == secondSplit[j])
                        {
                            if(!similarWords.Contains(firstSplit[i]))
                            {
                                similarWords.Add(firstSplit[i]);
                            }
                        }
                    }
                }

                if(similarWords.Count > 0)
                {
                    string scrambled = Scramble(chosenMsg1.Content, chosenMsg2.Content, similarWords[new Random().Next(similarWords.Count)]);

                    if (scrambled.Length > 2000)
                    {
                        scrambled = scrambled.Substring(0, 1997) + "...";
                    }

                    if(scrambled.ToLower() == chosenMsg1.Content.ToLower() ||
                       scrambled.ToLower() == chosenMsg2.Content.ToLower())
                    {
                        tries++;
                        lastMsg = scrambled;
                        continue;
                    }

                    return scrambled;
                }

                tries++;
            }

            if (lastMsg != null)
            {
                return lastMsg;
            }
            else
            {
                return language.GetString("no_matches");
            }
        }
    }
}
