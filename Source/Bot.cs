using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;

using Newtonsoft.Json;

using DSharpPlus.Entities;

using Serilog;

using Microsoft.Extensions.Logging;

using WinBot.Util;

namespace WinBot
{
    class Bot
    {
        static void Main(string[] args) => new Bot().RunBot().GetAwaiter().GetResult();

        public static DiscordClient client;
        public static CommandsNextExtension commands;
        public static BotConfig config;
        public static DiscordChannel logChannel;

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
                LoggerFactory = logFactory
            });
            commands = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { "." },
                EnableDefaultHelp = false,
                EnableDms = true
            });

            // Events
            commands.CommandErrored += async (CommandsNextExtension cnext, CommandErrorEventArgs e) =>
            {
                await logChannel.SendMessageAsync($"**Command Execution Failed!**\n**Command:** `{e.Command.Name}`\n**Message:** `{e.Context.Message.Content}`\n**Exception:** `{e.Exception}`");

                await e.Context.RespondAsync("There was an error executing your command! Are you sure you used it correctly?");

                string usage = WinBot.Commands.Main.HelpCommand.GetCommandUsage(e.Command.Name);
                if (usage != null)
                {
                    string upperCommandName = e.Command.Name[0].ToString().ToUpper() + e.Command.Name.Remove(0, 1);
                    DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
                    eb.WithColor(DiscordColor.Gold);
                    eb.WithTitle($"{upperCommandName} Command");
                    eb.WithDescription($"{usage}");
                    await e.Context.RespondAsync("", eb.Build());
                }
            };
            client.Ready += async (DiscordClient client, ReadyEventArgs e) => {
                logChannel = await client.GetChannelAsync(config.logChannel);
                await logChannel.SendMessageAsync("Ready.");
            };

            // Commands
            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            // Connect
            await client.ConnectAsync();

            await Task.Delay(-1);
        }
    }

    class BotConfig
    {
        public string token { get; set; }
        public string status { get; set; }
        public ulong logChannel { get; set; }
    }
}
