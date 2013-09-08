﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    public class SaveSettings
    {
        public bool Busy { get; set; }

        public DateTime CurrentTime { get; set; }

        public SaveSettings()
        {

        }

        public SaveSettings(bool busy, DateTime currentTime)
        {
            Busy = busy;
            CurrentTime = currentTime;
        }
    }
}