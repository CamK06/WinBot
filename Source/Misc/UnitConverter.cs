// A simple unit converter to assist those who are metrically impaired.

using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace WinBot.Misc
{
    public class UnitConverter
    {
        public static void Init()
        {
            Bot.client.MessageCreated += MessageCreated;
        }

        private static async Task MessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            DiscordMessage msg = e.Message;

            if(msg.Content.Contains("1F"))
                await e.Channel.SendMessageAsync($"I think {e.Author.Mention} meant to say: `{msg.Content.ConvertUnits()}`");
        }
    }

    public static class UnitStringConverter
    {
        public static string ConvertUnits(this string text)
        {
            

            return text;
        }
    }
}