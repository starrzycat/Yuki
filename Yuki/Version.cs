using Discord;

namespace Yuki
{
    public static class Version
    {
        public static int Major  { get; } = 3;
        public static int Minor  { get; } = 1;
        public static int Hotfix { get; } = 0;
        
        public static ReleaseType ReleaseType { get; } = ReleaseType.Development;

        public static string DiscordNetVersion { get; } = DiscordConfig.Version + $" (API v{DiscordConfig.APIVersion}, Voice API v{DiscordConfig.VoiceAPIVersion})";

        public static string Get() => $"{Major}.{Minor}.{Hotfix}";

        public static string GetFull() => Get() + $"-{ReleaseType}";

        public static new string ToString() => GetFull() + $" | Discord.Net v{DiscordNetVersion}";
    }

    public enum ReleaseType
    {
        Development,
        PreRelease,
        Release
    }
}
