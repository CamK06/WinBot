#if TOFU
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun.Tofu
{
    public class HailCommand : BaseCommandModule
    {
        [Command("hail")]
        [Description("Hail Tofu")]
        [Attributes.Category(Category.Fun)]
        public async Task Hail(CommandContext Context)
        {
            await Context.ReplyAsync(messages[new Random().Next(0, messages.Count)]);
        }

        public List<string> messages = new List<string>() {
            "May the blessings of bleating be upon you",
            "Follow the peaceful path of Tofu",
            "Where there was only one set of hoofprints, that's when Tofu was carrying you...",
            "Praise be"
        };
    }
}
#endif