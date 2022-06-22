using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.FunModule
{
    public partial class FunModule
    {
        [Command("emojify")]
        [Cooldown(1, 2, CooldownMeasure.Seconds, CooldownBucketType.User)]
        public async Task EmojifyStringAsync([Remainder] string text)
        {
            text = text.ToLower();
            string value = "";

            string[] numbers = { "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" };

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] >= 'a' && text[i] <= 'z')
                {
                    value += $":regional_indicator_{text[i]}:";
                }
                else
                {
                    if (text[i] == ' ')
                    {
                        value += "\t";
                    }
                    else if(text[i] == '!')
                    {
                        value += ":exclamation:";
                    }
                    else if(text[i] == '?')
                    {
                        value += ":question:";
                    }
                    else if (text[i] >= '0' && text[i] <= '9')
                    {
                        value += $":{numbers[text[i] - '0']}:";
                    }
                    else
                    {
                        value += text[i];
                    }
                }
            }
            await ReplyAsync(value);
        }
    }
}
