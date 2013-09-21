using Beursfuif.BL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beursfuif.Server.DataAccess
{
    public static class LogManager
    {
        private static string _logFile;
        
        public static void CreateNewLogFile()
        {
            Task.Factory.StartNew(() =>
            {
                //Log file is now XML
                _logFile = PathManager.LOG_FOLDER + "Log-" + DateTime.Now.ToString("dd-MM-yyyy-HH.mm") + ".log";
            });
        }

        //line of text is serialized log message
        private static void AppendToLog(string line)
        {
            Task.Factory.StartNew(() =>
            {
                using (StreamWriter sw = File.AppendText(_logFile))
                {
                    sw.WriteLine(line);
                }
            });
        }

        public static void AppendToLog(LogMessage message)
        {
            AppendToLog(message.ToString());
        }
    }
}
