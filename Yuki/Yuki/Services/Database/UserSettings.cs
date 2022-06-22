using Discord;
using LiteDB;
using System.Linq;
using Yuki.Data;
using Yuki.Data.Objects.Database;

namespace Yuki.Services.Database
{
    public static class UserSettings
    {
        private const string collection = "user_settings";

        private static YukiUser DefaultUser(ulong userId)
        {
            return new YukiUser()
            {
                Id = userId,
                CanGetMsgs = false,
                IsPatron = false,
                langCode = Config.GetConfig().default_lang
            };
        }

        public static void AddOrUpdate(YukiUser user)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                users.Upsert(user);

                /*if (users.Find(usr => usr.Id == user.Id).Equals(default(YukiUser)))
                {
                    users.Insert(user);
                }
                else
                {
                    users.Update(user);
                }*/
            }
        }

        public static void Remove(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                if (users.Find(usr => usr.Id == userId).Equals(default(YukiUser)))
                {
                    users.Delete(userId);
                }
            }
        }

        public static bool IsPatron(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                YukiUser user = users.Find(usr => usr.Id == userId).FirstOrDefault();

                if(!user.Equals(default(YukiUser)))
                {
                    return user.IsPatron;
                }
            }

            return false;
        }

        public static ulong[] GetPatrons()
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                ulong[] patrons = users.Find(usr => usr.IsPatron).Select(usr => usr.Id).ToArray();

                if (patrons != null && patrons.Length > 0)
                {
                    return patrons;
                }
            }

            return null;
        }

        public static bool CanGetMsgs(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                YukiUser user = users.Find(usr => usr.Id == userId).FirstOrDefault();
                
                return !user.Equals(default(YukiUser)) ? user.CanGetMsgs : false;
            }
        }

        public static void SetCanGetMessages(ulong userId, bool state, ITextChannel channel)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                YukiUser user = users.Find(usr => usr.Id == userId).FirstOrDefault();

                string msg = $"user exists in db: {!user.Equals(default(YukiUser))}\n\n";

                if (user.Equals(default(YukiUser)))
                {
                    user = DefaultUser(userId);
                    AddOrUpdate(user);
                }

                user.CanGetMsgs = state;

                msg += $"<@{userId}> CanGetMessages set to {user.CanGetMsgs} locally, inserting into db\n\n";

                users.Update(user);

                msg += $"CanGetMessages status inside db: {users.Find(usr => usr.Id == user.Id).FirstOrDefault().CanGetMsgs}";

                //channel.SendMessageAsync(msg);
            }
        }

        public static void AddPatron(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                YukiUser user = users.Find(usr => usr.Id == userId).FirstOrDefault();

                if (user.Equals(default(YukiUser)))
                {
                    user = DefaultUser(userId);
                    AddOrUpdate(user);
                }

                user.IsPatron = true;

                users.Update(user);
            }
        }

        public static void RemovePatron(ulong userId)
        {
            using (LiteDatabase db = new LiteDatabase(FileDirectories.SettingsDB))
            {
                LiteCollection<YukiUser> users = db.GetCollection<YukiUser>(collection);

                YukiUser user = users.Find(usr => usr.Id == userId).FirstOrDefault();

                if (user.Equals(default(YukiUser)))
                {
                    user = DefaultUser(userId);
                    AddOrUpdate(DefaultUser(userId));
                }

                user.IsPatron = false;

                users.Update(user);
            }
        }
    }
}
