// NOTE: The code in this file is hilariously shit, I'm just lazy and want this to work lol. Don't care about the quality.

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

using MarkovSharp.TokenisationStrategies;

namespace WinBot.Commands.Fun
{
    public class MarkovQuoteCommand : BaseCommandModule
    {
        [Command("lyrics")]
        [Description("Get markov chains lyrics")]
        [Usage("[artist] [lines]")]
        [Category(Category.Fun)]
        public async Task MarkovQuote(CommandContext Context, string input, int lines = 5)
        {
            DiscordEmbedBuilder eb;
            if(input.ToLower() == "list") {
                eb = new DiscordEmbedBuilder();
                eb.WithColor(DiscordColor.Gold);
                eb.WithDescription($"```\n{string.Join(' ', Directory.GetFiles("Resources/Lyrics")).Replace("Resources/Lyrics/", "").Replace(".txt", "")} all```");
                await Context.ReplyAsync(eb);
                return;
            }
            else if(!File.Exists($"Resources/Lyrics/{input.ToLower()}.txt") && input.ToLower() != "all")
                throw new Exception("Invalid artist! Run 'lyrics list' to get a list of available artists");
            else if(lines > 25)
                throw new Exception("You cannot have more than 25 lines!");

            // Get input text, this code sucks but I do not give a crap
            List<string> txt = new List<string>();
            DiscordMessage msg = null;
            if(input.ToLower() == "all") {
                txt.Clear();
                foreach(string file in Directory.GetFiles("Resources/Lyrics"))
                    foreach(string line in File.ReadAllLines(file))
                        txt.Add(line);
                msg = await Context.ReplyAsync("Training...\nThis will take a little while.");
                await Context.Channel.TriggerTypingAsync();
            }
            else 
                File.ReadAllLines($"Resources/Lyrics/{input.ToLower()}.txt").ToList();

            // Generate the markov text
            StringMarkov model = new StringMarkov(1);
            model.Learn(txt);
            model.EnsureUniqueWalk = true;

            // Create and send lyric embed
            eb = new DiscordEmbedBuilder();
            eb.WithTitle($"Generated `{input}` Lyrics");
            eb.WithColor(DiscordColor.Gold);
            eb.WithDescription($"```\n{string.Join('\n', model.Walk(lines)).Truncate(4096)}```");
            eb.WithFooter($"Trained on {txt.Count} lines of lyrical hell");
            await Context.ReplyAsync(eb);
            if(msg != null)
                await msg.DeleteAsync();
        }
    }
}