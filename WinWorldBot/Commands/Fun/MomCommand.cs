using System.Net;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Discord.Commands;
using Discord.WebSocket;

namespace WinWorldBot.Commands
{
    public class MomCommand : ModuleBase<SocketCommandContext>
    {
        /*
        [Command("mom")]
        [Summary("Use a random yo momma joke on someone|[User]")]
        [Priority(Category.Fun)]
        private async Task Mom(SocketUser user = null)
        {
            await Context.Message.DeleteAsync();

            string json = "";
            // Download the insult string from the API
            using (WebClient client = new WebClient())
                json = client.DownloadString("https://api.yomomma.info/");
            dynamic insult = JsonConvert.DeserializeObject(json);
            
            // Insult the right person (person running the commmand if no input is given, person inputted if input is given)
            if(user == null)
                await ReplyAsync($"{Context.Message.Author.Mention}, {(string)insult.joke}");
            else
                await ReplyAsync($"{user.Username}, {(string)insult.joke}");
        }*/
    }
}