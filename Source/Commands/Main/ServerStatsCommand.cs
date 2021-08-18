using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

using ScottPlot;

using WinBot.Misc;
using WinBot.Util;

namespace WinBot.Commands.Main
{
    public class ServerStatsCommand : BaseCommandModule
    {
        [Command("serverstats")]
        [Description("Show basic statistics about the server")]
        [Category(Category.Main)]
        public async Task Serverstats(CommandContext Context, [RemainingText] string args = null)
        {
            // Report loading
            List<DailyReport> reports = new List<DailyReport>();
            foreach (string file in Directory.GetFiles("DailyReports"))
            {
                string json = File.ReadAllText(file).Replace("UsersLeft", "usersLeft").Replace("UsersJoined", "usersJoined")
                                                    .Replace("CommandsRan", "commandsRan").Replace("MessagesSent", "messagesSent")
                                                    .Replace("DayOfReport", "dayOfReport");
                DailyReport newReport = JsonConvert.DeserializeObject<DailyReport>(json);
                reports.Add(newReport);
            }
            reports = reports.OrderByDescending(grp => grp.dayOfReport.DayOfYear).Reverse().ToList();

            double[] messages, commands, ys;
#if TOFU
            double[] userJoin, userLeave;
#endif
            string[] xticks;

            // Data parsing... this is a huge mess but oh well, I'm lazy and it just works
            messages = new double[reports.Count];
            commands = new double[reports.Count];
            ys = new double[reports.Count];
            xticks = new string[reports.Count];
#if TOFU
                userJoin = new double[reports.Count];
                userLeave = new double[reports.Count];
#endif
            int realCount;
            for (int i = 0; i < reports.Count; i++)
            {
                if(DateTime.Now.Subtract(reports[i].dayOfReport).TotalDays <= 14) {
                ys[i] = i;
                xticks[i] = reports[i].dayOfReport.ToShortDateString();
                messages[i] += reports[i].messagesSent;
                commands[i] += reports[i].commandsRan;
#if TOFU
                    userJoin[i] += reports[i].usersJoined;
                    userLeave[i] += reports[i].usersLeft;
#endif
                }
            }

            // Plotting
            Plot plt = new Plot(1920, 1080);
            plt.Style(System.Drawing.Color.FromArgb(52, 54, 60), System.Drawing.Color.FromArgb(52, 54, 60), null, System.Drawing.Color.White, System.Drawing.Color.White, System.Drawing.Color.White);
            plt.XLabel("Day", null, null, null, 25.5f, false);
            plt.YLabel("Count", null, null, 25.5f, null, false);
            plt.PlotFillAboveBelow(ys, messages, "Messages", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(100, 119, 183), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(100, 119, 183), fillColorAbove: System.Drawing.Color.FromArgb(100, 119, 183));
            plt.PlotFillAboveBelow(ys, commands, "Command Executions", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(252, 186, 3), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(252, 186, 3), fillColorAbove: System.Drawing.Color.FromArgb(252, 186, 3));
#if TOFU
            plt.PlotFillAboveBelow(ys, userJoin, "Users Joined", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(252, 3, 3), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(252, 3, 3), fillColorAbove: System.Drawing.Color.FromArgb(252, 3, 3));
            plt.PlotFillAboveBelow(ys, userLeave, "Users Left", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(15, 252, 3), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(15, 252, 3), fillColorAbove: System.Drawing.Color.FromArgb(15, 252, 3));
#endif
            //plt.TightenLayout(0, true);
            plt.Layout(xScaleHeight: 128);
            plt.XTicks(xticks);
            //plt.Ticks(dateTimeX: true, xTickRotation: 75);
            plt.Title($"{Context.Guild.Name} Stats", null, null, 45.5f, null, true);
            //plt.Grid(xSpacing: 1, xSpacingDateTimeUnit: ScottPlot.Config.DateTimeUnit.Day);
            plt.Legend(true, null, 30, null, null, System.Drawing.Color.FromArgb(100, 52, 54, 60), null, legendLocation.upperRight, shadowDirection.lowerRight, null, null);

            // Save and send
            plt.SaveFig("stats.png");
            if (args != null)
            {
                await Context.Channel.SendFileAsync("stats.png");
                return;
            }

            // Draw it onto C H A D
            Bitmap chad = new Bitmap(Bitmap.FromFile("chad.png"));
            Bitmap stats = new Bitmap(Bitmap.FromFile("stats.png"));
            using (Graphics canvas = Graphics.FromImage(chad))
            {
                canvas.DrawImage(stats, graphPos);
                canvas.Save();
            }

            chad.Save("stats.png");
            await Context.Channel.SendFileAsync("stats.png");
        }

        static Point[] graphPos = new Point[] {
            new Point(900, 500),  // Top left
			new Point(1440, 385), // Top right
			new Point(985, 890),  // Bottom left
		};
    }
}