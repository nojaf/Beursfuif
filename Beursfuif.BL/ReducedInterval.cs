using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL
{
    //Only contains an id and toString representation of the actual value of the "real" interval
    public class ReducedInterval
    {
        public int Id { get; set; }
        public string Value { get; set; }

        public ReducedInterval()
        {

        }

        public ReducedInterval(Interval interval)
        {
            this.Id = interval.Id;
            this.Value = interval.StartTime.ToString("hh:mm") + " - " + interval.EndTime.ToString("hh:mm");
        }

        public ReducedInterval(string specialString)
        {
            Value = specialString;
            Id = int.MaxValue;
        }
    }
}
