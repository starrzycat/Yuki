using Nett;
using System.IO;
using Yuki.Data.Objects;

namespace Yuki.Services
{
    public static class PollingService
    {
        public static Poll GetPoll(string pollId) => Load(pollId);

        public static void Save(Poll poll)
            => Toml.WriteFile<Poll>(poll, FileDirectories.PollRoot + poll.Id + ".toml");

        public static Poll Load(string pollId)
        {
            string file = FileDirectories.PollRoot + pollId + ".toml";

            if (File.Exists(file))
            {
                return Toml.ReadString<Poll>(File.ReadAllText(file));
            }
            else
            {
                return default;
            }
        }
    }
}
