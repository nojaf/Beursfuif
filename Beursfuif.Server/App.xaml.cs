using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace Beursfuif.Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {       
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
            CheckIfBeursfuifFolderExists();
            base.OnStartup(e);
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.AppendToLog(new LogMessage((e.ExceptionObject as Exception).Message, LogType.ERROR));

        }

        void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogManager.AppendToLog(new LogMessage(e.Exception.Message,LogType.ERROR));
        }

        private void CheckIfBeursfuifFolderExists()
        {
            bool directoriesPresent = System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif");

            if (!directoriesPresent)
            {
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif");
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Assets");
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data");
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\AutoSavedData");
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Log");
            }

            LogManager.CreateNewLogFile();

            if (!directoriesPresent)
            {
                LogManager.AppendToLog(new LogMessage("Directories created",  LogType.INFO));
            }
        }
    }
}
