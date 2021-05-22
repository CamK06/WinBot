using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.EventArgs;

using Serilog;

namespace WinBot.Misc
{
    public class Leveling
    {
        static Dictionary<ulong, DateTime> lastMessages = new Dictionary<ulong, DateTime>();

        public static void Init()
        {
            Bot.client.MessageCreated += MessageCreated;
            Log.Write(Serilog.Events.LogEventLevel.Information, "Leveling service started");
        }

        private static async Task MessageCreated(DiscordClient sender, MessageCreateEventArgs e)
        {
            // NOTE: This has the minor bug where if a user sends a message during one hour and sends the next message one hour later during the same minute
            // it will not add XP. This isn't really game-breaking so to speak though, so I don't think it's worth doing either an hour check or getting time between the DateTimes
            if((lastMessages.ContainsKey(e.Author.Id) && lastMessages[e.Author.Id].Minute != DateTime.Now.Minute) || !lastMessages.ContainsKey(e.Author.Id)) {
                lastMessages[e.Author.Id] = DateTime.Now;

                User user = UserData.GetOrCreateUser(e.Author);
                user.xp += new Random().Next(15, 25);
                // Level up
                if(user.xp >= ((user.level+1)*5)*40) {
                    user.level++;
                    user.xp = 0;
                }
            }
        }
    }
}