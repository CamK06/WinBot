using System;

namespace WinBot.Util
{
    public static class MiscUtil
    {
        public static string FormatDate(DateTimeOffset dt)
        {
            return dt.ToString("dddd, dd MMMM yyyy");
        }
    }
}