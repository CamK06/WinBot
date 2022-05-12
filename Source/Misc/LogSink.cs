using System.Timers;

using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Configuration;

namespace WinBot.Misc
{
    public class LogSink : ILogEventSink
    {
        private string logBuffer = "";

        private readonly IFormatProvider _formatProvider;

        public LogSink(IFormatProvider formatProvider)
        {
            _formatProvider = formatProvider;

            System.Timers.Timer t = new System.Timers.Timer(5000);
            t.AutoReset = true;
            t.Elapsed += async (s, e) => {
                if(!string.IsNullOrWhiteSpace(logBuffer) && Global.logChannel != null) {
                    await Global.logChannel.SendMessageAsync(logBuffer);
                    logBuffer = "";
                }
            };
            t.Start();
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            string finalMessage =$"[{DateTime.Now.ToShortDateString().Replace("/", "-")} {DateTime.Now.ToShortTimeString()} {logEvent.Level.ToString()}] " + message;

            // Change console colour
            if(logEvent.Level == LogEventLevel.Warning)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else if(logEvent.Level == LogEventLevel.Error || logEvent.Level == LogEventLevel.Fatal)
                Console.ForegroundColor = ConsoleColor.Red;
            else if(logEvent.Level == LogEventLevel.Debug || logEvent.Level == LogEventLevel.Verbose)
                Console.ForegroundColor = ConsoleColor.DarkGray;

            // Print and write the log message
            Console.WriteLine(finalMessage);
            File.AppendAllText($"Logs/{DateTime.Now.Month}-{DateTime.Now.Day}-{DateTime.Now.Year}.log", finalMessage + "\n");
            Console.ResetColor();

            // Store the log in the Discord buffer
            logBuffer += finalMessage + "\n";
        }
    }

    public static class DiscordSinkExtensions
    {
        public static LoggerConfiguration LogSink(this LoggerSinkConfiguration loggerSinkConfiguration, IFormatProvider formatProvider = null)
        {
            return loggerSinkConfiguration.Sink(new LogSink(formatProvider));
        }
    }
}