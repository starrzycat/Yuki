using Discord;
using Nett;
using Qmmands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Yuki.Commands;
using Yuki.Data.Objects;
using Yuki.Extensions;
using Yuki.Services.Database;

namespace Yuki.Core
{
    public static class Localization
    {
        public static Dictionary<string, Language> Languages { get; private set; } = new Dictionary<string, Language>();

        public static void LoadLanguages()
        {
            if (!Directory.Exists(FileDirectories.LangRoot))
            {
                Directory.CreateDirectory(FileDirectories.LangRoot);
            }

            if (Languages.Count < 1)
            {
                string[] langFiles = Directory.GetFiles(FileDirectories.LangRoot);

                for (int i = 0; i < langFiles.Length; i++)
                {
                    Language lang = Toml.ReadFile<Language>(langFiles[i]);
                    lang.data = Toml.ReadFile(langFiles[i]);

                    Languages.Add(lang.Code, lang);
                }
            }
        }

        public static void Reload()
        {
            if (Languages.Count > 0)
            {
                Languages.Clear();
                LoadLanguages();
            }
        }

        public static Language GetLanguage(string code)
        {
            foreach(string langCode in Languages.Keys)
            {
                if(code.ToLower() == langCode.ToLower())
                {
                    return Languages[langCode];
                }
            }
            
            return Languages["en_US"];
        }

        public static Language GetLanguage(YukiCommandContext context)
        {
            string langCode = "en_US";

            if (context.Channel is IGuildChannel)
            {
                langCode = GuildSettings.GetGuild(context.Guild.Id).LangCode;
            }

            if (string.IsNullOrWhiteSpace(langCode))
            {
                langCode = "en_US";
            }

            return GetLanguage(langCode);
        }

        public static void CheckTranslations()
        {
            Dictionary<string, List<Module>> modules = new Dictionary<string, List<Module>>();
            GetModules(modules);

            foreach (KeyValuePair<string, Language> lang in Languages)
            {
                Logger.LogStatus($"Checking translations for language w/code {lang.Value.Code}...");

                int invalidTranslations = 0;

                //Iterate over every command in each module
                foreach (KeyValuePair<string, List<Module>> pair in modules)
                {
                    foreach (Module module in pair.Value)
                    {
                        foreach (Command command in module.Commands)
                        {
                            string cmdName = null;

                            if (cmdName != null)
                            {
                                cmdName += "_";
                            }

                            cmdName += command.Name.Replace(' ', '_');

                            if (!lang.Value.data.Any(x => x.Key == $"command_{cmdName}_desc"))
                            {
                                Logger.LogWarning($"   No translation found for command_{cmdName}_desc");
                                invalidTranslations++;
                            }
                            else if (!lang.Value.data.Any(x => x.Key == $"command_{cmdName}_usage"))
                            {
                                Logger.LogWarning($"   No translation found for command_{cmdName}_usage");
                                invalidTranslations++;
                            }
                        }
                    }
                }

                if (invalidTranslations > 0)
                {
                    Logger.LogWarning($"{lang.Value.Code} has {invalidTranslations} missing translations!");
                    Console.ReadKey();
                }
            }
        }

        private static void GetModules(Dictionary<string, List<Module>> modules)
        {
            foreach (Module module in YukiBot.Discord.CommandService.GetAllModules())
            {
                if (!module.IsSubmodule())
                {
                    string moduleName = module.Name.Split('_')[0].ToLower();

                    if (!modules.ContainsKey(moduleName))
                    {
                        modules.Add(moduleName, new List<Module>());
                    }

                    modules[moduleName].Add(module);

                    if (module.Submodules.Count > 0)
                    {
                        foreach (Module subModule in module.Submodules)
                        {
                            modules[moduleName].Add(subModule);
                        }
                    }
                }
            }
        }
    }
}