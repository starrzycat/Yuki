using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Yuki.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToPrettyTime(this DateTime dateTime, bool getTimeLeft, bool showTime)
        {
            string prettyTime = string.Empty;

            if(getTimeLeft)
            {
                TimeSpan timeLeft;

                if(dateTime < DateTime.UtcNow)
                {
                    timeLeft = DateTime.UtcNow - dateTime;
                }
                else
                {
                    timeLeft = dateTime - DateTime.UtcNow;
                }

                if(timeLeft.Days > 0)
                {
                    prettyTime += timeLeft.Days + " days, ";
                }

                if(timeLeft.Hours > 0)
                {
                    prettyTime += timeLeft.Hours + " hours, ";
                }

                if(timeLeft.Minutes > 0)
                {
                    prettyTime += timeLeft.Minutes + " minutes, ";
                }

                if(prettyTime == string.Empty)
                {
                    if(timeLeft.Seconds > 0)
                    {
                        prettyTime += timeLeft.Seconds + " seconds  ";
                    }
                }

                if(prettyTime.Length > 2)
                {
                    return prettyTime.Remove(prettyTime.Length - 2);
                }
                else
                {
                    return prettyTime;
                }
            }
            else
            {
                string end = $"{dateTime.DayOfWeek} {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(dateTime.Month)} {dateTime.Day}, {dateTime.Year}";

                if(showTime)
                {
                    return $"{end} @ {dateTime.ToUniversalTime().ToShortTimeString()} UTC";
                }

                return end;
            }
        }

        public static DateTime ToDateTime(this string src)
        {
            DateTime nowDate = DateTime.UtcNow;
            DateTime dateTime = nowDate;

            string[] strs = Regex.Split(src, @"\s*[ ]\s*");

            for(int i = 0; i < strs.Length; i++)
            {
                char endChar = strs[i][strs[i].Length - 1];

                /*new string(strs[i].Where(c => char.IsDigit(c)).ToArray())*/
                if(int.TryParse(strs[i].Remove(strs[i].Length - 1), out int val))
                {
                    val = Math.Abs(val);

                    switch (endChar)
                    {
                        case 'd':
                            dateTime = dateTime.AddDays(val);
                            break;
                        case 'h':
                            dateTime = dateTime.AddHours(val);
                            break;
                        case 'm':
                            dateTime = dateTime.AddMinutes(val);
                            break;
                    }
                }
            }

            return (dateTime == nowDate) ?
                        default :
                        dateTime;
        }
    }
}
