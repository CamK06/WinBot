using System;
using System.IO;

namespace WinWorldBot.Utils
{
    public class Log
    {
        public static void Write(string message)
        {
            // Create and send the log message
            message = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] {message}";
            Console.WriteLine(message);
            File.AppendAllText($"Logs/{DateTime.Now.ToShortDateString().Replace("/", "-")}", message + '\n');
        }
    }
}