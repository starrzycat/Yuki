using CSharpFunctionalExtensions;
using Discord.Interactions;
using Newtonsoft.Json;

namespace Yuki.SlashCommands {
    using IJsonResult = Result<IJsonDeserializable, string>;

    public interface IJsonDeserializable {
        public static async Task<IJsonResult> GetJson<T>(string url) where T : IJsonDeserializable, new() {
            var taskResult = Task<T>.CompletedTask;

            using HttpClient client = new();

            HttpResponseMessage responseMessage;

            try {
                responseMessage = await client.GetAsync(url);
            } catch (Exception exp) {
                return Result.Failure<IJsonDeserializable, string>($"Http request failed with the following error: {exp.Message}");
            }

            if (responseMessage.IsSuccessStatusCode) {
                var jsonString = await responseMessage.Content.ReadAsStringAsync();

                try {
                    var data = new T().FromJson(jsonString);
                
                    if (data.IsSuccess) {
                        return data;
                    }
                    else {
                        return Result.Failure<IJsonDeserializable, string>($"Json parse failed with the following error: {data.Error}");
                    }
                } catch(Exception exp) {
                    return Result.Failure<IJsonDeserializable, string>($"Could not convert json to the specified data type: {exp.Message}");
                }
            }
            else {
                return Result.Failure<IJsonDeserializable, string>($"Json fetch returned status code {responseMessage.StatusCode}");
            }
        }

        public IJsonResult FromJson(string jsonString);
        
        public string GetUrl();
    }

    public struct CatObject : IJsonDeserializable {
        public string Id {get; set; }
        public string Url {get; set; }
        public string Width {get; set; }
        public string Height {get; set; }

        public IJsonResult FromJson(string jsonString) {
            var data = JsonConvert.DeserializeObject<CatObject[]>(jsonString);
            
            if (data != null && data.Length > 0) {
                return Result.Success<IJsonDeserializable, string>(data[0]);
                // do something
            } else {
                return Result.Failure<IJsonDeserializable, string>("Data is null or has a length of 0");
            }
        }

        public string GetUrl() {
            return Url;
        }
    }

    public struct DogObject : IJsonDeserializable {
        public string Message { get; set; }
        public string Status { get; set; }

        public IJsonResult FromJson(string jsonString) {
            var data = JsonConvert.DeserializeObject<DogObject>(jsonString);
            
            if (data.Status == "success") {
                return Result.Success<IJsonDeserializable, string>(data);
            } else {
                return Result.Failure<IJsonDeserializable, string>($"API returned status {data.Status}");
            }
        }

        public string GetUrl() {
            return Message;
        }
    }

    public enum ImageSendType {
        Dog,
        Cat
    }

    public class ImageSlashModule : InteractionModuleBase {
        private const string DOG_URL = "https://dog.ceo/api/breeds/image/random";
        private const string CAT_URL = "https://api.thecatapi.com/v1/images/search";

        [SlashCommand("image", "Sends an image")]
        public async Task ImageCommandAsync([Summary(description: "The image you want to send")] ImageSendType image) {
            var maybeResult = Maybe<IJsonResult>.None;

            switch (image) {
                case ImageSendType.Dog:
                    maybeResult = await IJsonDeserializable.GetJson<DogObject>(DOG_URL);
                    break;
                case ImageSendType.Cat:
                    maybeResult = await IJsonDeserializable.GetJson<CatObject>(CAT_URL);
                    break;
            };

            if (maybeResult.HasValue) {
                var result = maybeResult.Value;

                if (result.IsSuccess) {
                    var jsonResult = result.Value;
                    
                    await RespondAsync(jsonResult.GetUrl());
                } else {
                    await RespondAsync(result.Error);
                }
            } else {
                await RespondAsync("Cannot fetch image (unimplemented type)");
            }
        }
    }
}