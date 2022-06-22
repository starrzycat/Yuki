using Discord;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Services
{
    public class NegaStars
    {
        public const string Emote = "❌";

        public static async Task Manage(IUserMessage message, ITextChannel channel, SocketReaction reaction)
        {
            IGuild guild = (channel as IGuildChannel).Guild;

            IGuildUser user = await guild.GetUserAsync(message.Author.Id);

            GuildConfiguration config = GuildSettings.GetGuild(guild.Id);

            if(!config.Equals(default(GuildConfiguration)) && config.EnableNegaStars && !reaction.User.Value.IsBot &&
                config.NegaStarIgnoredChannels != null && !config.NegaStarIgnoredChannels.Contains(message.Channel.Id))
            {
                int negaCount = message.Reactions.Keys.Select(r => r.Name == Emote) != null ? message.Reactions.FirstOrDefault(r => r.Key.Name == Emote).Value.ReactionCount : 0;
                
                if(negaCount >= config.NegaStarRequirement)
                {
                    await message.DeleteAsync();

                    await channel.SendMessageAsync($"{user.Username}#{user.Discriminator}'s message has been deleted");
                }
            }
        }
    }
}
