using System;
using System.IO;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Configuration;

namespace WinBot.Util
{
    public class DiscordSink : ILogEventSink
    {
        private readonly IFormatProvider _formatProvider;

        public DiscordSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            string finalMessage =$"[{DateTime.Now.ToShortDateString().Replace("/", "-")} {DateTime.Now.ToShortTimeString()}] " + message;

            // Print and write the log message
            Console.WriteLine(finalMessage);
            File.AppendAllText($"Logs/{DateTime.Now.ToShortDateString().Replace("/", "-")}.log", finalMessage + "\n");

            // Send the message to Discord
            Bot.logChannel.SendMessageAsync(finalMessage);
        }
    }

    public static class DiscordSinkExtensions
    {
        public static LoggerConfiguration DiscordSink(this LoggerSinkConfiguration loggerSinkConfiguration, IFormatProvider formatProvider = null)
        {
            return loggerSinkConfiguration.Sink(new DiscordSink(formatProvider));
        }
    }
}