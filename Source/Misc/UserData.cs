using System.IO;
using System.Linq;
using System.Timers;
using System.Collections.Generic;

using DSharpPlus.Entities;

using Serilog;

using Newtonsoft.Json;

namespace WinBot.Misc
{
    public class UserData
    {
        static List<User> users;

        public static void Init()
        {
            // Load/create data
            if(File.Exists("userdata.json"))
                users = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("userdata.json"));
            else {
                users = new List<User>();
                File.WriteAllText("userdata.json", JsonConvert.SerializeObject(users, Formatting.Indented));
            }

            // Autosave
            Timer t = new Timer(300000);
            t.Elapsed += (e, s) => { 
                File.WriteAllText("userdata.json", JsonConvert.SerializeObject(users, Formatting.Indented));
            };
            t.AutoReset = true;
            t.Start();

            Log.Write(Serilog.Events.LogEventLevel.Information, "User data system initialized");
        }

        public static User GetOrCreateUser(DiscordUser user)
        {
            // This probably could and should be improved but oh well, it works.
            if(users.FirstOrDefault(x => x.id == user.Id) != null) {
                return users.FirstOrDefault(x => x.id == user.Id);
            }
            
            // Possible trouble point; adding to a list but returning original valie
            // Remove these comments if no issues arise
            User newUser = new User(user);
            users.Add(newUser);
            Log.Write(Serilog.Events.LogEventLevel.Information, $"Created data entry for user: {user.Username}#{user.Discriminator} ({user.Id})");
            return newUser;
        }
    }

    public class User
    {
        public User(DiscordUser user) {
            this.id = user.Id;
            this.username = user.Username;
            this.xp = 0;
            this.level = 1;
        }

        public ulong id { get; set; }
        public string username { get; set; }
        public float xp { get; set; }
        public int level { get; set; }
    }
}