using LiteDB;
using System.Linq;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public class Patreon
    {
        private const string collection = "patreon";

        public static bool IsPatron(ulong userId)
        {
            return UserSettings.IsPatron(userId);
        }

        public static void AddCommand(PatronCommand command)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<PatronCommand> commands = db.GetCollection<PatronCommand>(collection);

                if (commands.Find(cmd => cmd.UserId == command.UserId).Equals(default(PatronCommand)))
                {
                    commands.Insert(command);
                }
                else
                {
                    commands.Update(command);
                }
            }
        }

        public static PatronCommand GetCommand(ulong userId, string name)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<PatronCommand> commands = db.GetCollection<PatronCommand>(collection);

                if (commands.Find(cmd => cmd.UserId == userId).Equals(default(PatronCommand)))
                {
                    return default;
                }
                else
                {
                    return commands.Find(cmd => cmd.UserId == userId && cmd.Name.ToLower() == name.ToLower()).FirstOrDefault();
                }
            }
        }
    }
}
