using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;


namespace Yuki.Extensions
{
    public static class StringExtensions
    {
        public static bool HasUrl(this string content, out int[] indexes)
        {
            string[] split = content.Split(" ");

            indexes = new int[split.Length];

            for(int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = -1;
            }

            for(int i = 0; i < split.Length; i++)
            {
                if(split[i].IsUrl())
                {
                    indexes[i] = i;
                }
            }

            indexes = indexes.Where(i => i > -1).ToArray();

            return indexes.Length > 0;
        }

        public static bool IsUrl(this string url)
            => Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

        public static bool IsMedia(this string url)
        {
            List<string> recognisedImageExtensions = new List<string>() { ".jpeg", ".jpg", ".png", ".gif" };

            foreach (string extension in recognisedImageExtensions)
            {
                if (extension.Equals(Path.GetExtension(url)))
                {
                    return true;
                }
            }

            return false;
        }

        public static Discord.Color AsColor(this string hex)
        {
            hex = hex.TrimStart('#');

            System.Drawing.Color col;
            if (hex.Length == 6)
                col = System.Drawing.Color.FromArgb(255, // hardcoded opaque
                            int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber));
            else // assuming length of 8
                col = System.Drawing.Color.FromArgb(
                            int.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                            int.Parse(hex.Substring(6, 2), NumberStyles.HexNumber));
            
            return new Discord.Color(col.R, col.G, col.B);
        }

        public static double Calculate(this string expression)
        {
            DataTable table = new DataTable();

            table.Columns.Add("expression", string.Empty.GetType(), expression);

            DataRow row = table.NewRow();

            table.Rows.Add(row);

            return double.Parse((string)row["expression"]);
        }

        public static T GetEnum<T>(this string str)
        {
            Array v = Enum.GetValues(typeof(T));

            foreach (object t in v)
            {
                if (t.ToString().ToLower() == str.ToLower())
                {
                    return (T)t;
                }
            }
            return default;
        }

        public static string[] SplitAt(this string source, params int[] index)
        {
            index = index.Distinct().OrderBy(x => x).ToArray();
            string[] output = new string[index.Length + 1];
            int pos = 0;

            for (int i = 0; i < index.Length; pos = index[i++])
                output[i] = source.Substring(pos, index[i] - pos);

            output[index.Length] = source.Substring(pos);
            return output;
        }
    }
}
