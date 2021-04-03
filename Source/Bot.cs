using System;
using System.IO;
using System.Threading.Tasks;
using System.Reflection;

using DSharpPlus;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;

using Newtonsoft.Json;

using WinBot.Commands;
using DSharpPlus.CommandsNext.Builders;
using DSharpPlus.Entities;

using Serilog;
using Serilog.Extensions;
using Serilog.Configuration;

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
            commands.CommandErrored += (CommandsNextExtension cnext, CommandErrorEventArgs e) =>
            {
                e.Context.RespondAsync("There was an error executing your command! Are you sure you used it correctly?");

                string usage = WinBot.Commands.HelpCommand.GetCommandUsage(e.Command.Name);
                if (usage != null)
                {
                    string upperCommandName = e.Command.Name[0].ToString().ToUpper() + e.Command.Name.Remove(0, 1);
                    DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
                    eb.WithColor(DiscordColor.Gold);
                    eb.WithTitle($"{upperCommandName} Command");
                    eb.WithDescription($"{usage}");
                    e.Context.RespondAsync("", eb.Build());
                }
                return Task.CompletedTask;
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
    }
}
