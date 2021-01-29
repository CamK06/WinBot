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
        [Command("whois"), Alias("whoami")]
        [Summary("Shows information about a given user|[User]")]
        [Priority(Category.Main)]
        private async Task WhoIs([Remainder]SocketGuildUser user = null)
        {
            if(user == null) user = Context.Message.Author as SocketGuildUser;

            try
            {
                EmbedBuilder Embed = new EmbedBuilder();
                Embed.WithColor(Bot.config.embedColour);
                if (user != null) Embed.WithAuthor(user);
                if (user.GetAvatarUrl() != null)
                    Embed.WithThumbnailUrl(user.GetAvatarUrl());
                else
                    Embed.WithThumbnailUrl(user.GetDefaultAvatarUrl());
                Embed.AddField("**ID**", user.Id, false);
                if (!string.IsNullOrWhiteSpace(user.Nickname))
                    Embed.AddField("**Nickname**", user.Nickname, true);
                if (user.CreatedAt != null) Embed.AddField("**Created On**", $"{MiscUtil.FormatDate(user.CreatedAt)} ({(int)DateTime.Now.Subtract(user.CreatedAt.DateTime).TotalDays} days ago)", true);
                if (user.JoinedAt.HasValue) Embed.AddField("**Joined On**", $"{MiscUtil.FormatDate(user.JoinedAt.Value)} ({(int)DateTime.Now.Subtract(user.JoinedAt.Value.DateTime).TotalDays} days ago)", true);

                // This is kinda messy but oh well, idc
                if (user.Activity != null)
                {
                    if (user.Activity.Type == ActivityType.Listening && !string.IsNullOrWhiteSpace(user.Activity.ToString())) Embed.AddField("**Listening**", user.Activity.ToString(), true);
                    else if (user.Activity.Type == ActivityType.Playing && !string.IsNullOrWhiteSpace(user.Activity.ToString())) Embed.AddField("**Playing**", user.Activity.ToString(), true);
                    else if (user.Activity.Type == ActivityType.Watching && !string.IsNullOrWhiteSpace(user.Activity.ToString())) Embed.AddField("**Watching**", user.Activity.ToString(), true);
                    else if (user.Activity.Type == ActivityType.Streaming && !string.IsNullOrWhiteSpace(user.Activity.ToString())) Embed.AddField("**Streaming**", user.Activity.ToString(), true);
                    else if (user.Activity.Type == ActivityType.CustomStatus && !string.IsNullOrWhiteSpace(user.Activity.ToString())) Embed.AddField("**Status**", user.Activity.ToString(), true);
                }

                //Embed.AddField("**Subscription**", "Buy our shitty premium to instantly regret our 'perks'!", true);

                await ReplyAsync("", false, Embed.Build());
            }
            catch (Exception ex)
            {
                await ReplyAsync("Error: " + ex.Message + "\nStack Trace:" + ex.StackTrace);
            }
        }
    }
}