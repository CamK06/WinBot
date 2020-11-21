using System;

namespace WinWorldBot.Utils
{
    internal class MiscUtil {
        public static string FormatDate(DateTimeOffset dt)
        {
            return dt.ToString("dddd, dd MMMM yyyy");
        }
    }
}