using System;
using System.IO;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Serilog;

using static WinBot.Util.ResourceManager;
using WinBot.Util;

namespace WinBot.Misc
{
	public class DailyReportSystem
	{
		public static DailyReport report = new DailyReport()
		{
			dayOfReport = DateTime.Now,
			commandsRan = 0,
			messagesSent = 0,
			usersJoined = 0,
			usersLeft = 0
		};

		public static List<DailyReport> reports = new List<DailyReport>();

		public static void Init()
		{
			// Load the backup report if it exists.
			if (File.Exists(TempManager.GetTempFile("dailyreport")))
				report = JsonConvert.DeserializeObject<DailyReport>(File.ReadAllText(TempManager.GetTempFile("dailyreport")));

			// Load previous reports
			if (File.Exists(GetResourcePath("dailyReports", ResourceType.JsonData)))
				reports = JsonConvert.DeserializeObject<List<DailyReport>>(File.ReadAllText(GetResourcePath("dailyReports", ResourceType.JsonData)));

			// E v e n t s
			Bot.client.MessageCreated += (DiscordClient client, MessageCreateEventArgs e) => {
				
				if(!e.Author.IsBot)
					report.messagesSent++;
				if (e.Message.Content.StartsWith(".")) // Inaccurate but oh well.
					report.commandsRan++;
				return Task.CompletedTask;
			};
			Bot.client.GuildMemberAdded += (DiscordClient client, GuildMemberAddEventArgs e) => {

				report.usersJoined++;
				return Task.CompletedTask;
			};
			Bot.client.GuildMemberRemoved += (DiscordClient client, GuildMemberRemoveEventArgs e) => {

				report.usersLeft++;
				return Task.CompletedTask;
			};

			// Report timer
			Timer t = new Timer(30000); // 30 seconds
			t.AutoReset = true;
			t.Elapsed += (object sender, ElapsedEventArgs e) => { if (DateTime.Now.Day != report.dayOfReport.Day) SendReport(); };
			t.Start();

			// Backup timer
			Timer t2 = new Timer(300000); // 5 minutes
			t2.AutoReset = true;
			t2.Elapsed += (object sender, ElapsedEventArgs e) => { CreateBackup(); };
			t2.Start();

			Log.Write(Serilog.Events.LogEventLevel.Information, "Daily report service started");
		}

		private async static void SendReport()
		{
			// Write the reports
			reports.Add(report);
			File.WriteAllText(GetResourcePath("dailyReports", ResourceType.JsonData),
							  JsonConvert.SerializeObject(reports, Formatting.Indented));

			// Create and send the report embed
			DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
			eb.WithTitle($"Daily Report For {report.dayOfReport.ToString("dddd, dd, MMMM, yyyy")}");
			eb.WithTimestamp(report.dayOfReport);
			eb.WithColor(DiscordColor.Gold);
			eb.AddField("Messages Sent", report.messagesSent.ToString(), true);
			eb.AddField("Commands Ran", report.commandsRan.ToString(), true);
			eb.AddField("Users Joined", report.usersJoined.ToString(), true);
			eb.AddField("Users Left", report.usersLeft.ToString(), true);
			await Global.logChannel.SendMessageAsync("", eb.Build());

			// Reset the report
			report = new DailyReport()
			{
				dayOfReport = DateTime.Now,
				commandsRan = 0,
				messagesSent = 0,
				usersJoined = 0,
				usersLeft = 0
			};

			// Remove the backup so it doesn't pass onto the next day
			TempManager.RemoveTempFile("dailyreport");
		}

		// This is done in its own method rather than the lambda function so it can be called from other parts of the program
		public static void CreateBackup()
		{
			File.WriteAllText(TempManager.GetTempFile("dailyreport"), JsonConvert.SerializeObject(report, Formatting.Indented));
		}
	}

	public class DailyReport
	{
		public DateTime dayOfReport { get; set; }
		public int messagesSent { get; set; }
		public int commandsRan { get; set; }
		public int usersJoined { get; set; }
		public int usersLeft { get; set; }
	}
}