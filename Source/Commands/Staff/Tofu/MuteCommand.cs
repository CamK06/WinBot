#if TOFU
using System.IO;
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Util;
using WinBot.Commands.Attributes;
using static WinBot.Util.ResourceManager;

using Newtonsoft.Json;

using Serilog;

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
            // Try to set the muted role
            if(Global.mutedRole == null)
                Global.mutedRole = Global.targetGuild.GetRole(Bot.ids.mutedRole);
            if(Global.mutedRole == null)
                Log.Error("Shitcord is failing to return a valid muted role");

            if (!Global.mutedUsers.Contains(user.Id)) {
                await user.GrantRoleAsync(Global.mutedRole);
                Global.mutedUsers.Add(user.Id);
                await Context.ReplyAsync($"Muted {user.Username}!");
            }
            else {
                await user.RevokeRoleAsync(Global.mutedRole);
                Global.mutedUsers.Remove(user.Id);
                await Context.ReplyAsync($"Unmuted {user.Username}!");
            }
            File.WriteAllText(GetResourcePath("mute", ResourceType.JsonData), JsonConvert.SerializeObject(Global.mutedUsers, Formatting.Indented));
        }
    }
}
#endif