using LiteDB;
using System;

namespace Yuki.Data.Objects.Database
{
    public struct StarboardPost
    {
        [BsonId]
        public ulong Id { get; set; }

        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong PostId { get; set; }
        public int Score { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}
