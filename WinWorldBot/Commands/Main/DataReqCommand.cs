using System;
using System.IO;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using Newtonsoft.Json;

using WinWorldBot.Data;

namespace WinWorldBot.Commands
{
    public class DataReqCommand : ModuleBase<SocketCommandContext>
    {
        [Command("requestdata")]
        [Summary("Request all data stored about you|")]
        [Priority(Category.Main)]
        private async Task RequestData()
        {
            // Skip creating the json and all file checks if the data exists already
            if(File.Exists($"DataRequests/{Context.Message.Author.Username}-{DateTime.Now.ToShortDateString().Replace("/", "-")}.json")) goto send;

            // Get the data
            await ReplyAsync("Converting data to Json...");
            string json = JsonConvert.SerializeObject(UserData.GetUser(Context.Message.Author), Formatting.Indented);

            // Write the file
            if(!Directory.Exists("DataRequests")) Directory.CreateDirectory("DataRequests");
            File.WriteAllText($"DataRequests/{Context.Message.Author.Username}-{DateTime.Now.ToShortDateString().Replace("/", "-")}.json", json);

            // Verify the file size
            send:
            if(File.ReadAllBytes($"DataRequests/{Context.Message.Author.Username}-{DateTime.Now.ToShortDateString().Replace("/", "-")}.json").Length > 7500000)
            {
                await ReplyAsync("Your data was unable to be sent due to the absurdly small file size limit from Discord. Starman has been notified and should manually provide your data.");
                await Context.Guild.GetUser(Globals.StarID).SendMessageAsync($"Oi, {Context.Message.Author}'s data could not be delivered thanks to Discord. The file name is ``DataRequests/{Context.Message.Author.Username}-{DateTime.Now.ToShortDateString()}.json``. GIVE IT TO THEM YA FOOKIN WANKER");
                return;
            }

            // Send the data
            await Context.Message.Author.SendFileAsync($"DataRequests/{Context.Message.Author.Username}-{DateTime.Now.ToShortDateString().Replace("/", "-")}.json", "Here's your data!");
            await ReplyAsync("Your data has been sent! Check your DMs.");
        }
    }
}