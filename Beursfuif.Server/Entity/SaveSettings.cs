using System;

namespace Beursfuif.BL
{
    public class SaveSettings
    {
        public bool Busy { get; set; }

        public DateTime CurrentTime { get; set; }

        public int Port { get; set; }

        public string BackupLocation { get; set; }

        public bool IsBeursfuifCompleted { get; set; }

        public SaveSettings()
        {

        }

        public SaveSettings(bool busy, DateTime currentTime, int port,bool isBeursfuifCompleted, string backupLocation = null)
        {
            Busy = busy;
            CurrentTime = currentTime;
            Port = port;
            IsBeursfuifCompleted = isBeursfuifCompleted;
            BackupLocation = backupLocation;
        }

        
    }
}
