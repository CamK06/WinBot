using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;

using Serilog;

// TODO: Re-do command handler(?)

namespace WinBot.Commands
{
    public class CommandHandler
    {
        public static async Task HandleCommand(DiscordClient client, MessageCreateEventArgs e)
        {
            DiscordMessage msg = e.Message;
            
            if(Bot.blacklistedUsers.Contains(msg.Author.Id) || e.Author.IsBot)
                return;

#if !DEBUG
            if(mutedRole == null)
                mutedRole = client.GetGuildAsync(774566729379741706).Result.GetRole(874370290900140084); // TODO: make role and guild ID not hardcoded
#endif
#if TOFU
            if(!msg.Author.IsBot) {
                if(e.Message.Content.ToLower().Contains("brett") || e.Message.Content.ToLower().Contains("bret")) {
                    await msg.Channel.SendMessageAsync("Brent*");
                    await msg.CreateReactionAsync(DiscordEmoji.FromGuildEmote(client, 838910961485742130));
                }
            }
#endif
            // Prefix check
            int start = msg.GetStringPrefixLength(Bot.config.prefix);
            if(start == -1) return;

            string prefix = msg.Content.Substring(0, start);
            string cmdString = msg.Content.Substring(start);

            // Multi-command check and execution
            if(cmdString.Contains(" && ")) {
                string[] commands = cmdString.Split(" && ");
                if(commands.Length > 2 && e.Author.Id != client.CurrentApplication.Owners.FirstOrDefault().Id) return;
                for(int i = 0; i < commands.Length; i++) {
                    DoCommand(commands[i], prefix, msg);
                }
                return;
            }

            // Execute single command
            Command cmd = Bot.commands.FindCommand(cmdString, out var args);
            if(cmd == null) return;
            CommandContext ctx = Bot.commands.CreateContext(msg, prefix, cmd, args);
            await Task.Run(async () => await Bot.commands.ExecuteCommandAsync(ctx).ConfigureAwait(false));
        }

        private static void DoCommand(string commandString, string prefix, DiscordMessage msg) {
            Command cmd = Bot.commands.FindCommand(commandString, out var args);
            if(cmd == null) return;
            CommandContext ctx = Bot.commands.CreateFakeContext(msg.Author, msg.Channel, commandString, prefix, cmd, args);
            _ = Task.Run(async () => await Bot.commands.ExecuteCommandAsync(ctx).ConfigureAwait(false));
        }

        public static async Task HandleError(CommandsNextExtension cnext, CommandErrorEventArgs e)
        {
            string msg = e.Exception.Message;
            if(msg == "One or more pre-execution checks failed.")
                msg += " This is likely a permissions issue.";
            
            await Bot.logChannel.SendMessageAsync($"**Command Execution Failed!**\n**Command:** `{e.Command.Name}`\n**Message:** `{e.Context.Message.Content}`\n**Exception:** `{e.Exception}`");
            await e.Context.ReplyAsync($"There was an error executing your command!\nMessage: `{msg}`");
        }
    }
}