using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;

using Newtonsoft.Json;

using Serilog;
using Microsoft.Extensions.Logging;

using WinBot.Util;
using WinBot.Commands;
using static WinBot.Util.ResourceManager;

namespace WinBot
{
    class Bot
    {
        public const string VERSION = "4.0.0-Dev";

        public static void Main(string[] args) => new Bot().RunBot().GetAwaiter().GetResult();

        // DSharpPlus
        public static DiscordClient client;
        public static CommandsNextExtension commands;

        // Bot
        public static BotConfig config;

        public async Task RunBot()
        {
            // Stop it.
            // DO NOT. DONT DO IT.
            // DONT RUN IT ON THAT ACCURSED OS!
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                return;

            // Change workingdir in debug mode
#if DEBUG
            if (!Directory.Exists("WorkingDirectory"))
                Directory.CreateDirectory("WorkingDirectory");
            Directory.SetCurrentDirectory("WorkingDirectory");
#endif

            // Logging
            Log.Logger = new LoggerConfiguration()
                .WriteTo.DiscordSink()
                .CreateLogger();
            ILoggerFactory logFactory = new LoggerFactory().AddSerilog();
            Log.Information($"WinBot {VERSION}");
            Log.Information($"Starting bot...");

            VerifyIntegrity();
            LoadConfigs();
            
            // Set up the Discord client
            client = new DiscordClient(new DiscordConfiguration()
            {
                Token = config.token,
                TokenType = TokenType.Bot,
                LoggerFactory = logFactory,
                Intents = DiscordIntents.All
            });
            commands = client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { config.prefix },
                EnableDefaultHelp = false,
                EnableDms = true,
                UseDefaultCommandHandler = false
            });
            HookEvents();
            commands.RegisterCommands(Assembly.GetExecutingAssembly());
            await client.ConnectAsync();

            await Task.Delay(-1);
        }

        async Task Ready(DiscordClient client, ReadyEventArgs e)
        {
            // Set guilds
            Global.hostGuild = await client.GetGuildAsync(config.ids.hostGuild);
            Global.targetGuild = await client.GetGuildAsync(config.ids.targetGuild);

            // Set channels
            if(config.ids.logChannel != 0)
                Global.logChannel = await client.GetChannelAsync(config.ids.logChannel);
            if(Global.logChannel == null)
                Log.Error("Shitcord is failing to return a valid log channel or no channel ID is set in the config");

            // Set misc stuff

            await client.UpdateStatusAsync(new DiscordActivity() { Name = config.status });
            Log.Information("Ready");
        }

        void HookEvents()
        {
            // Bot
            client.Ready += Ready;
            client.MessageCreated += CommandHandler.HandleCommand;

            // Commands
            commands.CommandErrored += CommandHandler.HandleError;
        }

        void VerifyIntegrity()
        {
            Log.Write(Serilog.Events.LogEventLevel.Information, "Verifying integrity of bot files...");

            // Verify directories
            if(!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");
            if(!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            if(!Directory.Exists("Images"))
                Directory.CreateDirectory("Images");
            if(!Directory.Exists("Temp"))
                Directory.CreateDirectory("Temp");

            // Verify configs & similar files
            if(!ResourceExists("config", ResourceType.Config)) {

                // Create a blank config
                config = new BotConfig();
                config.token = "TOKEN";
                config.status = " ";
                config.prefix = ".";
                config.ids = new IDConfig();

                // Write the config and quit
                File.WriteAllText(GetResourcePath("config", ResourceType.Config), JsonConvert.SerializeObject(config, Formatting.Indented));
                Log.Fatal("No configuration file found. A template config has been written to config.json");
                Environment.Exit(-1);
            }
            if(!ResourceExists("blacklist", ResourceType.JsonData))
                File.WriteAllText(GetResourcePath("blacklist", ResourceType.JsonData), "[]");
#if TOFU
            if(!ResourceExists("mute", ResourceType.JsonData))
                File.WriteAllText(GetResourcePath("mute", ResourceType.JsonData), "[]");
#endif

            // TODO: Add resource verification
        }

        void LoadConfigs()
        {
            // Main bot config
            config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(GetResourcePath("config", ResourceType.Config)));
            if(config == null) {
                Log.Fatal("Failed to load configuration!");
                Environment.Exit(-1);
            }
            Global.blacklistedUsers = JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText(GetResourcePath("blacklist", ResourceType.JsonData)));
#if TOFU
            Global.mutedUsers = JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText(GetResourcePath("mute", ResourceType.JsonData)));
#endif
        }
    }

    class BotConfig
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public string status { get; set; }
        public IDConfig ids { get; set; }
    }
    
    class IDConfig
    {
        public ulong hostGuild { get; set; } = 0;   // Where logs etc are
        public ulong targetGuild { get; set; } = 0; // Where muted role etc are
        public ulong logChannel { get; set; } = 0;
        public ulong mutedRole { get; set; } = 0;
    }

    class Global
    {
        public static DiscordGuild hostGuild;
        public static DiscordGuild targetGuild;
        public static DiscordChannel logChannel = null;
        
        // Moderation
        public static List<ulong> blacklistedUsers = new List<ulong>();
#if TOFU
        public static List<ulong> mutedUsers = new List<ulong>();
        public static DiscordRole mutedRole;
#endif
    }
}