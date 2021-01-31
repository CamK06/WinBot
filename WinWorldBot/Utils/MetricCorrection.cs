using System.Threading.Tasks;
using System.Collections.Generic;

using Discord.WebSocket;

namespace WinWorldBot.Utils
{
    internal class MetricCorrection
    {
        private static Dictionary<string, string> units = new Dictionary<string, string>()
        {
            { "feet", "meters" }, { "foot", "meter(s)" },
            { "miles", "kilometers" },{ "mile", "kilometer(s)" },
            { "inch", "centimeter(s)" }, { "inches", "centimeters" }
        };

        public static void Init()
        {
            Bot.client.MessageReceived += MessageReceived;
        }

        private async static Task MessageReceived(SocketMessage msg)
        {
            if(msg.Author.IsBot || Bot.blacklistedUsers.Contains(msg.Author.Id)) return;
            string[] words = msg.Content.ToLower().Split(' ');
            
            for(int i = 0; i < words.Length; i++)
            {
                if(units.ContainsKey(words[i]))
                {
                    float realVal = ConvertToRealUnits(words[i-1], words[i]);
                    if(realVal == -65021) continue;
                    await msg.Channel.SendMessageAsync($"I think {msg.Author.Mention} meant to say: ``{realVal} {units[words[i]]}``");
                }
            }
        }

        private static float ConvertToRealUnits(string number, string unit)
        {
            // Shitty way of implementing a fail condition :P
            if(!float.TryParse(number, out float originalVal)) {
                return -65021;
            }

            // Do conversions and shit
            switch(unit)
            {
                case "feet":
                case "foot":
                    return originalVal / 3.281f;

                case "miles":
                case "mile":
                    return originalVal * 1.609f;

                case "inch":
                case "inches":
                    return originalVal * 2.54f;
            }
            
            return -65021;
        }
    }
}