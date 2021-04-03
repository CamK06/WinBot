using System;

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
            Console.WriteLine($"[{DateTime.Now.ToShortDateString().Replace("/", "-")} {DateTime.Now.ToShortTimeString()}] " + message);
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