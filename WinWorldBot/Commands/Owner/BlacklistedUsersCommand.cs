using System;
using System.Text;
using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace WinWorldBot.Commands
{
    public class BlacklistedUsersCommand : ModuleBase<SocketCommandContext>
    {
        [Command("blacklistusers")]
        [Summary("Shows all blacklisted users|")]
        [Priority(Category.Owner)]
        private async Task BlacklistUsers()
        {
            try {
            SocketGuildUser author = Context.Message.Author as SocketGuildUser;
            if(author.Id != Globals.StarID && !author.GuildPermissions.KickMembers) {
                await Context.Message.DeleteAsync();
                return;
            }
            // Create a string list of blacklisted users
            StringBuilder builder = new StringBuilder();
            foreach(ulong userId in Bot.blacklistedUsers) {
                SocketGuildUser user = Context.Guild.GetUser(userId);
                if(user != null) builder.AppendLine(user.ToString());
            }
            
            // Create an embed
            EmbedBuilder eb = new EmbedBuilder();
            eb.WithTitle("Blacklisted Users");
            eb.WithColor(Bot.config.embedColour);
            eb.WithDescription($"```\n{builder.ToString()}```");

            // Send the embed
            await ReplyAsync("", false, eb.Build());
            }
            catch(Exception ex) {
                await ReplyAsync($"Error: {ex.Message}\n{ex.TargetSite}\n{ex.Source}\n{ex.StackTrace}\n{ex.InnerException}");
            }
        }
    }
}