using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Newtonsoft.Json;

using Serilog;

using static WinBot.Util.ResourceManager;

namespace WinBot.Misc
{
    public class Leveling
    {
        static Dictionary<ulong, DateTime> lastMessages = new Dictionary<ulong, DateTime>();
        static Dictionary<int, ulong> levelRoles = new Dictionary<int, ulong>();
        static string levelRolesJson;

        public static void Init()
        {
            levelRolesJson = GetResourcePath("levelRoles", Util.ResourceType.JsonData);

            Bot.client.MessageCreated += MessageCreated;

            if(File.Exists(levelRolesJson))
                levelRoles = JsonConvert.DeserializeObject<Dictionary<int, ulong>>(File.ReadAllText(levelRolesJson));

            Log.Write(Serilog.Events.LogEventLevel.Information, "Leveling service started");
        }

        private static async Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            if(e.Author.IsBot || e.Channel.IsPrivate || e.Message.Content.StartsWith(Bot.config.prefix))
                return;

            // Message logging stuff
            User user = UserData.GetOrCreateUser(e.Author);
            if(user.messages == null)
                user.messages = new List<string>();
            if(!user.optedOutOfMessages && !string.IsNullOrWhiteSpace(e.Message.Content))
                user.messages.Add(e.Message.Content);

            // NOTE: This has the minor bug where if a user sends a message during one hour and sends the next message one hour later during the same minute
            // it will not add XP. This isn't really game-breaking so to speak though, so I don't think it's worth doing either an hour check or getting time between the DateTimes
            if((lastMessages.ContainsKey(e.Author.Id) && lastMessages[e.Author.Id].Minute != DateTime.Now.Minute) || !lastMessages.ContainsKey(e.Author.Id)) {
                lastMessages[e.Author.Id] = DateTime.Now;

                int inc = new Random().Next(15, 25);
                user.xp += inc*3;
                user.totalxp += inc*3;
                // Level up
                if(user.xp >= ((user.level+1)*5)*40) {
                    user.level++;
                    user.xp = 0;
                    if(user.levelMessages)
                        await e.Guild.GetMemberAsync(user.id).Result.SendMessageAsync($"You've just advanced to level {user.level}!");
                }

                // Check for level roles
                foreach(var lvlRole in levelRoles)
                    if(user.level >= lvlRole.Key) {
                        // Check for the role
                        DiscordRole role = e.Channel.Guild.GetRole(lvlRole.Value);
                        if(e.Channel.Guild.GetMemberAsync(user.id).Result.Roles.Contains(role) || role == null)
                            continue;

                        // Add the role
                        try {
                            await e.Channel.Guild.GetMemberAsync(user.id).Result.GrantRoleAsync(role, "Level up");
                        } catch {}
                    }
            }
        }

        public static void AddRole(ulong id, int level)
        {
            levelRoles.Add(level, id);
            File.WriteAllText(levelRolesJson, JsonConvert.SerializeObject(levelRoles, Formatting.Indented));
        }

        // Just an abstraction to linq because nobody likes doing linq stuff over and over
        public static List<User> GetOrderedLeaderboard()
        {
            return UserData.users.OrderByDescending(x => x.totalxp).ToList();
        }
    }
}
