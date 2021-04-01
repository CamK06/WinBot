using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace WinBot.Commands.Main
{
	public class CovidCommand : ModuleBase<SocketCommandContext>
	{
		[Command("covid")]
		[Summary("Get info on the state of the pandemic in ontario schools|")]
		[Priority(Category.Main)]
		public async Task Covid()
		{
            // Set up the Chrome engine
            string totalStr, studentStr, staffStr, cTotalStr, date;
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("headless", "disable-gpu", "no-sandbox");

            // Fetch the data
            using(ChromeDriver driver = new ChromeDriver(".", options))
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                driver.Navigate().GoToUrl("https://www.ontario.ca/page/covid-19-cases-schools-and-child-care-centres");
                totalStr = wait.Until<string>(driver => driver.FindElement(By.Id("en-table-school-total-new")).Text);
                cTotalStr = wait.Until<string>(driver => driver.FindElement(By.Id("en-table-school-total-cumulative")).Text);
                studentStr = wait.Until<string>(driver => driver.FindElement(By.Id("en-table-school-student-new")).Text);
                staffStr = wait.Until<string>(driver => driver.FindElement(By.Id("en-table-school-staff-new")).Text);
                date = wait.Until<string>(driver => driver.FindElement(By.Id("en-school-summary-date")).Text);
            }

            // Parse values into integers
            int.TryParse(totalStr, out int total);
            int.TryParse(studentStr, out int student);
            int.TryParse(staffStr, out int staff);
            int.TryParse(cTotalStr.Replace(",",""), out int cTotal);
            
            // Create a summary
            string summary = $"Today saw students with {MathF.Round((float)student/total*100)}% of school cases and staff with {MathF.Round((float)staff/total*100)}%. These cases amount to {MathF.Round((float)total/cTotal*100)}% of total school cases.";
            // TODO: Make the summary more in-depth by involving the previous day's data aswell as provincial total.

            // Create embed
			EmbedBuilder eb = new EmbedBuilder();
			eb.WithColor(Color.Gold);
            eb.WithTitle($"Ontario School Covid Stats For {date.Split("at")[0]}");
            eb.WithDescription(summary);
            eb.AddField("Total New Cases", total, true);
            eb.AddField("Total New Student Cases", student, true);
            eb.AddField("Total New Staff Cases", staff, true);
            eb.AddField("Total School Cases", cTotalStr, true);
            eb.WithFooter("Data gets updated every school day at 10:30AM EST");
            eb.WithThumbnailUrl("https://i.imgur.com/Seq3SZh.png"); // D U N G  F O R D

			await ReplyAsync("", false, eb.Build());
		}
	}
}