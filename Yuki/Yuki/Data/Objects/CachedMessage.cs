using System;

namespace Yuki.Data.Objects
{
    public struct CacheableMessage
    {
        public ulong Id { get; set; }

        public DateTime SendDate { get; set; }

        public ulong AuthorId { get; set; }
        public ulong ChannelId { get; set; }

        public string Content { get; set; }
    }

    public struct CachedMessage
    {
        public ulong Id { get; set; }

        public DateTime SendDate { get; set; }
        
        public ulong ChannelId { get; set; }

        public string Content { get; set; }
    }
}
