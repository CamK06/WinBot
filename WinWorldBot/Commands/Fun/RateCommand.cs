using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class RateCommand : ModuleBase<SocketCommandContext>
    {
        [Command("rate")]
        [Summary("Rate something|[Thing to rate]")]
        [Priority(Category.Fun)]
        private async Task Rate([Remainder]string option)
        {
            Random r = new Random();
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);
            eb.WithTitle($"🤔 I give **{option}** a solid {r.Next(1,10)}/10");
            await ReplyAsync("", false, eb.Build());
        }
    }
}