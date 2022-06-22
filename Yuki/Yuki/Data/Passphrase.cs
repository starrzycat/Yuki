using System;
using System.Linq;

namespace Yuki.Data
{
    public static class Passphrase
    {
        const int MinIDLength = 10;

        private static string alphabetLower = "abcdefghijlmnopqrstuvwxyz";
        private static string alphabetCap = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private static string numbers = "0123456789";
        private static string symbols = "!@&-_=.?";

        public static string Generate(int length = MinIDLength)
        {
            string chars = alphabetLower + alphabetCap + numbers + symbols;

            string phrase = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[(new Random()).Next(s.Length)]).ToArray());

            return phrase;
        }

        public static string GenerateAlpha(int length = MinIDLength)
        {
            string chars = alphabetLower + alphabetCap;

            string phrase = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[(new Random()).Next(s.Length)]).ToArray());
            return phrase;
        }
    }
}
