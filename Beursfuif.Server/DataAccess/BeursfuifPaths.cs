using System;

namespace Beursfuif.Server.DataAccess
{
    public static class BeursfuifPaths
    {
        public static string BeursfuifFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif";

        public static string DataFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\";

        public static string DrinkPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "drinks.json";

        public static string AssetsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Assets\\";

        public static string IntervalPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "intervals.json";

        public static string CurrentIntervalPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "current_intervals.json";

        public static string SettingsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "settings.json";

        public static string AllOrders = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "auto_saved_all_orders.json";

        public static string LogFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Log\\";
    }
}
