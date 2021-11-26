using System;
using System.Threading.Tasks;
using System.Collections.Generic;

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
            "As if", "Ask me if I care.",
            "Dumb question, ask another.", "Forget about it.",
            "Get a clue.", "In your dreams.",
            "Fuck if I know.", "Not a chance.",
            "Obviously.", "Oh, please.",
            "That's ridiculous.", "Well maybe, idk.",
            "What do you think?", "Whatever.",
            "Who cares?", "Yeah, and I'm the king of England.",
            "succ.", "Yes? No? Maybe?",
            "They don't pay me enough to answer questions like this.", "Yes. Break out the champagne."
        };
    }
}
