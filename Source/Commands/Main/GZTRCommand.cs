using System;
using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class GZTRCommand : BaseCommandModule
    {
        [Command("gztr")]
        [Description("A")]
        [Category(Category.Main)]
        public async Task About(CommandContext Context, [RemainingText]string buckets)
        {
            string outW = "";
            foreach(string word in buckets.Split(' '))
                outW += "I'm a clown ";
            await Context.ReplyAsync(outW);
        }
    }
}
