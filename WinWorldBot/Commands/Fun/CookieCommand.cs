using System.Threading.Tasks;

using Discord.Commands;
using Discord.WebSocket;


namespace WinWorldBot.Commands
{
    public class CookieCommand : ModuleBase<SocketCommandContext>
    {
        [Command("cookie")]
        [Summary("Give someone a cookie|[User]")]
        [Priority(Category.Fun)]
        private async Task Cookie([Remainder]SocketGuildUser user = null)
        {
            if(user == null) 
            {
                await ReplyAsync("You must provide a user to give a cookie to!");
                return;
            }

            await ReplyAsync($"{user.Mention}: {Context.Message.Author.Username} gave you a cookie! 🍪");
        }
    }
}