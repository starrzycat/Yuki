using Nett;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data;
using Yuki.Services.Database;

namespace Yuki
{
    public class YukiBot
    {
        public const string PatronUrl = "https://www.patreon.com/user?u=7361846";
        public const string PayPalUrl = "https://paypal.me/veenus2247";
        public const string ServerUrl = "https://discord.gg/KwHQzuy";
        public const string BotInvUrl = "https://discordapp.com/oauth2/authorize?client_id=338887651677700098&scope=bot&permissions=271690950";
        public const string GithubUrl = "https://github.com/VeeDeeOh/Yuki/";
        public const string WikiUrl   = "https://github.com/VeeDeeOh/Yuki/wiki/";

        public static DiscordBot Discord { get; private set; }
        
        /* Prevent errors on client disconnect */
        public static bool ShuttingDown;

        public YukiBot()
        {
            Logger.LogInfo("Loading languages....");
            Localization.LoadLanguages();

            FileDirectories.CheckCreateDirectories();

            Discord = new DiscordBot();
        }

        public async Task RunAsync()
        {
            await Logger.LogInfo(DateTime.Now.ToShortDateString() + " @ " + DateTime.Now.ToShortTimeString() + " " + TimeZoneInfo.Local.DisplayName);
            string token;

            if (!File.Exists(FileDirectories.ConfigFile))
            {
                Config c = new Config();

                Console.Write("Please enter your bot's token: ");
                c.token = Console.ReadLine();

                Toml.WriteFile(c, FileDirectories.ConfigFile);
            }

            token = Config.GetConfig(reload: true).token;

            await Discord.LoginAsync(token);
            await Logger.LogInfo($"Client has been recommended {Discord.ShardCount} shards");

            Discord.Client.Log += Logger.Log;

            await Discord.Client.StartAsync();

            Localization.CheckTranslations();

            await Logger.LogDebug($"Found {Discord.CommandService.GetAllCommands().Count} command(s)");

            //UserMessageCache.Update();
            UserMessageCache.LoadFromFile();
            StarboardData.Load();

            await Task.Delay(-1);
        }

        public void Stop()
        {
            ShuttingDown = true;
            Logger.LogStatus("Stopping client and saving data...");

            UserMessageCache.SaveToFile();
            StarboardData.Save();
            Discord.StopAsync().GetAwaiter().GetResult();

            Thread.Sleep(1000);
        }
    }
}
