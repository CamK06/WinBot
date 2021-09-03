#if TOFU
using System.Threading.Tasks;

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using WinBot.Commands.Attributes;

namespace WinBot.Commands.Staff
{
    public class LockdownCommand : BaseCommandModule
    {
        [Command("lockdown")]
        [Description("Lock the server from new members")]
        [Category(Category.Staff)]
        [RequireUserPermissions(DSharpPlus.Permissions.ManageGuild)]
        public async Task Lockdown(CommandContext Context)
        {
            Bot.serverLocked = !Bot.serverLocked;
            await Context.ReplyAsync("Server " + (Bot.serverLocked ? "Locked" : "Unlocked"));
        }
    }
}
#endif