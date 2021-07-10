// The system for haunting the bot in channels. Same idea as the DM system but operating within the server
#if TOFU
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.IO;
using Newtonsoft.Json;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace WinBot
{
    public class HauntSystem
    {
        public static List<Chat> chats = new List<Chat>();

        public static void Init()
        {
            Bot.client.MessageCreated += async (DiscordClient client, MessageCreateEventArgs args) => {
                if(args.Author.IsBot)
                    return;

                Chat chat;
                // Target to host
                if(chats.FirstOrDefault(x => x.target.Id == args.Channel.Id) != null) {

                    chat = chats.FirstOrDefault(x => x.target.Id == args.Channel.Id);

                    // Relay the message via webhook
                    if(chat.webhook != null) {
                        DiscordWebhookBuilder wb = new DiscordWebhookBuilder();
                        wb.WithUsername(args.Author.Username);
                        wb.WithAvatarUrl(args.Author.AvatarUrl);
                        wb.WithContent(args.Message.Content);
                        await chat.webhook.ExecuteAsync(wb);
                    }
                    // Relay the message via standard message
                    else {
                        await chat.host.SendMessageAsync($"**<{args.Author.Username}/{chat.target.Mention}>** {args.Message.Content}");
                    }
                }
                // Host to target
                else if(chats.FirstOrDefault(x => x.host.Id == args.Channel.Id) != null) {
                    if(args.Message.Content.StartsWith("."))
                        return;
                    chat = chats.FirstOrDefault(x => x.host.Id == args.Channel.Id);
                    await chat.target.SendMessageAsync(args.Message.Content);
                }
            };
        }

        public static async void Open(DiscordChannel target, DiscordChannel host)
        {
            Chat chat = new Chat() {
                target = target,
                host = host
            };

            // Try to create a webhook; if one cannot be created, default to a value of 1
            // This lets the rest of the system know to use <Username/HS> messages instead of 
            // Webhook messages
            try { 
                DiscordWebhook webhook = await host.CreateWebhookAsync(target.Name);
                chat.webhook = webhook;
            } catch { 
                chat.webhook = null;
            };

            // Finish up
            await host.SendMessageAsync($"Successfully created a chat portal to {target.Mention}! **ALL** messages sent here will be sent to {target.Mention} so be careful!");
            chats.Add(chat);
        }

        public static async void Close(DiscordChannel host)
        {
            Chat chat = chats.FirstOrDefault(x => x.host == host);

            // Delete the webhook if it exists
            if(chat.webhook != null) {
                await chat.webhook.DeleteAsync();
            }

            chats.Remove(chat);
        } 
    }

    public class Chat
    {
        public DiscordChannel target { get; set; }
        public DiscordChannel host { get; set; }
        public DiscordWebhook webhook { get; set; }
    }
}
#endif