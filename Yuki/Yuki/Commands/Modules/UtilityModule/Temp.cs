using Qmmands;
using System.Threading.Tasks;

namespace Yuki.Commands.Modules.UtilityModule
{
    public partial class UtilityModule
    {
        [Command("temp")]
        public async Task ConvertTempAsync(string arg)
        {
            if(double.TryParse(arg.Substring(0, arg.Length - 1), out double temp))
            {
                char measurement = arg.ToUpper()[arg.Length - 1];

                switch(measurement)
                {
                    case 'C':
                        await ReplyAsync($"{(temp * 1.8d) + 32}F");
                        break;
                    case 'F':
                        await ReplyAsync($"{(temp - 32) / 1.8d}C");
                        break;
                    default:
                        await ReplyAsync(Language.GetString("temp_invalid"));
                        break;
                }
            }
            else
            {
                await ReplyAsync(Language.GetString("temp_invalid"));
            }
        }
    }
}
