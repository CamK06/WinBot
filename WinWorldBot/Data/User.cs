using System;
using System.Collections.Generic;

namespace WinWorldBot.Data
{
    [Serializable]
    public class User
    {
        public string Username { get; set; }
        public ulong Id { get; set; }
        public DateTime StartedLogging { get; set; }
        public List<UserMessage> Messages { get; set; }
        public string FMName { get; set; }
        public int CorrectTrivia { get; set; }
        public int IncorrectTrivia { get; set; }
    }
    
    [Serializable]
    public class UserMessage
    {
        public string Content { get; set; }
        public string Channel { get; set; }
        public DateTime SentAt { get; set; }
        public ulong Id { get; set; }
    }
}