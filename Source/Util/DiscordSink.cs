using System;
using System.IO;
using System.Timers;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Configuration;

namespace WinBot.Util
{
    public class DiscordSink : ILogEventSink
    {
        private string logBuffer = "";

        private readonly IFormatProvider _formatProvider;

        public DiscordSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;

            Timer t = new Timer(5000);
            t.AutoReset = true;
            t.Elapsed += async (s, e) => {
                if(!string.IsNullOrWhiteSpace(logBuffer)) {
                    await Bot.logChannel.SendMessageAsync(logBuffer);
                    logBuffer = "";
                }
            };
            t.Start();
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            string finalMessage =$"[{DateTime.Now.ToShortDateString().Replace("/", "-")} {DateTime.Now.ToShortTimeString()}] " + message;

            // Print and write the log message
            Console.WriteLine(finalMessage);
            File.AppendAllText($"Logs/{DateTime.Now.ToShortDateString().Replace("/", "-")}.log", finalMessage + "\n");

            // Store the log in the Discord buffer
            logBuffer += finalMessage + "\n";
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