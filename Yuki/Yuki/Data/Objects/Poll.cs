using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using Yuki.Extensions;

namespace Yuki.Data.Objects
{
    public class Poll
    {
        public string Title { get; set; }
        public string Id { get; set; }

        public bool ShowVotes { get; set; }

        public ulong GuildId { get; set; }

        public DateTime Deadline { get; set; }

        public List<PollItem> Items { get; set; }

        public int TotalVotes() => Items.Sum(item => item.GetVoteCount());

        public bool ItemExists(string itemId, out PollItem item)
        {
            PollItem[] items = Items.ToArray();


            if (int.TryParse(itemId, out int index))
            {
                if(index > 0 && index <= items.Length)
                {
                    if(index - 1 > 0)
                    {
                        item = items[--index];
                        return true;
                    }
                }
            }
            else
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if (items[i].Id.ToLower() == itemId.ToLower())
                    {
                        item = items[i];
                        return true;
                    }
                }
            }

            item = default;
            return false;
        }

        /// <summary>
        /// Checks that the specified user is in the guild for this poll
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool UserCanVote(ulong userId)
            => YukiBot.Discord.Client.Guilds.FirstOrDefault(guild => guild.Id == GuildId && guild.GetUser(userId) != null) != null;

        public bool HasUserVoted(ulong userId)
            => HasUserVoted(userId, out PollItem item);

        public bool HasUserVoted(ulong userId, out PollItem item)
        {
            PollItem[] items = Items.ToArray();

            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].HasUserVoted(userId))
                {
                    item = items[i];
                    return true;
                }
            }

            item = default;
            return false;
        }

        public Poll()
        {
            Items = new List<PollItem>();
        }

        public static Poll Create(string title, string[] items, ulong guildId, DateTime deadline, bool showVotes)
        {
            Poll poll = new Poll();

            poll.Id = Passphrase.Generate();

            poll.Title = title;
            poll.GuildId = guildId;
            poll.Deadline = deadline;
            poll.ShowVotes = showVotes;

            for (int i = 0; i < items.Length; i++)
            {
                poll.Items.Add(new PollItem(items[i]));
            }

            return poll;
        }

        private string PollItemsString(bool userAllowed)
        {
            string pollItems = string.Empty;

            foreach(PollItem item in Items)
            {
                pollItems += $"**{Items.IndexOf(item) + 1}**. ";

                if (ShowVotes || userAllowed)
                {
                    pollItems += $"{item.Id} *({item.GetVoteCount()} votes)*";
                }

                pollItems += "\n";
            }

            return pollItems;
        }

        public Embed CreateEmbed(bool userAllowed = false)
            => new EmbedBuilder()
                .WithTitle(Title)
                .WithColor(Color.Green)
                .WithDescription(PollItemsString(userAllowed))
                .WithFooter(((Deadline - DateTime.Now).Days > 1) ? Deadline.ToPrettyTime(getTimeLeft: false, showTime: true) + " UTC" : Deadline.ToPrettyTime(getTimeLeft: true, showTime: false))
                .Build();
    }
}
