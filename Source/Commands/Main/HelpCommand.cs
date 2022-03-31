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
        [Command("halp")]
        [Description("Lists commands or gets info on a specific command")]
        [Usage("[command]")]
        [Category(Category.Main)]
        public async Task Help(CommandContext Context, [RemainingText] string command = null)
        {
            // Embed setup
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithFooter($"Type \"{Bot.config.prefix}halp [command]\" 4 mor infs on da comnd");

            if(command == null) {
                
                // List all commands
                eb.WithTitle($"{Bot.client.CurrentUser.Username} comens");
                eb.AddField("**mani**", GetCommands(Category.Main), false);
                eb.AddField("**fon**", GetCommands(Category.Fun), false);
                eb.AddField("**img manipelatin**", GetCommands(Category.Images), false);
                eb.AddField("**neds hit**", GetCommands(Category.NerdStuff), false);
                eb.AddField("**stfawaf**", GetCommands(Category.Staff), false);
                eb.AddField("**idot**", GetCommands(Category.Owner), false);
            }
            else {

                // Get the usage of a specific command
                string usage = GetCommandUsage(command);
                if (usage != null) {
                    string upperCommandName = command[0].ToString().ToUpper() + command.Remove(0, 1);
                    eb.WithTitle($"{upperCommandName} cmod");
                    eb.WithDescription($"{usage}");
                }
                else {
                    await Context.ReplyAsync("dat cmd noting of exist");
                    return;
                }
            }

            await Context.ReplyAsync("", eb.Build());
        }

        static string GetCommands(Category searchCategory)
        {
            string finalString = "";
            foreach (Command command in Bot.commands.RegisteredCommands.Values) {

                if(command.IsHidden)
                    continue;

                // I fucking hate linq but I cba to come up with easier ways to do this stuff
                Category category = ((CategoryAttribute)command.CustomAttributes.FirstOrDefault(x => x.GetType() == typeof(CategoryAttribute))).category;
                if (category != searchCategory)
                    continue;

                // Add the command to the main text
                if(finalString.Contains($"{command.Name}")) 
                    continue;
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
            string desc = $"{command.Description}\n\n**uagege:** {Bot.config.prefix}{commandName}";
            if (usage != null)
                desc += $" {usage.Usage}";

            return desc;
        }
    }
}