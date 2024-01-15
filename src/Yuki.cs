using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Yuki.Services;

namespace Yuki {
    public class Yuki {
        public static readonly IServiceProvider Services = ConfigureServices();
        public static readonly DiscordSocketClient Client = Services.GetRequiredService<DiscordSocketClient>();
        public static readonly CommandService Commands = Services.GetRequiredService<CommandService>();
        public static readonly InteractionService Interactions = Services.GetRequiredService<InteractionService>();
        public static readonly LoggingService Log = Services.GetRequiredService<LoggingService>();

        private static IServiceProvider ConfigureServices() {
            var discordConfig = new DiscordConfig() {

            };

            var collection = new ServiceCollection()
                    .AddSingleton(discordConfig)
                    .AddSingleton<DiscordSocketClient>()
                    .AddSingleton<CommandService>()
                    .AddSingleton<InteractionService>()
                    .AddSingleton<LoggingService>();

            return collection.BuildServiceProvider();
        }

        public async Task RunYukiAsync() {
            string token = "token";

            Client.Ready += ClientReady;

            await Client.LoginAsync(TokenType.Bot, token);
            await Client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private async Task ClientReady() {
            Console.WriteLine("OK");



            await Task.CompletedTask;
        }
    }
}