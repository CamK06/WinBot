#if TOFU
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun.Tofu
{
    public class BrentCommand : BaseCommandModule
    {
        [Command("brent")]
        [Description("Brent.")]
        [Category(Category.Fun)]
        public async Task Hail(CommandContext Context)
        {
            await Context.ReplyAsync(messages[new Random().Next(0, messages.Count)]);
        }

        public List<string> messages = new List<string>() {
            "https://cdn.discordapp.com/emojis/843244671923781732.png",
            "https://cdn.discordapp.com/emojis/846125412244258897.png",
            "https://cdn.discordapp.com/emojis/820054937945767986.png",
            "https://cdn.discordapp.com/emojis/846124954210402314.png"
        };
    }
}
#endif