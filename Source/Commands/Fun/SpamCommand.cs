#if !TOFU
using System;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using Newtonsoft.Json;

using WinBot.Util;
using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class SpamCommand : BaseCommandModule
    {
        [Command("spam")]
        [Description("Gets a random spam e-mail")]
        [Category(Category.Fun)]
        public async Task Spam(CommandContext Context, [RemainingText] int spamID = 0)
        {
            string json = "";
            // Grab the json string from the API
            using (WebClient client = new WebClient())
            json = client.DownloadString("http://www.nick99nack.com/spam/spam.json");
            dynamic spam = JsonConvert.DeserializeObject(json); // Deserialize the string into a dynamic object

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

            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle("Random Spam E-mail");
            eb.WithThumbnail("http://www.nick99nack.com/img/mail.gif");
            eb.AddField($"Subject: {spamSubject}", $"{spamContent}");
            eb.WithFooter($"ID: {spamID}");
            eb.WithUrl("http://www.nick99nack.com/spam/");
            await Context.ReplyAsync("", eb.Build());
        }
    }
}
#endif