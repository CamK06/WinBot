// We only use global usings for essentials, the rest are to be re-included in other files as needed
global using System;
global using System.Reflection;
global using System.Text.Json;
global using System.Threading.Tasks;
global using System.Text.Json.Serialization;

global using WinBot.Misc;
global using static WinBot.Helpers.Resources;
global using static WinBot.Helpers.JsonHelper;

global using DSharpPlus;
global using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using Serilog;
using Microsoft.Extensions.Logging;

namespace WinBot
{
    class Bot 
    {
        // Pass along execution in the entrypoint directly to asynchronous code
        public static void Main(string[] args) => new Bot().RunBotAsync().GetAwaiter().GetResult();
        public const string VERSION = "5.0.0 EarlyDev";

        public static DiscordClient client;
        public static CommandsNextExtension commands;
        public static BotConfig config;

        public async Task RunBotAsync()
        {
#if DEBUG
            if(!Directory.Exists("WorkingDir"))
                Directory.CreateDirectory("WorkingDir");
            Directory.SetCurrentDirectory("WorkingDir");
#endif

            // Set up logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LogSink()
                .CreateLogger();
            ILoggerFactory logFactory = new LoggerFactory().AddSerilog();
            Log.Information($"WinBot {VERSION}");
            Log.Information($"Starting bot...");

            // Load the bot config
            if(File.Exists("config.json"))
                config = LoadFromJson<BotConfig>("config.json");
            else {
                Log.Warning("No configuration file was found. A template config has been written to config.json");
                config = new BotConfig();
                WriteToJson("config.json", config);
                Environment.Exit(-1);
            }

            // Set up the Discord client+commands
            client = new DiscordClient(new DiscordConfiguration() {
                Token = config.token,
                TokenType = TokenType.Bot,
                LoggerFactory = logFactory,
                Intents = DiscordIntents.All
            });
            commands = client.UseCommandsNext(new CommandsNextConfiguration() {
                StringPrefixes = config.prefixes,
                EnableDefaultHelp = false,
                EnableDms = true,
                UseDefaultCommandHandler = true // TODO: CHANGE THIS ONCE CUSTOM HANDLER IS ADDED
            });
            commands.RegisterCommands(Assembly.GetExecutingAssembly());

            // Hook events then connect to Discord
            HookEvents();
            await client.ConnectAsync();
            await Task.Delay(-1);
        }

        public void HookEvents()
        {
            client.Ready += Ready;
        }

        async Task Ready(DiscordClient client, ReadyEventArgs e)
        {
            await client.UpdateStatusAsync(new DiscordActivity(config.status, config.activityType), UserStatus.DoNotDisturb);
        }
    }
}