using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using ScottPlot;

using Newtonsoft.Json;

using WinBot.Util;

namespace WinBot.Commands.Main
{
	public class ServerStatsCommand : ModuleBase<SocketCommandContext>
	{
		[Command("serverstats")]
		[Priority(Category.Main)]
		[Summary("Show basic statistics about the server|")]
		public async Task ServerStats()
		{
			// Report loading
            List<DailyReport> reports = new List<DailyReport>();
            foreach(string file in Directory.GetFiles("DailyReports")) {
                DailyReport newReport = JsonConvert.DeserializeObject<DailyReport>(File.ReadAllText(file));
                reports.Add(newReport);
            }
			reports = reports.OrderByDescending(grp => grp.dayOfReport.DayOfYear).Reverse().ToList();

            // Data parsing
            double[] messages = new double[reports.Count];
            double[] commands = new double[reports.Count];
            double[] userJoin = new double[reports.Count];
            double[] userLeave = new double[reports.Count];
            string[] xticks = new string[reports.Count];
            for(int i = 0; i < reports.Count; i++) {
                messages[i] += reports[i].messagesSent;
                commands[i] += reports[i].commandsRan;
                userJoin[i] += reports[i].usersJoined;
                userLeave[i] += reports[i].usersLeft;
            }

            double[] ys = new double[reports.Count];
            for(int i = 0; i < reports.Count; i++) {
                ys[i] = i;
                xticks[i] = reports[i].dayOfReport.ToShortDateString();
            }
            
            // Plotting
            Plot plt = new Plot(1920, 1080);
            plt.Style(System.Drawing.Color.FromArgb(52, 54, 60), System.Drawing.Color.FromArgb(52, 54, 60), null, System.Drawing.Color.White, System.Drawing.Color.White, System.Drawing.Color.White);
            plt.XLabel("Day", null, null, null, 25.5f, false);
            plt.YLabel("Count", null, null, 25.5f, null, false);
            plt.PlotFillAboveBelow(ys, messages, "Messages", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(100, 119, 183), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(100, 119, 183), fillColorAbove: System.Drawing.Color.FromArgb(100, 119, 183));
            plt.PlotFillAboveBelow(ys, commands, "Command Executions", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(252, 186, 3), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(252, 186, 3), fillColorAbove: System.Drawing.Color.FromArgb(252, 186, 3));
            plt.PlotFillAboveBelow(ys, userJoin, "Users Joined", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(252, 3, 3), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(252, 3, 3), fillColorAbove: System.Drawing.Color.FromArgb(252, 3, 3));
            plt.PlotFillAboveBelow(ys, userLeave, "Users Left", lineWidth: 4, lineColor: System.Drawing.Color.FromArgb(15, 252, 3), fillAlpha: .5, fillColorBelow: System.Drawing.Color.FromArgb(15, 252, 3), fillColorAbove: System.Drawing.Color.FromArgb(15, 252, 3));
            plt.TightenLayout(0, true);
            plt.XTicks(xticks);
            plt.Title("WinWorldPC Statistics", null, null, 45.5f, null, true);
            plt.Legend(true, null, 30, null, null, System.Drawing.Color.FromArgb(100, 52, 54, 60), null, legendLocation.upperRight, shadowDirection.lowerRight, null, null);

            // Save and send
            plt.SaveFig("stats.png");
			await Context.Channel.SendFileAsync("stats.png");
		}
	}
}