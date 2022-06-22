using System;
using Yuki.Core;

namespace Yuki
{
    public class Program
    {
        private static YukiBot bot;

        static void Main(string[] args)
        {
            bot = new YukiBot();

            Console.CancelKeyPress += (s, ev) => bot.Stop();
            AppDomain.CurrentDomain.ProcessExit += (s, ev) => bot.Stop();
            AppDomain.CurrentDomain.UnhandledException += Yuki_UnhandledException;

            Console.Title = "Yuki v" + Version.ToString();

            /* Run the bot */
            bot.RunAsync().GetAwaiter().GetResult();
        }

        static void Yuki_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.LogError(e);
        }
    }
}
