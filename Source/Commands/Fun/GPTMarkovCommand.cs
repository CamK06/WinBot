#if BLOAT
// NOTE: The code in this file is hilariously shit, I'm just lazy and want this to work lol. Don't care about the quality.

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using WinBot.Misc;
using WinBot.Util;
using WinBot.Commands.Attributes;
using MarkovSharp.TokenisationStrategies;
namespace WinBot.Commands.Fun
{
    public class GPTMarkovCommand : BaseCommandModule
    {
        [Command("mkgpt")]
        [Description("GPT Markov chain")]
        [Usage("[length]")]
        [Category(Category.Fun)]
        public async Task GPTMarkov(CommandContext Context, int length = 5)
        {
            List<string> data = null;
            string sourceName = "Source: User-Submitted Messages";
            User AssGPT = UserData.GetOrCreateUser(await Bot.client.GetUserAsync(973361005000265778));
            User CursedGPT = UserData.GetOrCreateUser(await Bot.client.GetUserAsync(1085993398810460341));
            User NortGPT = UserData.GetOrCreateUser(await Bot.client.GetUserAsync(1085992824492798023));

            // Not sure if this is the best way to do this, it works and
            // the only other way I could think of is using for loops.
            data = AssGPT.messages
                .Concat(CursedGPT.messages)
                .Concat(NortGPT.messages).ToList();

            sourceName = "Source: GPT Hell";

            // Generate the markov text
            StringMarkov model = new StringMarkov(1);
            model.Learn(data);
            model.EnsureUniqueWalk = true;

            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithAuthor(sourceName);
            eb.WithColor(DiscordColor.Gold);
            eb.WithFooter(data.Count + " messages in data. Better results will be achieved with more messages.");
            eb.WithDescription(string.Join(' ', model.Walk(length)).Truncate(4096));
            await Context.ReplyAsync(eb);
        }
    
    }
}
#endif