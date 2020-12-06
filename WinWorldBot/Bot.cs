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

using WinWorldBot.Utils;

namespace WinWorldBot
{
    class Bot
    {
        static void Main(string[] args) => new Bot().RunBot().GetAwaiter().GetResult(); // Point the main function to the async RunBot task so the bot can operate asynchronously

        public static DiscordSocketClient client = new DiscordSocketClient();
        public static CommandService commands = new CommandService();
        public static IServiceProvider services;
        public static BotConfig config;
        public static DateTime startTime = DateTime.Now;

        public static List<ulong> blacklistedUsers = new List<ulong>();

        public async Task RunBot()
        {
#if DEBUG
            Directory.SetCurrentDirectory("../WorkingDir");
#endif
            blacklistedUsers = MiscUtil.LoadBlacklist();

            if(!Directory.Exists("Logs"))
                Directory.CreateDirectory("Logs");

            services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(commands)
                .BuildServiceProvider();
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);

            // If no config file is present, create a new template one and quit
            if(!File.Exists("config.json")) {
                config = new BotConfig()
                {
                    Prefix = "~",
                    Token = "",
                    Status = ""
                };
                File.WriteAllText("config.json", JsonConvert.SerializeObject(config, Formatting.Indented));
                Log.Write("No configuration file present!");
                Log.Write("A template configuration file has been written to config.json");
                Environment.Exit(0);
            } else // If there is a config, read it
                config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("config.json"));

            
            // Set up events
            client.Log += (LogMessage message) => {
                Log.Write(message.Message);
                return Task.CompletedTask;
            };
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), null);


            // Start the bot
            await client.LoginAsync(TokenType.Bot, config.Token);
            await client.StartAsync();
            await client.SetGameAsync(config.Status);

            await Task.Delay(-1); // Wait forever
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {

            // ok counter
            if(arg.Author.Id == 694392238133870693 && arg.Content.ToLower().Contains("ok")) {
                string text = File.ReadAllText("ok");
                int.TryParse(text, out int okay);
                okay++;
                File.WriteAllText("ok", okay.ToString());
            }

            // oh counter
            if(arg.Author.Id == 694392238133870693 && arg.Content.ToLower().Contains("oh")) {
                string text = File.ReadAllText("oh");
                int.TryParse(text, out int okay);
                okay++;
                File.WriteAllText("oh", okay.ToString());
            }
            
            // SUPER IMPORTANT NORTON COUNTER
            if(arg.Content.ToLower().Contains("norton")) {
                string text = File.ReadAllText("nortons");
                int.TryParse(text, out int norton);
                norton++;
                File.WriteAllText("nortons", norton.ToString());
            }

#if RELEASE
            // This is a messy fix to allow commands outside of media but oh well
            if(arg.Author.Id != Globals.StarID && !arg.Content.ToLower().Contains("ev") && !arg.Content.ToLower().Contains("wiki") && !arg.Content.ToLower().Contains("mcinfo") && arg.Channel.Id != 474350814387765250) return;
#endif      // Basic setup for handling the command
            string messageLower = arg.Content.ToLower();
            SocketUserMessage message = arg as SocketUserMessage;
            if(message is null || message.Author.IsBot) return;
            int argumentPos = 0; // The location where the prefix should be found

            if(message.HasStringPrefix(config.Prefix, ref argumentPos) || message.HasMentionPrefix(client.CurrentUser, ref argumentPos)) { // If the message has the bots prefix or a mention of the bot, it is a command.
                if(blacklistedUsers.Contains(arg.Author.Id)) {
                    await arg.Channel.SendMessageAsync("You cannot use this command!");
                    return;
                }

                if(arg.Content.ToLower().Contains(" ae") || arg.Content.ToLower().Contains(" Æ") || arg.Content.ToLower().Contains("á") || arg.Content.ToLower().Contains("é")) {
                    await arg.Channel.SendMessageAsync("Fuck off with that shit");
                    return;
                }
                
                SocketCommandContext context = new SocketCommandContext(client, message); // Create context for the command, this is things like channel, guild, etc
                var result = await commands.ExecuteAsync(context, argumentPos, services); // Execute the command with the above context
                
                // Command error handling
                if(!result.IsSuccess) {
                    if(!result.ErrorReason.ToLower().Contains("unknown command")) {
                        Log.Write(result.ErrorReason);
                        await message.Channel.SendMessageAsync(result.ErrorReason);
                    }
                }
            }
        }
    }

    public class BotConfig
    {
        public string Token { get; set; }
        public string Prefix { get; set; }
        public string Status { get; set; }
        public string CatAPIKey { get; set; }
        public Color embedColour { get; set; } = Color.Gold;
    }
}
