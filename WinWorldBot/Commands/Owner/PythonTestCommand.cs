using System;
using System.Threading.Tasks;
using Discord.Commands;
using IronPython.Hosting;

namespace WinWorldBot
{
    public class PythonTestCommand : ModuleBase<SocketCommandContext>
    {
        [Command("py")]
        private Task PyCMD([Remainder]string script)
        {
            if(Context.Message.Author.Id != Globals.StarID) return Task.CompletedTask;

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

            return Task.CompletedTask;
        }
    }
}