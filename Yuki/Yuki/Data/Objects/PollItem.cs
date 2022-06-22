using System.Collections.Generic;

namespace Yuki.Data.Objects
{
    public struct PollItem
    {
        public string Id { get; set; }

        public List<ulong> Votes { get; set; }

        public int GetVoteCount() => Votes.Count;

        public bool HasUserVoted(ulong userId)
            => Votes.Contains(userId);

        public PollItem(string id)
        {
            Id = id;
            Votes = new List<ulong>();
        }

        public void Vote(ulong userId)
        {
            Votes.Add(userId);
        }

        public void RemoveVote(ulong userId)
        {
            Votes.Remove(userId);
        }
    }
}
