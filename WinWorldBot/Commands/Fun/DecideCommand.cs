using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class DecideCommand : ModuleBase<SocketCommandContext>
    {
        [Command("decide")]
        [Summary("Decide between up to four options|[Option 1] [Option 2] [Option 3] [Option 4]")]
        [Priority(Category.Fun)]
        private async Task Decide(string option1 = null, string option2 = null, string option3 = null, string option4 = null)
        {
            // nothing is inputted || not enough options
            if (option1 == null || option2 == null)
            {
                await ReplyAsync("You must provide at least two options!");
                return;
            }

            // Set up the embed
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);

        reRoll:
            Random r = new Random();
            int rand = r.Next(0, 3);

            if (rand == 0) eb.WithTitle($"🤔 I pick {option1}");
            else if (rand == 1) eb.WithTitle($"🤔 I pick {option2}");
            else if (rand == 2)
            {
                if (!string.IsNullOrWhiteSpace(option3)) eb.WithTitle($"🤔 I pick {option3}");
                else goto reRoll;
            }
            else if (rand == 3)
            {
                if (!string.IsNullOrWhiteSpace(option4)) eb.WithTitle($"🤔 I pick {option4}");
                else goto reRoll;
            }
            else goto reRoll;

            await ReplyAsync("", false, eb.Build());
        }
    }
}