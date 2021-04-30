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
        public const string VERSION = "3.1";

        static void Main(string[] args) => new Bot().RunBot().GetAwaiter().GetResult();

        public static DiscordClient client;
        public static CommandsNextExtension commands;
        public static BotConfig config;
        public static DiscordChannel logChannel;
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
                DailyReportSystem.Init();
                await WWRSS.Init();
                await client.UpdateStatusAsync(new DiscordActivity() { Name = config.status });
                await logChannel.SendMessageAsync("Ready.");
                Log.Write(Serilog.Events.LogEventLevel.Information, $"Running on host: {MiscUtil.GetHost()}");
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
        public string weatherAPIKey { get; set; }
        public ulong ownerId { get; set; }
        public string catAPIKey { get; set; }
        public string wikihowAPIKey { get; set; }
        public ulong rssChannel { get; set; }
    }
}
