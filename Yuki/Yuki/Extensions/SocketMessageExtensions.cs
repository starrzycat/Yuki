using Discord;
using Discord.WebSocket;

namespace Yuki.Extensions
{
    public static class SocketMessageExtensions
    {
        public static string Cleanse(this SocketMessage message)
        {
            return (message as SocketUserMessage).Cleanse();
        }

        public static string Cleanse(this SocketUserMessage message)
        {
            return message.Resolve(TagHandling.FullName, TagHandling.NameNoPrefix, TagHandling.Name, TagHandling.Name, TagHandling.FullNameNoPrefix);
        }
    }
}
