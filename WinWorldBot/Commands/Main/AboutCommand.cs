using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class AboutCommand : ModuleBase<SocketCommandContext>
    {
        [Command("about")]
        [Summary("Shows information about the bot|")]
        [Priority(Category.Main)]
        private async Task About()
        {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithColor(Bot.config.embedColour);
            Embed.WithTitle("WinWorld Bot");
            Embed.WithCurrentTimestamp();
            Embed.WithThumbnailUrl(Bot.client.CurrentUser.GetAvatarUrl());
            Embed.AddField("Language", "C#", true);
            Embed.AddField("Library", "Discord.NET", true);
            Embed.AddField("Author", "Starman#8456", true);

            await ReplyAsync("", false, Embed.Build());
        }
    }
}