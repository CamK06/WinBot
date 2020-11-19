using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class ExecCommand : ModuleBase<SocketCommandContext>
    {
        [Command("exec"), Alias("ex")]
        [Summary("It's an exec command.|[Code]")]
        [Priority(Category.Owner)]
        private async Task Exec([Remainder]string command)
        {
            command = command.Replace("```cs", "");
            command = command.Replace("```", "");
            command = command.Replace("``", "");

            if(Context.Message.Author.Id == Globals.StarID) {
            var output = command.Bash();
            if(!string.IsNullOrWhiteSpace(output))
            {
		if(output.Length < 1024) { 
                	EmbedBuilder eb = new EmbedBuilder();
                	eb.WithTitle("Exec");
                	eb.WithColor(Bot.config.embedColour);
                	eb.WithCurrentTimestamp();
                	eb.AddField("Input", $"```sh\n{command}```");
                	eb.AddField("Output", $"```sh\n{output}```");
                	await ReplyAsync("", false, eb.Build());
		}
		else {
			await ReplyAsync($"**Input:** {command}\n\n\n\n**Output:**\n```\n{output}```");
		}
            }
            else{
                EmbedBuilder eb = new EmbedBuilder();
                eb.WithTitle("Exec");
                eb.WithColor(Color.Red);
                eb.WithCurrentTimestamp();
                eb.AddField("Input", $"```sh\n{command}```");
                eb.AddField("Output", $"```\nThe command had no output. This could be due to an error```");
                await ReplyAsync("", false, eb.Build());
            }
            }
        }
    }
}
