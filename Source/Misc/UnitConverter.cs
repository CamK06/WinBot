// A simple unit converter to assist those who are metrically impaired.
/*
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Serilog;
using Serilog.Events;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Text.RegularExpressions;

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

            if(msg.Content.IsMetricallyImpaired())
                await e.Channel.SendMessageAsync($"I think {e.Author.Mention} meant to say: `{msg.Content.ConvertUnits()}`");
        }
    }

    public static class UnitStringConverter
    {
        public static bool IsMetricallyImpaired(this string text)
        {
            // Check for imperial units in the string
            string[] words = text.Split(' ');
            foreach(string word in words) {
                
            }

            return true;
        }

        public static string ConvertUnits(this string text)
        {
            

            return text;
        }
    }

    public enum Units
    {
        Mile, Fahrenheit, Feet,
        Yard, Pound, Ounce,
        Gallon, Inch, Acre
    }
}*/