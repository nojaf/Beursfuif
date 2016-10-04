using System;

namespace Beursfuif.BL.Event
{
    public class NewClientEventArgs:EventArgs
    {
        public string Name { get; set; }
        public string Ip { get; set; }
        public Guid Id { get; set; }

        public NewClientEventArgs(string name, string ip, Guid id)
        {
            Name = name;
            Ip = ip;
            Id = id;
        }


    }
}
