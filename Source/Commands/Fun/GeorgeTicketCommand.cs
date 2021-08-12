#if !TOFU
using System;
using System.Net;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;

using RestSharp;

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

            if (ticketID < 10){
                ticketUrl = $"https://cdn.chroniclesofgeorge.com/images/00{ticketID}.png";
            }
            else if(ticketID > 9 && ticketID < 100){
                ticketUrl = $"https://cdn.chroniclesofgeorge.com/images/0{ticketID}.png";
            }
            else{
                ticketUrl = $"https://cdn.chroniclesofgeorge.com/images/{ticketID}.png";
            }
        
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle("Random George Ticket");
            eb.WithImageUrl(ticketUrl);
            eb.WithFooter($"ID: {ticketID}");
            eb.WithUrl("https://www.chroniclesofgeorge.com/");
            await Context.RespondAsync("", eb.Build());
        }
    }
}
#endif
