using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.IO.Compression;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

using Newtonsoft.Json;

using Serilog;
using Microsoft.Extensions.Logging;

using WinBot.Util;
using WinBot.Misc;
using WinBot.Commands;
using static WinBot.Util.ResourceManager;

using ImageMagick;

namespace WinBot
{
    class Bot
    {
        public const string VERSION = "4.0.3";

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
            client.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = DSharpPlus.Interactivity.Enums.PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(60)
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
#if TOFU
            if(config.ids.welcomeChannel != 0)
                Global.welcomeChannel = await client.GetChannelAsync(config.ids.welcomeChannel);
            if(Global.welcomeChannel == null)
                Log.Error("Shitcord is failing to return a valid welcome channel or no channel ID is set in the config");
#endif
            // Set misc stuff

            // Start misc systems
            UserData.Init();
            Leveling.Init();
            TempManager.Init();
            DailyReportSystem.Init();
            MagickNET.Initialize(); 
#if !TOFU
            if(Bot.config.ids.rssChannel != 0)
                await WWRSS.Init();
#endif

            await client.UpdateStatusAsync(new DiscordActivity() { Name = config.status });
            Log.Information("Ready");
        }

        void HookEvents()
        {
            // Bot
            client.Ready += Ready;
            client.MessageCreated += CommandHandler.HandleMessage;
            client.MessageUpdated += (DiscordClient client, MessageUpdateEventArgs e) => {
                if(DateTime.Now.Subtract(e.Message.Timestamp.DateTime).TotalMinutes < 1 && DateTime.Now.Subtract(e.Message.Timestamp.DateTime).TotalSeconds > 2)
                    CommandHandler.HandleCommand(e.Message, e.Author);
                return Task.CompletedTask;
            };
#if TOFU
            client.GuildMemberAdded += async (DiscordClient client, GuildMemberAddEventArgs e) => {
                if(!Global.mutedUsers.Contains(e.Member.Id))
                    await Global.welcomeChannel.SendMessageAsync($"Welcome, {e.Member.Mention} to Cerro Gordo! Be sure to read the <#774567486069800960> before chatting!");
                else {
                    await Global.welcomeChannel.SendMessageAsync($"Welcome, {e.Member.Mention} to Cerro Gordo! Unfortunately it seems as if you have failed to read <#774567486069800960>, have fun in the hole!");
                    await e.Member.GrantRoleAsync(Global.mutedRole, "succ");
                }
            };
#endif
            // Commands
            commands.CommandErrored += CommandHandler.HandleError;

            EventLogging.Init();
        }

        void VerifyIntegrity()
        {
            Log.Write(Serilog.Events.LogEventLevel.Information, "Verifying integrity of bot files...");

            // Verify directories
            if(!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");
            if(!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            if(!Directory.Exists("Resources"))
                Directory.CreateDirectory("Resources");
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
                config.apiKeys = new APIConfig();

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

            // Verify and download resources
            Log.Information("Verifying resources...");
            WebClient webClient = new WebClient();
            string resourcesJson = webClient.DownloadString("https://raw.githubusercontent.com/CamK06/WinBot/main/Resources/resources.json");
            string[] resources = JsonConvert.DeserializeObject<string[]>(resourcesJson);
            foreach(string resource in resources) {
                if(!ResourceExists(resource, ResourceType.Resource)) {
                    webClient.DownloadFile($"https://raw.githubusercontent.com/CamK06/WinBot/main/Resources/{resource}", GetResourcePath(resource, ResourceType.Resource));
                    Log.Information("Downloaded " + resource + "");
                }
            }

            if(!Directory.Exists("Resources/Lyrics"))
                ZipFile.ExtractToDirectory("Resources/Lyrics.zip", "Resources/");
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
        public APIConfig apiKeys { get; set; }
    }
    
    class IDConfig
    {
        public ulong hostGuild { get; set; } = 0;   // Where logs etc are
        public ulong targetGuild { get; set; } = 0; // Where muted role etc are
        public ulong logChannel { get; set; } = 0;
        public ulong mutedRole { get; set; } = 0;
#if TOFU
        public ulong welcomeChannel { get; set; } = 0;
#else
        public ulong rssChannel { get; set; } = 0;
#endif
    }

    class APIConfig
    {
        public string weatherAPI { get; set; } = "";
    }

    class Global
    {
        public static List<List<string>> reminders = new List<List<string>>();
        public static DiscordGuild hostGuild;
        public static DiscordGuild targetGuild;
        public static DiscordChannel logChannel = null;
	
        // Moderation
        public static List<ulong> blacklistedUsers = new List<ulong>();
#if TOFU
        public static List<ulong> mutedUsers = new List<ulong>();
        public static DiscordRole mutedRole;
        public static DiscordChannel welcomeChannel = null;
#endif
    }
}
