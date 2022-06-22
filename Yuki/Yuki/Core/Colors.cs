using Discord;
using System;
using System.Collections.Generic;

namespace Yuki.Core
{
    public static class Colors
    {
        public static Color[] DiscordColors()
        {
            return new[] { Color.Blue, Color.DarkBlue, Color.DarkerGrey, Color.DarkGreen, Color.DarkGrey, Color.DarkMagenta, Color.DarkOrange,
                           Color.DarkPurple, Color.DarkRed, Color.DarkTeal, Color.Gold, Color.Green, Color.LighterGrey, Color.LightGrey, Color.LightOrange,
                           Color.Magenta, Color.Orange, Color.Purple, Color.Red, Color.Teal };
        }

        public static Color[] Get()
        {
            List<Color> Colors = new List<Color>();
            Colors.Add(Pink);

            Colors.AddRange(DiscordColors());

            return Colors.ToArray();

        }

        public static Color RandomColor
            => Get()[new Random().Next(Get().Length)];

        public static Color Pink = new Color(228, 11, 210);

        public static Color Yellow = new Color(255, 255, 0);
    }
}
