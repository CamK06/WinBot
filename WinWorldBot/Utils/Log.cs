using System;
using System.IO;

namespace WinWorldBot.Utils
{
    public class Log
    {
        public static void Write(string message, bool print = true)
        {
            // Create and send the log message
            message = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] {message}";
            if(print) Console.WriteLine(message);
            File.AppendAllText($"Logs/{DateTime.Now.ToShortDateString().Replace("/", "-")}", message + '\n');
        }
    }
}