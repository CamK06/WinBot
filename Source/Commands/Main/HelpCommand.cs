using System.Linq;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class HelpCommand : BaseCommandModule
    {
        [Command("help")]
        [Description("Lists commands or gets info on a specific command")]
        [Usage("[command]")]
        [Category(Category.Main)]
        public async Task Help(CommandContext Context, [RemainingText] string command = null)
        {
            // Embed setup
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithFooter($"Type \"{Bot.config.prefix}help [command]\" for more info on a specific command");

            if(command == null) {
                
                // List all commands
                eb.WithTitle($"{Bot.client.CurrentUser.Username} Commands");
                eb.AddField("**Main**", GetCommands(Category.Main), false);
                eb.AddField("**Fun**", GetCommands(Category.Fun), false);
                eb.AddField("**Staff**", GetCommands(Category.Staff), false);
                eb.AddField("**Owner**", GetCommands(Category.Owner), false);
            }
            else {

                // Get the usage of a specific command
                string usage = GetCommandUsage(command);
                if (usage != null) {
                    string upperCommandName = command[0].ToString().ToUpper() + command.Remove(0, 1);
                    eb.WithTitle($"{upperCommandName} Command");
                    eb.WithDescription($"{usage}");
                }
                else {
                    await Context.ReplyAsync("That command doesn't seem to exist.");
                    return;
                }
            }

            await Context.ReplyAsync("", eb.Build());
        }

        static string GetCommands(Category searchCategory)
        {
            string finalString = "";
            foreach (Command command in Bot.commands.RegisteredCommands.Values) {

                // I fucking hate linq but I cba to come up with easier ways to do this stuff
                Category category = ((CategoryAttribute)command.CustomAttributes.FirstOrDefault(x => x.GetType() == typeof(CategoryAttribute))).category;
                if (category != searchCategory)
                    continue;

                // Add the command to the main text
                if (!string.IsNullOrWhiteSpace(finalString)) finalString += $" | `{command.Name}`";
                else finalString = $"`{command.Name}`";
            }

            if(finalString.Length <= 0)
                finalString = "*No commands found in category*";
            return finalString;
        }

        public static string GetCommandUsage(string commandName)
        {
            // Fetch the command and its usage attribute
            Command command = Bot.commands.FindCommand(commandName.ToLower(), out string args);
            if (command == null)
                return null;
            UsageAttribute usage = (UsageAttribute)command.CustomAttributes.FirstOrDefault(x => x.GetType() == typeof(UsageAttribute));
            
            // Create the usage string
            string desc = $"{command.Description}\n\n**Usage:** {Bot.config.prefix}{commandName}";
            if (usage != null)
                desc += $" {usage.Usage}";

            return desc;
        }
    }
}