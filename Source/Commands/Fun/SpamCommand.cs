#if !TOFU
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Newtonsoft.Json;

using WinBot.Util;
using WinBot.Commands.Attributes;

using Serilog;

namespace WinBot.Commands.Main
{
    public class SpamCommand : BaseCommandModule
    {
        [Command("spam")]
        [Description("Gets a random spam e-mail")]
        [Attributes.Category(Category.Fun)]
        public async Task Spam(CommandContext Context, [RemainingText] int spamID = 0)
        {
            // Fetch the spam email json
            string json;
            if(!TempManager.TempFileExists("spamEmail.json")) {
                WebClient client = new WebClient();
                json = client.DownloadString("http://www.nick99nack.com/spam/spam.json");
                File.WriteAllText(TempManager.GetTempFile("spamEmail.json"), json);
                Log.Information("Downloaded spam email json");
            }
            else
                json = File.ReadAllText(TempManager.GetTempFile("spamEmail.json"));

            // Deserialize the json and fetch an email
            dynamic spam = JsonConvert.DeserializeObject(json);
            string spamSubject, spamContent;
            if (spamID == 0) {
                int spamTotal = ((int)spam[0].count);
                var rand = new Random();
                spamID = rand.Next(1, spamTotal);

                spamSubject = spam[spamID].subject;
                spamContent = ((string)spam[spamID].content).Truncate(950);
            }
            else {
                spamSubject = spam[spamID].subject;
                spamContent = ((string)spam[spamID].content).Truncate(950);
            }

            // Create and send an embed
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle("Random Spam E-mail");
            eb.WithThumbnail("http://www.nick99nack.com/img/mail.gif");
            eb.AddField($"Subject: {spamSubject}", $"{spamContent}");
            eb.WithFooter($"ID: {spamID}");
            eb.WithColor(DiscordColor.Gold);
            eb.WithUrl("http://www.nick99nack.com/spam/");
            await Context.ReplyAsync("", eb.Build());
        }
    }
}
#endif