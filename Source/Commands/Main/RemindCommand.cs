using System;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace WinBot.Commands.Main
{
	public class RemindCommand : ModuleBase<SocketCommandContext>
	{
		[Command("remind")]
		[Summary("Remind you of something|[Time] [Time Unit (second, minute, hour, day)] [Message] (Note that days may not be reliable due to potential bot restarts)")]
		[Priority(Category.Main)]
		public async Task Remind(int time, string unit, [Remainder]string message)
		{
			Timer t;

			// Filter bad values
			if(time <= 0) {
				await ReplyAsync("Timer length must be greater than 0!");
				return;
			}
			switch(unit) // Should really use an if statement here but oh well idc
			{
				// Second
				case "second":
				case "s":
					t = new Timer(time*1000);
				break;
				// Minute
				case "minute":
				case "m":
					t = new Timer(time*60000);
				break;
				// Hour
				case "hour":
				case "h":
					t = new Timer(time*3600000);
				break;
				// Day
				case "day":
				case "d":
					t = new Timer(time*432000000);
				break;

				default:
					throw new Exception("Invalid time unit!");
			}

			// Start the timer
			t.AutoReset = false;
			t.Elapsed += async (object sender, ElapsedEventArgs args) => {
				await ReplyAsync($"REMINDER: {message}");
			};
			t.Start();

			await ReplyAsync("Your reminder has been set!");
		}
	}
}