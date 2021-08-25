#if TOFU
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

using Newtonsoft.Json;

namespace WinBot.Commands.Staff
{
    public class MuteCommand : BaseCommandModule
    {
        [Command("mute")]
        [Description("Mute a dirtbag")]
        [Usage("[user]")]
        [Category(Category.Staff)]
        [RequireUserPermissions(DSharpPlus.Permissions.KickMembers)]
        public async Task Mute(CommandContext Context, DiscordMember user)
        {
            if (!Bot.mutedUsers.Contains(user.Id))
            {
                user.GrantRoleAsync(Bot.mutedRole);
                Bot.mutedUsers.Add(user.Id);
                await Context.ReplyAsync($"Muted {user.Username}!");
            }
            else
            {
                user.RevokeRoleAsync(Bot.mutedRole);
                Bot.mutedUsers.Remove(user.Id);
                await Context.ReplyAsync($"Unmuted {user.Username}!");
            }
            File.WriteAllText("muted.json", JsonConvert.SerializeObject(Bot.mutedUsers));
        }
    }
}
#endif