using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class ImageCommand : ModuleBase<SocketCommandContext>
    {
        [Command("b64img")]
        [Summary("Does stuff|")]
        [Priority(Category.Owner)]
        private async Task Image64([Remainder]string input = null)
        {
            byte[] bytes;
            if(input == null && Context.Message.Attachments.Count >= 1) {
                WebClient client = new WebClient();
                client.DownloadFile(Context.Message.Attachments.FirstOrDefault().Url, "text");
                client.Dispose();
                bytes = Convert.FromBase64String(File.ReadAllText("text"));
            }
            else if(input != null) {
                bytes = Convert.FromBase64String(input);
            }
            else{
                await ReplyAsync("Invalid or no input! Please provide a base64 string");
                return;
            }

            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            image.Save("image.png");
            await Context.Channel.SendFileAsync("image.png");
        }
    }
}