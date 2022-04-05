using System.Threading.Tasks;

using DSharpPlus.Entities;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Main
{
    public class AvatarCommand : BaseCommandModule
    {
        [Command("avatar")]
        [Aliases("pfp")]
        [Description("Gets your profile picture/avatar")]
        [Category(Category.Main)]
        public async Task Avatar(CommandContext Context)
        {
            DiscordEmbedBuilder eb = new DiscordEmbedBuilder();
            eb.WithTitle($"{Context.User.Username}'s Avatar");
            eb.WithImageUrl(Context.User.GetAvatarUrl(DSharpPlus.ImageFormat.Png));
            eb.WithColor(DiscordColor.Gold);
            await Context.Channel.SendMessageAsync(eb);
        }
    }
}