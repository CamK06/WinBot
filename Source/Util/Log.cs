using System;
using System.IO;

using Discord.WebSocket;

namespace WinBot.Util
{
	class Log
	{
		public static void Write(string text, bool print = true)
		{
			if(!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");
			string message = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] {text}";

			if(print) {
				Console.WriteLine(message);
				if(Bot.config.logChannel != 1) 
					((SocketTextChannel)Bot.client.GetChannel(Bot.config.logChannel)).SendMessageAsync(message);
			}
		}
	}
}