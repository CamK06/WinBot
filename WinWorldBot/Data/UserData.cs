using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using WinWorldBot.Utils;

using Newtonsoft.Json;

using Discord.WebSocket;

namespace WinWorldBot.Data
{
    class UserData
    {
        public static List<User> Users = new List<User>();

        public static User GetUser(SocketUser user)
        {
            User u = Users.FirstOrDefault(x => x.Id == user.Id);
            if (u == null) // Create a new user if they don't exist
            {
                u = new User()
                {
                    Username = user.Username,
                    Id = user.Id,
                    Messages = new List<UserMessage>(),
                    StartedLogging = DateTime.Now,
                    CorrectTrivia = 0,
                    IncorrectTrivia = 0
                };
                Users.Add(u);
                SaveData();
            }

            return u;
        }

        public static void LoadData()
        {
            if (File.Exists("UserData.json"))
            {
                string json = File.ReadAllText("UserData.json");
                Users = JsonConvert.DeserializeObject<List<User>>(json);
                Log.Write("Loaded user data");
            }
            else
            {
                Users = new List<User>();
                SaveData();
                Log.Write("Created user data file");
            }
        }

        public static void SaveData()
        {
            File.WriteAllText("UserData.json", JsonConvert.SerializeObject(Users, Formatting.Indented));
        }
    }
}