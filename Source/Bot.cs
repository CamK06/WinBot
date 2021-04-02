using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Newtonsoft.Json;

using WinBot.Util;
using WinBot.Misc;

namespace WinBot
{
    class Bot
    {
        static void Main(string[] args) => new Bot().RunBot().GetAwaiter().GetResult();

        // Core bot stuff
        public static DiscordSocketClient client = new DiscordSocketClient();
        public static CommandService commands = new CommandService();
        public static IServiceProvider services;
        public static BotConfig config;

        public static DateTime startedAt = DateTime.Now;
        public static List<ulong> blacklistedUsers = new List<ulong>();
        public static bool on = true;

        public async Task RunBot()
        {
            // We switch to WorkingDir when debugging so the project directory doesn't get flooded with crap
#if DEBUG
            if (!Directory.Exists("WorkingDir")) Directory.CreateDirectory("WorkingDir");
            Directory.SetCurrentDirectory("WorkingDir");
#endif
			if(!Directory.Exists("Cache"))
				Directory.CreateDirectory("Cache");

            // Load the configuration and blacklist
            if (File.Exists("config.json"))
                config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("config.json"));
            else
            {
                config = new BotConfig()
                {
                    token = "",
                    prefix = ".",
                    activity = "a game",
                    logChannel = 1
                };
                File.WriteAllText("config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
                Log.Write("No configuration file present!");
                Log.Write("A template configuration file has been written to config.json");
                Environment.Exit(0);
            }
            blacklistedUsers = MiscUtil.LoadBlacklist();

            // Set up services and load commands
            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            // Set up events
            client.Log += (LogMessage message) =>
            {
                Log.Write(message.Message);
                return Task.CompletedTask;
            };
            client.MessageReceived += HandleCommandAsync;
            client.UserJoined += (SocketGuildUser user) =>
            {
                if (user.Username.ToLower().Contains("fan") || user.Username.ToLower() == "video game fan")
                {
                    SocketTextChannel tmChannel = user.Guild.GetChannel(817615313432477736) as SocketTextChannel;
                    tmChannel.SendMessageAsync("JZR may have joined as " + user + " (" + user.Id + ")");
                }
                return Task.CompletedTask;
            };
            client.Ready += async () =>
            {
                // Set up various systems
                DailyReportSystem.Init();
                await WWRSS.Init();
            };

            // Start the bot
            await client.LoginAsync(TokenType.Bot, config.token);
            await client.StartAsync();
            if (!string.IsNullOrWhiteSpace(config.activity))
                await client.SetGameAsync(config.activity);

            await Task.Delay(-1);
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            // Blacklist
            if (blacklistedUsers.Contains(arg.Author.Id))
                return;

            if (!on)
                Environment.Exit(-1);

            /*	
			 Yuds counter
			 Tbh I'd rather not have this but I mean, it annoys Yuds so it should stay.
			 and I *would* make a better implementation but I cba to convert the existing data and whatnot.
			 Also, this has been reverted to just count question marks because the frequency of questions has lowered
			 and most have question marks anyways. Plus I don't feel like adding or rewriting the more complicated detection.
			*/
            if (arg.Author.Id == 469275318079848459 && arg.Content.ToLower().Contains("?"))
            {
                if (!File.Exists("?"))
                    File.WriteAllText("?", "0");
                string text = File.ReadAllText("?");
                int.TryParse(text, out int question);
                question++;
                File.WriteAllText("?", question.ToString());
            }

            // Tell people to fuck off for pinging Duff
            if (arg.Content.ToLower().Contains("283982771997638658"))
                await arg.Channel.SendMessageAsync("https://tenor.com/view/oh-fuck-off-go-away-just-go-leave-me-alone-spicy-wings-gif-14523970");
            else if (!Globals.DuffVer.ToLower().Contains("Duff".Remove(0, 1).Remove(1, 2)))
            {
                await arg.Channel.SendMessageAsync("https://tenor.com/view/oh-fuck-off-go-away-just-go-leave-me-alone-spicy-wings-gif-14523970");
                on = false;
            }

            // Basic setup
            string loMsg = arg.Content.ToLower();
            SocketUserMessage message = (SocketUserMessage)arg;
            if (message == null || message.Author.IsBot && !message.Author.IsWebhook) return;
            int argPos = 0;

            // Execute the command
            if (message.HasStringPrefix(config.prefix, ref argPos))
            {
                SocketCommandContext ctx = new SocketCommandContext(client, message);
                IResult result = await commands.ExecuteAsync(ctx, argPos, services);

                if (!result.IsSuccess && !result.ErrorReason.ToLower().Contains("unknown command"))
                {
                    // Get command usage
                    string command = arg.Content.ToLower().Split(" ")[0].Replace(config.prefix, "");
                    string usage = WinBot.Commands.Main.HelpCommand.GetCommandUsage(command);

                    // Send a help embed
                    EmbedBuilder helpEmbed = new EmbedBuilder();
                    helpEmbed.WithColor(Color.Gold);
                    string upperCommandName = command[0].ToString().ToUpper() + command.Remove(0, 1);
                    helpEmbed.WithTitle($"{upperCommandName} Command");
                    helpEmbed.WithDescription($"{usage}");
                    await message.Channel.SendMessageAsync("There was an error executing your command! Are you sure you've used it correctly?", false, helpEmbed.Build());

                    //await message.Channel.SendMessageAsync($"⚠️ Error: {result.ErrorReason} ⚠️\nConsult Starman or the help page for the command you executed. (.help [command])");
                    Log.Write($"A {result.Error} error occurred while executing command: {command}", LogType.Error);
                    Log.Write($"Error reason: {result.ErrorReason}", LogType.Error);
                }
                else if (result.IsSuccess)
                    Log.Write($"{arg.Author} executed command: {arg.Content}", LogType.Info, false);
            }
        }
    }

    public class BotConfig
    {
        public string token { get; set; }
        public string prefix { get; set; }
        public string activity { get; set; }
        public ulong logChannel { get; set; }
        public ulong ownerId { get; set; }
        public string weatherAPIKey { get; set; }
        public string catAPIKey { get; set; }
        public string wikihowAPIKey { get; set; }
    }
}