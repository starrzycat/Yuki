using LiteDB;

namespace Yuki.Data.Objects.Database
{
    public struct YukiUser
    {
        [BsonId]
        public ulong Id { get; set; }

        public string langCode { get; set; }

        public bool IsPatron { get; set; }
        public bool CanGetMsgs { get; set; }
    }
}
