using System.Linq;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;

using WinWorldBot.Data;

using IF.Lastfm.Core.Objects;

namespace WinWorldBot.Commands
{
    public class SongCommand : ModuleBase<SocketCommandContext>
    {
        [Command("fm")]
        [Summary("Shows your current song|")]
        [Priority(Category.Main)]
        public async Task FM()
        {
            await Context.Channel.TriggerTypingAsync();

            User User = UserData.GetUser(Context.Message.Author);
            if(string.IsNullOrWhiteSpace(User.FMName)) {
                await ReplyAsync("You must set your Last.FM username with the `setfm` command!");
                return;
            }
            var Scrobbles = await Bot.FM.User.GetRecentScrobbles(User.FMName);

            if (Scrobbles.Success && Scrobbles != null && Scrobbles.Content != null)
            {
                LastTrack CurrentSong = Scrobbles.FirstOrDefault();
                EmbedBuilder Embed = new EmbedBuilder();

                // Basic data for the embed
                string AuthorText = $"Current song for {Context.User.Username}";
                if (Scrobbles.First().IsNowPlaying == null || !Scrobbles.First().IsNowPlaying.Value) AuthorText = $"Previous song for {Context.User.Username}";
                string AvatarUrl = Context.User.GetAvatarUrl();
                if (Context.User.GetAvatarUrl() == null || Context.User.GetAvatarUrl() == "") AvatarUrl = Context.User.GetDefaultAvatarUrl();
                string Album = CurrentSong.AlbumName;
                if (Album == null || Album == "") Album = "Unknown";

                // Set up the embed
                Embed.WithAuthor(AuthorText, AvatarUrl);
                Embed.WithUrl(CurrentSong.Url.AbsoluteUri);
                Embed.WithTitle(CurrentSong.Name);
                Embed.WithDescription($"By **{CurrentSong.ArtistName}** on **{Album}**");
                Embed.WithColor(Bot.config.embedColour);
                Embed.WithThumbnailUrl(CurrentSong.Images.First().AbsoluteUri.Replace("34s", "200s")); // The album cover, we replace 34s with 200s to get a larger image

                await ReplyAsync("", false, Embed.Build());
            }
            else await ReplyAsync("Failed to get your current song! This is most likely caused by your Last.FM username not being set, use ~setfm to set it.");
        }

        [Command("setfm")]
        [Summary("Set your last.fm username|[Username]")]
        [Priority(Category.Main)]
        public async Task SetFM(string Username)
        {
            User user = UserData.GetUser(Context.Message.Author);

            // Get the users info, just as a way to verify that they exist
            var info = Bot.FM.User.GetInfoAsync(Username);
            if (info.Result.Success)
            {
                user.FMName = Username;
                await ReplyAsync("Successfully set your Last.FM username to ``" + Username + "``!");
                UserData.SaveData();
            }
            else await ReplyAsync(":warning: Invalid Last.FM username! Please try again. :warning:");
        }
    }
}