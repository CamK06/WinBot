using System.IO;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Serilog;

namespace DSharpPlus.CommandsNext
{
    public static class DSharpImprovements
    {
        public static async Task<DiscordMessage> SendFileAsync(this DiscordChannel channel, string fileName)
        {
            if(!File.Exists(fileName)) {
                Log.Warning($"File does not exist! (SendFileAsync @ {channel.Name})");
                return null;
            }

            FileStream fStream = new FileStream(fileName, FileMode.Open);
            DiscordMessage msg = await new DiscordMessageBuilder().WithFile(fileName, fStream).SendAsync(channel);
            fStream.Close();

            return msg;
        }

        public static async Task<DiscordMessage> SendFileAsync(this DiscordChannel channel, Stream file, string fileName)
        {
            file.Position = 0;
            DiscordMessage msg = await new DiscordMessageBuilder().WithFile(fileName, file).SendAsync(channel);

            return msg;
        }

        public static async Task<DiscordMessage> SendFileAsync(this DiscordChannel channel, string message, Stream file, string fileName)
        {
            file.Position = 0;
            DiscordMessage msg = await new DiscordMessageBuilder().WithFile(fileName, file).WithContent(message).SendAsync(channel);

            return msg;
        }

        public static async Task<DiscordMessage> ReplyAsync(this CommandContext Context, string Content)
        {
            return await Context.Channel.SendMessageAsync(Content);
        }

        public static async Task<DiscordMessage> ReplyAsync(this CommandContext Context, string Content, DiscordEmbed Embed)
        {
            return await Context.Channel.SendMessageAsync(Content, Embed);
        }

        public static async Task<DiscordMessage> ReplyAsync(this CommandContext Context, DiscordEmbed Embed)
        {
            return await Context.Channel.SendMessageAsync("", Embed);
        }
    }
}