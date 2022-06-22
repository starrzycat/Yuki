using LiteDB;

namespace Yuki.Data.Objects.Database
{
    public struct GuildRole
    {
        [BsonId]
        public ulong Id { get; set; }
        
        public bool IsTeamRole { get; set; }
        public bool IsTeamRoleRemovable { get; set; }
        public bool IsAutoRole { get; set; }
        public string Group { get; set; }
    }
}
