
using System;
using System.Timers;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class RemindCommand : BaseCommandModule
    {
        [Command("remind")]
        [Description("Remind you about something")]
        [Usage("[Time] [Time Unit (seconds/s, minutes/m, hours/h, days/d) [Message] (Note that long timespans are likely unreliable due to bot restarts)]")]
        [Category(Category.Main)]
        public async Task Remind(CommandContext Context, string timeStr, [RemainingText] string message = "")
        {
            Timer t;
            int time = 0;
            string unit = "";

            int.TryParse(Regex.Replace(timeStr, "[A-Za-z ]", ""), out time);
            unit = Regex.Match(timeStr, "[A-Za-z ]", RegexOptions.None).Value;

            // Filter bad values
            if (time <= 0) {
                throw new Exception("Timer length must be greater than 0!");
            }
            switch (unit) // Should really use an if statement here but oh well idc
            {
                // Second
                case "seconds":
                case "s":
                    t = new Timer(time * 1000);
                    break;
                // Minute
                case "minutes":
                case "m":
                    t = new Timer(time * 60000);
                    break;
                // Hour
                case "hours":
                case "h":
                    t = new Timer(time * 3600000);
                    break;
                // Day
                case "days":
                case "d":
                    t = new Timer(time * 432000000);
                    break;

                default:
                    throw new Exception("Invalid time unit!");
            }

            // Start the timer
            t.AutoReset = false;
            t.Elapsed += async (object sender, ElapsedEventArgs args) => {
                await Context.ReplyAsync($"{Context.User.Mention}{(message == "" ? "" : ":")} {message.Replace("@", "-")}");
            };
            t.Start();

            await Context.ReplyAsync("Your reminder has been set!");
        }
    }
}
