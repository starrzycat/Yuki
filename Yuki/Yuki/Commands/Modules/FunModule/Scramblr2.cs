using Discord;
using MarkovSharp.TokenisationStrategies;
using Qmmands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Data;
using Yuki.Data.Objects;
using Yuki.Services.Database;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("scramblr2", "scrambler2")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task Scramblr2(params IUser[] users)
        {
            bool canGetMsgs = UserSettings.CanGetMsgs(Context.User.Id);

            try
            {
                DateTime now = DateTime.Now;

                StringMarkov model = new StringMarkov();
                model.EnsureUniqueWalk = true;

                if (canGetMsgs)
                {
                    List<string> lines = UserMessageCache.GetMessagesFromUser(Context.User.Id).Select(msg => msg.Content).ToList();

                    if (lines.Count < 4)
                    {
                        await ReplyAsync(Language.GetString("main_few_msgs").Replace("{user}", Context.User.Username));
                        return;
                    }

                    for (int i = 0; i < users.Length; i++)
                    {
                        if (UserSettings.CanGetMsgs(users[i].Id))
                        {
                            lines.AddRange(UserMessageCache.GetMessagesFromUser(users[i].Id).Select(msg => msg.Content));
                        }
                        else
                        {
                            await ReplyAsync(Language.GetString("scramblr_not_enabled").Replace("{user}", $"{users[i].Username}#{users[i].Discriminator}"));
                            return;
                        }
                    }

                    model.Learn(lines);

                    string phrase = model.Walk(1).First();

                    int tries = 0;

                    bool success = false;
                    while (!success && tries < 25)
                    {
                        phrase = model.Walk(1).First();

                        bool found = false;
                        for (int i = 0; i < lines.Count; i++)
                        {
                            if (lines[i].ToLower().Replace(" ", "") == phrase.ToLower().Replace(" ", ""))
                            {
                                tries++;
                                found = true;
                                break;
                            }
                        }

                        success = !found;
                    }

                    await ReplyAsync(phrase);
                }
                else
                {
                    await ReplyAsync(Language.GetString("scramblr_not_enabled").Replace("{user}", $"{Context.User.Username}#{Context.User.Discriminator}"));
                }
            }
            catch (Exception e)
            {
                await ReplyAsync($"Caught exception!\n\n{e}");
            }
        }
    }
}
