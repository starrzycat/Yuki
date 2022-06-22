using Discord;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Yuki.Commands.TypeParsers
{
    public sealed class UserTypeParser<TUser> : TypeParser<TUser> where TUser : IUser
    {
        public override ValueTask<TypeParserResult<TUser>> ParseAsync(Parameter param, string value, CommandContext _context)
        {
            if(!(_context is YukiCommandContext))
            {
                return TypeParserResult<TUser>.Unsuccessful("Cannot use a context of this type!");
            }

            YukiCommandContext context = _context as YukiCommandContext;

            List<TUser> users = context.Client.Guilds.SelectMany(guild => guild.Users).OfType<TUser>().ToList();
            
            TUser user = default;

            if (ulong.TryParse(value, out var id) || MentionUtils.TryParseUser(value, out id))
            {
                user = users.FirstOrDefault(_user => _user.Id == id);
            }

            if (user == null)
            {
                user = users.FirstOrDefault(_user => _user.ToString().Equals(value, StringComparison.OrdinalIgnoreCase));
            }

            if (user == null)
            {
                List<TUser> match = users.Where(_user => _user.Username.Equals(value, StringComparison.OrdinalIgnoreCase) ||
                                                        (_user as IGuildUser).Nickname.Equals(value, StringComparison.OrdinalIgnoreCase)).ToList();
                if (match.Count > 1)
                {
                    return TypeParserResult<TUser>.Unsuccessful("Multiple users found! Try mentioning the user or using their ID.");
                }

                user = match.FirstOrDefault();
            }

            return (user == null) ?
                        TypeParserResult<TUser>.Unsuccessful("User not found.") :
                        TypeParserResult<TUser>.Successful(user);
        }
    }
}
