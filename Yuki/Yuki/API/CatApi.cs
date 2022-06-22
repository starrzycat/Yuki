using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Yuki.Core;
using Yuki.Data.Objects.API;

namespace Yuki.API
{
    public static class CatApi
    {
        public static async Task<string> GetImage()
        {
            using (HttpClient http = new HttpClient())
            {
                try
                {
                    CatJson result = JsonConvert.DeserializeObject<CatJson[]>((await http.GetAsync("https://api.thecatapi.com/v1/images/search")).Content.ReadAsStringAsync().Result)[0];
                    await Logger.LogDebug(result.Url.AbsoluteUri);
                    return result.Url.AbsoluteUri;
                }
                catch(Exception e)
                {
                    await Logger.LogDebug(e);
                    return "";
                }
            }
        }
    }
}
