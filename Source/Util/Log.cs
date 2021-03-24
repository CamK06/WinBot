using System;
using System.IO;

using Discord.WebSocket;

namespace WinBot.Util
{
	class Log
	{
		public static void Write(string text, LogType type = LogType.Info, bool print = true)
		{
			if(!Directory.Exists("Logs")) Directory.CreateDirectory("Logs");
			string message = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()} {type.ToString().ToUpper()}] {text}";
			File.AppendAllText($"Logs/{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}.log", message + '\n');
			if(print) {
				Console.WriteLine(message);
				if(Bot.config.logChannel != 1) 
					((SocketTextChannel)Bot.client.GetChannel(Bot.config.logChannel)).SendMessageAsync(message);
			}
		}
	}

	enum LogType
	{
		Info, Error, Warning
	}
}