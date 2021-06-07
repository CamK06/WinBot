using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;

using Newtonsoft.Json;

using Serilog;

using Microsoft.Extensions.Logging;

using WinBot.Util;
using WinBot.Misc;

namespace WinBot
{
    class Bot
    {
        public const string VERSION = "3.4.2";

        static void Main(string[] args) => new Bot().RunBot().GetAwaiter().GetResult();

        public static DiscordClient client;
        public static CommandsNextExtension commands;
        public static BotConfig config;
        public static DiscordChannel logChannel;
#if TOFU
        public static DiscordChannel welcomeChannel;
        public static DiscordChannel staffChannel;
#endif
        public static List<ulong> blacklistedUsers = new List<ulong>();

        public async Task RunBot()
        {
            // Change the working directory for debug mode
#if DEBUG
            if (!Directory.Exists("WorkingDirectory"))
                Directory.CreateDirectory("WorkingDirectory");
            Directory.SetCurrentDirectory("WorkingDirectory");
#endif

            // Verify directory structure
            if (!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");
            if(!Directory.Exists("Cache"))
                Directory.CreateDirectory("Cache");

            // Load blacklisted users
            if(!File.Exists("blacklist.json"))
                File.WriteAllText("blacklist.json", "[]");
            blacklistedUsers = JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText("blacklist.json"));

            // Load the config
            if (File.Exists("config.json"))
                config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("config.json"));
            else
            {
                // Create a blank config
                config = new BotConfig();
                config.token = "TOKEN";
                config.status = " ";

                // Write the config and quit
                File.WriteAllText("config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
                Console.WriteLine("No configuration file found. A template config has been written to config.json");
                return;
            }

            // Logger
            Log.Logger = new LoggerConfiguration()
                .WriteTo.DiscordSink()
                .CreateLogger();
            var logFactory = new LoggerFactory().AddSerilog();

            // Set up the client
            client = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.token,
                TokenType = TokenType.Bot,
                LoggerFactory = logFactory,
                Intents = DiscordIntents.All
            });
            commands = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { "." },
                EnableDefaultHelp = false,
                EnableDms = true,
                UseDefaultCommandHandler = false
            });

            // Events
            commands.CommandErrored += async (CommandsNextExtension cnext, CommandErrorEventArgs e) =>
            {
                await logChannel.SendMessageAsync($"**Command Execution Failed!**\n**Command:** `{e.Command.Name}`\n**Message:** `{e.Context.Message.Content}`\n**Exception:** `{e.Exception}`");
                await e.Context.RespondAsync($"There was an error executing your command!\nMessage: `{e.Exception.Message}`");

                // string usage = WinBot.Commands.Main.HelpCommand.GetCommandUsage(e.Command.Name);
                // if (usage != null)
                // {
                //     string upperCommandName = e.Command.Name[0].ToString().ToUpper() + e.Command.Name.Remove(0, 1);
                //     DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
                //     eb.WithColor(DiscordColor.Gold);
                //     eb.WithTitle($"{upperCommandName} Command");
                //     eb.WithDescription($"{usage}");
                //     await e.Context.RespondAsync("Are you sure you used it correctly?", eb.Build());
                // }
            };
            client.Ready += async (DiscordClient client, ReadyEventArgs e) => {
                logChannel = await client.GetChannelAsync(config.logChannel);
#if TOFU
                welcomeChannel = await client.GetChannelAsync(config.welcomeChannel);
                staffChannel = await client.GetChannelAsync(config.staffChannel);
#endif
                UserData.Init();
                DailyReportSystem.Init();
                Leveling.Init();
#if !TOFU
                await WWRSS.Init();
#endif
                await client.UpdateStatusAsync(new DiscordActivity() { Name = config.status });
                Log.Write(Serilog.Events.LogEventLevel.Information, $"Running on host: {MiscUtil.GetHost().Replace('\n', ' ')}");
                Log.Write(Serilog.Events.LogEventLevel.Information, "Ready");
            };
            // Edit logging
            client.MessageUpdated += async (DiscordClient client, MessageUpdateEventArgs e) => {
                if(e.MessageBefore.Content == e.Message.Content) // Just fixing Discords issues.... ffs
                    return;

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.WithColor(DiscordColor.Gold);
                builder.WithDescription($"**{e.Author.Username}#{e.Author.Discriminator}** updated a message in {e.Channel.Mention} \n" + Formatter.MaskedUrl("Jump to message!", e.Message.JumpLink));
                builder.AddField("Before", e.MessageBefore.Content, true);
                builder.AddField("After", e.Message.Content, true);
                builder.AddField("IDs", $"```cs\nUser = {e.Author.Id}\nMessage = {e.Message.Id}\nChannel = {e.Channel.Id}```");
                builder.WithTimestamp(DateTime.Now);
                await logChannel.SendMessageAsync("", builder.Build());
            };
            // Delete logging
            client.MessageDeleted += async (DiscordClient client, MessageDeleteEventArgs e) => {
                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.WithColor(DiscordColor.Gold);
                builder.WithDescription($"**{e.Message.Author.Username}#{e.Message.Author.Discriminator}**'s message in {e.Channel.Mention} was deleted");
                builder.AddField("Content", e.Message.Content, true);
                builder.AddField("IDs", $"```cs\nUser = {e.Message.Author.Id}\nMessage = {e.Message.Id}\nChannel = {e.Channel.Id}```");
                builder.WithTimestamp(DateTime.Now);
                await logChannel.SendMessageAsync("", builder.Build());
            };
            client.MessageCreated += CommandHandler;
#if TOFU
            client.GuildMemberAdded += async (DiscordClient client, GuildMemberAddEventArgs e) => {
                await welcomeChannel.SendMessageAsync($"Welcome, {e.Member.Mention} to Cerro Gordo! Be sure to read the <#774567486069800960> before chatting!");
            };
#endif
            
            // Commands
            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            // Connect
            await client.ConnectAsync();

            await Task.Delay(-1);
        }

        private async Task CommandHandler(DiscordClient client, MessageCreateEventArgs e)
        {
            DiscordMessage msg = e.Message;

            // DONT PING DUFF!
            if(msg.Content.Contains("283982771997638658")) {
                await msg.Channel.SendMessageAsync("https://tenor.com/view/gordon-ramsay-fuck-off-hells-kitchen-gif-5239890");
            }

            // IRC handling
            if(e.Author.IsBot && msg.Content.ToLower().Contains("/irc>")) {
                Log.Write(Serilog.Events.LogEventLevel.Information, "Reached IRC handler");
                string[] msgSplit = msg.Content.Split("/IRC>** ");

                Log.Write(Serilog.Events.LogEventLevel.Information, $"```\n{msg.Author}\n{msg.Content}```");
                Log.Write(Serilog.Events.LogEventLevel.Information, $"SPLIT:\n{msgSplit[0]}\n{msgSplit[1]}");

                // Prefix stuff
                int ircStart = msg.GetStringPrefixLength($"{msgSplit[1]}/IRC>** ");
                Log.Write(Serilog.Events.LogEventLevel.Information, $"{ircStart}");
                //if(ircStart == -1) return;
                string ircPrefix = msg.Content.Substring(0, ircStart);
                string ircCmdString = msg.Content.Substring(ircStart);

                Log.Write(Serilog.Events.LogEventLevel.Information, $"{msg.Author}\n{msg.Content}\n{ircPrefix}\n{ircCmdString}\n{msgSplit[1]}");

                await DoCommand(ircCmdString, ircPrefix, msg);
                return;
            }

            // Prefix check
            int start = msg.GetStringPrefixLength(".");
            if(start == -1) return;

            string prefix = msg.Content.Substring(0, start);
            string cmdString = msg.Content.Substring(start);

            // Multi-command check and execution
            if(cmdString.Contains(" && ")) {
                string[] commands = cmdString.Split(" && ");
                if(commands.Length > 2) return;
                for(int i = 0; i < commands.Length; i++) {
                    await DoCommand(commands[i], prefix, msg);
                    await Task.Delay(500);
                }
                return;
            }

            // Execute single command
            Command cmd = commands.FindCommand(cmdString, out var args);
            if(cmd == null) return;
            CommandContext ctx = commands.CreateContext(msg, prefix, cmd, args);
            await commands.ExecuteCommandAsync(ctx);
        }

        private async Task DoCommand(string commandString, string prefix, DiscordMessage msg) {
            Command cmd = commands.FindCommand(commandString, out var args);
            if(cmd == null) return;
            CommandContext ctx = commands.CreateFakeContext(msg.Author, msg.Channel, commandString, prefix, cmd, args);
            await commands.ExecuteCommandAsync(ctx);
        }
    }

    class BotConfig
    {
        public string token { get; set; }
        public string status { get; set; }
        public ulong logChannel { get; set; }
        public string weatherAPIKey { get; set; }
        public ulong ownerId { get; set; }
        public string catAPIKey { get; set; }
        public string wikihowAPIKey { get; set; }
#if !TOFU
        public ulong rssChannel { get; set; }
#else
        public ulong welcomeChannel { get; set; }
        public ulong staffChannel { get; set; }
#endif
    }
}
