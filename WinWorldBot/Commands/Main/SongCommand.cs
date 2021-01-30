using System.Threading.Tasks;

using Discord;
using Discord.Commands;

namespace WinWorldBot.Commands
{
    public class SongCommand : ModuleBase<SocketCommandContext>
    {
        [Command("song"), Alias("cs")]
        [Summary("Gets your current song|")]
        [Priority(Category.Main)]
        private async Task Song()
        {
            if(Context.User.Activity == null || Context.User.Activity.Type != ActivityType.Listening) {
                await ReplyAsync("You are not listening to a song! You must have Spotify linked to Discord with song status enabled. A custom status or game may also be interfering (thanks, Discord for having a dumb API!)");
                return;
            }

            // Create and send embed
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);
            eb.WithAuthor($"Current song for {Context.User}", Context.User.GetAvatarUrl());
            eb.WithDescription(Context.User.Activity.ToString());

            await ReplyAsync("", false, eb.Build());

            /* Todo: Add Last.FM support */
        }
    }
}