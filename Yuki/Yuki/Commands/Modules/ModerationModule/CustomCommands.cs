using Qmmands;
using System.Threading.Tasks;
using Yuki.Data.Objects.Database;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.ModerationModule
{
    public partial class ModerationModule
    {
        [Group("customcommands", "ccommands")]
        public class CustomCommands : YukiModule
        {
            [Command("add")]
            public async Task AddCustomCommandAsync([Remainder] string commandStr)
            {
                string[] split = commandStr.Split(' ', 2);

                GuildCommand command = new GuildCommand()
                {
                    Name = split[0].ToLower(),
                    Response = split[1]
                };

                bool canAdd = true;

                foreach(Command cmd in YukiBot.Discord.CommandService.GetAllCommands())
                {
                    if(cmd.Name.ToLower() == command.Name)
                    {
                        canAdd = false;
                        break;
                    }
                }

                if(canAdd)
                {
                    GuildCommand[] commands = GuildSettings.GetGuild(Context.Guild.Id).Commands.ToArray();

                    for(int i = 0; i < commands.Length; i++)
                    {
                        if(commands[i].Name == command.Name)
                        {
                            canAdd = false;
                            break;
                        }
                    }
                }

                if(canAdd)
                {
                    GuildSettings.AddCommand(command, Context.Guild.Id);
                    await ReplyAsync(Language.GetString("command_added").Replace("{command}", command.Name));
                }
                else
                {
                    await ReplyAsync(Language.GetString("command_exists").Replace("{command}", command.Name));
                }
            }

            [Command("remove", "rem")]
            public async Task RemoveSelfRoleAsync([Remainder] string command)
            {
                GuildSettings.RemoveCommand(command, Context.Guild.Id);

                await ReplyAsync(Language.GetString("command_removed").Replace("{command}", command));
            }
        }
    }
}
