using System;
using System.Threading.Tasks;

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

using Serilog;

namespace WinBot.Misc
{
    class No
    {
        public static void Init()
        {
            Bot.client.MessageCreated += OnMessage;
        }

        private static async Task OnMessage(DiscordClient sender, MessageCreateEventArgs e)
        {
            if(!e.Message.Content.Contains("?") && !e.Message.Content.Contains("starman"))
                return;

            DiscordWebhook hook = await e.Channel.CreateWebhookAsync("Starman0620");
            DiscordWebhookBuilder builder = new DiscordWebhookBuilder();
            builder.WithAvatarUrl(e.Guild.GetMemberAsync(363850072309497876).Result.GetAvatarUrl(ImageFormat.Png));
            builder.WithUsername("Starman0620");
            builder.WithContent("No.");         
            await builder.SendAsync(hook);   
            await hook.DeleteAsync();
        }
    }
}