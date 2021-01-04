using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class EightBallCommand : ModuleBase<SocketCommandContext>
    {
        [Command("8ball")]
        [Summary("Ask a magic 8 ball something|[Question]")]
        [Priority(Category.Fun)]
        private async Task EightBall([Remainder] string question = null)
        {
            if(question == null) {
                await ReplyAsync("You must provide a question!");
                return;
            }

            // Pick an option
            Random r = new Random();
            string option = options[r.Next(0, options.Count)];

            // Create the embed
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle($"**{option}**");
            eb.WithColor(Bot.config.embedColour);

            // Send the embed
            await ReplyAsync("", false, eb.Build());
        }

        List<string> options = new List<string>()
        {
            "As I see it, yes.", "Ask again later.",
            "Better not tell you now.", "Cannot predict now.",
            "Concentrate and ask again.", "Don't count on it.",
            "It is certain.", "It is decidedly so.",
            "Most likely.", "My reply is no.",
            "My sources say no.", "Outlook not so good.",
            "Outlook good.", "Reply hazy, try again.",
            "Signs point to yes.", "Very doubtful.",
            "Without a doubt.", "Yes.",
            "Yes - definitely", "You may rely on it."
        };
    }
}