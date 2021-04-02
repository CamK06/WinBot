using System;
using System.IO;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.WebSocket;

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

		public static void Init()
		{
			// Load the backup report if it exists.
			if (File.Exists("DailyReports/backup.json"))
				report = JsonConvert.DeserializeObject<DailyReport>(File.ReadAllText("DailyReports/backup.json"));

			// Create the report directory if it isn't present
			if (!Directory.Exists("DailyReports"))
				Directory.CreateDirectory("DailyReports");

			// E v e n t s
			Bot.client.MessageReceived += (SocketMessage Message) =>
			{
				report.messagesSent++;
				if (Message.Content.StartsWith(".")) // Inaccurate but oh well.
					report.commandsRan++;
				return Task.CompletedTask;
			};
			Bot.client.UserJoined += (SocketGuildUser User) =>
			{
				report.usersJoined++;
				return Task.CompletedTask;
			};
			Bot.client.UserLeft += (SocketGuildUser User) =>
			{
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
		}

		private async static void SendReport()
		{
			// Write the report
			File.WriteAllText($"DailyReports/{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} Report.json", JsonConvert.SerializeObject(report, Formatting.Indented));

			// Create and send the report embed
			EmbedBuilder eb = new EmbedBuilder();
			eb.WithTitle($"Daily Report For {report.dayOfReport.ToString("dddd, dd, MMMM, yyyy")}");
			eb.WithTimestamp(report.dayOfReport);
			eb.WithColor(Color.Gold);
			eb.AddField("Messages Sent", report.messagesSent, true);
			eb.AddField("Commands Ran", report.commandsRan, true);
			eb.AddField("Users Joined", report.usersJoined, true);
			eb.AddField("Users Left", report.usersLeft, true);
			await ((SocketTextChannel)Bot.client.GetChannel(Bot.config.logChannel)).SendMessageAsync("", false, eb.Build());

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
			File.Delete("DailyReports/backup.json");
		}

		// This is done in its own method rather than the lambda function so it can be called from other parts of the program
		public static void CreateBackup()
		{
			File.WriteAllText("DailyReports/backup.json", JsonConvert.SerializeObject(report, Formatting.Indented));
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