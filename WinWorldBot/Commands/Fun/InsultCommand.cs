using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace WinWorldBot.Commands
{
    public class InsultCommand : ModuleBase<SocketCommandContext>
    {
        [Command("insult")]
        [Summary("Insult someone|")]
        [Priority(Category.Fun)]
        private async Task Insult(SocketUser user = null)
        {
            await Context.Message.DeleteAsync();

            string insult = "";
            // Download the insult string from the API
            using (WebClient client = new WebClient())
                insult = client.DownloadString("https://evilinsult.com/generate_insult.php?lang=en&type=text");
            
            if(user == null)
                await ReplyAsync($"{Context.Message.Author.Mention}, {insult}");
            else
                await ReplyAsync($"{user.Username}, {insult}");
        }
    }
}