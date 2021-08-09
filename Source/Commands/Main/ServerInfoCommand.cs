using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;
using WinBot.Util;

namespace WinBot.Commands.Main
{
    public class ServerInfoCommand : BaseCommandModule
    {
        [Command("serverinfo")]
        [Description("Gets info about the current server")]
        [Category(Category.Main)]
        public async Task ServerInfo(CommandContext Context)
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithColor(DiscordColor.Gold);
            eb.WithTitle(Context.Guild.Name);
            eb.WithThumbnail(Context.Guild.IconUrl);
            eb.AddField("Users", $"{Context.Guild.MemberCount}", true);
            eb.AddField("Created", MiscUtil.FormatDate(Context.Guild.CreationTimestamp), true);
            eb.AddField("Channels", $"{Context.Guild.Channels.Count}", true);
            eb.AddField("Owner", Context.Guild.Owner.Username, true);
            await Context.ReplyAsync("", eb.Build());
        }
    }
}