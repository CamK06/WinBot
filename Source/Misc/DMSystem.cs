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

// This whole file is a clusterfuck since it's ported year old code from Discord.NET

namespace WinBot
{
    public class DMSystem
    {
        public static List<DMChat> chats = new List<DMChat>();

        public static void Init()
        {
            Bot.client.TypingStarted += async (DiscordClient client, TypingStartEventArgs args) => {
                var chat = chats.FirstOrDefault(x => x.user.Id == args.User.Id || x.channelId == args.Channel.Id);
                if(chat != null) {
                    var user = args.User as DiscordMember;
                    if(user != null) {

                        if(args.Channel.Id == chat.channelId) {
                            var dmChannel = await user.CreateDmChannelAsync();
                            if(dmChannel != null) await dmChannel.TriggerTypingAsync();
                        }
                        else if(user.Id == chat.user.Id) {
                            var channel = await client.GetChannelAsync(chat.channelId);
                            if(channel != null) await channel.TriggerTypingAsync();
                        }
                    }
                }
            };

            Bot.client.MessageCreated += async (DiscordClient client, MessageCreateEventArgs args) => {

                if(chats.FirstOrDefault(x => x.user.Id == args.Author.Id) != null && args.Channel.IsPrivate) {

                    var chat = chats.FirstOrDefault(x => x.user.Id == args.Author.Id);
                    DiscordChannel channel = await client.GetChannelAsync(chat.channelId);

                    var webhook = channel.GetWebhooksAsync().Result.FirstOrDefault(x => x.Id == chat.webhookId);
                    if(webhook != null) {
                        DiscordWebhookBuilder wb = new DiscordWebhookBuilder();
                        wb.WithUsername(args.Author.Username);
                        wb.WithAvatarUrl(args.Author.AvatarUrl);
                        wb.WithContent(args.Message.Content);
                        await webhook.ExecuteAsync(wb);

                        chat.messages.Add(new Message()
                        {
                            content = args.Message.Content,
                            time = DateTime.Now,
                            id = args.Message.Id,
                            userId = args.Author.Id,
                            userName = args.Author.Username
                        });
                    }
                }
#if TOFU
                else if(args.Channel.IsPrivate) {
                    Bot.staffChannel.SendMessageAsync("DM From " + args.Author.Username + ": " + args.Message.Content);
                }
#endif
                var chat2 = chats.FirstOrDefault(x => x.channelId == args.Channel.Id);
                if(chat2 != null && !args.Author.IsBot && !args.Message.Content.ToLower().Contains(".cdm")) {
                    if(chat2.showNames) await chat2.user.SendMessageAsync($"**{args.Author.Username}:** {args.Message.Content}");
                    await chat2.user.SendMessageAsync(args.Message.Content);
                }
            };
        }

        public static async void Open(DiscordMember user, ulong channelId, bool names = false)
        {
            DMChat chat = new DMChat() {
                user = user,
                showNames = names,
                messages = new List<Message>(),
                channelId = channelId
            };;

            DiscordChannel channel = Bot.client.GetChannelAsync(channelId).Result;
            if(channel != null) {
                var webhook = await channel.CreateWebhookAsync(user.Username);
                chat.webhookId = webhook.Id;
                await channel.SendMessageAsync($"Successfully created DM chat in this channel with " + user.Username + " **ALL** messages sent here will be sent to them, so be careful!");
                chats.Add(chat);
            }
        }

        public static void Save(ulong userId)
        {
            var user = Bot.client.GetUserAsync(userId).Result;
            string json = JsonConvert.SerializeObject(chats.FirstOrDefault(x => x.user.Id == userId), Formatting.Indented);
            if (!Directory.Exists($"Chats/{user.Username}")) Directory.CreateDirectory($"Chats/{user.Username}");
            File.WriteAllText($"Chats/{user.Username} {DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day} @ {DateTime.Now.Hour}-{DateTime.Now.Minute}-{DateTime.Now.Second}.json", json);
        }

        public static async void Close(ulong channelId)
        {
            var chat = chats.FirstOrDefault(x => x.channelId == channelId);
            var webhook = Bot.client.GetChannelAsync(chat.channelId).Result.GetWebhooksAsync().Result.FirstOrDefault(x => x.Id == chat.webhookId);
            await webhook.DeleteAsync();

            chats.Remove(chat);
        }
    }

    public class DMChat
    {
        public DiscordMember user { get; set; }
        public bool showNames { get; set; }
        public List<Message> messages { get; set; }
        public ulong channelId { get; set; }
        public ulong webhookId { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
        public ulong id { get; set; }
        public DateTime time { get; set; }
        public ulong userId { get; set; }
        public string userName { get; set; }
    }
}