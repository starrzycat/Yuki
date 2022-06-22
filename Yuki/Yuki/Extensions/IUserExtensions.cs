using Discord;

namespace Yuki.Extensions
{
    public static class IUserExtensions
    {
        public static string GetBigAvatarUrl(this IUser user)
            => user.GetAvatarUrl(ImageFormat.Auto, 2048);
    }
}
