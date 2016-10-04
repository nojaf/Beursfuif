using System;
using Beursfuif.Server.Entity;

namespace Beursfuif.BL
{
    [Serializable]
    public class LogMessage
    {
        public string Message { get; set; }
        public DateTime Time { get; set; }
        public LogType Type { get; set; }

        public LogMessage()
        {

        }

        public LogMessage(string message, LogType type = LogType.ALL)
        {
            Message = message;
            Time = DateTime.Now;
            Type = type;
        }

        public override string ToString()
        {

            return $"{Time.ToString("HH:MM:ss")}::{Type}::{Message}";
        }
    }
}
