using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace WinWorldBot.Commands
{
    public class BedCommand : ModuleBase<SocketCommandContext>
    {
        [Command("bed")]
        [Summary("Tell someone to go the hell to bed|[User]")]
        [Priority(Category.Fun)]
        private async Task Bed(SocketGuildUser user)
        {
            // Create and send the embed
            var eb = new EmbedBuilder();
            eb.WithColor(Bot.config.embedColour);
            eb.WithImageUrl("https://cdn.discordapp.com/attachments/563206142755471381/801593276271820861/parzgotobed.jpg");
            await ReplyAsync($"GO TO BED, {user.Mention}", false, eb.Build());
        }
    }
}