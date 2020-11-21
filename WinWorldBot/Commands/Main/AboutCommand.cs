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
            Embed.WithThumbnailUrl(Bot.client.CurrentUser.GetAvatarUrl());
            Embed.AddField("Language", "C#", true);
            Embed.AddField("Library", "Discord.NET", true);
            Embed.AddField("Author", "Starman#8456", true);
            Embed.AddField("Features", "This bot only has features that are needed. It's not bloated to hell and back with features nobody uses and shitty 'premium' subscriptions... it's a fucking bot. None of that is even half necessary", true);

            await ReplyAsync("", false, Embed.Build());
        }
    }
}