using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using IronPython.Hosting;
using WinWorldBot.Utils;
using WinWorldBot.Commands;

namespace WinWorldBot
{
    public class PythonTestCommand : ModuleBase<SocketCommandContext>
    {
        [Command("py")]
        [Summary("Run some Python code|")]
        [Priority(Category.Owner)]
        private async Task PyCMD([Remainder]string script)
        {
            SocketGuildUser author = Context.Message.Author as SocketGuildUser;
            if(author.Id != Globals.StarID && !author.GuildPermissions.KickMembers) {
                await Context.Message.DeleteAsync();
                return;
            }
            // Basic script formatting and automatic references
            script = script.Replace("```py", "");
            script = script.Replace("```", "");
            script = script.Replace("sys", "");
            script = "import clr\nclr.AddReference(\"Discord.Net.Core\")\nclr.AddReference(\"Discord.Net.Rest\")\nclr.AddReference(\"Discord.Net.WebSocket\")\nclr.AddReference(\"Discord.Net.Commands\")\n" + script;

            // Set up the Python engine
            var engine = Python.CreateEngine();
            dynamic scope = engine.CreateScope();
            Action<string> reply = async (string text) => {
                await ReplyAsync(text);
            };
            scope.ctx = Context;
            scope.reply = reply;

            // Execute the Python code
            engine.Execute(script, scope);
        }
    }
}
