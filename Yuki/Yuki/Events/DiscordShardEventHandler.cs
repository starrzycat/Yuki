using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using System.Linq;
using Yuki.Core;
using Discord;
using Yuki.Services;
using Yuki.Services.Database;
using System.Collections.Generic;
using Yuki.Data.Objects.Database;

namespace Yuki.Events
{
    public static class DiscordShardEventHandler
    {
        private static int connectedShards = 0;

        public static async Task ShardReady(DiscordSocketClient client)
        {
            connectedShards++;

            /* Disable the Ready event */
            if(connectedShards == YukiBot.Discord.ShardCount)
            {
                YukiBot.Discord.Client.ShardReady -= ShardReady;
            }

            SetClientEvents(client);

            string message = System.IO.File.ReadAllLines(FileDirectories.StatusMessages)[0];

            await Logger.LogInfo($"Client logged in as {YukiBot.Discord.Client.CurrentUser.Username}#{YukiBot.Discord.Client.CurrentUser.Discriminator}");

            await client.SetGameAsync(name: message.Replace("{shard}", client.ShardId.ToString())
                                             .Replace("{users}", client.Guilds.Select(guild => guild.MemberCount).Sum().ToString())
                                             .Replace("{guildcount}", client.Guilds.Count.ToString()),
                                             streamUrl: null, type: ActivityType.Playing);

            /*await Logger.LogInfo("Updating timestamps...");
            _ = Task.Run(async () => { await StarboardData.UpdatePosts(); });
            await Logger.LogInfo("Updating timestamps complete!");*/
        }

        private static async Task TimerElapsed()
        {
            await Logger.LogDebug("test [DiscordShardEventHandler]");
            await Starboard.CheckTopPosts();

            List<GuildConfiguration> dirtyGuilds = GuildSettings.GetDirtyGuilds();

            for (int i = 0; i < dirtyGuilds.Count; i++)
            {
                if (dirtyGuilds[i].LeaveDate != DateTime.MinValue && (DateTime.Now - dirtyGuilds[i].LeaveDate).Days > 7)
                {
                    GuildSettings.DeleteGuildData(dirtyGuilds[i].Id);
                    StarboardData.RemovePostsFromGuild(dirtyGuilds[i].Id);
                }
            }
        }

        public static async Task ShardConnected(DiscordSocketClient client)
        {
            await Logger.LogStatus($"Shard {client.ShardId} connected");

            System.Timers.Timer hourlyTimer = new System.Timers.Timer(TimeSpan.FromHours(1).TotalMilliseconds);
            //System.Timers.Timer hourlyTimer = new System.Timers.Timer(10 * 1000);
            hourlyTimer.AutoReset = true;

            hourlyTimer.Elapsed += async (sender, e) =>
            {
                await TimerElapsed();
            };

            hourlyTimer.Start();
            await TimerElapsed();
        }

        public static Task ShardDisconnected(Exception e, DiscordSocketClient client)
        {
            if (!YukiBot.ShuttingDown)
            {
                Logger.LogError($"Shard {client.ShardId} disconnected. Reason: " + e);

                //await YukiBot.Discord.StopAsync();
                //Thread.Sleep(500);
                //await YukiBot.Discord.LoginAsync(Config.GetConfig().token);
            }

            return Task.CompletedTask;
        }

        private static void SetClientEvents(DiscordSocketClient client)
        {
            client.MessageReceived += DiscordSocketEventHandler.MessageReceived;

            client.MessageUpdated += DiscordSocketEventHandler.MessageUpdated;
            client.MessageDeleted += DiscordSocketEventHandler.MessageDeleted;


            client.ReactionAdded += DiscordSocketEventHandler.ReactionAdded;
            client.ReactionRemoved += DiscordSocketEventHandler.ReactionRemoved;
            client.ReactionsCleared += DiscordSocketEventHandler.ReactionsCleared;
            
            client.UserBanned += DiscordSocketEventHandler.UserBanned;
            client.UserJoined += DiscordSocketEventHandler.UserJoined;
            client.UserLeft += DiscordSocketEventHandler.UserLeft;
            client.UserUnbanned += DiscordSocketEventHandler.UserUnbanned;

            client.RoleDeleted += DiscordSocketEventHandler.RoleDeleted;

            client.JoinedGuild += DiscordSocketEventHandler.JoinedGuild;
            client.LeftGuild += DiscordSocketEventHandler.LeftGuild;
        }
    }
}
