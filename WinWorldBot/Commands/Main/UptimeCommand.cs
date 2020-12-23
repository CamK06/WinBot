using System;
using System.Threading.Tasks;

using Discord.WebSocket;
using Discord;
using Discord.Rest;
using Discord.Net;
using Discord.Commands;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class UptimeCommand : ModuleBase<SocketCommandContext>
    {
        [Command("uptime")]
        [Summary("Shows the uptime of the bot|")]
        [Priority(Category.Main)]
        private async Task Uptime()
        {
            TimeSpan elapsed = DateTime.Now.Subtract(Bot.startTime);

            string days = "", hours = "", minutes = "", seconds = "";
            if(elapsed.Days > 0) days = $"{elapsed.Days} days, ";
            if(elapsed.Hours > 0) hours = $"{elapsed.Hours} hours, ";
            if(elapsed.Minutes > 0) minutes = $"{elapsed.Minutes} minutes, ";
            if(elapsed.Seconds > 0) seconds = $"{elapsed.Seconds} seconds";
            await ReplyAsync($"The bot has been online for {days}{hours}{minutes}{seconds}");
        }
    }
}