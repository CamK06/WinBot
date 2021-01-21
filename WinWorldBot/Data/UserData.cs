using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

using WinWorldBot.Utils;

using Newtonsoft.Json;

using Discord.WebSocket;

namespace WinWorldBot.Data
{
    class UserData
    {
        public static List<User> Users = new List<User>();
        private static BinaryFormatter formatter = new BinaryFormatter();

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
            if (File.Exists("UserData.bin"))
            {
                FileStream file = new FileStream("UserData.bin", FileMode.OpenOrCreate);
                Users = (List<User>)formatter.Deserialize(file);
                file.Close();
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
            FileStream file = new FileStream("UserData.bin", FileMode.OpenOrCreate);
            formatter.Serialize(file, Users);
            file.Close();
        }

        /* Outdated JSON code, this is left here just in case it's needed for some reason in the future
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
        */
    }
}