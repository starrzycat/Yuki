using Discord;
using System;
using System.IO;
using System.Threading.Tasks;
using Yuki.ColoredConsole;

namespace Yuki.Core
{
    public enum LogLevel
    {
        DiscordNet,
        Debug,
        Info,
        Error,
        Warning,
        Status
    }

    public static class Logger
    {
        private static string latestLogFile;
        
        static Logger()
        {
            if(!Directory.Exists(FileDirectories.LogRoot))
            {
                Directory.CreateDirectory(FileDirectories.LogRoot);
            }

            if(File.Exists(FileDirectories.LogRoot + "latest.log"))
            {
                File.Delete(FileDirectories.LogRoot + "latest.log");
            }

            latestLogFile = FileDirectories.LogRoot + "latest.log";

            File.Create(latestLogFile).Dispose();
        }

        public static Task Log(LogMessage logMessage)
        {
            Write(LogLevel.DiscordNet, logMessage.Message);

            return Task.CompletedTask;
        }
        
        public static Task LogDiscord(object o)
        {
            Write(LogLevel.DiscordNet, o);

            return Task.CompletedTask;
        }
        
        public static Task LogInfo(object o)
        {
            Write(LogLevel.Info, o);

            return Task.CompletedTask;
        }
        
        public static Task LogDebug(object o)
        {
            Write(LogLevel.Debug, o);

            return Task.CompletedTask;
        }
        
        public static Task LogError(object o)
        {
            Write(LogLevel.Error, o);

            return Task.CompletedTask;
        }
        
        public static Task LogStatus(object o)
        {
            Write(LogLevel.Status, o);

            return Task.CompletedTask;
        }
        
        public static Task LogWarning(object o)
        {
            Write(LogLevel.Warning, o);

            return Task.CompletedTask;
        }

        private static void Write(LogLevel logLevel, object o)
        {
            /* only print debug info if its a prerelease or dev build */
            if((logLevel == LogLevel.Debug) && (Version.ReleaseType != ReleaseType.Development && Version.ReleaseType != ReleaseType.PreRelease))
            {
                return;
            }

            ColoredConsoleColor color = ConsoleColors.Gray;

            switch(logLevel)
            {
                case LogLevel.Debug:
                    color = ConsoleColors.Cyan;
                    break;
                case LogLevel.DiscordNet:
                    color = ConsoleColors.Blurple;
                    break;
                case LogLevel.Error:
                    color = ConsoleColors.Red;
                    break;
                case LogLevel.Info:
                    color = ConsoleColors.Gray;
                    break;
                case LogLevel.Status:
                    color = ConsoleColors.Green;
                    break;
                case LogLevel.Warning:
                    color = ConsoleColors.Orange;
                    break;
            }

            string obj = (o != null ? o.ToString() : "Sent object was null!");
            string line = $"[{DateTime.Now.ToShortTimeString()}] [{logLevel}] {obj}";
            string lineColored = $"[{DateTime.Now.ToShortTimeString()}] {color}[{logLevel}] {obj}";

            using (FileStream file = new FileStream(latestLogFile, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.WriteLine(line);
                    sw.Flush();
                }
            }


            Console.WriteLine(lineColored + ConsoleColors.Gray);

            if(logLevel == LogLevel.Error)
            {
                WriteCrashFile(o);
            }
        }

        private static void WriteCrashFile(object o)
        {
            if (Version.ReleaseType != ReleaseType.Development)
            {
                string log = FileDirectories.LogRoot + $"crash {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToShortTimeString()}.log";

                File.Copy(latestLogFile, log);

                // send a crash log to the Yuki server so that I get notified asap
                YukiBot.Discord.Client.GetGuild(620246094756184064).GetTextChannel(674991918647869461).SendFileAsync(log, Localization.GetLanguage("en_US").GetString("crash"));
                File.Delete(log); // cleanup

                //throw new Exception(text);
            }
        }
    }
}
