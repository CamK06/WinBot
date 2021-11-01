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

namespace WinBot.Misc
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

                    // TEMPORARY: Attempt to respond using chat system
                    // TODO: Replace this with standalone chat system
                    // TODO: Clean this up, it's a mess and sucks.
                    Random r = new Random();
                    //if(r.Next(0, 100) < 65) {
                        string response = ChatSystem.Respond(args.Message.Content, args.Author);
                        if(!string.IsNullOrWhiteSpace(response)) {
                            bool hasGif = false;
                            string gif = "";
                            if(response.Contains("@g")) {
                                response = response.Replace("@g", "");
                                hasGif = true;
                                string[] gifs = File.ReadAllLines("chatGifs");
                                gif = gifs[r.Next(0, gifs.Length)];
                            }

                            await chat.target.SendMessageAsync(response);
                            await chat.host.SendMessageAsync($"**<TOFUCHAT/{chat.target.Mention}>** {response.Replace("@", "-")}");
                            if(hasGif && r.Next(0, 100) < 35)
                                await chat.target.SendMessageAsync(gif);
                            await Task.Delay(250);
                        }
                    //}
                }
                // Host to target
                if(chats.FirstOrDefault(x => x.host.Id == args.Channel.Id) != null) {
                    if(args.Message.Content.StartsWith($"{Bot.config.prefix}"))
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