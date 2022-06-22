using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Yuki.API
{
    public static class DogApi
    {
        public static async Task<string> GetImage()
        {
            using (HttpClient http = new HttpClient())
            {
                DogJson result = JsonConvert.DeserializeObject<DogJson>((await http.GetAsync("https://dog.ceo/api/breeds/image/random")).Content.ReadAsStringAsync().Result);

                if (result.status == "success")
                {
                    return result.message;
                }
                else
                {
                    return await GetImage();
                }
            }
        }
    }

    public class DogJson
    {
        public string status;
        public string message;
    }
}
