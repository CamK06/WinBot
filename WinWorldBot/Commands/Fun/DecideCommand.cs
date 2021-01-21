using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class DecideCommand : ModuleBase<SocketCommandContext>
    {
        [Command("decide")]
        [Summary("Decide between multiple options|[Options]")]
        [Priority(Category.Fun)]
        private async Task Decide([Remainder]string options)
        {
            // Ensure there is one or more options
            if(!options.Contains("|")) {
                await ReplyAsync("You must provide at least two options separated with ``|``");
                return;
            }

            // Pick an option
            string[] splitOptions = options.Split("|", 25);
            Random r = new Random();
            int index = r.Next(0, splitOptions.Count());

            if(Context.Message.Author.Id == 469275318079848459)
                index = 0;

            // Create and send the embed
            EmbedBuilder eb = new EmbedBuilder();
            if(!splitOptions[index].StartsWith(" ")) eb.WithTitle($"🤔 I pick {splitOptions[index]}");
            else eb.WithTitle($"🤔 I pick{splitOptions[index]}");
            eb.WithColor(Bot.config.embedColour);
            await ReplyAsync("", false ,eb.Build());
        }
    }
}