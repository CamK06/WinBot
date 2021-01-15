using System;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using WinWorldBot.Utils;

namespace WinWorldBot.Commands
{
    public class WhoIsCommand : ModuleBase<SocketCommandContext>
    {
        [Command("whois")]
        [Summary("Shows information about a given user|[User]")]
        [Priority(Category.Main)]
        private async Task WhoIs(SocketGuildUser user)
        {
            try {
            EmbedBuilder Embed = new EmbedBuilder();
            Embed.WithColor(Bot.config.embedColour);
            if(user != null) Embed.WithAuthor(user);
            if(user.GetAvatarUrl() != null)
                Embed.WithThumbnailUrl(user.GetAvatarUrl());
            else
                Embed.WithThumbnailUrl(user.GetDefaultAvatarUrl());
            if(user.Id != null) Embed.AddField("**ID**", user.Id, false);
            if(!string.IsNullOrWhiteSpace(user.Nickname))
                Embed.AddField("**Nickname**", user.Nickname);
            Embed.AddField("**Created On**", MiscUtil.FormatDate(user.CreatedAt), true);
            Embed.AddField("**Joined On**", MiscUtil.FormatDate(user.JoinedAt.Value), true);
            //Embed.AddField("**Subscription**", "Buy our shitty premium to instantly regret our 'perks'!", true);
            
            await ReplyAsync("", false, Embed.Build());
            }
            catch(Exception ex) {
                await ReplyAsync("Error: " + ex.Message + "\nStack Trace:" + ex.StackTrace);
            }
        }
    }
}