#if !TOFU
using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Fun
{
    public class TicketCommand : BaseCommandModule
    {
        [Command("ticket")]
        [Description("Tickets from the worst tech support rep in the world")]
        [Category(Category.Fun)]
        public async Task Ticket(CommandContext Context)
        {
            int ticketID = 0;
            string ticketUrl = "";
            var rand = new Random();
            ticketID = rand.Next(1, 223);

            ticketUrl = $"https://cdn.chroniclesofgeorge.com/images/{ticketID.ToString().PadLeft(3,'0')}.png";
        
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle("Random George Ticket");
            eb.WithImageUrl(ticketUrl);
            eb.WithFooter($"ID: {ticketID}");
	    eb.WithColor(DiscordColor.Gold);
            eb.WithUrl("https://www.chroniclesofgeorge.com/");
            await Context.ReplyAsync("", eb.Build());
        }
    }
}
#endif
