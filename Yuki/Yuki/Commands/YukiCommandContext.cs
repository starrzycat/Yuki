using Discord;
using Discord.WebSocket;
using Interactivity;
using Interactivity.Pagination;
using Microsoft.Extensions.DependencyInjection;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects;
using Yuki.Extensions;

namespace Yuki.Commands
{
    public sealed class YukiCommandContext : CommandContext
    {
        public Language Language => Localization.GetLanguage(this);

        public InteractivityService Interactivity { get; } = YukiBot.Discord.Services.GetRequiredService<InteractivityService>();

        public DiscordShardedClient Client { get; }

        public IGuild Guild { get; }

        public IMessageChannel Channel { get; }

        public IUser User { get; }

        public IMessage Message { get; }

        public YukiCommandContext(DiscordShardedClient client, IUserMessage msg, IServiceProvider provider) : base(provider)
        {

            Client = client;
            Guild = (msg.Channel as IGuildChannel)?.Guild;
            Channel = msg.Channel;
            User = msg.Author;
            Message = msg;
        }

        public EmbedAuthorBuilder Author
            => new EmbedAuthorBuilder()
            {
                IconUrl = User.GetAvatarUrl(),
                Name = User.Username
            };

        public Embed CreateEmbed(string title) => CreateEmbedBuilder(title).Build();
        public Embed CreateEmbed(string content, EmbedAuthorBuilder author = null) => CreateEmbedBuilder(content, author).Build();

        public EmbedBuilder CreateImageEmbedBuilder(string title, string url)
            => CreateEmbedBuilder("", new EmbedAuthorBuilder().WithName(title))
                .WithImageUrl(url)
                .WithFooter($"Yuki v{Version.Get()}");

        public EmbedBuilder CreateEmbedBuilder(string title)
            =>  new EmbedBuilder().WithColor(Colors.Pink)
                .WithAuthor(title)
                .WithFooter($"Yuki v{Version.Get()}");

        public EmbedBuilder CreateEmbedBuilder(string content, EmbedAuthorBuilder author)
            => new EmbedBuilder().WithColor(Colors.Pink)
                    .WithAuthor(author ?? Author)
                    .WithDescription(content ?? string.Empty)
                    .WithFooter($"Yuki v{Version.Get()}");

        public EmbedBuilder CreateColoredEmbed(Color color, EmbedAuthorBuilder author, string text)
                => CreateEmbedBuilder(text, author)
                    .WithColor(color);

        public async Task<bool> TryDeleteAsync(IUserMessage message)
        {
            try
            {
                await message.DeleteAsync();
                return true;
            }
            catch (Exception) { return false; }
        }

        public bool UserHasPermission(GuildPermission permision)
        {
            if (Channel is IDMChannel)
            {
                return false;
            }

            return (User as IGuildUser).GuildPermissions.Has(permision) || (User as IGuildUser).GuildPermissions.Administrator;
        }

        public bool UserHasPriority(IUser executor, IUser otherUser)
        {
            if (Channel is IDMChannel)
            {
                return false;
            }

            return ((IGuildUser)executor).HighestRole().Position >
                   ((IGuildUser)otherUser).HighestRole().Position;
        }

        public bool RoleHasPriority(IRole role, IRole otherRole)
        {
            if (Channel is IDMChannel)
            {
                return false;
            }

            return role.Position > otherRole.Position;
        }

        public async Task<bool> TryDeleteAsync(SocketMessage message)
            => await TryDeleteAsync((IUserMessage)message);


        public Task<IUserMessage> ReplyAsync(string content, Embed embed) => Channel.SendMessageAsync(content, false, embed);
        public Task<IUserMessage> ReplyAsync(string content, EmbedBuilder embed) => Channel.SendMessageAsync(content, false, embed.Build());
        public Task<IUserMessage> ReplyAsync(string content) => Channel.SendMessageAsync(content);
        public Task<IUserMessage> ReplyAsync(object content) => Channel.SendMessageAsync(content.ToString());
        public Task<IUserMessage> ReplyAsync(Embed embed) => embed.SendToAsync(Channel);
        public Task<IUserMessage> ReplyAsync(EmbedBuilder embed) => embed.SendToAsync(Channel);

        public Task<IUserMessage> SendFileAsync(string fileName, EmbedBuilder embed) => SendFileAsync(fileName, embed.Build());
        public Task<IUserMessage> SendFileAsync(string fileName, Embed embed) => Channel.SendFileAsync(fileName, null, false, embed, null, false);
        public Task<IUserMessage> SendFileAsync(string fileName, string text, Embed embed) => Channel.SendFileAsync(fileName, text, false, embed, null, false);

        public Task PagedReplyAsync(string title, IEnumerable<object> items, int contentPerPage, bool showNumbers = false)
        {
            if(items != null)
            {
                int totalPages = items.Count() / contentPerPage;

                Paginator pager = new LazyPaginatorBuilder()
                    .WithUsers(User as SocketUser)
                    .WithPageFactory(PageFactory)
                    .WithMaxPageIndex(totalPages)
                    .WithFooter(PaginatorFooter.PageNumber)
                    .WithDefaultEmotes()
                    .Build();

                Task<PageBuilder> PageFactory(int page)
                {
                    string[] pageItems = (from d in items.Skip(page * contentPerPage).Take(contentPerPage) select d.ToString()).ToArray();

                    string content = "";

                    for(int i = 0; i < pageItems.Length; i++)
                    {
                        content += $"{(showNumbers ? $"{i + 1}. " : "")}{pageItems[i]}\n";
                    }

                    return Task.FromResult(new PageBuilder()
                        .WithDescription(content)
                        .WithTitle($"{title} (page {page + 1}/{totalPages + 1})")
                        .WithColor(Colors.Pink));
                }

                return Interactivity.SendPaginatorAsync(pager, Channel, TimeSpan.FromMinutes(2));
            }
            else
            {
                Channel.SendMessageAsync("Items was null, something went wrong!");
                return Task.CompletedTask;
            }
        }

        public Task ReactAsync(string unicode) => Message.AddReactionAsync(new Emoji(unicode));
    }
}