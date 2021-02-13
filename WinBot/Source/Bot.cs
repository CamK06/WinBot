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

		public async Task RunBot()
		{
			// We switch to ../WorkingDir when debugging so the project directory doesn't get flooded with crap
#if DEBUG
			if (!Directory.Exists("../WorkingDir")) Directory.CreateDirectory("../WorkingDir");
			Directory.SetCurrentDirectory("../WorkingDir");
#endif

			// Load the configuration
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
			
			// Start the bot
			await client.LoginAsync(TokenType.Bot, config.token);
			await client.StartAsync();
			if(!string.IsNullOrWhiteSpace(config.activity))
				await client.SetGameAsync(config.activity);
			
			
			await Task.Delay(-1);
		}
	}

	public class BotConfig
	{
		public string token { get; set; }
		public string prefix { get; set; }
		public string activity { get; set; }
		public ulong logChannel { get; set; }
	}
}