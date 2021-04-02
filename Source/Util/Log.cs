using System;
using System.IO;

namespace WinBot.Util
{
    public class Log
    {
        public static void Write(string text, bool print = true)
        {
            string message = $"[{DateTime.Now.ToShortDateString().Replace("/", "0")} {DateTime.Now.ToShortTimeString()}] " + text;

            if(print)
                Console.WriteLine(message);
            File.AppendAllText($"Logs/{DateTime.Now.ToShortDateString().Replace("/", "-")}.log", message + "\n");
        }
    }
}