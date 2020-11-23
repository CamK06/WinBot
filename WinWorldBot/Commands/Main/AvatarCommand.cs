using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class AvatarCommand : ModuleBase<SocketCommandContext>
    {
        [Command("avatar")]
        [Summary("Shows a user's avatar|")]
        [Priority(Category.Main)]
        private async Task Avatar(SocketGuildUser user = null)
        {
            if(user == null)
                user = Context.Message.Author as SocketGuildUser;

            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithColor(Bot.config.embedColour);
            Embed.WithAuthor(user);
            Embed.WithTitle("Avatar URL");
            
            // Set the image properly
            if(user.GetAvatarUrl() != null)
                Embed.WithImageUrl(user.GetAvatarUrl().Replace("size=128", "size=256"));
            else
                Embed.WithImageUrl(user.GetDefaultAvatarUrl().Replace("size=128", "size=256"));
            
            Embed.WithUrl(Embed.ImageUrl);
            await ReplyAsync("", false, Embed.Build());
        }
    }
}