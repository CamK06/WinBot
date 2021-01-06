using System.IO;
using System.Threading.Tasks;
using Discord.Commands;
using Discord;

namespace WinWorldBot.Commands
{
    public class YudsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("yuds")]
        private async Task Yuds()
        {
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);
            eb.WithTitle($"Yuds has asked {File.ReadAllText("?")} questions thus far.");

            await ReplyAsync("", false, eb.Build());
        }
    }
}