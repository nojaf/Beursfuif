using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.Server.DataAccess
{
    public static class PathManager
    {
        public static string DRINK_XML_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "drinks.xml";

        public static string ASSETS_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Assets\\";

        public static string INTERVAL_BINARY_PATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data\\" + "intervals.bin";
    }
}
