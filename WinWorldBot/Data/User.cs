using System;
using System.Collections.Generic;

namespace WinWorldBot.Data
{
    class User
    {
        public string Username { get; set; }
        public ulong Id { get; set; }
        public DateTime StartedLogging { get; set; }
        public List<UserMessage> Messages { get; set; }
    }
    
    class UserMessage
    {
        public string Channel { get; set; }
        public DateTime SentAt { get; set; }
        public ulong Id { get; set; }
    }
}