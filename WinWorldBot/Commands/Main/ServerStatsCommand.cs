using System;
using System.Threading.Tasks;


using Discord.Commands;

using System.Drawing;

using ScottPlot;

using WinWorldBot.Data;

namespace WinWorldBot.Commands
{
    public class ServerStatsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("serverstats")]
        [Priority(Category.Main)]
        [Summary("Get stats on the server (duh)|")]
        private async Task ServerStats()
        {
            // All of the necessary arrays and shit
            DateTime fourteenDays = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day-14);
            int days = (int)DateTime.Now.Subtract(fourteenDays).TotalDays;
            string[] dayText = new string[days];
            double[] xs = new double[days];
            double[] ys = new double[days];

            // Populate the labels and x positions
            for(int i = 0; i < days; i++) {
                xs[i] = i;
                dayText[i] = fourteenDays.AddDays(i).ToShortDateString();
            }

            // Populate the y positions
            foreach(var user in UserData.Users) {
                foreach(var msg in user.Messages) {
                    int index = (int)msg.SentAt.Subtract(fourteenDays).TotalDays;
                    if(index != days && index >= 0)
                        ys[index]++;
                }
            }
            
            // Set up the graph and plot
            Plot stats = new Plot(1920, 720);
            stats.Style(Color.FromArgb(52, 54, 60), Color.FromArgb(52, 54, 60), null, Color.White, Color.White, Color.White);
            stats.XLabel("Days", null, null, null, 12.5f, false);
            stats.YLabel("Messages", null, null, 12.5f, null, false);
            stats.Title($"{Context.Guild.Name} Messages in the Past 14 Days", null, null, 25.5f, null, false);
            stats.XTicks(dayText);
            stats.PlotFillAboveBelow(xs, ys, "thing", lineWidth: 4, lineColor: Color.FromArgb(100, 119, 183), fillAlpha: .5, fillColorBelow: Color.FromArgb(100, 119, 183), fillColorAbove: Color.FromArgb(100, 119, 183));
            stats.TightenLayout(0, true);

            // Send the graph
            stats.SaveFig("serverstats.png");
            await Context.Channel.SendFileAsync("serverstats.png");
        }
    }
}