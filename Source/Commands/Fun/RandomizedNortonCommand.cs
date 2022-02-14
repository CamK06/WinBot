using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class RandomizedNortonCommand : BaseCommandModule
    {
        [Command("randnorton")]
        [Description("Sends a random Norton.")]
        [Category(Category.Fun)]
        public async Task Hail(CommandContext Context)
        {
            await Context.ReplyAsync(norton[new Random().Next(0, norton.Count)]);
        }

        public List<string> norton = new List<string>()
        {
            "https://cdn.discordapp.com/attachments/829033969353097238/942804204853026817/Temp_76755-47974-emote.png",
            "https://cdn.discordapp.com/attachments/829033969353097238/942804220623593522/Temp_58352-89733-emote.png",
            "https://cdn.discordapp.com/attachments/829033969353097238/942804243264438272/Temp_43909-45184-emote.png",
            "https://cdn.discordapp.com/attachments/829033969353097238/942804283848532038/Temp_57744-14716-emote.png",
            "https://media.discordapp.net/attachments/829033969353097238/942806466316877864/emoji.png",
            "https://cdn.discordapp.com/attachments/829033969353097238/942806678406049792/emoji.png"
        };
    }
}