using System;
using System.IO;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

using Newtonsoft.Json;

namespace WinBot.Commands.Main
{
    public class CovidCommand : ModuleBase<SocketCommandContext>
    {
        [Command("covid")]
        [Summary("Get info on the state of the pandemic in ontario schools|")]
        [Priority(Category.Main)]
        public async Task Covid()
        {
            CovidDay today;

            // Cache stuff
            if (!Directory.Exists("CovidCache"))
                Directory.CreateDirectory("CovidCache");

            // Fetch new values for today if they don't exist
            if (!File.Exists($"CovidCache/{DateTime.Now.ToShortDateString().Replace("/", "-")}.json"))
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

                File.WriteAllText($"CovidCache/{DateTime.Now.ToShortDateString().Replace("/", "-")}.json", JsonConvert.SerializeObject(today, Formatting.Indented));
            }
            else
                today = JsonConvert.DeserializeObject<CovidDay>(File.ReadAllText($"CovidCache/{DateTime.Now.ToShortDateString().Replace("/", "-")}.json"));

            // Parse values into integers
            int.TryParse(today.total, out int total);
            int.TryParse(today.student, out int student);
            int.TryParse(today.staff, out int staff);
            int.TryParse(today.cTotal.Replace(",", ""), out int cTotal);

            // Create a summary
            string summary = $"Today saw students with {MathF.Round((float)student / total * 100)}% of school cases and staff with {MathF.Round((float)staff / total * 100)}%. These cases amount to {MathF.Round((float)total / cTotal * 100)}% of total school cases.";
            // TODO: Make the summary more in-depth by involving the previous day's data aswell as provincial total.

            // Create embed
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Color.Gold);
            eb.WithTitle($"Ontario School Covid Stats For {today.date.Split("at")[0]}");
            eb.WithDescription(summary);
            eb.AddField("Total New Cases", total, true);
            eb.AddField("Total New Student Cases", student, true);
            eb.AddField("Total New Staff Cases", staff, true);
            eb.AddField("Total School Cases", today.cTotal, true);
            eb.WithFooter("Data gets updated every school day at 10:30AM EST");
            eb.WithThumbnailUrl("https://i.imgur.com/Seq3SZh.png"); // D U N G  F O R D

            await ReplyAsync("", false, eb.Build());
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