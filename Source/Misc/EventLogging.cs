using System;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Serilog;

namespace WinBot.Misc
{
    public class EventLogging
    {
        public static void Init()
        {
            // Edit logging
            Bot.client.MessageUpdated += async (DiscordClient client, MessageUpdateEventArgs e) => {
                if(e.MessageBefore.Content == e.Message.Content) // Just fixing Discords issues.... ffs
                    return;

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.WithColor(DiscordColor.Gold);
                builder.WithDescription($"**{e.Author.Username}#{e.Author.Discriminator}** updated a message in {e.Channel.Mention} \n" + Formatter.MaskedUrl("Jump to message!", e.Message.JumpLink));
                builder.AddField("Before", e.MessageBefore.Content, true);
                builder.AddField("After", e.Message.Content, true);
                builder.AddField("IDs", $"```cs\nUser = {e.Author.Id}\nMessage = {e.Message.Id}\nChannel = {e.Channel.Id}```");
                builder.WithTimestamp(DateTime.Now);
                await Global.logChannel.SendMessageAsync("", builder.Build());
            };
            // Delete logging
            Bot.client.MessageDeleted += async (DiscordClient client, MessageDeleteEventArgs e) => {
                try { 
                    DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                    builder.WithColor(DiscordColor.Gold);
                    builder.WithDescription($"**{e.Message.Author.Username}#{e.Message.Author.Discriminator}**'s message in {e.Channel.Mention} was deleted");
                    if(!string.IsNullOrWhiteSpace(e.Message.Content))
                        builder.AddField("Content", e.Message.Content, true);
                    else
                        builder.AddField("Content", "[Content is media or an embed]");
                    builder.AddField("IDs", $"```cs\nUser = {e.Message.Author.Id}\nMessage = {e.Message.Id}\nChannel = {e.Channel.Id}```");
                    builder.WithTimestamp(DateTime.Now);
                    await Global.logChannel.SendMessageAsync("", builder.Build());
                }
                catch (Exception ex) {
                    Log.Write(Serilog.Events.LogEventLevel.Information, ex.Message);
                }
            };
            Bot.client.GuildMemberAdded += async (DiscordClient client, GuildMemberAddEventArgs e) => {

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.WithColor(DiscordColor.Gold);
                builder.WithDescription($"**{e.Member.Username}#{e.Member.Discriminator}** joined the server");
                builder.AddField("IDs", $"```cs\nUser = {e.Member.Id}```");
                builder.WithTimestamp(DateTime.Now);
                await Global.logChannel.SendMessageAsync("", builder.Build());
            };
            Bot.client.GuildMemberRemoved += async (DiscordClient client, GuildMemberRemoveEventArgs e) => {

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.WithColor(DiscordColor.Gold);
                builder.WithDescription($"**{e.Member.Username}#{e.Member.Discriminator}** left the server");
                builder.AddField("IDs", $"```cs\nUser = {e.Member.Id}```");
                builder.WithTimestamp(DateTime.Now);
                await Global.logChannel.SendMessageAsync("", builder.Build());
            };
            Bot.client.InviteCreated += async (DiscordClient client, InviteCreateEventArgs e) => {

                DiscordEmbedBuilder builder = new DiscordEmbedBuilder();
                builder.WithColor(DiscordColor.Gold);
                builder.WithDescription($"**{e.Invite.Inviter.Username}#{e.Invite.Inviter.Discriminator}** created an invite in **{e.Channel.Mention}**");
                builder.AddField("Code", e.Invite.Code);
                builder.AddField("IDs", $"```cs\nUser = {e.Invite.Inviter.Id}\nChannel = {e.Channel.Id}```");
                builder.WithTimestamp(DateTime.Now);
                await Global. logChannel.SendMessageAsync("", builder.Build());
            };
        }
    }
}