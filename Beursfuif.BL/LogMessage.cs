using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

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

            return Time.ToString("HH:MM:ss") + "::" + Type + "::" + Message;
        }
    }

    public enum LogType
    {
        [Description("Alles")]
        ALL = 0,
        [Description("Weetjes")]
        INFO = 1,
        [Description("Error")]
        ERROR = 2,
        [Description("Good news")]
        GOOD_NEWS = 4,
        [Description("Bericht van de client")]
        FROM_CLIENT = 8,
        [Description("Dranken")]
        DRINK_VM = 16,
        [Description("Intervallen")]
        INTERVAL_VM = 32,
        [Description("Clients")]
        CLIENT_VM = 64,
        [Description("Instellingen")]
        SETTINGS_VM = 128,
        [Description("Bestellingen")]
        ORDER_VM = 256,
        [Description("LogBerichten")]
        LOG_VM = 512,
        [Description("Voorspellingen")]
        PREDICT_VM = 1024,
        [Description("Gebruikersfout")]
        USER_ERROR = 2048,
        [Description("Client-Server fout")]
        CLIENT_SERVER_ERROR = 4096
    }
}
