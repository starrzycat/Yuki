using System;

namespace Yuki.Commands
{
    [Flags]
    public enum CooldownBucketType
    {
        Guild = 1,
        Channel = 2,
        User = 4,
        Global = 8
    }
}
