using System.Net;
using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;

namespace WinWorldBot.Commands
{
    public class InsultCommand : ModuleBase<SocketCommandContext>
    {
        /*
        [Command("insult")]
        [Summary("Insult someone|[User]")]
        [Priority(Category.Fun)]
        private async Task Insult(SocketUser user = null)
        {
            await Context.Message.DeleteAsync();

            string insult = "";
            // Download the insult string from the API
            using (WebClient client = new WebClient())
                insult = client.DownloadString("https://evilinsult.com/generate_insult.php?lang=en&type=text");
            
            // Insult the right person (person running the commmand if no input is given, person inputted if input is given)
            if(user == null)
                await ReplyAsync($"{Context.Message.Author.Mention}, {insult}");
            else
                await ReplyAsync($"{user.Username}, {insult}");
        }*/
    }
}