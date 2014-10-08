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

        public static string DRINK_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "drinks.json";

        public static string ASSETS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Assets\\";

        public static string INTERVAL_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "intervals.json";

        public static string CURRENT_INTERVAL_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "current_intervals.json";

        public static string SETTINGS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "settings.json";

        public static string AUTO_SAVE_ALL_ORDERS = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "auto_saved_all_orders.json";

        public static string LOG_FOLDER = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Log\\";
    }
}
