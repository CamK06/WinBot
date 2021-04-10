
using System;
using System.Timers;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class RemindCommand : BaseCommandModule
    {
        [Command("remind")]
        [Description("Remind you about something")]
        [Usage("[Time] [Time Unit (second, minute, hour, day) [Message] (Note that long timespans are likely unreliable due to bot restarts)]")]
        [Category(Category.Main)]
        public async Task Remind(CommandContext Context, int time, string unit, [RemainingText] string message)
        {
            Timer t;

            // Filter bad values
            if (time <= 0)
            {
                await Context.RespondAsync("Timer length must be greater than 0!");
                return;
            }
            switch (unit) // Should really use an if statement here but oh well idc
            {
                // Second
                case "second":
                case "s":
                    t = new Timer(time * 1000);
                    break;
                // Minute
                case "minute":
                case "m":
                    t = new Timer(time * 60000);
                    break;
                // Hour
                case "hour":
                case "h":
                    t = new Timer(time * 3600000);
                    break;
                // Day
                case "day":
                case "d":
                    t = new Timer(time * 432000000);
                    break;

                default:
                    throw new Exception("Invalid time unit!");
            }

            // Start the timer
            t.AutoReset = false;
            t.Elapsed += async (object sender, ElapsedEventArgs args) =>
            {
                await Context.RespondAsync($"{Context.User.Mention}: {message.Replace("@", "-")}");
            };
            t.Start();

            await Context.RespondAsync("Your reminder has been set!");
        }
    }
}