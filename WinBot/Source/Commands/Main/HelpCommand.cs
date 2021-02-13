using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace WinBot.Commands.Main
{
	public class HelpCommand : ModuleBase<SocketCommandContext>
	{
		[Command("help")]
		[Priority(Category.Main)]
		public async Task Help()
		{
			// Embed setup
			EmbedBuilder helpEmbed = new EmbedBuilder();
			helpEmbed.WithTitle("WinBot Commands");
			helpEmbed.WithColor(Color.Gold);

			// Embed contents
			helpEmbed.AddField("**Main**", GetCommands(Category.Main), false);

			await ReplyAsync("", false, helpEmbed.Build());
		}

		static string GetCommands(int category)
		{
			string finalString = "";
			// Loop over every command
			for(int i = 0; i < Bot.commands.Commands.Count(); i++) 
			{
				CommandInfo command = Bot.commands.Commands.ToArray()[i];
				// If the command is in the category we're looking for
				if(command.Priority == category)
				{
					if(!string.IsNullOrWhiteSpace(finalString)) finalString += $" | `{command.Name}`";
					else finalString = $"`{command.Name}`";
				}
			}

			return finalString;
		}
	}
}