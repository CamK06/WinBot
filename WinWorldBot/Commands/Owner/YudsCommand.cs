using System.IO;
using System.Threading.Tasks;
using WinWorldBot.Utils;
using Discord.Commands;
using Discord;

namespace WinWorldBot.Commands
{
    public class YudsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("yuds")]
        private async Task Yuds()
        {
            if(Context.Message.Author.Id == 363850072309497876) await Context.Message.DeleteAsync();

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);

            eb.WithTitle($"Yuds has asked {File.ReadAllText("?")} questions thus far.");

            await ReplyAsync("", false, eb.Build());
        }

        [Command("yudstest")]
        private async Task YudsTest([Remainder]string input)
        {
            //if(Context.Message.Author.Id == 363850072309497876) await Context.Message.DeleteAsync();

            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);

            if(YudsCounter.IsQuestion(input))
                eb.WithTitle($"I believe this IS in fact a question.");
            else
                eb.WithTitle("I believe this is not a question.");

            // eb.WithTitle($"Yuds has asked {File.ReadAllText("?")} questions thus far.");

            await ReplyAsync("", false, eb.Build());
        }
    }
}