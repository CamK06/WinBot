using System.Linq;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;

namespace WinBot.Commands
{
    public class CommandHandler
    {
        public static Task HandleMessage(DiscordClient client, MessageCreateEventArgs e)
        {
#if TOFU
            if(!e.Message.Author.IsBot) {
                if(e.Message.Content.ToLower().Contains("brett") || e.Message.Content.ToLower().Contains("bret")) {
                    e.Message.Channel.SendMessageAsync("Brent*");
                    e.Message.CreateReactionAsync(DiscordEmoji.FromGuildEmote(client, 838910961485742130));
                }
            }
#endif
            
            HandleCommand(e.Message, e.Author);
            return Task.CompletedTask;
        }

        public static async void HandleCommand(DiscordMessage msg, DiscordUser author)
        {
            if(Global.blacklistedUsers.Contains(author.Id) || author.IsBot)
                return;

            // Prefix check
            int start = msg.GetStringPrefixLength(Bot.config.prefix);
            if(start == -1) return;

            string prefix = msg.Content.Substring(0, start);
            string cmdString = msg.Content.Substring(start);

            // Multi-command check and execution
            if(cmdString.Contains(" && ")) {
                string[] commands = cmdString.Split(" && ");
                if(commands.Length > 2 && author.Id != Bot.client.CurrentApplication.Owners.FirstOrDefault().Id) return;
                for(int i = 0; i < commands.Length; i++) {
                    DoCommand(commands[i], prefix, msg);
                }
                return;
            }

            // Execute single command
            Command cmd = Bot.commands.FindCommand(cmdString, out var args);
            if(cmd == null) return;
            CommandContext ctx = Bot.commands.CreateContext(msg, prefix, cmd, args);
            await ctx.Channel.TriggerTypingAsync();
            await Task.Run(async () => await Bot.commands.ExecuteCommandAsync(ctx).ConfigureAwait(false));
        }

        private static void DoCommand(string commandString, string prefix, DiscordMessage msg) {
            Command cmd = Bot.commands.FindCommand(commandString, out var args);
            if(cmd == null) return;
            CommandContext ctx = Bot.commands.CreateFakeContext(msg.Author, msg.Channel, commandString, prefix, cmd, args);
            ctx.Channel.TriggerTypingAsync();
            _ = Task.Run(async () => await Bot.commands.ExecuteCommandAsync(ctx).ConfigureAwait(false));
        }

        public static async Task HandleError(CommandsNextExtension cnext, CommandErrorEventArgs e)
        {
            string msg = e.Exception.Message;
            if(msg == "One or more pre-execution checks failed.")
                msg += " This is likely a permissions issue.";
            if(msg.ToLower().Contains(": 413.")) {
                await e.Context.RespondAsync("https://http.cat/413");
                return;
            }
            
            await Global.logChannel.SendMessageAsync($"**Command Execution Failed!**\n**Command:** `{e.Command.Name}`\n**Message:** `{e.Context.Message.Content}`\n**Exception:** `{e.Exception}`");
            await e.Context.RespondAsync($"There was an error executing your command!\nMessage: `{msg}`");
        }
    }
}