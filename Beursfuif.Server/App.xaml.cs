using Beursfuif.BL;
using Beursfuif.Server.DataAccess;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using Beursfuif.Server.Entity;

namespace Beursfuif.Server
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var beursfuifProcesses = Process.GetProcesses().Where(x => x.ProcessName == "Beursfuif.Server");

            if (beursfuifProcesses.Count() > 1)
            {
               var result = MessageBox.Show("An instance of Beursfuif.Server is already running");
               this.Shutdown();
            }

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

        private static void CheckIfBeursfuifFolderExists()
        {
            var directories = new[]
            {
                BeursfuifPaths.BeursfuifFolder,
                BeursfuifPaths.AssetsPath,
                BeursfuifPaths.DataFolder,
                BeursfuifPaths.LogFolder
            };

            foreach (string directory in directories)
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }

            LogManager.CreateNewLogFile();

        }
    }
}
