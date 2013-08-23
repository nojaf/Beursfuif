using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Beursfuif.BL.Event
{
    public class NewClientEventArgs:EventArgs
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Id { get; set; }

        public NewClientEventArgs(string name, string ip, int id)
        {
            Name = name;
            Ip = ip;
            Id = id;
        }


    }
}
