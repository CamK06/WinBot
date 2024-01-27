using System;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Staff
{
    public class PurgeCommand : BaseCommandModule
    {
        [Command("purge")]
        [Description("Purge *x* amount of messages")]
        [Usage("[messages to remove]")]
        [Attributes.Category(Category.Staff)]
        [RequireUserPermissions(DSharpPlus.Permissions.ManageMessages)]
        public async Task Purge(CommandContext Context, int count = 0)
        {
            if(count <= 0)
                throw new Exception("Cannot purge 0 messages!");

            // Delete the messages
            var messages = await Context.Channel.GetMessagesAsync(count+1);
            await Context.Channel.DeleteMessagesAsync(messages, $"Purged by {Context.User.Username}#{Context.User.Discriminator}");

            // Logging
            DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
            builder.WithDescription($"**{Context.User.Username}#{Context.User.Discriminator}** purged {count} messages in {Context.Channel.Mention}");
            builder.WithTimestamp(DateTime.Now);
            builder.WithColor(DiscordColor.Gold);
            builder.AddField("IDs", $"```cs\nMod = {Context.User.Id}\nChannel = {Context.Channel.Id}```");
            await Global.logChannel.SendMessageAsync("", builder.Build());

            // Purged message
            DiscordMessage msg = await Context.ReplyAsync($"{count} messages purged. This message will be deleted in 3 seconds.");
            await Task.Delay(3000);
            await msg.DeleteAsync();
        }
    }
}