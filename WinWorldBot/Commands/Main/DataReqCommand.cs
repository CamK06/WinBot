using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

using Discord;
using Discord.Commands;

using Newtonsoft.Json;

using WinWorldBot.Data;

namespace WinWorldBot.Commands
{
    public class DataReqCommand : ModuleBase<SocketCommandContext>
    {
        [Command("requestdata")]
        [Summary("Request all data stored about you|[xml | json]")]
        [Priority(Category.Main)]
        private async Task RequestData(string arg = null)
        {
            if(!Directory.Exists("DataRequests")) Directory.CreateDirectory("DataRequests");

            // Determine whether json or xml should be used
            bool useJson = true;
            if (arg != null && arg.ToLower() == "xml") useJson = false;
            string path = $"DataRequests/{Context.Message.Author.Username}-{DateTime.Now.ToShortDateString().Replace("/", "-")}.json";
            if (!useJson) path = $"DataRequests/{Context.Message.Author.Username}-{DateTime.Now.ToShortDateString().Replace("/", "-")}.xml";

            // Make sure the file doesn't already exist
            if (File.Exists(path)) goto send;
            string format = useJson ? "json" : "xml";
            await ReplyAsync($"Converting data to {format}...");

            // Serialize the data
            if (useJson)
            {
                string json = JsonConvert.SerializeObject(UserData.GetUser(Context.Message.Author), Formatting.Indented);
                File.WriteAllText(path, json);
            }
            else
            {
                XmlSerializer s = new XmlSerializer(typeof(User));
                FileStream file = new FileStream(path, FileMode.OpenOrCreate);
                s.Serialize(file, UserData.GetUser(Context.Message.Author));
                file.Close();
            }

            // Verify the file size
            send:
            if (File.ReadAllBytes(path).Length > 7500000)
            {
                await ReplyAsync("Your data was unable to be sent due to the absurdly small file size limit from Discord. Starman has been notified and should manually provide your data.");
                await Context.Guild.GetUser(Globals.StarID).SendMessageAsync($"Oi, {Context.Message.Author}'s data could not be delivered thanks to Discord. The file name is ``DataRequests/{Context.Message.Author.Username}-{DateTime.Now.ToShortDateString()}.xml``. GIVE IT TO THEM YA FOOKIN WANKER");
                return;
            }

            // Send the data
            await Context.Message.Author.SendFileAsync(path, "Here's your data!");
            await ReplyAsync("Your data has been sent! Check your DMs.");
        }
    }
}