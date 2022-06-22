using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Services;

namespace Yuki.Commands.Modules.GamblingModule
{
    public partial class GamlingModule
    {
        [Group("roulette")]
        public class RussianRoulette : YukiModule
        {
            [Command]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task Base()
            {
                try
                {
                    if (RussianRouletteService.GameExists(Context.Guild.Id, Context.Channel.Id) &&
                        RussianRouletteService.GetGame(Context.Guild.Id, Context.Channel.Id).IsCurrentPlayer(Context.User.Id))
                    {
                        RouletteResult result = RussianRouletteService.GetGuild(Context.Guild.Id).PullTriggerInGame(Context.Channel.Id, Context.User.Id);

                        if (result == RouletteResult.Safe || result == RouletteResult.Game_Over || result == RouletteResult.Killed)
                        {
                            await ReplyAsync(Language.GetString("roulette_trigger_pulled").Replace("{user}", Context.User.Username));

                            await Task.Delay(500);

                            await ReplyAsync(Language.GetString($"roulette_{result.ToString().ToLower()}").Replace("{user}", Context.User.Username));

                            RouletteGame game = RussianRouletteService.GetGame(Context.Guild.Id, Context.Channel.Id);

                            if (game.Players.Count > 1)
                            {
                                await ReplyAsync(Language.GetString("roulette_next_player").Replace("{bullets}", $"{6 - game.CurrentChamber}")
                                                                                           .Replace("{nuser}", (await Context.Guild.GetUserAsync(game.GetNextPlayer().Id)).Mention));
                            }
                            else
                            {
                                await ReplyAsync(Language.GetString("roulette_winner").Replace("{nuser}", (await Context.Guild.GetUserAsync(game.Players[0].Id)).Mention));

                                RussianRouletteService.GetGuild(Context.Guild.Id).RemoveGame(Context.Channel.Id);
                            }
                        }
                    }
                    else
                    {
                        await ReplyAsync(Language.GetString("roulette_no_game"));
                    }
                }
                catch (Exception e) { await ReplyAsync(e); }
            }

            [Command("start")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task RouletteStartAsync()
            {
                RouletteStartResult result = RussianRouletteService.GetGuild(Context.Guild.Id).StartGame(Context.Channel.Id, Context.User.Id);

                await ReplyAsync(Language.GetString($"roulette_{result.ToString().ToLower()}"));
            }

            [Command("join")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task RouletteJoinAsync()
            {
                bool result = RussianRouletteService.GetGuild(Context.Guild.Id).AddPlayerToGame(Context.Channel.Id, Context.User.Id);

                if (result)
                {
                    int playersLeft = RouletteGame.CHAMBER_COUNT - RussianRouletteService.GetGame(Context.Guild.Id, Context.Channel.Id).Players.Count;

                    await ReplyAsync(Language.GetString("roulette_join_success").Replace("{spotsleft}", playersLeft.ToString()));
                }
                else
                {
                    await ReplyAsync(Language.GetString("roulette_join_fail"));
                }
            }

            [Command("quit")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task RouletteQuitAsync()
            {
                RussianRouletteService.GetGuild(Context.Guild.Id).RemovePlayerFromGame(Context.Channel.Id, Context.User.Id);

                await ReplyAsync(Language.GetString("roulette_quit").Replace("{user}", Context.User.Username));
            }

            [Command("kick")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task RouletteKickAsync(ulong userId)
            {
                if (RussianRouletteService.GetGuild(Context.Guild.Id).KickUserFromGame(Context.Channel.Id, userId, Context.User.Id))
                {
                    await ReplyAsync(Language.GetString("roulette_player_kicked").Replace("{user}", (await Context.Guild.GetUserAsync(userId)).Mention));
                }
                else
                {
                    await ReplyAsync(Language.GetString("roulette_not_game_master"));
                }
            }

            [Command("players")]
            [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
            public async Task RoulettePlayerListAsync()
            {
                string[] players = RussianRouletteService.GetGuild(Context.Guild.Id).GetGame(Context.Channel.Id).Players.Select(player 
                                        => Context.Guild.GetUserAsync(player.Id).Result.Username + "#" + Context.Guild.GetUserAsync(player.Id).Result.Discriminator).ToArray();

                int currPIndex = RussianRouletteService.GetGuild(Context.Guild.Id).GetGame(Context.Channel.Id).CurrentPlayer;
                players[currPIndex] = "-> " + players[currPIndex];

                for(int i = 0; i < players.Length; i++)
                {
                    string concat;

                    if(i != currPIndex)
                    {
                        concat = "    ";
                    }
                    else
                    {
                        concat = " -> ";
                    }


                    players[i] = concat + players[i];
                }

                await ReplyAsync($"`{string.Join("\n", players)}`");
            }
        }
    }
}
