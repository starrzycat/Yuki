using Discord;
using Qmmands;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Commands;

namespace Yuki.Data.Objects
{
    public class Help
    {
        private EmbedBuilder helpEmbed;

        public async Task Get(YukiCommandContext Context, string commandStr)
        {
            helpEmbed = Context.CreateEmbedBuilder(Context.Language.GetString("help_title"))
                       .WithDescription(Context.Language.GetString("help_info_description")
                           .Replace("{botinvite}", YukiBot.BotInvUrl)
                           .Replace("{serverinvite}", YukiBot.ServerUrl)
                           .Replace("{github}", YukiBot.GithubUrl)
                           .Replace("{wiki}", YukiBot.WikiUrl));

            if (commandStr == "")
            {
                await Context.ReplyAsync(helpEmbed);
            }
            else if (commandStr == "list")
            {
                await Context.ReplyAsync(BuildList(Context));
            }
            else
            {
                /* TODO: list similar commands? */
                List<Command> commands = YukiBot.Discord.CommandService.GetAllCommands()
                    .Where(cmd => ((cmd.Module.Parent != null) ? $"{cmd.Module.Name.ToLower()}_{cmd.Name.ToLower()}" : cmd.Name.ToLower()) == commandStr.ToLower()).ToList();

                if (commands.Count() == 1)
                {
                    EmbedBuilder embed = GetSingularCommandHelp(Context, commands[0]);

                    if (embed != null)
                    {
                        await Context.ReplyAsync(embed);
                    }
                }
                else
                {
                    EmbedBuilder embed = GetSubCommandsHelp(Context, commands, commandStr);

                    if (embed != null)
                    {
                        await Context.ReplyAsync(embed);
                    }
                    else
                    {
                        await Context.ReplyAsync(helpEmbed);
                    }
                }
            }
        }

        private EmbedBuilder BuildList(YukiCommandContext Context)
        {
            IEnumerable<Module> modules = YukiBot.Discord.CommandService.GetAllModules().Where(mod => mod.Parent == null);

            EmbedBuilder embed = Context.CreateEmbedBuilder(Context.Language.GetString("modules_title")).WithFooter(Context.Language.GetString("modules_help"));

            foreach (Module module in modules)
            {
                int commandCount = module.Commands.Count + module.Submodules.Count;

                embed.AddField(module.Name, Context.Language.GetString("modules_count").Replace("{cmds}", commandCount.ToString()), true);
            }

            return embed;
        }

        private EmbedBuilder GetSingularCommandHelp(YukiCommandContext Context, Command command)
        {
            string name = "";

            if (command.Module.Parent != null)
            {
                name += command.Module.Name + " ";
            }

            name += command.Name;

            string aliases = string.Join(", ", command.Aliases.Where(alias => alias != command.Name));

            EmbedBuilder embed = Context.CreateEmbedBuilder(name)
                .AddField(Context.Language.GetString("help_aliases"), (string.IsNullOrWhiteSpace(aliases) ? Context.Language.GetString("commands_no_alias") : aliases))
                .AddField(Context.Language.GetString("help_description"), Context.Language.GetString("command_" + name.ToLower() + "_desc"))
                .AddField(Context.Language.GetString("help_usage"), Context.Language.GetString("command_" + name.ToLower() + "_usage"));

            return embed;
        }

        private EmbedBuilder GetSubCommandsHelp(YukiCommandContext Context, List<Command> commands, string commandStr)
        {
            List<Command> subCommands = YukiBot.Discord.CommandService.GetAllModules()
                                                            .Where(mod => mod.Parent != null && mod.Name.ToLower() == commandStr.ToLower())
                                                            .SelectMany(mod => mod.Commands).ToList();

            /* Is commandStr a command group? */
            if (subCommands != null && subCommands.Count > 0)
            {
                EmbedBuilder embed = Context.CreateEmbedBuilder(subCommands[0].Module.Name);

                foreach (Command subCommand in subCommands)
                {
                    string description = Context.Language.GetString("command_" + subCommand.Name.ToLower().Replace(' ', '_') + "_desc") + "\n\n" +
                                             Context.Language.GetString("command_" + subCommand.Name.ToLower().Replace(' ', '_') + "_usage") + "\n";

                    embed.AddField(subCommand.Name, description);
                }

                return embed;
            }
            /* If it's not, we want to list the commands in the module commandStr */
            else
            {
                Module module = YukiBot.Discord.CommandService.GetAllModules()
                                                .FirstOrDefault(mod => mod.Name.ToLower() == commandStr.ToLower());

                if (module != null && module.Commands.Count > 0)
                {
                    EmbedBuilder embed = Context.CreateEmbedBuilder(Context.Language.GetString("commands_title")).WithFooter(Context.Language.GetString("commands_help"));

                    foreach (Command command in module.Commands)
                    {
                        string embedValue = string.Join(", ", command.Aliases.Where(alias => alias != command.Name));

                        embed.AddField(command.Name, (!string.IsNullOrWhiteSpace(embedValue) ? embedValue : Context.Language.GetString("commands_no_alias")), true);
                    }

                    List<string> subModules = YukiBot.Discord.CommandService.GetAllModules()
                                                     .FirstOrDefault(mod => mod.Name.ToLower() == commandStr.ToLower()).Submodules.Select(mod => mod.Name).ToList();

                    foreach (string mod in subModules)
                    {
                        if (embed.Fields.Count < 25)
                        {
                            embed.AddField(mod, Context.Language.GetString("command_is_submodule"), true);
                        }
                        else
                        {
                            Context.Channel.SendMessageAsync("", false, embed.Build());

                            embed.Fields.Clear();
                            embed.AddField(mod, Context.Language.GetString("command_is_submodule"), true);
                        }
                    }

                    return embed;
                }
                /* module doesn't exist */
                else
                {
                    return null;
                }
            }
        }
    }
}
