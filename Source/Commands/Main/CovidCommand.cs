using System;
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using Newtonsoft.Json;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class CovidCommand : BaseCommandModule
    {
        [Command("covid")]
        [Description("Gets info on the state of the pandemic in Ontario schools")]
        [Category(Category.Main)]
        public async Task Covid(CommandContext Context)
        {
            CovidDay today;

            // Fetch new values for today if they don't exist
            if (!File.Exists($"Cache/covidschool-{DateTime.Now.ToShortDateString().Replace("/", "-")}.cache"))
            {
                // Set up the Chrome engine
                today = new CovidDay();
                ChromeOptions options = new ChromeOptions();
                options.AddArguments("headless", "disable-gpu", "no-sandbox");

                // Fetch the data
                using (ChromeDriver driver = new ChromeDriver(".", options))
                {
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    driver.Navigate().GoToUrl("https://www.ontario.ca/page/covid-19-cases-schools-and-child-care-centres");
                    today.total = wait.Until<string>(driver => driver.FindElement(By.Id("en-table-school-total-new")).Text);
                    today.cTotal = wait.Until<string>(driver => driver.FindElement(By.Id("en-table-school-total-cumulative")).Text);
                    today.student = wait.Until<string>(driver => driver.FindElement(By.Id("en-table-school-student-new")).Text);
                    today.staff = wait.Until<string>(driver => driver.FindElement(By.Id("en-table-school-staff-new")).Text);
                    today.date = wait.Until<string>(driver => driver.FindElement(By.Id("en-school-summary-date")).Text);
                }

                if(DateTime.Now.Hour > 10)
                    File.WriteAllText($"Cache/covidschool-{DateTime.Now.ToShortDateString().Replace("/", "-")}.cache", JsonConvert.SerializeObject(today, Formatting.Indented));
            }
            else
                today = JsonConvert.DeserializeObject<CovidDay>(File.ReadAllText($"Cache/covidschool-{DateTime.Now.ToShortDateString().Replace("/", "-")}.cache"));

            // Parse values into integers
            int.TryParse(today.total, out int total);
            int.TryParse(today.student, out int student);
            int.TryParse(today.staff, out int staff);
            int.TryParse(today.cTotal.Replace(",", ""), out int cTotal);

            // Create a summary
            string summary = $"Today saw students with {MathF.Round((float)student / total * 100)}% of school cases and staff with {MathF.Round((float)staff / total * 100)}%. These cases amount to {MathF.Round((float)total / cTotal * 100)}% of total school cases.";
            // TODO: Make the summary more in-depth by involving the previous day's data aswell as provincial total.

            // Create embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithTitle($"Ontario School Covid Stats For {today.date.Split("at")[0]}");
            eb.WithDescription(summary);
            eb.AddField("Total New Cases", today.total, true);
            eb.AddField("Total New Student Cases", today.student, true);
            eb.AddField("Total New Staff Cases", today.staff, true);
            eb.AddField("Total School Cases", today.cTotal, true);
            eb.WithFooter("Data gets updated every school day at 10:30AM EST");
            eb.WithThumbnail("https://i.imgur.com/Seq3SZh.png"); // D U N G  F O R D

            await Context.RespondAsync("", eb.Build());
        }
    }

    public class CovidDay
    {
        public string date { get; set; }
        public string total { get; set; }
        public string student { get; set; }
        public string staff { get; set; }
        public string cTotal { get; set; }
    }
}