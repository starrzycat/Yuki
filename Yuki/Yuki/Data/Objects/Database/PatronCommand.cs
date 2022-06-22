using LiteDB;

namespace Yuki.Data.Objects.Database
{
    public struct PatronCommand
    {
        [BsonId]
        public ulong UserId { get; set; }

        public string Name { get; set; }
        public string Response { get; set; }
    }
}
