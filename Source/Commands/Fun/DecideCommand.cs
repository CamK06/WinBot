using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class DecideCommand : BaseCommandModule
    {
        [Command("decide")]
        [Description("Have the bot decide something for you because you're too dumb to make up your own mind on things, so you rely on bad RNG")]
        [Usage("['|' separated options]")]
        [Category(Category.Fun)]
        public async Task Decide(CommandContext Context, [RemainingText]string rawOptions)
        {
            string[] options = rawOptions.Split("|");
            if(options.Length < 2)
                throw new Exception("You must provide at least two options!");
            string choice = options[new Random().Next(0, options.Length)];


            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithTitle($"🤔 I pick {choice}".Truncate(256));
            await Context.ReplyAsync("", eb.Build());
        }
    }
}