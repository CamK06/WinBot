using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace WinBot.Commands.Main
{
	public class HelpCommand : ModuleBase<SocketCommandContext>
	{
		[Command("help")]
		[Summary("Get a list of bot commands and their usage|[Command]")]
		[Priority(Category.Main)]
		public async Task Help([Remainder] string command = null)
		{
			EmbedBuilder helpEmbed = new EmbedBuilder();
			helpEmbed.WithColor(Color.Gold);
			helpEmbed.WithFooter("Type \".help [command]\" to get more info on a command");

			if (command == null)
			{
				helpEmbed.WithTitle("WinBot Commands");
				helpEmbed.AddField("**Main**", GetCommands(Category.Main), false);
				helpEmbed.AddField("**Fun**", GetCommands(Category.Fun), false);
				helpEmbed.AddField("**Owner**", GetCommands(Category.Owner), false);
			}
			else
			{
				string usage = GetCommandUsage(command);
				if(usage != null)
				{
					string upperCommandName = command[0].ToString().ToUpper() + command.Remove(0, 1);
					helpEmbed.WithTitle($"{upperCommandName} Command");
					helpEmbed.WithDescription($"{usage}");
				}
				else
				{
					await ReplyAsync("That command doesn't seem to exist.");
					return;
				}
			}

			await ReplyAsync("", false, helpEmbed.Build());
		}

		static string GetCommands(int category)
		{
			string finalString = "";
			// Loop over every command
			for (int i = 0; i < Bot.commands.Commands.Count(); i++)
			{
				CommandInfo command = Bot.commands.Commands.ToArray()[i];
				// If the command is in the category we're looking for
				if (command.Priority == category)
				{
					if (!string.IsNullOrWhiteSpace(finalString)) finalString += $" | `{command.Name}`";
					else finalString = $"`{command.Name}`";
				}
			}

			return finalString;
		}

		public static string GetCommandUsage(string commandName)
		{
			CommandInfo command = Bot.commands.Commands.FirstOrDefault(x => x.Name.ToLower() == commandName.ToLower());
			if (command != null && command.Summary.Contains("|"))
			{
				string desc = $"{command.Summary.Split('|')[0]}\n\n**Usage:** .{commandName} {command.Summary.Split('|')[1]}";
				return desc;
			}
			return null;
		}
	}
}