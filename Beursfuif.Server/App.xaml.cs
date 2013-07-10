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
            CheckIfBeursfuifFolderExists();
            base.OnStartup(e);
        }

        private void CheckIfBeursfuifFolderExists()
        {
            if (!System.IO.Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif"))
            {
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif");
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Assets");
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\Data");
                System.IO.Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Beursfuif\\AutoSavedData");
            }
        }
    }
}
