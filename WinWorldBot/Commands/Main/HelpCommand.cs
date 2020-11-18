using System;
using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class HelpCommand : ModuleBase<SocketCommandContext>
    {
        [Command("help")]
        [Summary("Shows a list of commands or info about a specific command.|[Command]")]
        [Priority(Category.Main)]
        private async Task Help([Remainder]string command = null)
        {
            if(command == null)
            {
                EmbedBuilder HelpEmbed = new EmbedBuilder();

                // Embed setup
                HelpEmbed.WithTitle("WinWorld Bot Commands");
                HelpEmbed.WithFooter("Type \"~help [Command]\" to get more info on a command.");
                HelpEmbed.WithColor(Bot.config.embedColour);

                // Embed content
                HelpEmbed.AddField("**Main**", GetCommands(0), false);
                //HelpEmbed.AddField("**Fun**", GetCommands(Category.Fun), false);
                HelpEmbed.AddField("**Owner**", GetCommands(2), false);

                await ReplyAsync("", false, HelpEmbed.Build());
            }
            else
            {
                string usage = GetCommandUsage(command);
                if(usage != null)
                {
                    string UpperCommandName = command[0].ToString().ToUpper() + command.Remove(0, 1);

                    var eb = new EmbedBuilder();
                    eb.WithTitle($"{UpperCommandName} Command");
                    eb.WithColor(Bot.config.embedColour);
                    eb.WithDescription($"{usage}");
                    await ReplyAsync("", false, eb.Build());
                }
            }
        }


        /// <summary>
        /// Gets every command in a category in string form, so it can be put right into an embed.
        /// </summary>
        public static string GetCommands(int Priority)
        {
            string FinalString = "";
            for(int i = 0; i < Bot.commands.Commands.Count() / 2; i++) {
                CommandInfo Command = Bot.commands.Commands.ToArray()[i];
                if(Command.Summary != null && Command.Priority == Priority)
                {
                    if(!string.IsNullOrWhiteSpace(FinalString)) FinalString += $" | {Command.Name}";
                    else FinalString = $"{Command.Name}";
                }
            }
            return FinalString;
        }

        /// <summary>
        /// Gets the usage of a command
        /// </summary>
        public static string GetCommandUsage(string CommandName)
        {
            CommandInfo Info = Bot.commands.Commands.FirstOrDefault(x => x.Name.ToLower() == CommandName.ToLower());
            if (Info.Summary.Contains("|"))
            {
                string description = $"{Info.Summary.Split('|')[0]}\n\n**Usage:** ~{CommandName} {Info.Summary.Split('|')[1]}";
                return description;
            }
            else return null;
        }
    }
}