﻿using System.ComponentModel;

namespace Beursfuif.Server.Entity
{
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
        CLIENT_SERVER_ERROR = 4096,
        [Description("Code bericht")]
        POINT_IN_CODE = 8192
    }
}