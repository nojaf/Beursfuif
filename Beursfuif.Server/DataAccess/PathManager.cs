using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.DataAccess
{
    public static class PathManager
    {
        public static string BEURSFUIF_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif";

        public static string DATA_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\";

        public static string DRINK_XML_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "drinks.xml";

        public static string ASSETS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Assets\\";

        public static string INTERVAL_BINARY_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "intervals.bin";

        public static string CURRENT_INTERVAL_XML_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "current_intervals.xml";

        public static string BUSY_AND_TIME_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "time_and_busy.xml";

        public static string AUTO_SAVE_ALL_ORDERS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "auto_saved_all_orders.bin";

        public static string LOG_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Log\\";
    }
}
