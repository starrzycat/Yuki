using Discord;
using Qmmands;
using System.Collections.Generic;
using System.Threading.Tasks;
using Yuki.Data.Objects;
using Interactivity;
using Yuki.Core;

namespace Yuki.Commands
{
    public abstract class YukiModule : ModuleBase<YukiCommandContext>
    {
        public Language Language => Localization.GetLanguage(Context);

        public InteractivityService Interactivity { get { return Context.Interactivity; } }

        public bool UserHasPriority(IUser executor, IUser otherUser) => Context.UserHasPriority(executor, otherUser);
        public bool RoleHasPriority(IRole role, IRole otherRole) => Context.RoleHasPriority(role, otherRole);

        public Task<IUserMessage> ReplyAsync(string content, Embed embed) => Context.ReplyAsync(content, embed);
        public Task<IUserMessage> ReplyAsync(string content, EmbedBuilder embed) => Context.ReplyAsync(content, embed);
        public Task<IUserMessage> ReplyAsync(string content) => Context.ReplyAsync(content);
        public Task<IUserMessage> ReplyAsync(object content) => Context.ReplyAsync(content);
        public Task<IUserMessage> ReplyAsync(Embed embed) => Context.ReplyAsync(embed);
        public Task<IUserMessage> ReplyAsync(EmbedBuilder embed) => Context.ReplyAsync(embed);
        public Task<IUserMessage> SendFileAsync(string file, string text) => Context.SendFileAsync(file, text, null);
        public Task<IUserMessage> SendFileAsync(string file, EmbedBuilder embedBuilder) => Context.SendFileAsync(file, embedBuilder);
        public Task<IUserMessage> SendFileAsync(string file, Embed embed) => Context.SendFileAsync(file, embed);

        public Task PagedReplyAsync(string title, IEnumerable<object> pages, int contentPerPage = 20, bool showNumbers = false) => Context.PagedReplyAsync(title, pages, contentPerPage, showNumbers);

        public Task ReactAsync(string unicode) => Context.ReactAsync(unicode);
    }
}
